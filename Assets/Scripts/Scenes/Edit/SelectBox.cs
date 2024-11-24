using Data.ChartEdit;
using Form.NotePropertyEdit;
using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Edit
{
    public class SelectBox : MonoBehaviour, ISelectBox
    {
        public Form.NoteEdit.NoteEdit noteEdit;
        public EventEdit eventEdit;
        public RectTransform thisSelectBoxRect;
        public ISelectBox selectBoxObjects => noteEdit == null ? eventEdit : noteEdit;
        public LabelWindowContent labelWindowContent => noteEdit == null ? eventEdit : noteEdit;
        private bool isNoteEdit => noteEdit == null ? false : true;
        public Image selectBoxTexture;
        public NotePropertyEdit notePropertyEdit => (NotePropertyEdit)labelWindowContent.labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
        List<ISelectBoxItem> selectedBoxItems = new();
        public Color enableSelectBoxTextureColor = new(1, 1, 1, 1);
        public Color disableSelectBoxTextureColor = new(1, 1, 1, 0);
        public Vector2 firstFramePositionInLabelWindow = Vector2.zero;
        public bool isPressing = false;
        private void Start()
        {
            //Debug.Log($"这里有问题，isPressing的正确性和窗口切换焦点事件被抢，导致对不上号");
            #region 当SelectBox的快捷键仅为鼠标左键时，需要执行以下代码
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

            selectBoxTexture.color = new(1, 1, 1, 0);
            if (noteEdit != null)
            {
                noteEdit.onNoteDeleted += noteEdit => selectedBoxItems.Remove(noteEdit);
                noteEdit.onNoteRefreshed += notes =>
                {
                    selectedBoxItems.Clear();
                    foreach (NoteEdit note in notes)
                    {
                        if (note.thisNoteData.isSelected)
                        {
                            note.SetSelectState(true);
                            selectedBoxItems.Add(note);
                        }
                    }
                };
                notePropertyEdit.onNoteValueChanged += TempNoteEditValueChangedCallBack;
            }

            if (eventEdit != null)
            {
                eventEdit.onEventDeleted += eventEditItem => selectedBoxItems.Remove(eventEditItem);
                eventEdit.onEventRefreshed += eventEditItems =>
                {
                    selectedBoxItems.Clear();
                    foreach (EventEditItem eventEditItem in eventEditItems)
                    {
                        if (eventEditItem.@event.IsSelected)
                        {
                            eventEditItem.SetSelectState(true);
                            selectedBoxItems.Add(eventEditItem);
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
                    Debug.Log($"SelectBox第一帧");
                    StartHandle();
                }
                Debug.Log($"SelectBox被按下，noteEdit={noteEdit}，eventEdit={eventEdit}");

                HoldHandle();

                return;
            }
            if (!selectBoxTexture.color.Equals(disableSelectBoxTextureColor))
            {
                Debug.Log($"SelectBox最后一帧");
                EndHandle();
            }
        }

        public void SetSingleNote(ISelectBoxItem selectBoxItem)
        {
            ClearSelectedBoxItems();
            selectedBoxItems.Add(selectBoxItem);
            selectBoxItem.SetSelectState(true);

            LabelWindow labelWindow = noteEdit == null ? eventEdit.labelWindow : noteEdit.labelWindow;
            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
            {
                NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
                notePropertyEdit.note = null;
                notePropertyEdit.@event = null;
                if (selectBoxItem.IsNoteEdit)
                {
                    notePropertyEdit.SelectedNote((NoteEdit)selectBoxItem);
                }
                else
                {
                    notePropertyEdit.SelectedNote((EventEditItem)selectBoxItem);
                }
            }
        }
        private void EndHandle()
        {
            selectBoxTexture.color = disableSelectBoxTextureColor;
            ClearSelectedBoxItems();
            NewSelectedBoxItems();
            SetValue2NotePropertyEdit();
        }
        Note originalnNoteData = new();
        Note tempNoteEdit = new();

        private void TempNoteEditValueChangedCallBack()
        {
            if (selectedBoxItems.Count <= 1) return;
            ForeachAllItems(note => !note.HitBeats.Equals(originalnNoteData.HitBeats), item => item.HitBeats = tempNoteEdit.HitBeats);
            ForeachAllItems(note => !(note.noteType == originalnNoteData.noteType), item => item.noteType = tempNoteEdit.noteType);
            ForeachAllItems(note => !note.holdBeats.Equals(originalnNoteData.holdBeats), item => item.holdBeats = tempNoteEdit.holdBeats);
            ForeachAllItems(note => !(note.effect == originalnNoteData.effect), item => item.effect = tempNoteEdit.effect == 0 ? 0 : tempNoteEdit.effect);
            ForeachAllItems(note => !note.positionX.Equals(originalnNoteData.positionX), item => item.positionX = tempNoteEdit.positionX);
            ForeachAllItems(note => !(note.isClockwise == originalnNoteData.isClockwise), item => item.isClockwise = tempNoteEdit.isClockwise);

            void ForeachAllItems(Predicate<Note> predicate, Action<Note> action)
            {
                if (predicate(tempNoteEdit))
                {
                    foreach (NoteEdit item in selectedBoxItems.Cast<NoteEdit>())
                    {
                        action(item.thisNoteData);
                    }
                    action(originalnNoteData);
                }
            }
        }

        private void SetValue2NotePropertyEdit()
        {
            if (isNoteEdit)
            {
                originalnNoteData = new()
                {
                    HitBeats = new(-1, 0, -1),
                    noteType = Data.ChartData.NoteType.Tap,
                    holdBeats = new(-1, 0, -1),
                    effect = 0,
                    positionX = float.NaN,
                    isClockwise = false
                };
                tempNoteEdit = new(originalnNoteData);
                notePropertyEdit.SelectedNote(tempNoteEdit);
            }
        }
        private void ClearSelectedBoxItems()
        {
            foreach (var item in selectedBoxItems)
            {
                item.SetSelectState(false);
            }

            selectedBoxItems.Clear();
        }

        private void NewSelectedBoxItems()
        {
            Vector3[] selectBoxPoints = new Vector3[4];
            thisSelectBoxRect.GetWorldCorners(selectBoxPoints);
            var points = selectBoxObjects.TransmitObjects();
            foreach (var item in points)
            {
                foreach (Vector3 point in item.GetCorners())
                {
                    if (point.x > selectBoxPoints[0].x && point.y > selectBoxPoints[0].y &&
                        point.x < selectBoxPoints[2].x && point.y < selectBoxPoints[2].y)
                    {
                        item.SetSelectState(true);
                        selectedBoxItems.Add(item);
                        Debug.Log($"0:{selectBoxPoints[0]};\n1:{selectBoxPoints[1]};\n2:{selectBoxPoints[2]};\n3{selectBoxPoints[3]};\np:{point}");
                        break;
                    }
                }
                LogCenter.Log($"成功选择{selectedBoxItems.Count}个{isNoteEdit switch { true => "音符", false => "事件" }}");

            }
            Debug.Log($@"已选择{selectedBoxItems.Count}个音符！");
        }

        private void HoldHandle()
        {
            Vector2 endPositionAndFirstFramePositionDelta = labelWindowContent.MousePositionInThisRectTransformCenter - firstFramePositionInLabelWindow;
            transform.localPosition = endPositionAndFirstFramePositionDelta / 2 + firstFramePositionInLabelWindow;
            endPositionAndFirstFramePositionDelta.Set(Mathf.Abs(endPositionAndFirstFramePositionDelta.x), Mathf.Abs(endPositionAndFirstFramePositionDelta.y));
            thisSelectBoxRect.sizeDelta = endPositionAndFirstFramePositionDelta;
        }

        private void StartHandle()
        {
            selectBoxTexture.color = enableSelectBoxTextureColor;
            firstFramePositionInLabelWindow = labelWindowContent.MousePositionInThisRectTransformCenter;
        }

        public List<ISelectBoxItem> TransmitObjects() => selectedBoxItems;
    }
}