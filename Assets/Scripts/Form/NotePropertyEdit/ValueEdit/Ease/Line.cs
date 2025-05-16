using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class Line : MonoBehaviour
    {
        public AnimationCurve curve;
        public List<Point> points;
        public LineRenderer lineRenderer;


        public RectTransform selfRect;

        private void Start()
        {
        }

        private void Update()
        {
        }

        public void HidePoints()
        {
            foreach (Point point in points)
            {
                point.gameObject.SetActive(false);
            }
        }

        public void ShowPoints()
        {
            foreach (Point point in points)
            {
                point.gameObject.SetActive(true);
            }
        }
        public void UpdateDraw(AnimationCurve curve)
        {            
            Vector3[] positions = new Vector3[100];
            Vector3[] corners = new Vector3[4];
            selfRect.GetLocalCorners(corners);
            for (int i = 0; i < positions.Length; i++)
            {
                //positions[i].
                Vector3 currentPosition = (corners[2] - corners[0]) * (i / (float)positions.Length) + corners[0];
                currentPosition.y =
                    curve.Evaluate(i / (float)positions.Length) * (corners[2].y - corners[0].y) +
                    corners[0].y;
                currentPosition.z = -1;
                positions[i] = currentPosition;
            }
            
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }

        public void UpdateDraw(List<Data.ChartEdit.Point> points)
        {
            //偷个懒，只支持两个算了（
            List<Keyframe> keyframes = new();
            Keyframe firstKeyframe = new();
            firstKeyframe.time = 0;
            firstKeyframe.value = 0;
            firstKeyframe.outTangent = points[0].y / points[0].x;
            firstKeyframe.outWeight = points[0].x;
            firstKeyframe.weightedMode = WeightedMode.Both;
            keyframes.Add(firstKeyframe);

            Keyframe lastKeyframe = new();
            lastKeyframe.time = 1;
            lastKeyframe.value = 1;
            lastKeyframe.inTangent = (1 - points[1].y) / (1 - points[1].x);
            lastKeyframe.inWeight = 1 - points[1].x;
            lastKeyframe.weightedMode = WeightedMode.Both;
            keyframes.Add(lastKeyframe);

            curve = new() { preWrapMode = WrapMode.ClampForever,postWrapMode = WrapMode.ClampForever,keys=keyframes.ToArray()};
            
            UpdateDraw(curve);
        }

        public void UpdateDraw()
        {
            List<Data.ChartEdit.Point> points = new();
            foreach (Point point in this.points)
            {
                points.Add(point.thisPointData);
            }
            UpdateDraw(points);
        }
    }
}