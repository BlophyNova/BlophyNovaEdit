using Log;
using Manager;
using Scenes.PublicScripts;

namespace Form.LabelWindow
{
    public class CloseWindow : PublicButton
    {
        public LabelWindow labelWindow;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                //labelWindow.gameObject.SetActive(false);
                LabelWindowsManager.Instance.SetUsedColor2Unused(labelWindow.labelColorIndex);
                LabelWindowsManager.Instance.windows.Remove(labelWindow);
                LogCenter.Log($"{labelWindow.labelColorIndex}号窗口被销毁");
                Destroy(labelWindow.gameObject);
            });
        }
    }
}