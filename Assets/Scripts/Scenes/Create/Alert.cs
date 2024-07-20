using System;
using System.Collections;
using System.Collections.Generic;
using Data.ChartData;
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public GameObject alert;
    public TMP_Text content;

    private void Start()
    {
        content = GetComponent<TMP_Text>();
    }

    public void EnableAlert(string text)
    {
        content.text = text;
        alert.SetActive(true);
    }

    public void DisableAlert()
    {
        alert.SetActive(false);
    }
}
