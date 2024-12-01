using Data.Interface;
using Form.LabelWindow;
using Manager;

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