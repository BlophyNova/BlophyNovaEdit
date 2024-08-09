using UnityEngine;
using UtilityCode.Singleton;

namespace Controller
{
    public class AntLineController : MonoBehaviourSingleton<AntLineController>
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        public Camera mainCamera;
        public LineRenderer lineRenderer;
        public Vector2[] lineRendererPoints;
        public Vector2 offset = Vector2.zero;
        
        private void Start()
        {
            InitAntLine();
        }

        public void InitAntLine()
        {
            const float is16To9 = 0.5625f;
            lineRenderer.positionCount = lineRendererPoints.Length;
            lineRenderer.startWidth = .05f;
            lineRenderer.endWidth = .05f;
            switch (Screen.height / (float)Screen.width)
            {
                case > is16To9:
                    //这里放平板的处理方法
                    lineRendererPoints[0].x = 0;
                    lineRendererPoints[1].x = 1;
                    lineRendererPoints[2].x = 1;
                    lineRendererPoints[3].x = 0;
                    lineRendererPoints[4].x = 0;
                    lineRendererPoints[0].y = 0;
                    lineRendererPoints[1].y = 0;
                    lineRendererPoints[2].y = 1;
                    lineRendererPoints[3].y = 1;
                    lineRendererPoints[4].y = 0;
                    break;
                case <= is16To9:
                    lineRendererPoints[0].x = 0;
                    lineRendererPoints[1].x = 0;
                    lineRendererPoints[2].x = 1;
                    lineRendererPoints[3].x = 1;
                    lineRendererPoints[4].x = 0;
                    lineRendererPoints[0].y = 0;
                    lineRendererPoints[1].y = 1;
                    lineRendererPoints[2].y = 1;
                    lineRendererPoints[3].y = 0;
                    lineRendererPoints[4].y = 0;
                    break;
            }
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, (Vector2)mainCamera.ViewportToWorldPoint(lineRendererPoints[i]));
            }
        }

        private void Update()
        {
            offset += Vector2.right * Time.deltaTime;
            lineRenderer.material.SetTextureOffset(MainTex, offset);
        }
    }
}
