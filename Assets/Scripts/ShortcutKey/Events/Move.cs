using Data.Enumerate;
using Manager;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class Move : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Started(InputAction.CallbackContext callbackContext)
        {
            base.Started(callbackContext);
            if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent
                    .labelWindowContentType == LabelWindowContentType.EventEdit)
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Started(
                    callbackContext);
            }

            if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent
                    .labelWindowContentType == LabelWindowContentType.NoteEdit)
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Started(
                    callbackContext);
            }
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent
                    .labelWindowContentType == LabelWindowContentType.EventEdit)
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(
                    callbackContext);
            }

            if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent
                    .labelWindowContentType == LabelWindowContentType.NoteEdit)
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(
                    callbackContext);
            }
        }
    }
}