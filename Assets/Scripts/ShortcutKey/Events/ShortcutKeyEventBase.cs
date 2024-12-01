using Data.Interface;
using Log;
using Scenes.Edit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class ShortcutKeyEventBase : MonoBehaviour, IInputEventCallback
    {
        public string inputActionName;

        public virtual void Started(InputAction.CallbackContext callbackContext)
        {
            LogCenter.Log($"{inputActionName}.Started被调用！");
        }

        public virtual void Performed(InputAction.CallbackContext callbackContext)
        {
            LogCenter.Log($"{inputActionName}.Performed被调用！");
        }

        public virtual void Canceled(InputAction.CallbackContext callbackContext)
        {
            LogCenter.Log($"{inputActionName}.Canceled被调用！");
        }

        protected void Init()
        {
            InputAction inputAction = ShortcutKeyManager.Instance.playerInput.actions[inputActionName];
            inputAction.started += Started;
            inputAction.performed += Performed;
            inputAction.canceled += Canceled;
            inputAction.Enable();
        }
    }
}