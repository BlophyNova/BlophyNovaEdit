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
    public LabelWindowContent currentLabelWindow;
    public bool focus;
    public float MinX => currentLabelWindow == null ? 100 : currentLabelWindow.minX;
    public float MinY => currentLabelWindow == null ? 100 : currentLabelWindow.minY;
    public float MaxX => 1920;
    public float MaxY => 1080;
    //public 
}
