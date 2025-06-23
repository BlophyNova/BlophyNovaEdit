using System;
using System.Collections.Generic;
using CustomSystem.Steps;
using Data.ChartEdit;
using Data.Interface;
using Form.EventEdit;
using Form.NoteEdit;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UnityEngine.EventSystems;
using Event = UnityEngine.Event;

namespace Scenes.Edit
{
    public class LengthAdjustment : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public bool isStart;
        //public NoteEdit noteEdit;
        public NoteEditItem noteEditItem;

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log($"Start或者End正在拖动中");
            
            noteEditItem.ThisNoteEdit.FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine,
                out RectTransform nearEventVerticalLine);
            transform.localPosition = new(transform.localPosition.x,nearBeatLine.transform.localPosition.y-noteEditItem.transform.localPosition.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            noteEditItem.ThisNoteEdit.FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine,
                out RectTransform nearEventVerticalLine);
            Note originNote = new(noteEditItem.thisNoteData);
            Note targetNote = noteEditItem.thisNoteData;
            
            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;
            void Undo()
            {
                if (isStart)
                {
                    targetNote.HitBeats = new(originNote.HitBeats);
                }
                targetNote.holdBeats = new(originNote.holdBeats);
            }

            void Redo()
            {
                if (isStart)
                {
                    BPM endBpm = targetNote.EndBeats;
                    targetNote.HitBeats = new(nearBeatLine.thisBPM);
                    targetNote.holdBeats = new BPM(endBpm) - new BPM(targetNote.HitBeats);
                }
                else
                {
                    targetNote.holdBeats = new BPM(nearBeatLine.thisBPM)-new BPM(targetNote.HitBeats);
                }
            }
        }        
        public void Finally()
        {
            GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1),
                new List<Type> { typeof(NoteEdit) });
            GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1), null);
        }
    }
}
