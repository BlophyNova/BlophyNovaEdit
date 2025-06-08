using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class Save : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            File.WriteAllText(
                new Uri($"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json").LocalPath,
                JsonConvert.SerializeObject(GlobalData.Instance.chartEditData),Encoding.UTF8);
            Alert.EnableAlert("喵~保存成功~");
        }
    }
}