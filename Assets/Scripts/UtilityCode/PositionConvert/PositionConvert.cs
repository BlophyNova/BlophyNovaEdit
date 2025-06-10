using UnityEngine;
using static UnityEngine.Camera;

namespace UtilityCode.PositionConvert
{
    public class PositionConvert
    {
        /// <summary>
        ///     ��������ת��Ϊ��Ļ����
        /// </summary>
        /// <param name="worldPoint">��Ļ����</param>
        /// <returns></returns>
        public static Vector2 WorldPointToScreenPoint(Vector3 worldPoint)
        {
            // Camera.main ���������
            Vector2 screenPoint = main.WorldToScreenPoint(worldPoint);
            return screenPoint;
        }

        /// <summary>
        ///     ��Ļ����ת��Ϊ��������
        /// </summary>
        /// <param name="screenPoint">��Ļ����</param>
        /// <param name="planeZ">��������� Z ƽ��ľ���</param>
        /// <returns></returns>
        public static Vector3 ScreenPointToWorldPoint(Vector2 screenPoint, float planeZ)
        {
            // Camera.main ���������
            Vector3 position = new(screenPoint.x, screenPoint.y, planeZ);
            Vector3 worldPoint = main.ScreenToWorldPoint(position);
            return worldPoint;
        }

        // RectTransformUtility.WorldToScreenPoint
        // RectTransformUtility.ScreenPointToWorldPointInRectangle
        // RectTransformUtility.ScreenPointToLocalPointInRectangle
        // ������������ת���ķ���ʹ�� Camera �ĵط�
        // �� Canvas renderMode Ϊ RenderMode.ScreenSpaceCamera��RenderMode.WorldSpace ʱ ���ݲ��� canvas.worldCamera
        // �� Canvas renderMode Ϊ RenderMode.ScreenSpaceOverlay ʱ ���ݲ��� null

        // UI ����ת��Ϊ��Ļ����
        public static Vector2 UIPointToScreenPoint(Vector3 worldPoint)
        {
            // RectTransform��target
            // worldPoint = target.position;
            Camera uiCamera = main;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPoint);
            return screenPoint;
        }

        // ��Ļ����ת��Ϊ UGUI ����
        public static Vector3 ScreenPointToUIPoint(RectTransform rt, Vector2 screenPoint)
        {
            Vector3 globalMousePos;
            //UI��Ļ����ת��Ϊ��������
            Camera uiCamera = main;

            // �� Canvas renderMode Ϊ RenderMode.ScreenSpaceCamera��RenderMode.WorldSpace ʱ uiCamera ����Ϊ��
            // �� Canvas renderMode Ϊ RenderMode.ScreenSpaceOverlay ʱ uiCamera ����Ϊ��
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out globalMousePos);
            // ת����� globalMousePos ʹ�����淽����ֵ
            // target Ϊ��Ҫʹ�õ� UI RectTransform
            // rt ������ target.GetComponent<RectTransform>(), Ҳ������ target.parent.GetComponent<RectTransform>()
            // target.transform.position = globalMousePos;
            return globalMousePos;
        }

        // ��Ļ����ת��Ϊ UGUI RectTransform �� anchoredPosition
        public static Vector2 ScreenPointToUILocalPoint(RectTransform parentRT, Vector2 screenPoint)
        {
            Vector2 localPos;
            Camera uiCamera = main;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screenPoint, uiCamera, out localPos);
            // ת����� localPos ʹ�����淽����ֵ
            // target Ϊ��Ҫʹ�õ� UI RectTransform
            // parentRT �� target.parent.GetComponent<RectTransform>()
            // ���ֵ target.anchoredPosition = localPos;
            return localPos;
        }
    }
}