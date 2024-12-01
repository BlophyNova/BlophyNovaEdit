using UnityEngine;
using UnityEngine.UI;

namespace Form.EventEdit
{
    public class EventLineRenderer : MonoBehaviour, ICanvasRaycastFilter
    {
        public RectTransform lineRendererTextureRect;
        public RawImage rawImage;
        public RenderTexture lineRenderTexture;

        private void Start()
        {
            //Texture texture = linerRenderTexture;
            //Texture2D texture2D = new(texture.width, texture.height, TextureFormat.RGBA32, false);
            //Sprite s = Sprite.Create(texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            //image.sprite = s;
            rawImage.texture = lineRenderTexture;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return false;
        }
    }
}