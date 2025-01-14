using Form.PropertyEdit;
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
            verticalLineDeltaDataForChartData=CalculatePositionXDelta(verticalLineLeftAndRightDelta);
        }

        private float CalculatePositionXDelta(Vector3 verticalLineLeftAndRightDelta)
        {
             return ((verticalLines[1].localPosition.x - verticalLines[0].localPosition.x) / verticalLineLeftAndRightDelta.x) * 2;
        }

        public void UpdateNoteLocalPosition()
        {
            for (int i = 0; i < notes.Count; i++)
            {
                notes[i].transform.localPosition = new Vector3(
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x -
                     (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) *
                    notes[i].thisNoteData.positionX,
                    YScale.Instance.GetPositionYWithBeats(notes[i].thisNoteData.HitBeats.ThisStartBPM));
            }
        }
    }
}
