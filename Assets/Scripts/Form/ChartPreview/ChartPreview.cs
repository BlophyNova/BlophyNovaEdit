using Data.Interface;
using Form.LabelWindow;
using Manager;
using System;
using System.Collections.Generic;

namespace Form.ChartPreview
{
    public class ChartPreview : LabelWindowContent, IRefresh
    {
        public void Refresh()
        {
            ProgressManager.Instance.OffsetTime(0);
        }
    }
}