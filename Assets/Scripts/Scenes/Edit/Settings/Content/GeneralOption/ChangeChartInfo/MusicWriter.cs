using System;
using System.IO;
using System.Text;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Scenes.Edit.Settings.Content.GeneralOption.ChangeChartInfo
{
    public class MusicWriter : ContentEdit
    {
        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.metaData.musicWriter = thisTMPInputField.text;
                File.WriteAllText(new Uri($"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/MetaData.json").LocalPath,JsonConvert.SerializeObject(GlobalData.Instance.metaData),Encoding.UTF8);
            });
        }

        private void OnEnable()
        {
            thisTMPInputField.SetTextWithoutNotify(GlobalData.Instance.metaData.musicWriter);
        }
    }
}
