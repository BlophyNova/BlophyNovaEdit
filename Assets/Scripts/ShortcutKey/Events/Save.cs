using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Save : ShortcutKeyEventBase
{
    private void Start()
    {
        Init();
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);
        File.WriteAllText($"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json",JsonConvert.SerializeObject(GlobalData.Instance.chartEditData));
        Alert.EnableAlert($"喵~保存成功~");
    }
}
