using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : PublicButton
{
    public Transform loading;
    // Start is called before the first frame update
    void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoadEdit());

        });
    }
    IEnumerator LoadEdit()
    {
        loading.gameObject.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GlobalData.Instance.chartEditData = JsonConvert.DeserializeObject<Data.ChartEdit.ChartData>(File.ReadAllText($"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json"));
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}
