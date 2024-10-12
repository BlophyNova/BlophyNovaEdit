using Scenes.DontDestroyOnLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using EventType = Data.Enumerate.EventType;
using Controller;
using Data.ChartEdit;
using Manager;
using Data.ChartData;
using System.Linq;
using Scenes.Edit;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

public partial class EventEdit
{
    public delegate void OnEventDeleted(EventEditItem eventEditItem);
    public event OnEventDeleted onEventDeleted = eventEditItem => { };

    public delegate void OnEventRefreshed(List<EventEditItem> eventEditItems);
    public event OnEventRefreshed onEventRefreshed=eventEditItems => { };

    public List<EventEditItem> eventClipboard = new();
    public bool isCopy = false;
    void Start2()
    {
        onEventRefreshed += EventEdit_onEventRefreshed;
    }

    private void EventEdit_onEventRefreshed(List<EventEditItem> eventEditItems)
    {
        eventClipboard.Clear();
        foreach (var item in eventEditItems)
        {
            if (item.@event.IsSelected)
            {
                eventClipboard.Add(item);
            }
        }
    }
    void SelectBoxDown()
    {
        selectBox.isPressing = true;
        selectBox.transform.SetAsLastSibling();
        Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
    }
    void SelectBoxUp()
    {
        selectBox.isPressing = false;
        selectBox.transform.SetAsFirstSibling();
        Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
    }
    private void AddNewEvent2EventList(EventEditItem eventEditItem)
    {
        AddNewEvent2EventList(eventEditItem.@event,eventEditItem.eventType);
    }
    private void AddNewEvent2EventList(Data.ChartEdit.Event @event,EventType eventType,bool isPaste=false)
    {
        List<Data.ChartEdit.Event> events = eventType switch
        {
            EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
            EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
            EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
            EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
            EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
            EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
            EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
            EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
            EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
            EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
            _ => null
        };
        if (!isPaste)
        {
            @event.startValue = @event.endValue = events[^1].endValue;
            @event.curve = GlobalData.Instance.easeData[0];
        }
        events.Add(@event);
        Algorithm.BubbleSort(events, (a, b) =>//排序
        {
            if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
            {
                return 1;
            }
            else if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
            {
                return -1;
            }
            return 0;
        });
        List<Data.ChartData.Event> chartDataEvents = eventType switch
        {
            EventType.CenterX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerX,
            EventType.CenterY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerY,
            EventType.MoveX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveX,
            EventType.MoveY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveY,
            EventType.ScaleX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleX,
            EventType.ScaleY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY,
            EventType.Rotate => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.rotate,
            EventType.Alpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.alpha,
            EventType.LineAlpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.lineAlpha,
            _ => null
        };
        ChartTool.RefreshChartEventByChartEditEvent(chartDataEvents, @event);

        if (eventType == EventType.ScaleX&& !isPaste)//同步scaleY
        {
            List<Data.ChartEdit.Event> scaleYEvents = GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY;
            scaleYEvents.Add(new(@event));
            Algorithm.BubbleSort(scaleYEvents, (a, b) =>//排序
            {
                if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
                {
                    return 1;
                }
                else if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
                {
                    return -1;
                }
                return 0;
            });
            ChartTool.RefreshChartEventByChartEditEvent(GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY, new(@event));
            RefreshEvents(-1);
        }
        #region 以下代码为speed事件处理相关专属代码，没啥bug的情况下一个字都别改
        if (eventType != EventType.Speed) return;
        List<Data.ChartEdit.Event> filledVoid = GameUtility.FillVoid(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed);
        for (int i = 0; i < GlobalData.Instance.chartData.boxes[currentBoxID].lines.Count; i++)
        {
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed = new();


            ChartTool.ForeachBoxEvents(filledVoid, GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed);
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career = new() { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career.keys = GameUtility.CalculatedSpeedCurve(GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed.ToArray()).ToArray();


            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far = new() { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far.keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
        }
        #endregion

        //Debug.LogError("错误记忆");
    }


    private void WindowSizeChanged_EventEdit2()
    {
        eventLineRenderer.lineRendererTextureRect.sizeDelta=labelWindow.labelWindowRect.sizeDelta;
    }

    private void LabelWindow_onWindowMoved()
    {
        eventLineRenderer.lineRendererTextureRect.anchoredPosition = labelWindow.labelWindowRect.anchoredPosition;
    }

    private void LabelWindow_onWindowGetFocus()
    {
        LabelWindowsManager.Instance.lineRendererParent.transform.SetAsLastSibling();
    }

    private void LabelWindow_onWindowLostFocus()
    {
    }
    /// <summary>
    /// 裁剪Texture2D
    /// </summary>
    /// <param name="originalTexture"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <param name="originalWidth"></param>
    /// <param name="originalHeight"></param>
    /// <returns></returns>
    public static Texture2D ScaleTextureCutOut(Texture2D originalTexture, int offsetX, int offsetY, float originalWidth, float originalHeight)
    {
        Texture2D newTexture = new(Mathf.CeilToInt(originalWidth), Mathf.CeilToInt(originalHeight));
        int maxX = originalTexture.width - 1;
        int maxY = originalTexture.height - 1;
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                float targetX = x + offsetX;
                float targetY = y + offsetY;
                int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                int x2 = Mathf.Min(maxX, x1 + 1);
                int y2 = Mathf.Min(maxY, y1 + 1);

                float u = targetX - x1;
                float v = targetY - y1;
                float w1 = (1 - u) * (1 - v);
                float w2 = u * (1 - v);
                float w3 = (1 - u) * v;
                float w4 = u * v;
                Color color1 = originalTexture.GetPixel(x1, y1);
                Color color2 = originalTexture.GetPixel(x2, y1);
                Color color3 = originalTexture.GetPixel(x1, y2);
                Color color4 = originalTexture.GetPixel(x2, y2);
                Color color = new(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                                        );
                newTexture.SetPixel(x, y, color);
            }
        }
        newTexture.anisoLevel = 2;
        newTexture.Apply();
        return newTexture;
    }

