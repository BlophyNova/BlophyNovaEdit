using Scenes.PublicScripts;
using UnityEngine;

namespace Scenes.Select
{
    public class Create : PublicButton
    {
        public RectTransform createChartUI;

        private void Start()
        {
            thisButton.onClick.AddListener(() => { createChartUI.gameObject.SetActive(true); });
        }
    }
}