using System.Collections.Generic;
using System.Linq;
using Data.Enumerate;
using Data.Interface;
using Form.EventEdit;
using Form.LabelWindow;
using Form.NotePropertyEdit;
using Log;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Algorithm;
using EventEditItem = Form.EventEdit.EventEditItem;

namespace Scenes.Edit
{
    public class SelectBox : MonoBehaviour, ISelectBox
    {
        public Form.NoteEdit.NoteEdit noteEdit;
        public EventEdit eventEdit;
        public RectTransform thisSelectBoxRect;
        public Image selectBoxTexture;
        public Color enableSelectBoxTextureColor = new(1, 1, 1, 1);
        public Color disableSelectBoxTextureColor = new(1, 1, 1, 0);
        public Vector2 firstFramePositionInLabelWindow = Vector2.zero;
        public bool isPressing;
        public readonly List<ISelectBoxItem> selectedBoxItems = new();
        private NotePropertyEdit notePropertyEdit;
        public ISelectBox selectBoxObjects => noteEdit == null ? eventEdit : noteEdit;
        public LabelWindowContent labelWindowContent => noteEdit == null ? eventEdit : noteEdit;
        private bool isNoteEdit => noteEdit == null ? false : true;

        public NotePropertyEdit NotePropertyEdit
        {
            get
            {
                if (notePropertyEdit == null)
                {
                    foreach (LabelItem item in labelWindowContent.labelWindow.associateLabelWindow.labels)
                    {
                        if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
                        {
                            notePropertyEdit = (NotePropertyEdit)item.labelWindowContent;
                        }
                    }
                }

                return notePropertyEdit;
            }
        }

