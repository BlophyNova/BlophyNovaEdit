using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

[Flags]
public enum LabelWindowContentType
{
    BPMList = 1,//1<<0
    ChartPreview = 2,//1<<1
    NoteEdit = 4,
    EventEdit = 8,
    Toolbar = 16,
    Menubar = 32,
    PropertyEdit = 64,//1<<6
    Kawaii = 128,
    ProgressBar = 256,
    ATimeLine = 512,
    DebugText = 1024,
    NotePropertyEdit = 2048,
    AssociateLabelWindow = 4096,
    BoxList = 8192,
    RuntimeInspector = 16384,
    RuntimeHierarchy = 32768,//1<<15
    VisualEase = 65536,//1<<16
    SpecialThanks = 1<<17,//131072
    Devices = 1<<18//262144
}
