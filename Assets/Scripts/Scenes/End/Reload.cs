using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.End
{
    public class Reload : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName("Gameplay").StartLoad());
        }
    }
}
