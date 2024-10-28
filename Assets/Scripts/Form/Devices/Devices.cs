using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fleck;
public class Devices : LabelWindowContent
{
    public TMP_Text connectionInfo;
    WebSocketServer server;
    List<IWebSocketConnection> webSocketConnections;
    private void Start()
    {
        server = new(LogCenter.Log("ws://0.0.0.0:1286"));
        server.Start(socket=>
        {
            //webSocketConnections.Add(socket);
            socket.OnOpen = () => LogCenter.Log("Open!");
            socket.OnClose = () => LogCenter.Log("Close!");
            socket.OnMessage = message => LogCenter.Log(message);
            socket.OnPing = content => socket.SendPong(content);
        });
        //IWebSocketConnection socket = webSocketConnections[^1];
    }
}
