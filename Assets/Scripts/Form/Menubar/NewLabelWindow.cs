using Manager;
using Scenes.PublicScripts;

namespace Form.Menubar
{
    public class NewLabelWindow : PublicButton
    {
        // Start is called before the first frame update
        private void Start()
        {
            thisButton.onClick.AddListener(() => { LabelWindowsManager.Instance.NewLabelWindow(); });
        }
    }
}