using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Flags]
public enum LabelWindowContentType
{
    BPMList=1,
    ChartPreview=2,
    NoteEdit=4,
    EventEdit=8,
    Toolbar=16,
    Menubar=32,
    PropertyEdit=64,
    Kawaii=128,
    ProgressBar=256,
    ATimeLine=512,
    DebugText=1024,
    NotePropertyEdit=2048,
    AssociateLabelWindow=4096,
    BoxList=8192,
    RuntimeInspector=16384,
    RuntimeHierarchy=32768
}
