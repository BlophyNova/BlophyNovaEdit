using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Fleck;
public class Devices : LabelWindowContent
{
    public string logRole = "ChartPlayer";
    
    public TMP_Text connectionInfo;
    WebSocketServer server;
    List<IWebSocketConnection> webSocketConnections=new();
    delegate void UpdateConnectionInfo();
    UpdateConnectionInfo updateConnectionInfo= () => { };
    private void Start()
    {
        webSocketConnections.Clear();
        connectionInfo.text = $"已连接{webSocketConnections.Count}台设备!";
        server = new(LogCenter.Log("ws://0.0.0.0:1286",logRole));
        server.Start(socket=>
        {

            socket.OnOpen = () => DeviceConnectOpen(socket);
            socket.OnClose = () => DeviceConnectClose(socket);
            socket.OnMessage = DeviceOnMessage;
            socket.OnPing = content => DeviceOnPing(content, socket);
        });
        StartCoroutine(CallLoop());
        //IWebSocketConnection socket = webSocketConnections[^1];
    }

    IEnumerator CallLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            updateConnectionInfo();
        }
    }
    private void OnApplicationQuit()
    {
        server.Dispose();
        webSocketConnections.Clear();
    }
    void DeviceConnectOpen(IWebSocketConnection socket)
    {
        webSocketConnections.Add(socket);
        LogCenter.Log($"Open: {webSocketConnections[^1].ConnectionInfo.ClientIpAddress}:{webSocketConnections[^1].ConnectionInfo.ClientPort} Connected!", logRole);
        //connectionInfo.text = $"已连接{webSocketConnections.Count}台设备!";
        updateConnectionInfo = () => connectionInfo.text = $"已连接{webSocketConnections.Count}台设备!";
    }

    void DeviceConnectClose(IWebSocketConnection socket)
    {
        
        LogCenter.Log($"Close: {webSocketConnections[^1].ConnectionInfo.ClientIpAddress}:{webSocketConnections[^1].ConnectionInfo.ClientPort} Disconnected!", logRole);
        webSocketConnections.Remove(socket);
        //connectionInfo.text = $"已连接{webSocketConnections.Count}台设备!";
        updateConnectionInfo = () => connectionInfo.text = $"已连接{webSocketConnections.Count}台设备!";
    }

    void DeviceOnMessage(string content)
    {
        LogCenter.Log(content, logRole);
    }

    void DeviceOnPing(byte[] content,IWebSocketConnection socket)
    {
        LogCenter.Log($"收到Ping消息：{content.Length}", logRole);
        socket.SendPong(content);
        LogCenter.Log($"回复Pong消息：{content.Length}", logRole);
    }
}
