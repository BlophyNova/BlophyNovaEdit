using Scenes.PublicScripts;
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
