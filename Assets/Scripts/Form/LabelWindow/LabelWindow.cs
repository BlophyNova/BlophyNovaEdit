using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LabelWindow : MonoBehaviour
{
    public AddLabel addLabel;
    public Transform labelParent;
    public List<LabelItem> labels;
    public DragIcon dragIcon;
    public Transform content;
    public LabelWindow associateLabelWindow;
    public bool focus;
    public float minX = 100;
    public float minY = 100;
    public float MaxX => 1920;
    public float MaxY =>1080;
    //public 
}
