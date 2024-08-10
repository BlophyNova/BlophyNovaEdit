using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : PublicButton
{
    public RectTransform createChartUI;
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            createChartUI.gameObject.SetActive(true);
        });
    }
}
