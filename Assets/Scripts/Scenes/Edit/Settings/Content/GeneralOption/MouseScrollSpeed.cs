using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Scenes.Edit.Settings.Content.GeneralOption
{
    public class MouseScrollSpeed : ContentEdit
    {
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                if (float.TryParse(thisTMPInputField.text,out float result))
                {
                    if(result<=0)return;
                    GlobalData.Instance.generalData.MouseWheelSpeed = result;
                }
            });
        }

        private void OnEnable()
        {
            thisTMPInputField.SetTextWithoutNotify($"{GlobalData.Instance.generalData.MouseWheelSpeed}");
        }
    }
}
