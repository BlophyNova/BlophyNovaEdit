using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Interface
{
    public interface ISelectBox
    {
        public List<ISelectBoxItem> TransmitObjects();
    }
    public interface ISelectBoxItem
    {
        public bool IsNoteEdit { get; }
        public Vector3[] GetCorners();
        public void SetSelectState(bool active);
        public float GetStartBeats();
    }
}