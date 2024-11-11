using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Notes
{
    [CustomEditor(typeof(Note), true)]
    public class NoteNote : Note<Note> 
    {
        protected override bool AlwaysShowNote => true;
        protected override bool RespectBaseInspector => false;
    }

    /// <summary>
    /// The base class for adding notes to Objects.
    /// </summary>
    /// <typeparam name="T">The Object to add a note to.</typeparam>
    [CanEditMultipleObjects]
    public class Note<T> : Editor
    {
        protected virtual bool AlwaysShowNote => false;
        protected virtual bool RespectBaseInspector => true;
        protected virtual bool IsHeader => false; //May not work properly on all objects.

        private int ID
        {
            get
            {
                Object sourceObject = null;

                if(PrefabUtility.IsPartOfAnyPrefab(target)) //if we've selected a prefab
                {
                    sourceObject = PrefabUtility.GetCorrespondingObjectFromSourceAtPath(target, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target));
                }
                else //regular instance
                {
                    return target.GetInstanceID();
                }

                if (sourceObject != null) //get prefab instance ID
                {
                    return sourceObject.GetInstanceID();
                }
                else //this should just never ever happen.
                {
                    allowNoteEdits = false;
                    return target.GetInstanceID(); //SHOULD (hopefully) NEVER HAPPEN
                }
            }
        }

        private SerializableNote note;
        private bool allowNoteEdits = true;

        //Unity's built-in editor
        private Type targetType;
        private Editor defaultEditor;

        void OnEnable()
        {
            targetType = Type.GetType($"UnityEditor.{typeof(T).Name}Inspector, UnityEditor");
            if (targetType != null)
            {
                //When this inspector is created, also create the built-in inspector
                defaultEditor = Editor.CreateEditor(targets, targetType);
            }

            if (!Notes.TryLoadNote(ID, out note))
            {
                note = new SerializableNote(ID, "", false);
            }
        }

        void OnDisable()
        {
            //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
            //Also, make sure to call any required methods like OnDisable
            if(defaultEditor != null)
            {
                MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (disableMethod != null)
                    disableMethod.Invoke(defaultEditor, null);
                DestroyImmediate(defaultEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            if(RespectBaseInspector)
            {
                if (targetType != null)
                {
                    defaultEditor.OnInspectorGUI();
                }
                else
                {
                    DrawDefaultInspector();
                }
            }

            if(!IsHeader)
            {
                DrawNoteGui();
            }
        }

        protected override void OnHeaderGUI()
        {
            if (RespectBaseInspector)
            {
                if (targetType != null)
                {
                    defaultEditor.DrawHeader();
                }
                else
                {
                    base.DrawHeader();
                }
            }

            if (IsHeader)
            {
                DrawNoteGui();
            }
        }

        public override void DrawPreview(Rect previewArea)
        {
            if (RespectBaseInspector)
            {
                if (targetType != null)
                {
                    defaultEditor.DrawPreview(previewArea);
                }
                else
                {
                    base.DrawPreview(previewArea);
                }
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (RespectBaseInspector)
            {
                if (targetType != null)
                {
                    defaultEditor.OnPreviewGUI(r, background);
                }
                else
                {
                    base.OnPreviewGUI(r, background);
                }
            }
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (RespectBaseInspector)
            {
                if (targetType != null)
                {
                    defaultEditor.OnInteractivePreviewGUI(r, background);
                }
                else
                {
                    base.OnInteractivePreviewGUI(r, background);
                }
            }
        }

        protected virtual void DrawNoteGui()
        {
            if (targets.Length > 1) { return; }

            if (allowNoteEdits)
            {
                bool guiEnabledCache = GUI.enabled;
                GUI.enabled = true;

                if (!AlwaysShowNote)
                {
                    if (GUILayout.Button(note.show ? "―" : (note.isEmpty ? "♡" : "♥"), EditorStyles.centeredGreyMiniLabel))
                    {
                        note.show = !note.show;
                    }
                }

                if (note.show || AlwaysShowNote)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUIStyle textAreaStyle = EditorStyles.textArea;
                    textAreaStyle.wordWrap = true;

                    string noteText = EditorGUILayout.TextArea(note.noteText == "" ? Notes.placeholder : note.noteText, textAreaStyle);
                    noteText = (noteText == Notes.placeholder ? "" : noteText);

                    if (note.noteText != noteText)
                    {
                        note.noteText = noteText;

                        Notes.SaveNote(note);

                        if (Notes.Exists)
                            Notes.Instance.Repaint();
                    }

                    if (!Notes.Exists)
                    {
                        GUIContent pingContent = new GUIContent(Notes.pingIcon.Texture);
                        float pingWidth = EditorStyles.miniButton.CalcSize(pingContent).x;
                        if (GUILayout.Button(pingContent, EditorStyles.miniButton, GUILayout.Width(pingWidth)))
                        {
                            Notes.ShowWindow();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUI.enabled = guiEnabledCache;
            }
        }
    }


    /// <summary>
    /// The static class that draws a note field in each Object's inspector header.
    /// </summary>
    [InitializeOnLoad]
    public static class HeaderNotes
    {
        private static int ID
        {
            get
            {
                if (cachedHeader == null || cachedHeader.target == null) { return 0; }

                Object sourceObject = null;

                if (PrefabUtility.IsPartOfAnyPrefab(cachedHeader.target)) //if we've selected a prefab
                {
                    sourceObject = PrefabUtility.GetCorrespondingObjectFromSourceAtPath(cachedHeader.target, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(cachedHeader.target));
                }
                else //regular instance
                {
                    return cachedHeader.target.GetInstanceID();
                }

                if (sourceObject != null) //get prefab instance ID
                {
                    return sourceObject.GetInstanceID();
                }
                else //this should just never ever happen.
                {
                    return cachedHeader.target.GetInstanceID(); //SHOULD (hopefully) NEVER HAPPEN
                }
            }
        }
        private static SerializableNote note;
        private static Editor cachedHeader;

        private static readonly Type[] unsupportedTypes = { typeof(AssetImporter) };

        static HeaderNotes()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGui;
        }

        private static void InitializeHeader(Editor header)
        {
            cachedHeader = header;

            if (!Notes.TryLoadNote(ID, out note))
            {
                note = new SerializableNote(ID, "", false);
            }
        }

        private static void OnPostHeaderGui(Editor header)
        {
            if(header.targets.Length > 1) { return; }

            foreach(Type unsupportedType in unsupportedTypes)
            {
                if(header.target.GetType().IsSubclassOf(unsupportedType))
                {
                    return;
                }
            }

            //================ Initialization ================

            if(cachedHeader != header)
            {
                InitializeHeader(header);
            }

            //================ Header ================

            bool guiEnabledCache = GUI.enabled;
            GUI.enabled = true;

            Color guiColorCache = GUI.color;
            GUI.color = note.Color;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(note.show ? "―" : (note.isEmpty ? "♡" : "♥"), EditorStyles.whiteMiniLabel))
            {
                note.show = !note.show;

                Notes.SaveNote(note);

                if (Notes.Exists)
                    Notes.Instance.Repaint();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUI.color = guiColorCache;

            if (note.show)
            {
                EditorGUILayout.BeginHorizontal();

                GUIStyle textAreaStyle = EditorStyles.textArea;
                textAreaStyle.wordWrap = true;

                string noteText = EditorGUILayout.TextArea(note.noteText == "" ? Notes.placeholder : note.noteText, textAreaStyle);
                noteText = (noteText == Notes.placeholder ? "" : noteText);

                if (note.noteText != noteText) {
                    note.noteText = noteText;

                    Notes.SaveNote(note);

                    if (Notes.Exists)
                        Notes.Instance.Repaint();
                }

                if (!Notes.Exists)
                {
                    GUIContent pingContent = new GUIContent(Notes.pingIcon.Texture);
                    float pingWidth = EditorStyles.miniButton.CalcSize(pingContent).x;
                    if (GUILayout.Button(pingContent, EditorStyles.miniButton, GUILayout.Width(pingWidth)))
                    {
                        Notes.ShowWindow();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            GUI.enabled = guiEnabledCache;
        }
    }
}