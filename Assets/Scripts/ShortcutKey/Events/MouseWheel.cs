using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
            LabelWindowContentType.ProgressBar |
            LabelWindowContentType.ATimeLine |
            LabelWindowContentType.DebugText |
            LabelWindowContentType.Kawaii;

        if (labelWindowContentType.HasFlag(LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType))
        {
            //LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Canceled(callbackContext);
            //ProgressManager.Instance.PausePlay();
            float offsetTime = Mathf.Sign(callbackContext.ReadValue<float>()) * BPMManager.Instance.GetSecondsTimeWithBeats((1f / GlobalData.Instance.chartEditData.beatSubdivision));
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

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        }

        Debug.Log($"MousePerformed：{callbackContext.ReadValue<float>()}");
    }
}
