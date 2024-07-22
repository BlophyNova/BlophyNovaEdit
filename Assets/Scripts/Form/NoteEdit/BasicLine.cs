using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicLine : MonoBehaviour
{
    public TMP_Text currentBeatsText;
    private void Update()
    {

        currentBeatsText.text = $"{BPMManager.Instance.GetCurrentBeatsWithSecondsTime(((float)ProgressManager.Instance.CurrentTime)):F2}\t";
    }
}
