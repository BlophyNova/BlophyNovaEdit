using System;
using System.Collections;
using System.Collections.Generic;
using Data.ChartData;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class Alert : PublicButton
{
    public TMP_Text content;
    static List<string> textList=new();
    static List<Action> actions = new();
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            DisableAlert();
        });
    }

    public static void EnableAlert( string text, Action action=null)
    {
        textList.Add(text);
        actions.Add(action);
        InstWindow(text);
    }

    private static void InstWindow(string text)
    {
        if (textList.Count == 1)
        {
            Alert a = Instantiate(Scenes.DontDestoryOnLoad.GlobalData.Instance.alert);
            a.content.text = text;
        }
    }

    static void EnableAlertWithoutAdd2List(string text)
    {
        InstWindow(text);
    }

    void DisableAlert()
    {
        actions[0]?.Invoke();
        textList.RemoveAt(0);
        actions.RemoveAt(0);
        if (textList.Count != 0)
        {
            EnableAlertWithoutAdd2List(textList[0]);
        }
        Destroy(gameObject);
    }
}
