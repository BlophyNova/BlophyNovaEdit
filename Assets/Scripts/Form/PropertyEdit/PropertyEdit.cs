using Form.LabelWindow;
using UnityEngine;
using UnityEngine.UI;

namespace Form.PropertyEdit
{
    public class PropertyEdit : LabelWindowContent
    {
        public GridLayoutGroup gridLayoutGroup;
        public BoxID boxID;
        public LineID lineID;
        
        public VerticalLineCount verticalLineCount;
        public BeatLineCount beatLineCount;
        public PlaySpeed playSpeed;
        public Offset offset;
        public YScale yScale;
        public LoopPlayback loopPlayback;

        //public override void WindowSizeChanged()
        //{
        //    base.WindowSizeChanged();
        //    Debug.Log(new Vector2(labelWindow.labelWindowRect.sizeDelta.x, gridLayoutGroup.cellSize.y));
        //    gridLayoutGroup.cellSize = new Vector2(labelWindow.labelWindowRect.sizeDelta.x, gridLayoutGroup.cellSize.y);
        //}
    }
}