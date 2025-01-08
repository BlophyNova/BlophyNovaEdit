using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtilityCode.Singleton;
public class AutoSave : MonoBehaviourSingleton<AutoSave>
{
    private void Start()
    {
        GlobalData.Instance.onStartEdit += () => StartCoroutine(StartAutoSave());
    }
    private IEnumerator StartAutoSave()
    {
        int saveCount = 0;
        while (true) 
        {
            yield return new WaitForSeconds(600);
            string saveData = JsonConvert.SerializeObject(GlobalData.Instance.chartEditData);
            File.WriteAllText($"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/AutoSave.{saveCount++}",saveData);
            if (saveCount>=10)
            {
                saveCount = 0;
            }
        }
    }
}
