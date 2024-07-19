using Scenes.DontDestoryOnLoad;
using TMPro;
using UnityEngine;
namespace Scenes.Settings
{
    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI isAutoplayText;
        public TextMeshProUGUI offsetText;
        private void Start()
        {
            isAutoplayText.text = GlobalData.Instance.isAutoplay switch
            {
                true => "自动播放: 开",
                false => "自动播放: 关"
            };
            offsetText.text = $"{GlobalData.Instance.offset * 1000:F0}";
        }
    }
}
