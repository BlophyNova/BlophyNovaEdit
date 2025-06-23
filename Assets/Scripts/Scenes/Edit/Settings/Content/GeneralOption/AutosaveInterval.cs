using System;
using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Scenes.Edit.Settings.Content.GeneralOption
{
    public class AutosaveInterval : ContentEdit
    {
        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                if (float.TryParse(thisTMPInputField.text,out float result))
                {
                    if(result<=0)return;
                    GlobalData.Instance.generalData.AutosaveInterval = result;
                }
            });
        }

        private void OnEnable()
        {
            thisTMPInputField.SetTextWithoutNotify($"{GlobalData.Instance.generalData.AutosaveInterval}");
        }
    }
}
