using Data.EaseData;
using Form.LabelWindow;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class VisualEase : MonoBehaviour
    {
        public NotePropertyEdit notePropertyEdit;
        public RectTransform selfRectTransform;
        public RectTransform contentRectTransform;
        public RectTransform viewport;
        public Line line;
        public RectTransform lineRectTransform;

        public Button create;
        public Button save;
        public Button delete;
        public Button exportToClipboard;
        public Button importFromClipboard;


        EaseEdit easeEdit => notePropertyEdit.EditEvent.easeEdit;
        EditEvent EditEvent => notePropertyEdit.EditEvent;
        bool isPresetEase => easeEdit.easeStyle.value == 0;

        // Start is called before the first frame update
        void Start()
        {
            notePropertyEdit.labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged; 
            LabelWindow_onWindowSizeChanged();

            easeEdit.easeStyle.onValueChanged.AddListener(value =>
            {
                if (value == 0)
                {
                    create.interactable = false;
                    save.interactable = false;
                    delete.interactable = false;
                    exportToClipboard.interactable = false;
                    importFromClipboard.interactable = false;
                }
                else
                {
                    create.interactable = true;
                    save.interactable = true;
                    delete.interactable = true;
                    exportToClipboard.interactable = true;
                    importFromClipboard.interactable = true;
                }
            });
            easeEdit.onValueChanged += EaseEdit_onValueChanged;
        }

        public void EaseEdit_onValueChanged(int value)
        {
            if (isPresetEase)
            {
                EaseData ease = GlobalData.Instance.easeDatas[value];
                Vector3[] positions = new Vector3[100];
                Vector3[] corners = new Vector3[4];
                lineRectTransform.GetLocalCorners(corners);
                for (int i = 0; i < positions.Length; i++)
                {
                    //positions[i].
                    Vector3 currentPosition = (corners[2] - corners[0]) * (i / (float)positions.Length) + corners[0];
                    currentPosition.y =
                        ease.thisCurve.Evaluate(i / (float)positions.Length) * (corners[2].y - corners[0].y) +
                        corners[0].y;
                    currentPosition.z = -1;
                    positions[i] = currentPosition;
                }
                line.UpdateDraw(100, positions);
            }
            else
            {
                //自定义曲线做什么事情
            }
        }

        private void LabelWindow_onWindowSizeChanged()
        {

            selfRectTransform.sizeDelta = new(viewport.rect.width, viewport.rect.width+250);
            contentRectTransform.sizeDelta = new(viewport.rect.width, viewport.rect.width);
        }
    }
}