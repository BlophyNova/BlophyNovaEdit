using Manager;
using UnityEngine;

namespace Form.LabelWindow
{
    public class Content : MonoBehaviour
    {
        public LabelWindow labelWindow;
        public RectTransform contentRectTransform;

        private void OnMouseDown()
        {
            labelWindow.transform.SetAsLastSibling();
            LabelWindowsManager.Instance.SetFocusWindow(labelWindow);
        }
        //private void OnMouseEnter()
        //{
        //    labelWindow.focus = true;
        //}
    }
}