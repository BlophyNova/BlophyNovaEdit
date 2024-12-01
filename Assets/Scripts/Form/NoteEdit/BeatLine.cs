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
        public TextMeshProUGUI thisText;
        public Color color;
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
                RectTransform rt = texture.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 3);
                texture.color = color;
                thisText.text = string.Empty;
            }

            return this;
        }
    }
}