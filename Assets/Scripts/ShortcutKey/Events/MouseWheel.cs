using System;
using System.Collections.Generic;
using Data.Enumerate;
using Data.Interface;
using Form.NoteEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class MouseWheel : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Performed(InputAction.CallbackContext callbackContext)
        {
            base.Performed(callbackContext);
            LabelWindowContentType labelWindowContentType =
                LabelWindowContentType.ChartPreview |
                LabelWindowContentType.NoteEdit |
                LabelWindowContentType.EventEdit |
                LabelWindowContentType.ProgressBar;

            if (labelWindowContentType.HasFlag(LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem
                    .labelWindowContent.labelWindowContentType))
            {
                //LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Canceled(callbackContext);
                //ProgressManager.Instance.PausePlay();
                float offsetTime = Mathf.Sign(callbackContext.ReadValue<float>()) *
                                   BPMManager.Instance.GetSecondsTimeByBeats(1f / GlobalData.Instance.chartEditData
                                       .beatSubdivision);
                Debug.Log($@"{offsetTime}");
                StateManager.Instance.IsPause = true;
                if (ProgressManager.Instance.CurrentTime + offsetTime < 0)
                {
                    ProgressManager.Instance.OffsetTime(-ProgressManager.Instance.CurrentTime);
                }
                else
                {
                    ProgressManager.Instance.OffsetTime(offsetTime);
                }

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),
                    new List<Type> { typeof(BasicLine) });
            }

            Debug.Log($"MousePerformed：{callbackContext.ReadValue<float>()}");
        }
    }
}