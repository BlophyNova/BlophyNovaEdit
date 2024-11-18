using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Fleck;
using WebSocketServer = Fleck.WebSocketServer;
using Unity.VisualScripting;
using System.Linq;
using Manager;
public class Devices : LabelWindowContent
{
    public TMP_Text connectionInfo;
    private void Start()
    {
        IThenStartup startup = (IThenStartup)new GameObject().AddComponent(Type.GetType("HuaWaterED.ThenStartup"));
        if (startup == null)
        {
            LogCenter.Log("未找到网络模块，此版本无互联网访问能力", "Internet");
            Alert.EnableAlert("未找到网络模块，此版本无互联网访问能力");
            return;
        }
        startup.ServerInit();
        startup.onDeviceCountChanged += deviceCount =>
        {
            connectionInfo.text = $"已连接{deviceCount}台设备!";
            if (deviceCount > 0)
            {
                AssetManager.Instance.musicPlayer.volume = 0;
            }
            else
            {
                AssetManager.Instance.musicPlayer.volume = 1;
            }
        };
    }
}
