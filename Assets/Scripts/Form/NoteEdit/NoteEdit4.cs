using Controller;
using Data.ChartData;
using Data.Interface;
using Form.PropertyEdit;
using Log;
using Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilityCode.ChartTool;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
using static UtilityCode.ChartTool.ChartTool;

namespace Form.NoteEdit
{
    //这里放所有的刷新方法
    public partial class NoteEdit
    {
        private Scenes.Edit.NoteEdit GetNoteType(Note item)
        {
            return item.noteType switch
            {
                NoteType.Tap => GlobalData.Instance.tapEditPrefab,
                NoteType.Hold => GlobalData.Instance.holdEditPrefab,
                NoteType.Drag => GlobalData.Instance.dragEditPrefab,
                NoteType.Flick => GlobalData.Instance.flickEditPrefab,
                NoteType.Point => GlobalData.Instance.pointEditPrefab,
                NoteType.FullFlickPink => GlobalData.Instance.fullFlickEditPrefab,
                NoteType.FullFlickBlue => GlobalData.Instance.fullFlickEditPrefab,
                _ => throw new Exception("滴滴~滴滴~错误~找不到音符拉~")
            };
        }
    }
}
