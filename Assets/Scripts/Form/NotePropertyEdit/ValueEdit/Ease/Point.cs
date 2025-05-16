using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Camera;
namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class Point : MonoBehaviour,IDragHandler
    {
        public Line line;

        public Data.ChartEdit.Point thisPointData=new();
        
        public RectTransform selfRect;
        public RectTransform parentRect;
        
        public TextMeshProUGUI xyInfo;
        // Start is called before the first frame update
        void Start()
        {
            Vector3[] corners = new Vector3[4];
            parentRect.GetLocalCorners(corners);
            CalculateXY(new(corners[2].x * .0001f, corners[2].y * .0001f));
        }
        public Vector2 MousePositionInThisRectTransform 
        { 
            get  
            {
                Vector2 mousePosition = Mouse.current.position.value;
                Vector2 mousePositionInWorldPosition = main.ScreenToWorldPoint(mousePosition);
                Vector2 mousePositionInLocalPosition = parentRect.InverseTransformPoint(mousePositionInWorldPosition);
                Vector2 result = mousePositionInLocalPosition;// + parentRect.sizeDelta / 2;
                return result;
            } 
        }
        public void OnDrag(PointerEventData eventData)
        {
            CalculateXY(MousePositionInThisRectTransform);
            line.UpdateDraw();
        }

        private void CalculateXY(Vector2 position)
        {
            selfRect.localPosition = position;
            Vector3[] corners = new Vector3[4];
            parentRect.GetLocalCorners(corners);
            Vector2 result = new(position.x/corners[2].x,position.y/corners[2].y);
            thisPointData.x = result.x;
            thisPointData.y = result.y;
            xyInfo.text = $"({result.x:F3},{result.y:F3})";
        }
    }
}