using Scenes.DontDestoryOnLoad;
using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.SelectChapter
{
    public class Settings : PublicButton
    {
        // Start is called before the first frame update
        private void Start()
        {
            GlobalData.Instance.whereToEnterSettings = "SelectChapter";
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName("Settings").StartLoad());
        }
    }
}
