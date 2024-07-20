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
    Action action;
    static List<string> textList=new();
    static List<Transform> parentList = new();
    static bool useList=true;
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            DisableAlert();
        });
    }

    public static void EnableAlert(Transform parent, string text, Action action=null)
    {
        if (useList)
        {
            textList.Add(text);
            parentList.Add(parent);
        }
        if (textList.Count == 1)
        {
            Alert a = Instantiate(Scenes.DontDestoryOnLoad.GlobalData.Instance.alert, parent);
            a.content.text = text;
            a.action = action;
        }
    }

    void DisableAlert()
    {
        action?.Invoke();
        textList.RemoveAt(0);
        parentList.RemoveAt(0);
        if (textList.Count != 0)
        {
            useList = false;
            EnableAlert(parentList[0], textList[0]);
        }
        Destroy(gameObject);
    }
}
