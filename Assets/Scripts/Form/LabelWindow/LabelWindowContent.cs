using Data.Enumerate;
using Data.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Camera;

namespace Form.LabelWindow
{
    public class LabelWindowContent : MonoBehaviour, IInputEventCallback
    {
        public string labelWindowName;
        public LabelWindow labelWindow;
        public LabelItem labelItem;
        public LabelWindowContentType labelWindowContentType;
        public int minX = 100;
        public int minY = 100;

        public Vector2 MousePositionInThisRectTransform =>
            (Vector2)transform.InverseTransformPoint(main.ScreenToWorldPoint(Mouse.current.position.value)) +
            labelWindow.labelWindowRect.sizeDelta / 2;

        public Vector2 MousePositionInThisRectTransformCenter =>
            transform.InverseTransformPoint(main.ScreenToWorldPoint(Mouse.current.position.value));

        public virtual void Started(InputAction.CallbackContext callbackContext)
        {
        }

        public virtual void Performed(InputAction.CallbackContext callbackContext)
        {
        }

        public virtual void Canceled(InputAction.CallbackContext callbackContext)
        {
        }

        public virtual void WindowSizeChanged()
        {
        }
    }
}