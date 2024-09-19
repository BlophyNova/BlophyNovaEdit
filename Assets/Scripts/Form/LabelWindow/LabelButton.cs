using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelButton : PublicButton
{
    public LabelItem labelItem;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            labelItem.labelWindow.currentLabelWindow.labelWindow.LabelLostFocus();
            labelItem.labelWindow.currentLabelWindow = labelItem.labelWindowContent;
            labelItem.labelWindow.currentLabelWindow.labelWindow.LabelGetFocus();
            //foreach (var item in labelItem.labelWindow.labels)
            //{
            //    item.labelWindowContent.gameObject.SetActive(false);
            //}
            //labelItem.labelWindowContent.gameObject.SetActive(true);
            labelItem.labelWindowContent.transform.SetAsLastSibling();
        });
    }
}
