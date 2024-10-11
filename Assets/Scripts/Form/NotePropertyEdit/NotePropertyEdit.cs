using Controller;
using Data.ChartEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.GameUtility;
using static Form.NotePropertyEdit.NotePropertyEdit;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;

namespace Form.NotePropertyEdit
{
    public class NotePropertyEdit : LabelWindowContent
    {
        public Event eventMemory;

        public EventEditItem @event;
        public Data.ChartEdit.Note note;

        public TMP_Dropdown noteType; //note
        public Toggle commonEffect; //note
        public Toggle ripple; //note
        public TMP_InputField startTime; //both
        public TMP_InputField endTime; //event、hold only
        public TMP_InputField startValue; //event
        public TMP_InputField endValue; //event
        public TMP_InputField postionX; //note
        public Toggle isClockwise; //note
        public TMP_Dropdown ease; //event

        public delegate void OnNoteValueChanged();
        public event OnNoteValueChanged onNoteValueChanged=()=> { };
        public delegate void OnEventValueChanged();
        public event OnEventValueChanged onEventValueChanged=()=> { };
        private void Start()
        {
            noteType.onValueChanged.AddListener((value) => NoteTypeChanged(value));
            commonEffect.onValueChanged.AddListener((value) => CommonEffectChanged(value));
            ripple.onValueChanged.AddListener((value) => RippleChanged(value));
            startValue.onEndEdit.AddListener((value) => StartValueChanged(value));
            endValue.onEndEdit.AddListener((value) => EndValueChanged(value));
            postionX.onEndEdit.AddListener((value) => PositionXChanged(value));
            isClockwise.onValueChanged.AddListener((value) => IsClockwiseChanged(value));
            ease.onValueChanged.AddListener((value) => EaseChanged(value));
        }

