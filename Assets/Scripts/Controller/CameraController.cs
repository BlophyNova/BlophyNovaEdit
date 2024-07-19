using UnityEngine;
namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        private const float Is16To9 = 0.5625f;
        public Camera thisCamera;
        private void Start()
        {
            CalculatedScreenArea();
        }
        private void CalculatedScreenArea()
        {

            if (thisCamera.pixelHeight / (float)thisCamera.pixelWidth > Is16To9)
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
            float w = Screen.height / Is16To9 / Screen.width;
            const float h = 1;
            float x = (1 - w) / 2;
            const float y = 0;
            thisCamera.rect = new Rect(x, y, w, h);
        }
        private void HeightManyLong()
        {
            const float w = 1;
            float h = Screen.width * Is16To9 / Screen.height;
            const float x = 0;
            float y = (1 - h) / 2;
            thisCamera.rect = new Rect(x, y, w, h);
        }
    }
}
