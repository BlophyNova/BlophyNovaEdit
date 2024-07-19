using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.End
{
    public class Next : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName("SelectMusic").StartLoad());
        }
    }
}
