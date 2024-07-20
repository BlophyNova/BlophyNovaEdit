using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Text = Data.ChartData.Text;

public class CreateChart : MonoBehaviour
{
    public GameObject alertContent;
    public GameObject songNameInputField;
    public GameObject songLocationInputField;
    public GameObject illustrationLocationInputField;
    private string songName;
    private string songLocation;
    private string illustrationLocation;

    private string chartLocation;

    private void creatChart()
    {
        if (songName != "" & songLocation != "" & illustrationLocation != "")
        {
            chartLocation = Application.streamingAssetsPath + "/" + songName;
            Debug.Log(chartLocation);
            if (!Directory.Exists(chartLocation))
            {
                if (File.Exists(songLocation) & File.Exists(illustrationLocation))
                {
                    Directory.CreateDirectory(chartLocation);
                    Directory.CreateDirectory(chartLocation + "/Background");
                    Directory.CreateDirectory(chartLocation + "/ChartFile");
                    Directory.CreateDirectory(chartLocation + "/Music");
                    System.IO.File.Copy(songLocation, chartLocation + "/Music/Music.mp3");
                    System.IO.File.Copy(illustrationLocation, chartLocation + "/Background/BG.png");
                }
                else
                {
                    alertContent.GetComponent<Alert>().EnableAlert("您填写的音乐或曲绘不存在！");
                }
            }
            else
            {
                alertContent.GetComponent<Alert>().EnableAlert("已存在同歌名谱面！");
            }
        }
        else
        {
            alertContent.GetComponent<Alert>().EnableAlert("输入内容不能为空！");
        }
    }
    
    public void onClick()
    {
        songName = songNameInputField.GetComponent<TMP_InputField>().text;
        songLocation = songLocationInputField.GetComponent<TMP_InputField>().text;
        illustrationLocation = illustrationLocationInputField.GetComponent<TMP_InputField>().text;
        creatChart();
    }
}
