using Data.ChartEdit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Form.BoxList
{
    public class BoxListItem : MonoBehaviour
    {
        public BoxList boxList;
        public TMP_Text boxIDText;
        public Button delete;
        public Box thisBox;
    }
}