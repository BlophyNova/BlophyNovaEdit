using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Scenes.Edit.Settings.Content.GeneralOption
{
    public class SetFps : ContentEdit
    {
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                if (int.TryParse(thisTMPInputField.text,out int result))
                {
                    if(result is < -1 or 0)return;
                    GlobalData.Instance.generalData.Fps = result;
                }
            });
        }

        private void OnEnable()
        {
            thisTMPInputField.SetTextWithoutNotify($"{GlobalData.Instance.generalData.Fps}");
        }
    }
}
