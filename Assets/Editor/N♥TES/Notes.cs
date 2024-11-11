using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Notes
{
    [System.Serializable]
    public class BuiltInEditorIcon
    {
        public string name;
        public Texture Texture
        {
            get
            {
                if(texture == null)
                {
                    texture = EditorGUIUtility.FindTexture(name);
                }

                return texture;
            }
        }

        private Texture texture;

        public BuiltInEditorIcon(string name)
        {
            this.name = name;
        }
    }

    [System.Serializable]
    public class SerializableNote : ISerializationCallbackReceiver
    {
        #region Properties

        public bool isEmpty => noteText == "";
        public string TypeName
        {
            get
            {
                if (typeName == "")
                {
                    Object objectFromInstanceID = EditorUtility.InstanceIDToObject(ObjectID);

                    if(objectFromInstanceID != null)
                    {
                        typeName = objectFromInstanceID.GetType().FullName;
                    }
                }

                return typeName;
            }
        }
        public int ObjectID
        {
            get
            {
                if(objectID == null)
                {
                    objectID = GlobalObjectId.GlobalObjectIdentifierToInstanceIDSlow(globalObjectID);
                }

                return objectID ?? default;
            }
        }
        public Color Color => Color.HSVToRGB(Mathf.Repeat(ObjectID / 256f, 1f), ColorS, ColorV);


        #endregion

        #region Variables

        const float ColorS = 0.75f;
        const float ColorV = 1f;

        public string noteText;
        public bool show;
        [SerializeField] private string typeName;
        [SerializeField] private string GlobalObjectSerialization;

        private GlobalObjectId globalObjectID;
        private int? objectID;

        #endregion

        #region Constructors

        public SerializableNote(int objectID, string noteText, bool show = false) : this(EditorUtility.InstanceIDToObject(objectID), noteText, show) { }

        public SerializableNote(Object attachedObject, string noteText, bool show = false)
        {
            if(attachedObject != null)
            {
                this.globalObjectID = GlobalObjectId.GetGlobalObjectIdSlow(attachedObject);
                this.typeName = attachedObject.GetType().FullName;
            }

            this.noteText = noteText;
            this.show = show;
        }

        #endregion

        #region Functions

        public Object GetAttachedObject()
        {
            return EditorUtility.InstanceIDToObject(ObjectID);
        }

        #endregion

        #region Serialisation

    public void OnBeforeSerialize()
        {
            GlobalObjectSerialization = globalObjectID.ToString();
        }

        public void OnAfterDeserialize()
        {
            GlobalObjectId.TryParse(GlobalObjectSerialization, out globalObjectID);
        }

        #endregion
    }

    [System.Serializable]
    public class NoteCache
    {
        public List<SerializableNote> notes = new List<SerializableNote>();

        public SerializableNote this[int objectID]
        {
            get { return notes.Where(n => n.ObjectID == objectID).FirstOrDefault(); }
        }
    }

    [InitializeOnLoad]
    public class LoadNotesOnStartup
    {
        static LoadNotesOnStartup()
        {
            Notes.LoadNotesFromDisk();
        }
    }

    public class Notes : EditorWindow
    {
        #region static

        public static string assetStoreUrl = "https://assetstore.unity.com/packages/slug/216374";
        public static string titleText = "Here you can view all of your notes! \nClick the Ping button to see which Object the note belongs to.";
        public static string noNotesText = "Click the ♥ to add notes to stuff or use the \"Note\" component.";
        public static string placeholder = "(ﾉ>ω<)ﾉ :｡･:*:･ﾟ’★,｡･:*:･ﾟ’☆ So much room for notes!";

        private static NoteCache noteCache;
        private static string noteCachePath => $"{Application.dataPath}/N♥TES/Editor/NoteCache.json";

        public static Notes Instance { get; private set; }
        public static bool Exists
        {
            get { return Instance != null; }
        }

        public static void SaveNote(SerializableNote note)
        {
            if (note == null) { return; }

            LoadNotesFromDisk();

            int noteIndex = noteCache.notes.IndexOf(noteCache[note.ObjectID]);

            if (noteIndex >= 0)
            {
                noteCache.notes[noteIndex] = note;
            }
            else
            {
                noteCache.notes.Add(note);
            }

            SaveNotesToDisk();

            LoadNotesFromDisk();
        }

        public static bool TryLoadNote(int objectID, out SerializableNote note)
        {
            LoadNotesFromDisk();

            SerializableNote cachedNote = noteCache.notes.Where(n => n.ObjectID == objectID).OrderBy(n => n.TypeName).FirstOrDefault();

            if(cachedNote != null)
            {
                note = cachedNote;
                return true;
            }

            note = null;
            return false;
        }

        public static void NukeNote(SerializableNote note)
        {
            noteCache.notes.Remove(note);

            SaveNotesToDisk();
        }

        public static void NukeAllNotes()
        {
            noteCache = new NoteCache();

            SaveNotesToDisk();
        }

        public static void LoadNotesFromDisk()
        {
            if (noteCache == null)
            {
                noteCache = new NoteCache();
            }

            if (File.Exists(noteCachePath))
            {
                NoteCache loadedCache = JsonUtility.FromJson<NoteCache>(File.ReadAllText(noteCachePath));

                noteCache = loadedCache == null ? noteCache : loadedCache;
            }
        }

        public static void SaveNotesToDisk()
        {
            if (noteCache != null)
            {
                if (!File.Exists(noteCachePath))
                {
                    File.Create(noteCachePath).Dispose();
                }

                File.WriteAllText(noteCachePath, JsonUtility.ToJson(noteCache, true));
            }

            LoadNotesFromDisk();
        }

        public static bool NoteIsInCache(SerializableNote note)
        {
            return noteCache.notes.Contains(note);
        }

        #endregion

        private List<SerializableNote> notes => noteCache.notes;
        private Vector2 scrollPosition;

        public static BuiltInEditorIcon notesIcon = new BuiltInEditorIcon("d_FilterByLabel");
        public static BuiltInEditorIcon helpIcon = new BuiltInEditorIcon("_Help");
        public static BuiltInEditorIcon pingIcon = new BuiltInEditorIcon("animationvisibilitytoggleon");
        public static BuiltInEditorIcon copyIcon = new BuiltInEditorIcon("d_UnityEditor.ConsoleWindow");
        public static BuiltInEditorIcon clearIcon = new BuiltInEditorIcon("Grid.EraserTool");
        public GUIStyle richTextHelpBoxStyle;

        [MenuItem("Window/N♥TES")]
        public static void ShowWindow()
        {
            Instance = GetWindow<Notes>("N♥TES");

            Instance.titleContent = new GUIContent(Instance.titleContent.text, notesIcon.Texture);

            Instance.Show();
        }

        private void OnEnable()
        {
            if (!Exists)
            {
                Instance = this;
            }
        }

        private void OnGUI()
        {
            richTextHelpBoxStyle = EditorStyles.helpBox;
            richTextHelpBoxStyle.richText = true;

            if (noteCache != null && notes != null && notes.Count > 0)
            {
                //================ Header ================

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(titleText, EditorStyles.centeredGreyMiniLabel);

                GUIContent helpContent = new GUIContent(helpIcon.Texture);
                if (GUILayout.Button(helpContent, EditorStyles.centeredGreyMiniLabel, GUILayout.Width(EditorStyles.centeredGreyMiniLabel.CalcSize(helpContent).x)))
                {
                    Application.OpenURL(assetStoreUrl);
                }

                EditorGUILayout.EndHorizontal();

                //================ Notes ================

                EditorGUILayout.Space();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                for (int n = 0; n < notes.Count; n++)
                {
                    if (notes[n] == null || notes[n].noteText == "") { continue; }

                    Object noteAttachedObject = notes[n].GetAttachedObject();

                    if(noteAttachedObject == null) { continue; }

                    GUILayout.BeginHorizontal();

                    GUIContent noteAssetContent = new GUIContent(noteAttachedObject.name, AssetPreview.GetMiniThumbnail(noteAttachedObject));
                    float nameHeight = EditorStyles.boldLabel.CalcHeight(new GUIContent(noteAttachedObject.name), position.width);
                    float nameWidth = EditorStyles.boldLabel.CalcSize(new GUIContent(noteAssetContent.text, notesIcon.Texture)).x;

                    EditorGUI.DrawRect(GUILayoutUtility.GetRect(2f, nameHeight), notes[n].Color);

                    GUILayout.Label(noteAssetContent, EditorStyles.boldLabel, GUILayout.Width(nameWidth), GUILayout.Height(nameHeight));

                    GUILayout.FlexibleSpace();

                    GUIContent pingContent = new GUIContent("Ping", pingIcon.Texture);
                    if (GUILayout.Button(pingContent, EditorStyles.miniButton, GUILayout.Width(EditorStyles.miniButton.CalcSize(pingContent).x)))
                    {
                        EditorGUIUtility.PingObject(noteAttachedObject);
                        Selection.activeObject = noteAttachedObject;
                    }

                    if (string.IsNullOrWhiteSpace(notes[n].noteText))
                        GUI.enabled = false;

                    GUIContent copyContent = new GUIContent("Copy", copyIcon.Texture);
                    if (GUILayout.Button(copyContent, EditorStyles.miniButton, GUILayout.Width(EditorStyles.miniButton.CalcSize(copyContent).x)))
                    {
                        EditorGUIUtility.systemCopyBuffer = notes[n].noteText;
                    }

                    GUI.enabled = true;

                    GUIContent clearContent = new GUIContent("Clear", clearIcon.Texture);
                    if (GUILayout.Button(clearContent, EditorStyles.miniButton, GUILayout.Width(EditorStyles.miniButton.CalcSize(clearContent).x)))
                    {
                        GUILayout.EndHorizontal();

                        GUILayout.Label(notes[n].noteText, richTextHelpBoxStyle);

                        EditorGUILayout.Space();

                        NukeNote(notes[n]);

                        continue;
                    }

                    GUILayout.EndHorizontal();

                    if (!string.IsNullOrWhiteSpace(notes[n].noteText)) {
                        EditorGUILayout.HelpBox(notes[n].noteText, MessageType.None, true);
                    }

                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                //================ No Notes ================

                GUILayout.FlexibleSpace();

                GUILayout.Label(noNotesText, EditorStyles.centeredGreyMiniLabel);

                GUILayout.FlexibleSpace();
            }
        }
    }
}