using Form.LabelWindow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class VisualEase : MonoBehaviour
    {
        public NotePropertyEdit notePropertyEdit;
        public RectTransform selfRectTransform;
        public RectTransform contentRectTransform;
        public RectTransform viewport;
        // Start is called before the first frame update
        void Start()
        {
            notePropertyEdit.labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged; 
            LabelWindow_onWindowSizeChanged();
        }

        private void LabelWindow_onWindowSizeChanged()
        {

            selfRectTransform.sizeDelta = new(viewport.rect.width, viewport.rect.width+250);
            contentRectTransform.sizeDelta = new(viewport.rect.width, viewport.rect.width);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}