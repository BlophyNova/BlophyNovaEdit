using UnityEngine;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

namespace Form.NoteEdit
{
    //这里放控件本身的方法
    public partial class NoteEdit
    {
        public override void WindowSizeChanged()
        {
            base.WindowSizeChanged();
            UpdateVerticalLineCount();
            UpdateNoteLocalPosition();
        }
        public void UpdateVerticalLineCount()
        {
            for (int i = 0; i < verticalLines.Count;)
            {
                RectTransform verticalLine = verticalLines[0];
                verticalLines.Remove(verticalLine);
                Destroy(verticalLine.gameObject);
            }

            int subdivision = GlobalData.Instance.chartEditData.verticalSubdivision;
            Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
            Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
            for (int i = 1; i < subdivision; i++)
            {
                RectTransform newVerticalLine = Instantiate(verticalLinePrefab, transform);
                newVerticalLine.localPosition =
                    (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) *
                    Vector2.right;
                newVerticalLine.SetSiblingIndex(4);
                verticalLines.Add(newVerticalLine);
            }
            //note.positionX = (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
            verticalLineDeltaDataForChartData = ((verticalLines[1].localPosition.x - verticalLines[0].localPosition.x) / verticalLineLeftAndRightDelta.x) * 2;
        }
    }
}
