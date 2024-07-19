using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.SelectMusic
{
    public class Play : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName("Gameplay").StartLoad());
        }
    }
}
