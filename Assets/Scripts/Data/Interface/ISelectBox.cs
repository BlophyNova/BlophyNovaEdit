using System;
using System.Collections;
using System.Collections.Generic;
using Scenes.Edit;
using UnityEngine;
public interface ISelectBox
{
    public List<ISelectBoxItem> TransmitObjects();
}

public interface ISelectBoxItem
{
    public bool IsNoteEdit { get; }
    public Vector3[] GetCorners();
    public void SetSelectState(bool active);
}