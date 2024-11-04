using UnityEditor;
using UnityEngine;

namespace Smart
{
    public partial class Inspector : EditorWindow
    {
        // Window
        private static Inspector window;
        // View ports
        private static InspectorView view = new InspectorView();
        private static InspectorTypeView typeView = new InspectorTypeView();
        private static InspectorMainView mainView = new InspectorMainView();
        private static InspectorSettings settingsView = new InspectorSettings();
        private static InspectorGridView gridView = new InspectorGridView();
        private static InspectorComponentView componentView = new InspectorComponentView();
        // Selection
        private static InspectorSelection selection = new InspectorSelection();

        [MenuItem("Tools/Smart/Inspector")]
        public static void Open()
        {
            GetWindow<Inspector>("Smart Inspector");
        }

        private void OnEnable()
        {
            window = this;

            OnSelectionChange();

            autoRepaintOnSceneChange = true;

            EditorApplication.playModeStateChanged -= PlayModeChanged;
            EditorApplication.playModeStateChanged += PlayModeChanged;

            EditorApplication.projectChanged -= OnSelectionChange;
            EditorApplication.projectChanged += OnSelectionChange;
        }

        void PlayModeChanged(PlayModeStateChange state)
        {
            OnSelectionChange();
        }


        private void OnSelectionChange()
        {
            selection.Clear();
            
            typeView.FREE();
            mainView.FREE();
            gridView.FREE();
            componentView.FREE();

            selection.Create();
        }

        //
        // Events
        //

        void OnGUI()
        {
            view.Draw();
            EventListener();
            Repaint();
        }

        void EventListener()
        {
            Event e = Event.current;

            if(e.type == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.Escape)
                {
                    showSettings.Value = !showSettings.Value;
                }
            }

            if(e.type != EventType.ValidateCommand)
            {
                return;
            }

            //Debug.Log(e.commandName);

            if (e.commandName == "Find")
            {
                showSearchBar.Value = !showSearchBar.Value;
            }

            if (e.commandName == "Copy")
            {
                //Debug.Log("Smart Copy");
                selection.Copy();
            }

            if(e.commandName == "Paste")
            {
                //Debug.Log("Smart Paste");
                selection.Paste();
            }

            if (e.commandName == "SoftDelete")
            {
                //Debug.Log("Smart Delete");
                selection.Delete();
            }
        }
    }
}