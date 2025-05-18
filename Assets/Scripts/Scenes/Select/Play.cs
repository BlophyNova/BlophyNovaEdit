using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.ChartEdit;
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
                    $"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json"));
            List<BPM> bpmList = GlobalData.Instance.chartEditData.bpmList;
            bpmList[0].lastBpmEndSeconds = 0;
            for (int i = 1; i < bpmList.Count; i++)
            {
                bpmList[i].lastBpmEndSeconds = bpmList[i - 1].lastBpmEndSeconds +
                                               60m / (decimal)bpmList[i - 1].currentBPM *
                                               ((decimal)bpmList[i].ThisStartBPM -
                                                (decimal)bpmList[i - 1].ThisStartBPM);
                bpmList[i].perSecond = bpmList[i].perSecond / 60;
            }
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single).completed += Play_completed;
        }

        private void Play_completed(AsyncOperation obj)
        {
            GlobalData.Instance.StartEdit();
        }
    }
}