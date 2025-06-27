using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hook;
using Newtonsoft.Json;
using Scenes.PublicScripts;
using Unity.VisualScripting;
using UnityEngine;
using UtilityCode.Singleton;
using UtilityCode.TimeUtility;

namespace Scenes.DontDestroyOnLoad
{
    public class AutoSave : MonoBehaviourSingleton<AutoSave>
    {
        private void Start()
        {
            //if (Application.isEditor)
            //{
                //return;
                //StartCoroutine(StartAutoSave());
            //}

            GlobalData.Instance.onStartEdit += () => StartCoroutine(StartAutoSave());
        }

        private IEnumerator StartAutoSave()
        {
            while (true)
            {
                yield return new WaitForSeconds(GlobalData.Instance.generalData.AutosaveInterval);
                string saveData = JsonConvert.SerializeObject(GlobalData.Instance.chartEditData);
                File.WriteAllText(
                    new Uri(
                            $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/AutoSave/{TimeUtility.GetCurrentTime()}.json")
                        .LocalPath,
                    saveData, Encoding.UTF8);
                List<string> autoSaves= 
                    Directory.GetFiles(new Uri(
                        $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/AutoSave").LocalPath)
                        .Where(t => t.EndsWith(".json"))
                        .Select(Path.GetFullPath)
                        //.Select(int.Parse)
                        .ToList();
                for (int i = 0; i < 10; i++)
                {
                    if(autoSaves.Count<=0)break;
                    autoSaves.RemoveAt(autoSaves.Count-1);
                }

                foreach (string autoSave in autoSaves)
                {
                    Debug.Log(autoSave);
                    File.Delete(new Uri(autoSave).LocalPath);
                }
            }
        }
        public void Save()
        {

            File.WriteAllText(
                new Uri(
                        $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json")
                    .LocalPath,
                JsonConvert.SerializeObject(GlobalData.Instance.chartEditData), Encoding.UTF8);
            Alert.EnableAlert("喵~保存成功~");
        }
    }
}