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
    static List<Transform> parentList = new();
    static List<Action> actions = new();
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            DisableAlert();
        });
    }

    public static void EnableAlert(Transform parent, string text, Action action=null)
    {
        textList.Add(text);
        parentList.Add(parent);
        actions.Add(action);
        InstWindow(parent, text);
    }

    private static void InstWindow(Transform parent, string text)
    {
        if (textList.Count == 1)
        {
            Alert a = Instantiate(Scenes.DontDestoryOnLoad.GlobalData.Instance.alert, parent);
            a.content.text = text;
        }
    }

    static void EnableAlertWithoutAdd2List(Transform parent, string text)
    {
        InstWindow(parent, text);
    }

    void DisableAlert()
    {
        actions[0]?.Invoke();
        textList.RemoveAt(0);
        parentList.RemoveAt(0);
        actions.RemoveAt(0);
        if (textList.Count != 0)
        {
            EnableAlertWithoutAdd2List(parentList[0], textList[0]);
        }
        Destroy(gameObject);
    }
}
