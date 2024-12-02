using CustomSystem;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class Redo : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            Steps.Instance.Redo();
        }
    }
}