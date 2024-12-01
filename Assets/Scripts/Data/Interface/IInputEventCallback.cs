using UnityEngine.InputSystem;

namespace Data.Interface
{
    public interface IInputEventCallback
    {
        public void Started(InputAction.CallbackContext callbackContext);
        public void Performed(InputAction.CallbackContext callbackContext);
        public void Canceled(InputAction.CallbackContext callbackContext);
    }
}