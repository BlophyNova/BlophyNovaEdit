using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Edit.Settings
{
    public class UIAdaptation : MonoBehaviour
    {
        [Tooltip("这个指向content本身")]
        public GridLayoutGroup gridLayoutGroup;
        [Tooltip("这个指向view本身")]
        public RectTransform rectTransform;

        private void OnEnable()
        {
            gridLayoutGroup.cellSize = new(rectTransform.rect.width,gridLayoutGroup.cellSize.y);
        }
    }
}
