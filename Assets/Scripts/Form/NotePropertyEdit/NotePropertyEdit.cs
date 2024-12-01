using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controller;
using Data.ChartData;
using Data.ChartEdit;
using Data.Enumerate;
using Data.Interface;
using Form.EventEdit;
using Form.LabelWindow;
using Log;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.ChartTool;
using UtilityCode.GameUtility;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Line = Data.ChartData.Line;
using Note = Data.ChartEdit.Note;

namespace Form.NotePropertyEdit
{
    public partial class NotePropertyEdit : LabelWindowContent
    {
        public delegate void OnEventValueChanged();

        public delegate void OnNoteValueChanged();

        public Event eventMemory;

        public EventEditItem @event;
        public Note note;

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

        private void Start()
        {
            noteType.onValueChanged.AddListener(NoteTypeChanged);
            commonEffect.onValueChanged.AddListener(CommonEffectChanged);
            ripple.onValueChanged.AddListener(RippleChanged);
            startValue.onEndEdit.AddListener(StartValueChanged);
            endValue.onEndEdit.AddListener(EndValueChanged);
            postionX.onEndEdit.AddListener(PositionXChanged);
            isClockwise.onValueChanged.AddListener(IsClockwiseChanged);
            ease.onValueChanged.AddListener(EaseChanged);
        }

        public event OnNoteValueChanged onNoteValueChanged = () => { };
        public event OnEventValueChanged onEventValueChanged = () => { };

        private void RefreshChartPreviewAndChartEditCanvas()
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
                    Debug.Log($@"scaleY中第{i}个事件的结果为：{eventMemory.Equals(scaleY[i])}");
                    if (eventMemory.Equals(scaleY[i]))
                    {
                        scaleY[i] = new Event(@event.@event);
                    }
                }

                ChartTool.ForeachBoxEvents(scaleY, GlobalData.Instance.chartData.boxes[eventEdit.currentBoxID].boxEvents
                    .scaleY);
            }

            eventEdit.RefreshEvents(-1);
            //GlobalData.Refresh<IRefreshUI>((interfaceMethod) => interfaceMethod.RefreshUI());
        }

        private void NoteHitBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log(
                    $"音符HitBeats从{note.HitBeats.integer}:{note.HitBeats.molecule}/{note.HitBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM hitBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                note.HitBeats = hitBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void NoteEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log(
                    $"音符EndBeats从{note.EndBeats.integer}:{note.EndBeats.molecule}/{note.EndBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM endBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                BPM hitBeats = new(note.HitBeats);
                BPM holdBeats = endBeats - hitBeats;
                note.holdBeats = holdBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void EventStartBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log(
                    $"事件StartBeats从{@event.@event.startBeats.integer}:{@event.@event.startBeats.molecule}/{@event.@event.startBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM startBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                @event.@event.startBeats = startBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void EventEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (match.Success)
            {
                LogCenter.Log(
                    $"事件EndBeats从{@event.@event.endBeats.integer}:{@event.@event.endBeats.molecule}/{@event.@event.endBeats.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
                BPM endBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value));
                @event.@event.endBeats = endBeats;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void EaseChanged(int value)
        {
            LogCenter.Log($"事件Ease从{@event.@event.Curve.easeType}变更为{GlobalData.Instance.easeDatas[value].easeType}");
            //@event.@event.Curve = GlobalData.Instance.easeData[value];
            @event.@event.curveIndex = value;
            RefreshChartPreviewAndChartEditCanvas();
        }

        private void IsClockwiseChanged(bool value)
        {
            LogCenter.Log($"音符IsClockWise从{note.isClockwise}变更为{value}");
            note.isClockwise = value;
            RefreshChartPreviewAndChartEditCanvas();
        }

        private void PositionXChanged(string value)
        {
            if (!float.TryParse(value, out float result))
            {
                return;
            }

            LogCenter.Log($"音符PositionX从{note.positionX}变更为{value}");
            note.positionX = result;
            RefreshChartPreviewAndChartEditCanvas();
        }

        private void EndValueChanged(string value)
        {
            if (!float.TryParse(value, out float result))
            {
                return;
            }

            LogCenter.Log($"事件EndValue从{@event.@event.endValue}变更为{result}");
            @event.@event.endValue = result;
            RefreshChartPreviewAndChartEditCanvas();
        }

        private void StartValueChanged(string value)
        {
            if (!float.TryParse(value, out float result))
            {
                return;
            }

            LogCenter.Log($"事件StartValue从{@event.@event.startValue}变更为{result}");
            @event.@event.startValue = result;
            RefreshChartPreviewAndChartEditCanvas();
        }

        private void RippleChanged(bool value)
        {
            note.effect = value switch
            {
                true => note.effect | NoteEffect.Ripple,
                false => note.effect ^ NoteEffect.Ripple
            };
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"成功{value switch { true => "添加", false => "取消" }}方框波纹特效");
        }

        private void CommonEffectChanged(bool value)
        {
            note.effect = value switch
            {
                true => note.effect | NoteEffect.CommonEffect,
                false => note.effect ^ NoteEffect.CommonEffect
            };
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"成功{value switch { true => "添加", false => "取消" }}普通打击特效");
        }

        private void NoteTypeChanged(int value)
        {
            int sourceValue = (int)note.noteType;
            int targetValue = value;
            LogCenter.Log($"音符类型从{note.noteType}变更为{(NoteType)value}");
            Steps.Instance.Add(UndoChange, RedoChange);
            RedoChange();
            return;

            void UndoChange()
            {
                note.noteType = (NoteType)sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }

            void RedoChange()
            {
                note.noteType = (NoteType)targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
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
            LogCenter.Log("音符属性编辑控件接收一个音符");
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
            LogCenter.Log("音符属性编辑控件接收一个事件");
        }
    }
}