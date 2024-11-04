using System.Collections.Generic;

using UnityEngine;
using Object = UnityEngine.Object;

using UnityEditor;
using UnityEditorInternal;

namespace Smart
{
    using static UnityEditorInternal.InternalEditorUtility;

    public partial class Inspector
    {
        public class InspectorSelection : Dictionary<string, Bucket>
        {
            public bool multiTypeSelected;
            //
            // Properties
            //

            public List<Component> clipboard = new List<Component>();

            public Editor[] editors
            {
                get => ActiveEditorTracker.sharedTracker.activeEditors;
            }

            public Object[] targets
            {
                get => Selection.objects;
            }

            public GameObject[] gameObjects
            {
                get => Selection.gameObjects;
            }

            public void Create()
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Add(targets[i]);
                }

                if(Keys.Count > 1)
                {
                    multiTypeSelected = true;

                    CreateButtons();
                }
                else
                {
                    if (0 == Keys.Count) { return; }

                    List<string> type = new List<string>(Keys);
                
                    NarrowSelection(type[0]);
                }
            }

            public void Add(Object value)
            {
                string type = value.GetType().Name;

                if(value is GameObject && PrefabUtility.IsPartOfPrefabAsset(value))
                {
                    type = "Prefab";
                }

                // Bucket exists
                if(ContainsKey(type))
                {
                    // Already on list
                    if(this[type].Contains(value))
                    {
                        return;
                    }
                }
                else
                {
                    // Bucket create
                    Add(type, new Bucket());
                }

                // Add to list
                this[type].Add(value);
            }
            public void Remove(Object value)
            {
                string type = value.GetType().Name;

                // Bucket exists
                if (ContainsKey(type))
                {
                    this[type].Remove(value);

                    if(0 == this[type].Count)
                    {
                        Remove(type);
                    }
                }
            }

            public void CreateButtons()
            {
                foreach (KeyValuePair<string, Bucket> pair in this)
                {
                    Object obj = (pair.Value.Count > 0) ? pair.Value[0] : null;

                    ButtonType button = new ButtonType(obj, pair.Key);

                    typeView.Add(button);
                }
            }

            public void NarrowSelection(string type)
            {
                multiTypeSelected = false;

                Selection.objects = this[type].ToArray();

                for(int i = 0; i < editors.Length; i++)
                {
                    //if(null == editors[i] || null == editors[i].serializedObject) { continue; }

                    CreateEditorInternal(editors[i]);
                }
            }

            void CreateEditorInternal(Editor value)
            {
                if(!value || !value.target)
                {
                    return;
                }

                if(value.target.GetType().Name == "AssetImporter")
                {
                    return;
                }

                if(0 == gameObjects.Length)
                {
                    mainView.Add(value);

                    return;
                }
                
                bool isMaterial = value.target is Material;
                bool isComponent = value.target is Component;


                if(isComponent || isMaterial)
                {
                    CreateSlots(value);
                }
                else
                {
                    mainView.Add(value);
                }
            }

            void CreateSlots(Editor value)
            {
                for(int i = 0; i < gameObjects.Length; i++)
                {
                    if(value.target is Material)
                    {
                        continue;
                    }

                    if(null == gameObjects[i].GetComponent(value.target.GetType()))
                    {
                        return;
                    }
                }

                gridView.Add(value);

                bool isExpanded = GetIsInspectorExpanded(value.target);

                if (isExpanded) { componentView.Add(value); }
            }

            public void Copy()
            {
                clipboard.Clear();

                for (int i = 0; i < gridView.Count; i++)
                {
                    if (componentView.Contains(gridView[i]))
                    {
                        clipboard.Add(gridView[i].target as Component);
                    }
                }

                string message = string.Format("Copied: {0} Components", clipboard.Count);

                window.ShowNotification(new GUIContent(message));
            }

            public void Paste()
            {
                Debug.LogFormat("gameObjects.Length: {0}", gameObjects.Length);
                Debug.LogFormat("clipboard.Count: {0}", clipboard.Count);

                for (int i = 0; i < gameObjects.Length; i++)
                {
                    for(int j = 0; j < clipboard.Count; j++)
                    {
                        ComponentUtility.CopyComponent(clipboard[j]);
                        ComponentUtility.PasteComponentAsNew(gameObjects[i]);
                    }

                    Undo.RecordObject(gameObjects[i], "PasteComponent");
                }

                string message = string.Format("Pasted: {0} Components", clipboard.Count);
                
                window.OnSelectionChange();
                window.ShowNotification(new GUIContent(message));
            }

            public void Delete()
            {
                for (int i = 0; i < gridView.Count; i++)
                {
                    if (componentView.Contains(gridView[i]))
                    {
                        Undo.DestroyObjectImmediate(gridView[i].target);
                    }
                }

                GetWindow<Inspector>().OnSelectionChange();
            }
        }

        //
        // BUCKET
        //
        public class Bucket : List<Object>
        {
        }
    }
}