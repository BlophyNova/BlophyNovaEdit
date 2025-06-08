using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Data.ChartEdit;
using Hook;
using Manager;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Select
{
    public class Play : PublicButton
    {
        public Transform loading;

        // Start is called before the first frame update
        private void Start()
        {
            thisButton.onClick.AddListener(() => { StartCoroutine(LoadEdit()); });
        }

        private IEnumerator LoadEdit()
        {
            loading.gameObject.SetActive(true);
            yield return new WaitForSeconds(.1f);
            GlobalData.Instance.chartEditData = JsonConvert.DeserializeObject<ChartData>(
                File.ReadAllText(
                    new Uri($"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json").LocalPath,Encoding.UTF8));
            
            BPMManager.UpdateInfo(GlobalData.Instance.chartEditData.bpmList);
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single).completed += Play_completed;
        }

        private void Play_completed(AsyncOperation obj)
        {
            GlobalData.Instance.StartEdit();
        }
    }
}