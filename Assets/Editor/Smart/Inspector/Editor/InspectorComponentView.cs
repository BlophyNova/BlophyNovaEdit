using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;

    using static EditorGUILayout;
    using static UnityEditorInternal.InternalEditorUtility;

    public partial class Inspector
    {
        public class InspectorComponentView : List<Editor>
        {
            private Vector2 componentPosition;

            public void Draw()
            {
                if (searchingProperty)
                {
                    Editor editor;

                    componentPosition = BeginScrollView(componentPosition);
                    for (int i = 0; i < gridView.Count; i++)
                    {
                        editor = gridView[i];

                        if (editor.target is Material)
                        {
                            continue;
                        }

                        DrawByProperty(editor);
                    }
                    GL.FlexibleSpace();
                    EndScrollView();

                    return;
                }

                componentPosition = BeginScrollView(componentPosition);
                for (int i = 0; i < Count; i++)
                {
                    if (null == this[i])
                    {
                        RemoveAt(i);

                        break;
                    }

                    if (Filter(this[i]))
                    {
                        continue;
                    }

                    Editor editor = this[i];

                    bool isExpanded = GetIsInspectorExpanded(editor.target);

                    if (editor.target is Material)
                    {
                        BeginVertical();
                        Editor.DrawFoldoutInspector(editor.target, ref editor);
                        EndVertical();

                        continue;
                    }

                    DrawDefault(isExpanded, editor);
                }
                GL.FlexibleSpace();
                EndScrollView();
            }

            void DrawByProperty(Editor editor)
            {
                bool editorTitleDrawned = false;
                
                editor.serializedObject.Update();
                SerializedProperty itr = editor.serializedObject.GetIterator();
                while (itr.NextVisible(true))
                {
                    GUI.enabled = itr.name != "m_Script";

                    if (!FilterProperty(itr.displayName))
                    {
                        if(!editorTitleDrawned)
                        {
                            if (ViewType.ContentsOnly != (ViewType)viewTypeValue.Value)
                            {
                                InspectorTitlebar(true, editor);
                            }

                            editorTitleDrawned = true;
                        }

                        PropertyField(itr, true);
                    }

                    GUI.enabled = true;
                }

                editor.serializedObject.ApplyModifiedProperties();
            }

            void DrawDefault(bool isExpanded, Editor editor)
            {

                if (ViewType.Page == (ViewType)viewTypeValue.Value)
                {
                    PageComponentHeader(editor);
                    BeginVertical();
                }
                else if (ViewType.ContentsOnly != (ViewType)viewTypeValue.Value)
                {
                    isExpanded = InspectorTitlebar(isExpanded, editor);
                }

                // WARNING: headache ensued -- No idea why I wrote this line, but it works without it
                //SetIsInspectorExpanded(editor.target, isExpanded);

                if (isExpanded) { editor.OnInspectorGUI(); }
                if (ViewType.Page == (ViewType)viewTypeValue.Value)
                { EndVertical(); }
            }

            void PageComponentHeader(Editor editor)
            {
                GUIContent content = new GUIContent();
                content = EditorGUIUtility.ObjectContent(editor.target, editor.target.GetType());
                content.text = "";

                GUILayoutOption width = GL.Width(48);
                GUILayoutOption height = GL.Height(24);

                GUIContent prev = EditorGUIUtility.IconContent("tab_prev@2x");
                GUIContent next = EditorGUIUtility.IconContent("tab_next@2x");

                GUIStyle label = new GUIStyle("boldLabel");
                label.fontSize = 13;

                string name = editor.target.GetType().Name;
                BeginHorizontal("Helpbox");
                if (GL.Button("Prev", "Button", height, width)) { PreviousButton(editor); }
                LabelField(content, width, height);
                //GL.Space(5);
                LabelField(name, label, height);
                if (GL.Button("Next", "Button", height, width)) { NextButton(editor); }
                EndHorizontal();
            }

            void PreviousButton(Editor editor)
            {
                int index = 0;

                for(int i = 0; i < gridView.Count; i++)
                {
                    if(editor == gridView[i]) { index = i; }
                }

                if(index - 1 >= 0)
                {
                    Editor prev = gridView[index - 1];
                    if (prev.target is Material) { return; }

                    SetIsInspectorExpanded(gridView[index].target, false);
                    Remove(editor);

                    SetIsInspectorExpanded(prev.target, true);
                    Add(prev);

                }
            }

            void NextButton(Editor editor)
            {
                int index = 0;

                for (int i = 0; i < gridView.Count; i++)
                {
                    if (editor == gridView[i]) { index = i; }
                }

                if (index + 1 <= gridView.Count - 1)
                {
                    Editor next = gridView[index + 1];
                    if (next.target is Material) { return; }

                    SetIsInspectorExpanded(editor.target, false);
                    Remove(editor);

                    SetIsInspectorExpanded(next.target, true);
                    Add(next);

                }
            }

            public void FREE()
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i] = null;
                }

                Clear();
            }

            public static GUILayoutOption ExpandWidth
            {
                get => GL.ExpandWidth(true);
            }
            public static GUILayoutOption CollapseWidth
            {
                get => GL.ExpandWidth(false);
            }
        }
    }
}
