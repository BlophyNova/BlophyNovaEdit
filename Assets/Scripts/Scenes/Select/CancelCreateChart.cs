using Scenes.PublicScripts;
using UnityEngine;

namespace Scenes.Select
{
    public class CancelCreateChart : PublicButton
    {
        public Transform createChartCanvas;

        private void Start()
        {
            thisButton.onClick.AddListener(() => { createChartCanvas.gameObject.SetActive(false); });
        }
    }
}