using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLabel : PublicButton
{
    public LabelItem labelItem;
    public LabelWindow labelWindow;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            labelWindow.labels.Remove(labelItem);
            if (labelWindow.labels.Count > 0)
            {
                labelWindow.labels[0].labelWindowContent.gameObject.SetActive(true);
            }
            Destroy(labelItem.labelWindowContent.gameObject);
            Destroy(labelItem.gameObject);
        });
    }
}
