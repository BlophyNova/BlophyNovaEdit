using System;
using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Scenes.Edit.Settings.Content.GeneralOption
{
    public class SyncFrame : ToggleEdit
    {
        // Start is called before the first frame update
        void Start()
        {
            thisToggle.onValueChanged.AddListener(value=>GlobalData.Instance.generalData.VerticalSync=value);
        }

        private void OnEnable()
        {
            thisToggle.SetIsOnWithoutNotify(GlobalData.Instance.generalData.VerticalSync);
        }
    }
}
