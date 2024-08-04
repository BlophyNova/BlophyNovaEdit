using Scenes.DontDestoryOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.BoxList
{
    public class Add : PublicButton
    {
        private void Start()
        {
            thisButton.onClick.AddListener(() => 
            {
                GlobalData.Instance.chartEditData.boxes.Add(GlobalData.Instance.CreateNewBox());
                GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            });
        }
    }
}