using Manager;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class PauseResume : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            StateManager.Instance.IsPause = !StateManager.Instance.IsPause;
        }
    }
}