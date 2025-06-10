using TMPro;
using UnityEngine;
using static UnityEngine.Camera;

namespace Scenes.PublicScripts
{
    public class ShowPointInGameView : MonoBehaviour
    {
        public TextMeshPro textMeshPro;
        public string lineID;
        public bool isShowText;

        private void Update()
        {
            if (isShowText)
            {
                Vector2 centerXY = main.WorldToViewportPoint(transform.position);
                int centerX = (int)(1600f * centerXY.x - 800f);
                int centerY = (int)(900f * centerXY.y - 450f);
                int moveX = (int)(transform.localPosition.x * 100f);
                int moveY = (int)(transform.localPosition.y * 100f);
                textMeshPro.text = $"({centerX},{centerY})\n{lineID}\n({moveX},{moveY})";
            }
            else
            {
                textMeshPro.text = $"{lineID}";
            }
        }
    }
}