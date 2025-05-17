using Controller;
using Data.Interface;
using Log;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;

namespace Form.Menubar
{
    public class RefreshUI : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                CameraController.Instance.CameraAreaUpdate();
                LogCenter.Log("成功刷新制谱器全局UI适配");
            });
        }
    }
}