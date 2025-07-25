using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Scenes.Edit.Settings.Content.GeneralOption
{
    public class MusicVolume : ContentEdit
    {
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                if (float.TryParse(thisTMPInputField.text,out float result))
                {
                    if(result is <0 or >1)return;
                    GlobalData.Instance.generalData.MusicVolume = result;
                }
            });
        }

        private void OnEnable()
        {
            thisTMPInputField.SetTextWithoutNotify($"{GlobalData.Instance.generalData.MusicVolume}");
        }
    }
}
