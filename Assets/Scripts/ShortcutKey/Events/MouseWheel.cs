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
                
                float offsetTime = Mathf.Sign(callbackContext.ReadValue<float>()) *
                                   GlobalData.Instance.generalData.MouseWheelSpeed;
                Debug.Log($@"offsetTime:{offsetTime}");
                StateManager.Instance.IsPause = true;
                if (ProgressManager.Instance.CurrentTime + offsetTime < 0)
                {
                    ProgressManager.Instance.OffsetTime(-ProgressManager.Instance.CurrentTime);
                }
                else
                {
                    ProgressManager.Instance.OffsetTime(offsetTime);
                }

                GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI(),new List<Type> { typeof(BasicLine) });
                
            }

            Debug.Log($"MousePerformed：{callbackContext.ReadValue<float>()}");
        }
    }
}