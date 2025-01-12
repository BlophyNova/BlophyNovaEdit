using Data.ChartEdit;
using Data.Enumerate;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityCode.Algorithm;
using static UtilityCode.ChartTool.ChartTool;
namespace Form.EventEdit
{
    //这里放为ChartData加入数据的方法,不负责刷新
    public partial class EventEdit
    {
        public void AddEvent2ChartData(Event @event, EventType eventType, int boxID)
        {
            List<Data.ChartData.Event> events = FindChartDataEventList(ChartData.boxes[boxID], eventType);
            int index = Algorithm.BinarySearch(events, m => m.startTime <BPMManager.Instance.GetSecondsTimeByBeats(@event.startBeats.ThisStartBPM), false);
            Data.ChartData.Event newEvent = new(@event);
            @event.chartDataEvent = newEvent;
            events.Insert(index, @event.chartDataEvent);
        }
        public void DeleteEvent2ChartData(Event @event, EventType eventType, int boxID)
        {
            List<Data.ChartData.Event> events = FindChartDataEventList(ChartData.boxes[boxID], eventType);
            events.Remove(@event.chartDataEvent);
        }
    }
}
