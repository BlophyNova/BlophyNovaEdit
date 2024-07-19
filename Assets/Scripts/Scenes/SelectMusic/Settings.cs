using Scenes.DontDestoryOnLoad;
using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.SelectMusic
{
    public class Settings : PublicButton
    {
        private void Start()
        {
            GlobalData.Instance.whereToEnterSettings = "SelectMusic";
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName("Settings").StartLoad());
        }
    }
}
