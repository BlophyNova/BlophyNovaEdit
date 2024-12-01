using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class Undo : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            
            Steps.Instance.Undo();
        }
    }
}