using Log;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Form.LabelWindow
{
    public class LabelWindow : MonoBehaviour
    {
        public delegate void OnWindowGetFocus();

        public delegate void OnWindowLostFocus();

        public delegate void OnWindowMoved();

        public delegate void OnWindowSizeChanged();

        public AddLabel addLabel;
        public RectTransform labelWindowRect;
        public Transform labelParent;
        public List<LabelItem> labels;
        public DragIcon dragIcon;
        public Transform content;
        public Image labelColor;
        public int labelColorIndex;
        public LabelWindow associateLabelWindow; //关联LabelWindow
        public LabelItem currentLabelItem;
        public Content labelWindowContent;
        public bool focus;
        public float MinX => currentLabelItem == null ? 100 : currentLabelItem.labelWindowContent.minX;
        public float MinY => currentLabelItem == null ? 100 : currentLabelItem.labelWindowContent.minY;
        public float MaxX => 1920;
        public float MaxY => 1080;
        public event OnWindowSizeChanged onWindowSizeChanged = () => { };
        public event OnWindowMoved onWindowMoved = () => { };
        public event OnWindowGetFocus onWindowGetFocus = () => { };
        public event OnWindowLostFocus onWindowLostFocus = () => { };

        public void WindowSizeChanged()
        {
            onWindowSizeChanged();
        }

        public void WindowMoved()
        {
            onWindowMoved();
        }

        public void WindowGetFocus()
        {
            onWindowGetFocus();
            LogCenter.Log($"{labelColorIndex}号窗口被激活");
        }

        public void WindowLostFocus()
        {
            onWindowLostFocus();
            LogCenter.Log($"{labelColorIndex}号窗口失活");
        }
        //public 
    }
}