        private void Start()
        {
            #region 当SelectBox的快捷键仅为鼠标左键时，需要执行以下代码

            //Debug.Log($"isPressing的正确性和窗口切换焦点事件被抢，导致对不上号");
            //if (noteEdit != null)
            //{
            //    noteEdit.labelWindow.onWindowLostFocus += () => isPressing = false;
            //    noteEdit.labelWindow.onWindowGetFocus += () => isPressing = true;

            //    noteEdit.labelWindow.onLabelLostFocus += () => isPressing = true;
            //    noteEdit.labelWindow.onLabelGetFocus += () => isPressing = false;
            //}
            //if (eventEdit != null)
            //{
            //    eventEdit.labelWindow.onWindowLostFocus += () => isPressing = false;
            //    eventEdit.labelWindow.onWindowGetFocus += () => isPressing = true;

            //    eventEdit.labelWindow.onLabelLostFocus += () => isPressing = true;
            //    eventEdit.labelWindow.onLabelGetFocus += () => isPressing = false;
            //}

            #endregion

            selectBoxTexture.color = new Color(1, 1, 1, 0);
            if (noteEdit != null)
            {
                noteEdit.onNotesAdded2UI += notes =>
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        if (notes[i].thisNoteData.isSelected)
                        {
                            selectedBoxItems.Add(notes[i]);
                        }
                    }
                };
                noteEdit.onNotesDeleted += notes =>
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        selectedBoxItems.Remove(notes[i].chartEditNote);
                    }
                };
                noteEdit.onNotesRefreshed += notes =>
                {
                    selectedBoxItems.Clear();
                    for (int i = 0; i < notes.Count; i++)
                    {
                        if (notes[i].isSelected)
                        {
                            selectedBoxItems.Add(notes[i].chartEditNote);
                        }
                    }
                };
            }

            if (eventEdit != null)
            {
                eventEdit.onEventsAdded2UI += eventEditItems =>
                {
                    for (int i = 0; i < eventEditItems.Count; i++)
                    {
                        if (eventEditItems[i].@event.IsSelected)
                        {
                            selectedBoxItems.Add(eventEditItems[i]);
                        }
                    }
                };
                eventEdit.onEventsDeleted += events =>
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        selectedBoxItems.Remove(events[i].chartEditEvent);
                    }
                };
                eventEdit.onEventsRefreshed += events =>
                {
                    selectedBoxItems.Clear();
                    for (int i = 0; i < events.Count; i++)
                    {
                        if (events[i].IsSelected)
                        {
                            selectedBoxItems.Add(events[i].chartEditEvent);
                        }
                    }
                };
            }
        }

        private void Update()
        {
            if (isPressing)
            {
                if (!selectBoxTexture.color.Equals(enableSelectBoxTextureColor))
                {
                    Debug.Log("SelectBox第一帧");
                    StartHandle();
                }

                HoldHandle();
                return;
            }

            if (selectBoxTexture.color.Equals(disableSelectBoxTextureColor))
            {
                return;
            }

            Debug.Log("SelectBox最后一帧");
            EndHandle();
        }

        public List<ISelectBoxItem> TransmitObjects()
        {
            return selectedBoxItems;
        }

        public void SetSingleNote(ISelectBoxItem selectBoxItem)
        {
            ClearSelectedBoxItems();
            selectBoxItem.SetStartAndEndVisibility(true);
            selectedBoxItems.Add(selectBoxItem);
            selectBoxItem.SetSelectState(true);

            if (selectBoxItem.IsNoteEdit)
            {
                //NotePropertyEdit.EditNote.Set(new List<ISelectBoxItem> { selectBoxItem });
                NotePropertyEdit.GetEditNoteWithGetFocus().Set(new List<ISelectBoxItem> { selectBoxItem });
            }
            else
            {
                //NotePropertyEdit.EditEvent.Set(new List<ISelectBoxItem> { selectBoxItem });
                NotePropertyEdit.GetEditEventWithGetFocus().Set(new List<ISelectBoxItem> { selectBoxItem });
            }
        }

        public void SetMutliNote(List<ISelectBoxItem> selectBoxItems)
        {
            ClearSelectedBoxItems();
            foreach (ISelectBoxItem selectBoxItem in selectBoxItems)
            {
                selectBoxItem.SetSelectState(true);
                selectBoxItem.SetStartAndEndVisibility(true);
            }

            selectedBoxItems.AddRange(selectBoxItems);
            SetValue2NotePropertyEdit();
        }

        private void EndHandle()
        {
            selectBoxTexture.color = disableSelectBoxTextureColor;
            NotePropertyEdit.labelItem.labelButton.ThisLabelGetFocus();
            SetMutliNote(NewSelectedBoxItems());
        }

        private void SetValue2NotePropertyEdit()
        {
            if (isNoteEdit)
            {
                NotePropertyEdit.EditNote.Set(selectedBoxItems);
                //NotePropertyEdit.GetEditNoteWithGetFocus().Set(selectedBoxItems);
            }
            else
            {
                NotePropertyEdit.EditEvent.Set(selectedBoxItems);
                //NotePropertyEdit.GetEditEventWithGetFocus().Set(selectedBoxItems);
            }
        }

        private void ClearSelectedBoxItems()
        {
            foreach (ISelectBoxItem item in selectedBoxItems)
            {
                item.SetStartAndEndVisibility(false);
                item.SetSelectState(false);
            }

            selectedBoxItems.Clear();
        }

        private List<ISelectBoxItem> NewSelectedBoxItems()
        {
            Vector3[] selectBoxPoints = new Vector3[4];
            thisSelectBoxRect.GetWorldCorners(selectBoxPoints);
            List<ISelectBoxItem> points = selectBoxObjects.TransmitObjects();
            List<ISelectBoxItem> result = new();
            foreach (ISelectBoxItem item in points)
            {
                foreach (Vector3 point in item.GetCorners())
                {
                    if (point.x > selectBoxPoints[0].x && point.y > selectBoxPoints[0].y &&
                        point.x < selectBoxPoints[2].x && point.y < selectBoxPoints[2].y)
                    {
                        //int index = Algorithm.BinarySearch(points, m => m.GetStartBeats() < item.GetStartBeats(), false);
                        //result.Insert(index, item);
                        result.Add(item);
                        Debug.Log(
                            $"0:{selectBoxPoints[0]};\n1:{selectBoxPoints[1]};\n2:{selectBoxPoints[2]};\n3{selectBoxPoints[3]};\np:{point}");
                        break;
                    }
                }

                LogCenter.Log($"成功选择{selectedBoxItems.Count}个{isNoteEdit switch { true => "音符", false => "事件" }}");
            }

            Algorithm.BubbleSort(result,
                (arg1, arg2) => arg1.GetStartBeats() > arg2.GetStartBeats() ? 1 : 0);


            Debug.Log($@"已选择{selectedBoxItems.Count}个音符！");
            return result;
        }

        private void HoldHandle()
        {
            Vector2 endPositionAndFirstFramePositionDelta = labelWindowContent.MousePositionInThisRectTransformCenter -
                                                            firstFramePositionInLabelWindow;
            transform.localPosition = endPositionAndFirstFramePositionDelta / 2 + firstFramePositionInLabelWindow;
            endPositionAndFirstFramePositionDelta.Set(Mathf.Abs(endPositionAndFirstFramePositionDelta.x),
                Mathf.Abs(endPositionAndFirstFramePositionDelta.y));
            thisSelectBoxRect.sizeDelta = endPositionAndFirstFramePositionDelta;
        }

        private void StartHandle()
        {
            selectBoxTexture.color = enableSelectBoxTextureColor;
            firstFramePositionInLabelWindow = labelWindowContent.MousePositionInThisRectTransformCenter;
        }
    }
}