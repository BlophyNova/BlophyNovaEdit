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

    public partial class Inspector
    {
        public class InspectorView
        {
            Rect gridComponentRect;
            public bool Ready
            {
                get => !selection.multiTypeSelected && objects.Length > 0;
            }

            public void Draw()
            {
                typeView.Draw();
                
                if(!Ready)
                {
                    // E x i t
                    return;
                }
                
                mainView.Draw();

                if(0 == gameObjects.Length)
                {
                    // E x i t
                    return;
                }

                // Settinsg
                settingsView.Draw();
                
                // Grid + Components
                gridComponentRect = BeginVertical();
                {
                    // Grid
                    gridView.Draw();
                    // Components
                    componentView.Draw();
                }
                EndVertical();

                // Drag n Drop
                DragAndDropEvents();
                // Instructions
                DrawInstructions();
            }


            public void DrawInstructions()
            {
                if (!showInfo.Value)
                {
                    return;
                }

                if (!Ready)
                {
                    return;
                }

                BeginVertical("Helpbox");
                GL.Label("Settings press (esc key)");
                GL.Label("Find press (ctrl + f)");
                GL.Label("Copy press (ctrl + c)");
                GL.Label("Paste press (ctrl + v)");
                GL.Label("Delete press (del key)");
                if (GL.Button("Close", "miniButton"))
                {
                    showInfo.Value = false;
                }
                EndVertical();
            }

            //
            // Drag n' Drop
            //

            void DragAndDropEvents()
            {
                Event e = Event.current;

                switch (e.type)
                {
                    case EventType.DragUpdated:
                        DragAndDropUpdated(e);
                        break;
                    case EventType.DragPerform:
                        DragAndDropPerformed(e);
                        break;
                }
            }

            void DragAndDropUpdated(Event e)
            {
                // E x i t
                if (!gridComponentRect.Contains(e.mousePosition)) { return; }

                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }

            void DragAndDropPerformed(Event e)
            {
                // E x i t
                if (!gridComponentRect.Contains(e.mousePosition)) { return; }

                Object[] others = DragAndDrop.objectReferences;
                MonoScript script;

                for (int i = 0; i < gameObjects.Length; i++)
                {
                    for (int j = 0; j < others.Length; j++)
                    {
                        script = others[j] as MonoScript;

                        if (!script) { continue; }

                        gameObjects[i].AddComponent(script.GetClass());
                    }
                }

                e.Use();
            }
        }
    }
}
