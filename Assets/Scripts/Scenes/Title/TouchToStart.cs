using Scenes.Loading;
using Scenes.PublicScripts;
using UnityEngine.SceneManagement;
namespace Scenes.Title
{
    public class TouchToStart : PublicButton
    {
        private void Start()
        {
            SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive).completed += a =>
                Loading.Controller.Instance.SetLoadSceneByName("SelectChapter");

            thisButton.onClick.AddListener(() => Loading.Controller.Instance.StartLoad());
        }
    }
}
