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
            foreach (var item in labelItem.labelWindow.labels)
            { 
                item.labelWindowContent.gameObject.SetActive(false);
            }
            labelItem.labelWindowContent.gameObject.SetActive(true);
            labelItem.labelWindow.currentLabelWindow=labelItem.labelWindowContent;
        });
    }
}
