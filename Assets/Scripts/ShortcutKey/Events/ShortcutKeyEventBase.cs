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
            string trackStr = new System.Diagnostics.StackTrace().ToString();
            LogCenter.Log($"{inputActionName}.Canceled被调用！{Time.frameCount}{trackStr}");
        }

        protected void Init() => ShortcutKeyManager.Instance.RegisterEvents(inputActionName, Started, Performed, Canceled);
        
    }
}