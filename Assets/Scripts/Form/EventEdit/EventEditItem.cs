using Data.ChartEdit;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEditItem : PublicButton
{
    public LabelWindow labelWindow;
    public RectTransform rectTransform;
    public EaseRenderer easeRenderer;
    public Data.ChartEdit.Event @event;
    public Data.Enumerate.EventType eventType;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            if (labelWindow.associateLabelWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
            {
                NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelWindow;
                notePropertyEdit.SelectedNote(this);
            }
        });
    }
}
