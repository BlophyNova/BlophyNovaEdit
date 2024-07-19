using Manager;
using Scenes.PublicScripts;
namespace Scenes.Gameplay
{
    public class Pause : PublicButton
    {
        // Start is called before the first frame update
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                ProgressManager.Instance.PausePlay();
                SpeckleManager.Instance.enabled = false;
                UIManager.Instance.pauseCanvas.gameObject.SetActive(true);
            });
        }
    }
}
