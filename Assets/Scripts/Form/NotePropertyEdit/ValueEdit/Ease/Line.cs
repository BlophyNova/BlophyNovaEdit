using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class Line : MonoBehaviour
    { 
        public LineRenderer lineRenderer;

        public void UpdateDraw(int positionCount,Vector3[] postions)
        {
            lineRenderer.positionCount = positionCount;
            lineRenderer.SetPositions(postions);
        }
    }
}