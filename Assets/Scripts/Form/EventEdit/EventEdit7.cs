using System.Collections.Generic;
using Data.ChartEdit;
using Manager;
using UtilityCode.Algorithm;
using static UtilityCode.ChartTool.ChartTool;

namespace Form.EventEdit
{
    //这里放为ChartData加入数据的方法,不负责刷新
    public partial class EventEdit
    {
        public void AddEvent2ChartData(Event @event, int boxID)
        {
            List<Data.ChartData.Event> events = FindChartDataEventList(ChartData.boxes[boxID], @event.eventType);
            if (events == null)
            {
                AddSpeedEvent2ChartData(@event, boxID);
                return;
            }

            int index = Algorithm.BinarySearch(events,
                m => m.startTime < BPMManager.Instance.GetSecondsTimeByBeats(@event.startBeats.ThisStartBPM), false);
            Data.ChartData.Event newEvent = new(@event);
            @event.chartDataEvent = newEvent;
            events.Insert(index, @event.chartDataEvent);
        }

        public void DeleteEvent2ChartData(Event @event, int boxID)
        {
            List<Data.ChartData.Event> events = FindChartDataEventList(ChartData.boxes[boxID], @event.eventType);
            if (events == null)
            {
                DeleteSpeedEvent2ChartData(@event, boxID);
                return;
            }

            events.Remove(@event.chartDataEvent);
        }

        private void AddSpeedEvent2ChartData(Event @event, int boxID)
        {
            ForeachSpeedEvents(ChartEditData.boxes[boxID], ChartData.boxes[boxID]);
        }

        private void DeleteSpeedEvent2ChartData(Event @event, int boxID)
        {
            ForeachSpeedEvents(ChartEditData.boxes[boxID], ChartData.boxes[boxID]);
        }
    }
}