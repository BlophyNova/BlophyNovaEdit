using Data.Enumerate;
using Manager;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class MirrorFlip : ShortcutKeyEventBase
    {
        // Start is called before the first frame update
        private void Start()
        {
            Init();
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent
                    .labelWindowContentType == LabelWindowContentType.NoteEdit)
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(
                    callbackContext);
            }
        }
    }
}