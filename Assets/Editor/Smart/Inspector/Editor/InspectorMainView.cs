using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Smart
{
    using static Selection;
    using static EditorGUILayout;
    using static EditorGUIUtility;

    public partial class Inspector
    {
        public class InspectorMainView : List<Editor>
        {
            private Vector2 mainPosition;
            
            public void Draw()
            {
                wideMode = true;
                Vector2 size = new Vector2(labelWidth, fieldWidth);

                if (0 == gameObjects.Length)
                {
                    mainPosition = BeginScrollView(mainPosition);
                }
                for (int i = 0; i < Count; i++)
                {
                    Editor editor = this[i];
                    
                    if (null == editor) { RemoveAt(i); window.OnSelectionChange(); break; }


                    if (editor.target is Material)
                    {

                        BeginVertical();
                        Editor.DrawFoldoutInspector(editor.target, ref editor);
                        EndVertical();

                        continue;
                    }

                    BeginVertical();
                    switch((ViewType)viewTypeValue.Value)
                    {
                        case ViewType.Comfortable:
                        case ViewType.Page:
                            editor.DrawHeader();
                            break;
                        case ViewType.Small:
                            InspectorTitlebar(true, editor.targets, false);
                            break;
                    }
                    editor.OnInspectorGUI();
                    EndVertical();
                }
                if (0 == gameObjects.Length)
                {
                    EndScrollView();
                }

                labelWidth = size.x;
                fieldWidth = size.y;
            }

            public void FREE()
            {
                for(int i = 0; i < Count; i++)
                {
                    this[i] = null;
                }

                Clear();
            }
        }
    }
}
