using Controller;
using Data.ChartData;
using Data.Enumerate;
using Data.Interface;
using Form.EventEdit;
using Manager;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.ChartTool;
using UtilityCode.GameUtility;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Line = Data.ChartData.Line;
using Note = Data.ChartEdit.Note;
namespace Form.NotePropertyEdit
{
    public partial class NotePropertyEdit
    {

        private void RefreshChartPreviewAndChartEditCanvas()
        {
            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType ==
                LabelWindowContentType.NoteEdit)
            {
                RefreshNotes();
                SelectedNote(note);
                onNoteValueChanged();
            }

            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType ==
                LabelWindowContentType.EventEdit)
            {
                RefreshEvents();
                SelectedNote(@event);
                onEventValueChanged();
            }

            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI());
        }

        public void RefreshNotes()
        {
            NoteEdit.NoteEdit noteEdit =
                (NoteEdit.NoteEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;

            GlobalData.Instance.chartData.boxes[noteEdit.currentBoxID] =
                ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[noteEdit.currentBoxID]);
            noteEdit.RefreshNotes(-1, -1);
            SpeckleManager.Instance.allLineNoteControllers.Clear();
            GameController.Instance.RefreshChartPreview();
            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI());
        }

        public void RefreshEvents()
        {
            EventEdit.EventEdit eventEdit =
                (EventEdit.EventEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            //@event.eventType
            List<Event> editBoxEvent = @event.eventType switch
            {
                EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.scaleY,
                EventType.MoveX => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.moveY,
                EventType.CenterX => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.centerY,
                EventType.Alpha => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.lineAlpha,
                EventType.Rotate => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID]
                    .boxEvents.rotate,
                _ => GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID].boxEvents.speed
            };
            List<Data.ChartData.Event> chartDataBoxEvent = @event.eventType switch
            {
                EventType.ScaleX => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .scaleX,
                EventType.ScaleY => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .scaleY,
                EventType.MoveX => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .moveX,
                EventType.MoveY => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .moveY,
                EventType.CenterX => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID]
                    .boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID]
                    .boxEvents.centerY,
                EventType.Alpha => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .alpha,
                EventType.LineAlpha => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID]
                    .boxEvents.lineAlpha,
                EventType.Rotate => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .rotate,
                _ => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].lines[0].speed
            };
            if (@event.eventType != EventType.Speed)
            {
                ChartTool.ForeachBoxEvents(editBoxEvent, chartDataBoxEvent);
            }
            else
            {
                for (int i = 0; i < GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].lines.Count; i++)
                {
                    Line line = GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].lines[i];
                    List<Event> filledVoid = GameUtility.FillVoid(editBoxEvent);
                    line.speed = new List<Data.ChartData.Event>();
                    ChartTool.ForeachBoxEvents(filledVoid, line.speed);
                    line.career = new AnimationCurve
                    { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                    line.career.keys = GameUtility.CalculatedSpeedCurve(line.speed.ToArray()).ToArray();
                    line.far = new AnimationCurve
                    { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                    line.far.keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
                }

                //SpeckleManager.Instance.allLineNoteControllers.Clear();
                //GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
                GlobalData.Instance.chartData.boxes =
                    ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
                SpeckleManager.Instance.allLineNoteControllers.Clear();
                GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
                GameController.Instance.RefreshChartPreview();
            }

            if (@event.eventType == EventType.ScaleX)
            {
                List<Event> scaleY = GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID].boxEvents.scaleY;
                for (int i = 0; i < scaleY.Count; i++)
                {
                    if (eventMemory.Equals(scaleY[i]))
                    {
                        scaleY[i] = new Event(@event.@event);
                    }
                }

                ChartTool.ForeachBoxEvents(scaleY, GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .scaleY);
            }

            eventEdit.RefreshEditEvents(-1);
            //GlobalData.Refresh<IRefreshUI>((interfaceMethod) => interfaceMethod.RefreshUI());
        }

        public void SelectedNote(Note note)
        {
            this.note = note;
            noteType.interactable = true;
            commonEffect.interactable = true;
            ripple.interactable = true;
            startTime.interactable = true;
            endTime.interactable = this.note.noteType == NoteType.Hold;
            startValue.interactable = false;
            endValue.interactable = false;
            postionX.interactable = true;
            isClockwise.interactable = true;
            ease.interactable = false;
            startTime.onEndEdit.RemoveAllListeners();
            endTime.onEndEdit.RemoveAllListeners();
            startTime.onEndEdit.AddListener(value => NoteHitBeatsChanged(value));
            endTime.onEndEdit.AddListener(value => NoteEndBeatsChanged(value));

            noteType.SetValueWithoutNotify((int)this.note.noteType);
            commonEffect.SetIsOnWithoutNotify(
                this.note.effect.HasFlag(NoteEffect.CommonEffect));
            ripple.SetIsOnWithoutNotify(this.note.effect.HasFlag(NoteEffect.Ripple));
            startTime.SetTextWithoutNotify(
                $"{this.note.HitBeats.integer}:{this.note.HitBeats.molecule}/{this.note.HitBeats.denominator}");
            if (this.note.noteType == NoteType.Hold)
            {
                endTime.SetTextWithoutNotify(
                    $"{this.note.EndBeats.integer}:{this.note.EndBeats.molecule}/{this.note.EndBeats.denominator}");
            }

            postionX.SetTextWithoutNotify($"{this.note.positionX}");
            isClockwise.SetIsOnWithoutNotify(this.note.isClockwise);
        }

        public void SelectedNote(EventEditItem @event)
        {
            this.@event = @event;
            eventMemory = new Event(@event.@event);
            noteType.interactable = false;
            commonEffect.interactable = false;
            ripple.interactable = false;
            startTime.interactable = true;
            endTime.interactable = true;
            startValue.interactable = true;
            endValue.interactable = true;
            postionX.interactable = false;
            isClockwise.interactable = false;
            ease.interactable = true;
            startTime.onEndEdit.RemoveAllListeners();
            endTime.onEndEdit.RemoveAllListeners();
            startTime.onEndEdit.AddListener(value => EventStartBeatsChanged(value));
            endTime.onEndEdit.AddListener(value => EventEndBeatsChanged(value));

            startTime.SetTextWithoutNotify(
                $"{this.@event.@event.startBeats.integer}:{this.@event.@event.startBeats.molecule}/{this.@event.@event.startBeats.denominator}");
            endTime.SetTextWithoutNotify(
                $"{this.@event.@event.endBeats.integer}:{this.@event.@event.endBeats.molecule}/{this.@event.@event.endBeats.denominator}");
            startValue.SetTextWithoutNotify($"{this.@event.@event.startValue}");
            endValue.SetTextWithoutNotify($"{this.@event.@event.endValue}");
            ease.SetValueWithoutNotify(@event.@event.curveIndex);

            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
        }
        public void SelectedNote(Scenes.Edit.NoteEdit note)
        {
            SelectedNote(note.thisNoteData);
        }
    }
}