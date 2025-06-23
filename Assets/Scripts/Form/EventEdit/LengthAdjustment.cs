using System;
using System.Collections.Generic;
using CustomSystem.Steps;
using Data.Interface;
using Form.NoteEdit;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UnityEngine.EventSystems;
using Event = Data.ChartEdit.Event;

namespace Form.EventEdit
{
    public class LengthAdjustment : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public bool isStart;
        public EventEdit eventEdit;
        public EventEditItem eventEditItem;

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log($"Start或者End正在拖动中");
            
            eventEdit.FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine,
                out EventVerticalLine nearEventVerticalLine);
            transform.localPosition = new(transform.localPosition.x,nearBeatLine.transform.localPosition.y-eventEditItem.transform.localPosition.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            eventEdit.FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine,
                out EventVerticalLine nearEventVerticalLine);
            Event originEvent = new(eventEditItem.@event);
            Event targetEvent = eventEditItem.@event;
            
            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;
            void Undo()
            {
                if (isStart)
                {
                    targetEvent.startBeats = new(originEvent.startBeats);
                }
                else
                {
                    targetEvent.endBeats = new(originEvent.endBeats);
                }
            }

            void Redo()
            {
                if (isStart)
                {
                    targetEvent.startBeats = new(nearBeatLine.thisBPM);
                }
                else
                {
                    targetEvent.endBeats = new(nearBeatLine.thisBPM);
                }
            }
        }        
        public void Finally()
        {
            GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1),
                new List<Type> { typeof(EventEdit) });
            GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1), null);
        }
    }
}
