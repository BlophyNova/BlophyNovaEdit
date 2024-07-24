using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour
{
    public LabelWindow labelWindow;
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
