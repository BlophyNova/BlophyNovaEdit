using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
public class LabelWindow : MonoBehaviour
{
    public AddLabel addLabel;
    public RectTransform labelWindowRect;
    public Transform labelParent;
    public List<LabelItem> labels;
    public DragIcon dragIcon;
    public Transform content;
    public Image labelColor;
    public int labelColorIndex;
    public LabelWindow associateLabelWindow;//关联LabelWindow
    public LabelItem currentLabelItem;
    public Content labelWindowContent;
    public delegate void OnWindowSizeChanged();
    public event OnWindowSizeChanged onWindowSizeChanged = () => { };
    public delegate void OnWindowMoved();
    public event OnWindowMoved onWindowMoved = () => { };
    public delegate void OnWindowGetFocus();
    public event OnWindowGetFocus onWindowGetFocus = () => { };
    public delegate void OnWindowLostFocus();
    public event OnWindowLostFocus onWindowLostFocus = () => { };
    public bool focus;
    public float MinX => currentLabelItem == null ? 100 : currentLabelItem.labelWindowContent.minX;
    public float MinY => currentLabelItem == null ? 100 : currentLabelItem.labelWindowContent.minY;
    public float MaxX => 1920;
    public float MaxY => 1080;
    public void WindowSizeChanged()=>onWindowSizeChanged();
    public void WindowMoved() => onWindowMoved();
    public void WindowGetFocus()=>onWindowGetFocus();
    public void WindowLostFocus()=>onWindowLostFocus();
    //public 
}
