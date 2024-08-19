using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : PublicButton
{
    // Start is called before the first frame update
    void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single).completed += (asyncOperation) =>
            {
                //GlobalData.Instance.chartData.boxes = ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
            };
        });
        //chartData.boxes= ChartTool.ConvertChartEdit2ChartData(chartEditData.boxes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
