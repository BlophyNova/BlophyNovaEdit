using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour
{
    public LabelWindow labelWindow;
    public RectTransform contentRectTransform;
    private void OnMouseDown()
    {
        labelWindow.transform.SetAsLastSibling();
        labelWindow.transform.SetSiblingIndex(labelWindow.transform.GetSiblingIndex() - 1);
        LabelWindowsManager.Instance.SetFocusWindow(labelWindow);
    }
    //private void OnMouseEnter()
    //{
    //    labelWindow.focus = true;
    //}
}
