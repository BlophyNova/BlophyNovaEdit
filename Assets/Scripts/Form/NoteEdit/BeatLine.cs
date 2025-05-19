using Data.ChartEdit;
using Form.PropertyEdit;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.NoteEdit
{
    public class BeatLine : MonoBehaviour
    {
        public Image texture;
        public RectTransform selfRect;
        public TextMeshProUGUI thisText;
        public Color color;
        public Color bigColor;
        public BPM thisBPM;

        public BeatLine Init(float currentBeats, BPM thisBPM)
        {
            this.thisBPM = new BPM(thisBPM);
            float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(currentBeats);
            float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);
            thisText.text = $"{thisBPM.integer}\t";
            transform.localPosition = Vector2.up * positionY;
            if (thisBPM.molecule != 0)
            {
                //小节拍线的一些设置
                selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x, 3);
                texture.color = color;
                thisText.text = string.Empty;
            }
            else
            {
                selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x, 5);
                texture.color = bigColor;
            }

            return this;
        }
    }
}