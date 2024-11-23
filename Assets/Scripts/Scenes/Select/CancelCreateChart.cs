using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelCreateChart : PublicButton
{
    public Transform createChartCanvas;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            createChartCanvas.gameObject.SetActive(false);
        });
    }
}
