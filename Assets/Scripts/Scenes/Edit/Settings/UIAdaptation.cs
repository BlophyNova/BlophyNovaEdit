using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Edit.Settings
{
    public class UIAdaptation : MonoBehaviour
    {
        public GridLayoutGroup gridLayoutGroup;
        public RectTransform rectTransform;

        private void OnEnable()
        {
            gridLayoutGroup.cellSize = new(rectTransform.rect.width,gridLayoutGroup.cellSize.y);
        }
    }
}
