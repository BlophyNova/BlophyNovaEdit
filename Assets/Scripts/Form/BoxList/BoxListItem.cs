using Data.ChartEdit;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Form.BoxList
{
    public class BoxListItem : MonoBehaviour
    {
        public int currentBoxIndex;
        
        public BoxList boxList;
        [FormerlySerializedAs("boxIDText")] 
        public TMP_Text boxIndexText;
        public TMP_Text boxId;
        public Button delete;
        public Button up;
        public Button down;
        public Box thisBox;
    }
}