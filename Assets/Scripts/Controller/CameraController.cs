using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;
using static UnityEngine.Camera;
namespace Controller
{
    public class CameraController : MonoBehaviourSingleton<CameraController>
    {
        //public Camera otherCamera;
        private const float Is16To9 = 0.5625f;

        public int lastWidth;
        public int lastHeight;
        private void Start()
        {
            CameraAreaUpdate();
        }
        public void CameraAreaUpdate()
        {
            //if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                lastWidth = Screen.width;
                lastHeight = Screen.height;
                CalculatedScreenArea();
                AntLineController.Instance.InitAntLine();
            }
        }

        private void CalculatedScreenArea()
        {

            if (main.pixelHeight / (float)main.pixelWidth > Is16To9)
            {
                //这里放平板的处理方法
                //1024/16*9/768
                HeightManyLong();
            }
            else
            {
                WidthManyLong();
            }
        }
        private void WidthManyLong()
        {
            //1080/9*16/2400
            float w = (float)Screen.height/ (float)Screen.width/Is16To9;
            //Debug.Log($"Height:{Screen.height}|Width:{Screen.width}|{Screen.height / Screen.width / Is16To9}");
            float x = (1 - w) / 2;
            /*otherCamera.rect =*/ main.rect = new Rect(x, 0f, w, 1f);
            
        }
        private void HeightManyLong()
        {
            float h = Screen.width * Is16To9 / Screen.height;
            float y = (1 - h) / 2;
            /*otherCamera.rect =*/ main.rect = new Rect(0f, y, 1f, h);
        }
    }
}
