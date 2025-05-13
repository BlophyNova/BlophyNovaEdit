using Data.Interface;
using Form.LabelWindow;
using Manager;
using System;
using System.Collections.Generic;

namespace Form.ChartPreview
{
    public class ChartPreview : LabelWindowContent, IRefreshPlayer
    {
        public void RefreshPlayer(int lineID, int boxID)
        {
            ProgressManager.Instance.OffsetTime(0);
        }
    }
}