    private void LabelItem_onLabelLostFocus()
    {
        eventLineRenderer.gameObject.SetActive(false);
    }

    private void LabelItem_onLabelGetFocus()
    {
        eventLineRenderer.gameObject.SetActive(true);
    }


    private void UndoNote()
    {
    }

    private void RedoNote()
    {
    }

    private void CopyEvent()
    {
        Debug.Log("复制事件");
        isCopy = true;
        AddEvent2EventClipboard();
    }

    private void PasteEvent()
    {
        Debug.Log("粘贴事件");
        FindNearBeatLineAndEventVerticalLine(out BeatLine beatLine, out EventVerticalLine verticalLine);
        BPM firstEventStartBeats = eventClipboard[0].@event.startBeats;
        for (var i = 0; i < eventClipboard.Count; i++)
        {
            var @event = eventClipboard[i];
            Data.ChartEdit.Event copyNewEvent = new(@event.@event);
            copyNewEvent.startBeats = new BPM(beatLine.thisBPM) +
                                      (new BPM(@event.@event.startBeats) - new BPM(firstEventStartBeats));
            copyNewEvent.endBeats = new BPM(beatLine.thisBPM) +
                                    (new BPM(@event.@event.endBeats) - new BPM(firstEventStartBeats));
            if (isCopy) copyNewEvent.IsSelected = false;
            //AddEventAndRefresh(copyNewEvent, currentBoxID);
            AddNewEvent2EventList(copyNewEvent, @event.eventType, true);
            //Debug.LogError("这里有问题");
        }

        if (!isCopy)
        {
            foreach (EventEditItem eventEditItem in eventClipboard)
            {
                DeleteEvent(eventEditItem);
                //Debug.LogError("这里有问题");
            }
        }
        
        LogCenter.Log($"成功{isCopy switch{true=>"复制",false=>"粘贴"}}{eventClipboard.Count}个音符");
        eventClipboard.Clear();
        RefreshEditAndChart();

        onEventRefreshed(eventEditItems);
    }

    private void RefreshEditAndChart()
    {
        RefreshEvents(-1);

        ChartTool.ConvertAllEditEvents2ChartDataEvents(GlobalData.Instance.chartEditData.boxes[currentBoxID], GlobalData.Instance.chartData.boxes[currentBoxID]);
    }

    private void AddEventAndRefresh(Data.ChartEdit.Event copyNewEvent,EventType eventType, int currentBoxID)
    {
        List<Data.ChartEdit.Event> events = eventType switch
        {
            EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
            EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
            EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
            EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
            EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
            EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
            EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
            EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
            EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
            EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
            _ => null
        };

        events.Add(copyNewEvent);
        Algorithm.BubbleSort(events, (a, b) =>//排序
        {
            if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
            {
                return 1;
            }
            else if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
            {
                return -1;
            }
            return 0;
        });
        RefreshEvents(-1);
    }

    private void CutEvent()
    {
        Debug.Log("剪切事件");
        isCopy = false;
        AddEvent2EventClipboard();
    }

    void AddEvent2EventClipboard()
    {
        eventClipboard.Clear();
        foreach (EventEditItem selectedEventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
        {
            eventClipboard.Add(selectedEventEditItem);
        }
        LogCenter.Log($@"已将{eventClipboard.Count}个事件发送至剪切板！");
    }
    private void MoveUp()
    {
        foreach (EventEditItem eventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
        {
            eventEditItem.@event.startBeats.AddOneBeat();
            eventEditItem.@event.endBeats.AddOneBeat();
        }
        LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个事件向上移动一格");
        
        RefreshEditAndChart();
    }

    private void MoveDown()
    {
        foreach (EventEditItem eventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
        {
            eventEditItem.@event.startBeats.SubtractionOneBeat();
            eventEditItem.@event.endBeats.SubtractionOneBeat();
        }
        LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个事件向下移动一格");

        RefreshEditAndChart();
    }
    private void DeleteEvent(EventEditItem eventEditItem)
    {
        List<Data.ChartEdit.Event> events = eventEditItem.eventType switch
        {
            EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
            EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
            EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
            EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
            EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
            EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
            EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
            EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
            EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
            EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
            _ => throw new Exception("耳朵耷拉下来，呜呜呜，没找到事件类型")
        };
        events.Remove(eventEditItem.@event);
        onEventDeleted(eventEditItem);
    }
}
