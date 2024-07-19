using Scenes.DontDestoryOnLoad;
using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.Settings
{
    public class Return : PublicButton
    {
        // Start is called before the first frame update
        private void Start()
        {
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName(GlobalData.Instance.whereToEnterSettings).StartLoad());
        }
    }
}
