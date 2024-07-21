using Controller;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshUI : PublicButton
{
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        { 
            CameraController.Instance.CameraAreaUpdate();
        });
    }
}