        void RefreshChartPreviewAndChartEditCanvas()
        {

            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType ==
                LabelWindowContentType.NoteEdit)
            {
                RefreshNotes();
                onNoteValueChanged();
            }

            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType ==
                LabelWindowContentType.EventEdit)
            {
                RefreshEvents();
                onEventValueChanged();
            }

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>((interfaceMethod) => interfaceMethod.RefreshUI());
        }

        public void RefreshNotes()
        {
            Form.NoteEdit.NoteEdit noteEdit = (Form.NoteEdit.NoteEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;

            GlobalData.Instance.chartData.boxes[noteEdit.currentBoxID]=ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[noteEdit.currentBoxID]);
            noteEdit.RefreshNotes(-1, -1);
            SpeckleManager.Instance.allLineNoteControllers.Clear();
            GameController.Instance.RefreshChartPreview();
            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>((interfaceMethod) => interfaceMethod.RefreshUI());
        }

        public void RefreshEvents()
        {
            EventEdit eventEdit = (EventEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            //@event.eventType
            List<Data.ChartEdit.Event> editBoxEvent = @event.eventType switch
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
                _ => GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].lines[0].speed,
            };
            if (@event.eventType != EventType.Speed)
                ChartTool.ForeachBoxEvents(editBoxEvent, chartDataBoxEvent);
            else
            {
                for (int i = 0; i < GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].lines.Count; i++)
                {
                    Data.ChartData.Line item = GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].lines[i];
                    List<Data.ChartEdit.Event> filledVoid = GameUtility.FillVoid(editBoxEvent);
                    item.speed = new();
                    ChartTool.ForeachBoxEvents(filledVoid, item.speed);
                    item.career = new() { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                    item.career.keys = GameUtility.CalculatedSpeedCurve(item.speed.ToArray()).ToArray();
                    item.far = new() { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                    item.far.keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
                }
                GameController.Instance.RefreshChartPreview();
            }

            if (@event.eventType == EventType.ScaleX)
            {
                List<Event> scaleY = GlobalData.Instance.chartEditData.boxes[eventEdit.currentBoxID].boxEvents.scaleY;
                for (int i = 0; i < scaleY.Count; i++)
                {
                    Debug.Log($@"scaleY中第{i}个事件的结果为：{eventMemory.Equals(scaleY[i])}");
                    if (eventMemory.Equals(scaleY[i]))
                    {
                        scaleY[i] = new(@event.@event);
                    }
                }
            }
            eventEdit.RefreshEvents(-1);
            //GlobalData.Refresh<IRefreshUI>((interfaceMethod) => interfaceMethod.RefreshUI());
        }

        void NoteHitBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log($"音符HitBeats从{note.HitBeats.integer}:{note.HitBeats.molecule}/{note.HitBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM hitBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                note.HitBeats = hitBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        void NoteEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log($"音符EndBeats从{note.EndBeats.integer}:{note.EndBeats.molecule}/{note.EndBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM endBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                BPM hitBeats = new(note.HitBeats);
                BPM holdBeats = endBeats - hitBeats;
                note.holdBeats = holdBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        void EventStartBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log($"事件StartBeats从{@event.@event.startBeats.integer}:{@event.@event.startBeats.molecule}/{@event.@event.startBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM startBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                @event.@event.startBeats = startBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        void EventEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log($"事件EndBeats从{@event.@event.endBeats.integer}:{@event.@event.endBeats.molecule}/{@event.@event.endBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM endBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                @event.@event.endBeats = endBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        void EaseChanged(int value)
        {
            LogCenter.Log($"事件Ease从{@event.@event.curve.easeType}变更为{GlobalData.Instance.easeData[value].easeType}");
            @event.@event.curve = GlobalData.Instance.easeData[value];
            RefreshChartPreviewAndChartEditCanvas();
        }

        void IsClockwiseChanged(bool value)
        {
            LogCenter.Log($"音符IsClockWise从{note.isClockwise}变更为{value}");
            note.isClockwise = value;
            RefreshChartPreviewAndChartEditCanvas();
        }

        void PositionXChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            LogCenter.Log($"音符PositionX从{note.positionX}变更为{value}");
            note.positionX = result;
            RefreshChartPreviewAndChartEditCanvas();
        }

        void EndValueChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            LogCenter.Log($"事件EndValue从{@event.@event.endValue}变更为{result}");
            @event.@event.endValue = result;
            RefreshChartPreviewAndChartEditCanvas();
        }

        void StartValueChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            LogCenter.Log($"事件StartValue从{@event.@event.startValue}变更为{result}");
            @event.@event.startValue = result;
            RefreshChartPreviewAndChartEditCanvas();
        }

        void RippleChanged(bool value)
        {
            note.effect = value switch
            {
                true => note.effect | Data.ChartData.NoteEffect.Ripple,
                false => note.effect ^ Data.ChartData.NoteEffect.Ripple
            };
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"成功{value switch{true=>"添加",false=>"取消"}}方框波纹特效");
        }

        void CommonEffectChanged(bool value)
        {
            note.effect = value switch
            {
                true => note.effect | Data.ChartData.NoteEffect.CommonEffect,
                false => note.effect ^ Data.ChartData.NoteEffect.CommonEffect
            };
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"成功{value switch{true=>"添加",false=>"取消"}}普通打击特效");
        }

        void NoteTypeChanged(int value)
        {
            LogCenter.Log($"音符类型从{note.noteType}变更为{(Data.ChartData.NoteType)value}");
            note.noteType = (Data.ChartData.NoteType)value;
            RefreshChartPreviewAndChartEditCanvas();
        }

        public void SelectedNote(Scenes.Edit.NoteEdit note)
        {
            SelectedNote(note.thisNoteData);
        }
        public void SelectedNote(Note note)
        {
            this.note = note;
            noteType.interactable = true;
            commonEffect.interactable = true;
            ripple.interactable = true;
            startTime.interactable = true;
            endTime.interactable = this.note.noteType == Data.ChartData.NoteType.Hold;
            startValue.interactable = false;
            endValue.interactable = false;
            postionX.interactable = true;
            isClockwise.interactable = true;
            ease.interactable = false;
            startTime.onEndEdit.RemoveAllListeners();
            endTime.onEndEdit.RemoveAllListeners();
            startTime.onEndEdit.AddListener((value) => NoteHitBeatsChanged(value));
            endTime.onEndEdit.AddListener((value) => NoteEndBeatsChanged(value));

            noteType.SetValueWithoutNotify((int)this.note.noteType);
            commonEffect.SetIsOnWithoutNotify(
                this.note.effect.HasFlag(Data.ChartData.NoteEffect.CommonEffect));
            ripple.SetIsOnWithoutNotify(this.note.effect.HasFlag(Data.ChartData.NoteEffect.Ripple));
            startTime.SetTextWithoutNotify(
                $"{this.note.HitBeats.integer}:{this.note.HitBeats.molecule}/{this.note.HitBeats.denominator}");
            if (this.note.noteType == Data.ChartData.NoteType.Hold)
            {
                endTime.SetTextWithoutNotify(
                    $"{this.note.EndBeats.integer}:{this.note.EndBeats.molecule}/{this.note.EndBeats.denominator}");
            }

            postionX.SetTextWithoutNotify($"{this.note.positionX}");
            isClockwise.SetIsOnWithoutNotify(this.note.isClockwise);
            LogCenter.Log($"音符属性编辑控件接收一个音符");
        }
        public void SelectedNote(EventEditItem @event)
        {
            this.@event = @event;
            eventMemory = new(@event.@event);
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
            startTime.onEndEdit.AddListener((value) => EventStartBeatsChanged(value));
            endTime.onEndEdit.AddListener((value) => EventEndBeatsChanged(value));

            startTime.SetTextWithoutNotify(
                $"{this.@event.@event.startBeats.integer}:{this.@event.@event.startBeats.molecule}/{this.@event.@event.startBeats.denominator}");
            endTime.SetTextWithoutNotify(
                $"{this.@event.@event.endBeats.integer}:{this.@event.@event.endBeats.molecule}/{this.@event.@event.endBeats.denominator}");
            startValue.SetTextWithoutNotify($"{this.@event.@event.startValue}");
            endValue.SetTextWithoutNotify($"{this.@event.@event.endValue}");
            ease.SetValueWithoutNotify(GlobalData.Instance.easeData.FindIndex((m) => m.Equals(this.@event.@event.curve)));

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            LogCenter.Log($"音符属性编辑控件接收一个事件");
        }

    }
}