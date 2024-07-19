using Scenes.DontDestoryOnLoad;
using Scenes.PublicScripts;
using TMPro;
namespace Scenes.Settings
{
    public class OffsetSubtract : PublicButton
    {
        public TextMeshProUGUI offsetText;
        // Start is called before the first frame update
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.offset -= .005f;
                offsetText.text = $"{GlobalData.Instance.offset * 1000:F0}";
            });
        }
    }
}
