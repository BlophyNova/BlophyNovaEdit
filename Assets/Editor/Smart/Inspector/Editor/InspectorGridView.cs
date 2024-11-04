using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using Object = UnityEngine.Object;

    using static Selection;
    using static EditorGUILayout;
    using static EditorGUIUtility;
    using static UnityEditorInternal.InternalEditorUtility;

    public partial class Inspector
    {
        public class InspectorGridView : /*List<Row>*/List<Editor>
        {
            static Rect lastRow;

            int margin = 2;

            public GUILayoutOption slotButtonWidth
            {
                get => GL.Width((currentViewWidth - (maxSlots.Value * margin + (maxSlots.Value + 1) / 2)) / maxSlots.Value);
            }
            public GUILayoutOption slotButtonHeight
            {
                get => GL.Height(slotHeight.Value);
            }

            public class Row
            {
                public int slot = 0;
                public bool open;
                
                public bool end
                { get => maxSlots.Value == slot + 1; }

                public bool start
                { get => slot == 0; }

                public void Start()
                {
                    if(start)
                    {
                        open = true;
                        lastRow = BeginHorizontal();
                    }
                }

                public void End()
                {
                    if (end)
                    {
                        open = false;

                        EndHorizontal();

                        slot = 0;
                    }
                    else
                    {

                        slot++;
                    }
                }

                public void GridEnded()
                {
                    if(open)
                    {
                        gridView.DrawAddComponent();

                        EndHorizontal();
                    }
                    else
                    {
                        Debug.Log("Drawing outside row");
                        BeginHorizontal();
                        gridView.DrawAddComponent();
                        EndHorizontal();
                    }
                }
            }

            public Row row = new Row();

            public void Draw()
            {
                if (!showButtons.Value)
                {
                    return;
                }

                Editor editor;

                row.slot = 0;
                row.open = false;

                for (int i = 0; i < Count; i++)
                {
                    editor = this[i];

                    if (Filter(editor))
                    {
                        continue;
                    }

                    row.Start();

                    bool selected = componentView.Contains(editor);

                    if (Button(editor.target, selected))
                    {
                        if (selected)
                        {
                            SetIsInspectorExpanded(editor.target, false);
                            componentView.Remove(editor);
                        }
                        else
                        {
                            SetIsInspectorExpanded(editor.target, true);
                            componentView.Add(editor);
                        }
                    }
                    row.End();
                }

                row.GridEnded();
            }

            GUIContent GetContent(Editor editor)
            {
                Object value = editor.target;
                GUIContent content = ObjectContent(value, value.GetType());
                content.tooltip = value.GetType().Name;
                content.text = "";

                return content;
            }

            bool Button(Object value, bool selected)
            {
                if (null == value) { return false; }

                GUIContent content = ObjectContent(value, value.GetType());
                content.tooltip = value.GetType().Name;
                content.text = "";

                GUIStyle style = selected ? "Helpbox" : "Button";
                style.alignment = TextAnchor.MiddleCenter;
                style.margin = new RectOffset(margin, margin, margin, margin);

                return GL.Button(content, style, slotButtonWidth, slotButtonHeight);
            }

            public void DrawAddComponent()
            {
                if (0 == gameObjects.Length)
                {
                    return;
                }

                if (GL.Button(TrIconContent("d_Toolbar Plus@2x"), slotButtonWidth, slotButtonHeight))
                {
                    OpenComponentWindow();
                }
            }

            void OpenComponentWindow()
            {
                Type type = Type.GetType("UnityEditor.AddComponent.AddComponentWindow, UnityEditor");
                MethodInfo method = type.GetMethod("Show", BindingFlags.Static | BindingFlags.NonPublic);

                //rect.y += standardVerticalSpacing;

                object[] args = new object[2]
                {
                    lastRow,
                    gameObjects
                };

                method.Invoke(null, args);
            }

            public void FREE()
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i] = null;
                }

                Clear();
            }
        }

        /*public class Row : List<Editor>
        {
        }*/
    }
}
