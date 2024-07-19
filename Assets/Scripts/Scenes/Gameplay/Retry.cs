using Scenes.Loading;
using Scenes.PublicScripts;
namespace Scenes.Gameplay
{
    public class Retry : PublicButton
    {
        // Start is called before the first frame update
        private void Start()
        {
            thisButton.onClick.AddListener(() => Loading.Controller.Instance.SetLoadSceneByName("Gameplay").StartLoad());
        }
    }
}
