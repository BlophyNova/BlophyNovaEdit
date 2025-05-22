using System.Collections;
using System.Collections.Generic;
using CustomSystem;
using Data.ChartEdit;
using Form.NoteEdit;
using Log;
using UnityEngine;
using Event = Data.ChartEdit.Event;

namespace Form.EventEdit
{
    public partial class EventEdit
    {
        /// <summary>
        ///     裁剪Texture2D
        /// </summary>
        /// <param name="originalTexture"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <returns></returns>
        public static Texture2D ScaleTextureCutOut(Texture2D originalTexture, int offsetX, int offsetY,
            float originalWidth, float originalHeight)
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
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4));
                    newTexture.SetPixel(x, y, color);
                }
            }

            newTexture.anisoLevel = 2;
            newTexture.Apply();
            return newTexture;
        }

        private void FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine,
            out EventVerticalLine nearEventVerticalLine)
        {
            nearBeatLine = FindNearBeatLine(MousePositionInThisRectTransform);

            nearEventVerticalLine = FindNearEventVerticalLine(MousePositionInThisRectTransform);
        }

        private EventVerticalLine FindNearEventVerticalLine(Vector2 mousePosition)
        {
            EventVerticalLine nearEventVerticalLine = null;
            float nearEventVerticalLineDis = float.MaxValue;
            foreach (EventVerticalLine item in eventVerticalLines)
            {
                float dis = Vector2.Distance(mousePosition,
                    (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearEventVerticalLineDis)
                {
                    nearEventVerticalLineDis = dis;
                    nearEventVerticalLine = item;
                }
            }

            return nearEventVerticalLine;
        }

        private BeatLine FindNearBeatLine(Vector2 mousePosition)
        {
            BeatLine nearBeatLine = null;
            float nearBeatLineDis = float.MaxValue;
            //第一次
            foreach (BeatLine item in basicLine.beatLines)
            {
                float dis = Vector2.Distance(mousePosition,
                    (Vector2)thisEventEditRect.InverseTransformPoint(item.transform.position) +
                    labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }

            return nearBeatLine;
        }

        public IEnumerator WaitForPressureAgain(EventEditItem eventEditItem)
        {
            List<Event> events = new();
            while (true)
            {
                if (waitForPressureAgain)
                {
                    break;
                }

                BeatLine beatLine = FindNearBeatLine(MousePositionInThisRectTransform);
                try
                {
                    eventEditItem.thisEventEditItemRect.sizeDelta = new Vector2(
                        Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition),
                        beatLine.transform.localPosition.y - eventEditItem.transform.localPosition.y);
                    eventEditItem.@event.endBeats = new BPM(beatLine.thisBPM);
                }
                catch
                {
                }

                yield return new WaitForEndOfFrame();
            }

            waitForPressureAgain = false;

            if (eventEditItem.@event.endBeats.ThisStartBPM - eventEditItem.@event.startBeats.ThisStartBPM <= .0001f)
            {
                Debug.LogError("哒咩哒咩，长度为0的Hold！");
                LogCenter.Log("用户尝试放置长度为0的Hold音符");
                eventEditItems.Remove(eventEditItem);
                Destroy(eventEditItem.gameObject);
            }
            else
            {
                //添加事件到对应的地方
                LogCenter.Log(
                    $"{eventEditItem.@event.eventType}新事件：{eventEditItem.@event.startBeats.integer}:{eventEditItem.@event.startBeats.molecule}/{eventEditItem.@event.startBeats.denominator}");
                events.Add(eventEditItem.@event);
                Steps.Instance.Add(Undo, Redo, default);
                //eventEditItem.DrawLineOnEEI();
                //eventEditItem.Init();
                eventEditItems.Add(eventEditItem);
                AddEvent(eventEditItem.@event, currentBoxID);
            }

            yield break;

            void Undo()
            {
                DeleteEvents(events, currentBoxID);
            }

            void Redo()
            {
                List<Event> newEvents = AddEvents(events, currentBoxID, true);
                BatchEvents(newEvents, @event => @event.IsSelected = false);
                eventEditItems.AddRange(AddEvents2UI(newEvents));
            }
        }
    }
}