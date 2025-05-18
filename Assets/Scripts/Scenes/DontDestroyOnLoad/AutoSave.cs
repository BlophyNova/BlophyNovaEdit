using System.Collections;
using System.IO;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UtilityCode.Singleton;

public class AutoSave : MonoBehaviourSingleton<AutoSave>
{
    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return;
        }

        GlobalData.Instance.onStartEdit += () => StartCoroutine(StartAutoSave());
    }

    private IEnumerator StartAutoSave()
    {
        int saveCount = 0;
        while (true)
        {
            yield return new WaitForSeconds(600);
            string saveData = JsonConvert.SerializeObject(GlobalData.Instance.chartEditData);
            File.WriteAllText(
                $"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/AutoSave.{saveCount++}",
                saveData);
            if (saveCount >= 10)
            {
                saveCount = 0;
            }
        }
    }
}