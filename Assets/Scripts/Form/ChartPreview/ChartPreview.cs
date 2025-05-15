using Data.Interface;
using Form.LabelWindow;
using Manager;
using System;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Form.ChartPreview
{
    public class ChartPreview : LabelWindowContent, IRefreshPlayer
    {
        public RectTransform selfRect;
        [SerializeField]Camera chartCamera;
        Camera ChartCamera
        {
            get
            {
                if (chartCamera == null)
                {
                    chartCamera = GameObject.FindWithTag("ChartCamera").GetComponent<Camera>();
                }
                return chartCamera;
            }
        }

        [SerializeField]private EventEdit.EventEdit eventEdit;

        private EventEdit.EventEdit EventEdit
        {
            get
            {
                if (eventEdit == null)
                {
                    foreach (LabelItem labelItem in labelWindow.labels)
                    {
                        if (labelItem.labelWindowContent is EventEdit.EventEdit)
                        {
                            eventEdit = (EventEdit.EventEdit)labelItem.labelWindowContent;
                        }
                    }
                }
                return eventEdit;
            }
        }
        [SerializeField]private NoteEdit.NoteEdit noteEdit;

        private NoteEdit.NoteEdit NoteEdit
        {
            get
            {
                if (noteEdit == null)
                {
                    foreach (LabelItem labelItem in labelWindow.labels)
                    {
                        if (labelItem.labelWindowContent is NoteEdit.NoteEdit)
                        {
                            noteEdit = (NoteEdit.NoteEdit)labelItem.labelWindowContent;
                        }
                    }
                }
                return noteEdit;
            }
        }
        private void Start()
        {
            /*
            labelWindow.onWindowGetFocus += () => focusIsMe = true;
            labelWindow.onWindowLostFocus += () => focusIsMe = false;
            labelItem.onLabelGetFocus += () => focusIsMe = true;
            labelItem.onLabelLostFocus += () => focusIsMe = false;
            */
        }

        private void Update()
        {
            if(!FocusIsMe)return;
            if(!Mouse.current.leftButton.isPressed)return;
            Ray ray =ChartCamera.ViewportPointToRay(MousePositionInThisTransformViewport);
            if (!Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity)) return;
            
            Debug.Log($"好玩的:{hit.collider.name}");
            Debug.DrawLine(ray.origin, hit.point, Color.cyan);
            NoteController noteController = hit.collider.GetComponent<NoteController>();
            NoteEdit.RefreshEdit(noteController.thisNote.currentLineID,noteController.thisNote.currentBoxID);
            EventEdit.RefreshEdit(-1,noteController.thisNote.currentBoxID);
        }

        public void RefreshPlayer(int lineID, int boxID)
        {
            ProgressManager.Instance.OffsetTime(0);
        }
    }
}