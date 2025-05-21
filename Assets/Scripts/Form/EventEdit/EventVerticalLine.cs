using TMPro;
using UnityEngine;
using EventType = Data.Enumerate.EventType;

namespace Form.EventEdit
{
    public class EventVerticalLine : MonoBehaviour
    {
        public TextMeshProUGUI displayEventTypeName;
        public TextMeshProUGUI displayCurrentEventValue;

        public EventType eventType;

        // Start is called before the first frame update
        private void Start()
        {
            displayEventTypeName.text = eventType.ToString();
        }
    }
}