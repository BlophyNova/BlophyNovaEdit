using System.Collections;
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
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }
    }
}