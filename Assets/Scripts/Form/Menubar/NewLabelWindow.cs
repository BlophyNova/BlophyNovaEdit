using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLabelWindow : PublicButton
{
    // Start is called before the first frame update
    void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            LabelWindowsManager.Instance.NewLabelWindow();
        });
    }
}
