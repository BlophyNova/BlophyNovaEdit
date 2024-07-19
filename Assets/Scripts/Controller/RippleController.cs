using Manager;
using UnityEngine;
namespace Controller
{
    public class RippleController : MonoBehaviour
    {
        public SpriteRenderer[] texture;
        public int textureLength = -1;
        public int TextureLength
        {
            get
            {
                if (textureLength < 0) textureLength = texture.Length;
                return textureLength;
            }
        }
        public Color currentColor = Color.black * .5f;
        public RippleController Init(float currentScaleX, float currentScaleY)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            currentColor.a = .5f;
            texture[0].transform.localScale =//第12根线都是水平的
                texture[1].transform.localScale =
                    new Vector2(2 - (ValueManager.Instance.boxFineness / currentScaleX), ValueManager.Instance.boxFineness / currentScaleY);

            texture[2].transform.localScale =//第34都是垂直的
                texture[3].transform.localScale =
                    new Vector2(2 + (ValueManager.Instance.boxFineness / currentScaleY), ValueManager.Instance.boxFineness / currentScaleX);
            return this;
        }
        private void Update()
        {
            transform.localScale += Vector3.one * Time.deltaTime;
            currentColor.a -= Time.deltaTime / 2;
            //下面这段代码如何优化才可以获得更快的运算效率
            texture[0].color =
                texture[1].color =
                    texture[2].color =
                        texture[3].color = currentColor;
        }
    }
}
