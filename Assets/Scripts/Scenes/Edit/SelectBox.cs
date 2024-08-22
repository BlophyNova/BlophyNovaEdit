using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Scenes.Edit
{
    public class SelectBox : MonoBehaviour
    {
        public Form.NoteEdit.NoteEdit noteEdit;
        public EventEdit eventEdit;
        public RectTransform thisSelectBoxRect;
        public ISelectBox selectBoxObjects => noteEdit == null ? eventEdit : noteEdit;
        public LabelWindowContent labelWindowContent => noteEdit == null ? eventEdit : noteEdit;
        public Image selectBoxTexture;
        public Color enableSelectBoxTextureColor = new(1, 1, 1, 1);
        public Color disableSelectBoxTextureColor = new(1, 1, 1, 0);
        public Vector2 firstFramePositionInLabelWindow = Vector2.zero;
        public bool isPressing = false;
        private void Start()
        {
            //Debug.Log($"这里有问题，isPressing的正确性和窗口切换焦点事件被抢，导致对不上号");
            if (noteEdit != null)
            {
                noteEdit.labelWindow.onWindowLostFocus += () => isPressing = false;
                noteEdit.labelWindow.onWindowGetFocus += () => isPressing = true;

                noteEdit.labelWindow.onLabelLostFocus += () => isPressing = true;
                noteEdit.labelWindow.onLabelGetFocus += () => isPressing = false;
            }
            if (eventEdit != null)
            {
                eventEdit.labelWindow.onWindowLostFocus += () => isPressing = false;
                eventEdit.labelWindow.onWindowGetFocus += () => isPressing = true;

                eventEdit.labelWindow.onLabelLostFocus += () => isPressing = true;
                eventEdit.labelWindow.onLabelGetFocus += () => isPressing = false;
            }
            selectBoxTexture.color = new(1, 1, 1, 0);
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

        private void EndHandle()
        {
            selectBoxTexture.color = disableSelectBoxTextureColor;
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
    }
}