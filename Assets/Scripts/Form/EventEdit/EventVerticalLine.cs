using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventVerticalLine : MonoBehaviour
{
    public TMP_Text displayEventTypeName;
    public Data.Enumerate.EventType eventType;
    // Start is called before the first frame update
    void Start()
    {
        displayEventTypeName.text=eventType.ToString();
    }
}
