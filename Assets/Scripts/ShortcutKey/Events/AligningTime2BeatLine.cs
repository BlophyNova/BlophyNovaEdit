using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AligningTime2BeatLine : ShortcutKeyEventBase
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
            float offsetBeats = BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime);
            offsetBeats -= (int)offsetBeats;
            Debug.Log($@"{offsetBeats}");
            StateManager.Instance.IsPause = true;
            if (ProgressManager.Instance.CurrentTime + offsetBeats < 0)
            {
                ProgressManager.Instance.OffsetTime(-ProgressManager.Instance.CurrentTime);
            }
            else
            {
                ProgressManager.Instance.OffsetTime(-BPMManager.Instance.GetSecondsTimeByBeats(offsetBeats));
            }

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        }

        Debug.Log($"AligningTime2BeatLinePerformed：{callbackContext.ReadValue<float>()}");
    }
}
