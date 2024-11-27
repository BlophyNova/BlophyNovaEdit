using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeatLine : MonoBehaviour
{
    public Image texture;
    public TextMeshProUGUI thisText;
    public Color color;
    public BPM thisBPM;
    public BeatLine Init(float currentBeats, BPM thisBPM)
    {
        this.thisBPM = new(thisBPM);
        float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(currentBeats);
        float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);
        thisText.text = $"{thisBPM.integer}\t";
        transform.localPosition = Vector2.up * positionY;
        if (thisBPM.molecule != 0)
        {
            RectTransform rt = texture.GetComponent<RectTransform>();
            rt.sizeDelta = new(rt.sizeDelta.x, 3);
            texture.color = color;
            thisText.text = string.Empty;
        }
        return this;
    }
}
