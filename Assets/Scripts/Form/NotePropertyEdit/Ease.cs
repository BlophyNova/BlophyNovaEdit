using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Form.NotePropertyEdit
{
    public class Ease : MonoBehaviour
    {
        public TMP_Dropdown ease;
        private void Start()
        {
            ease.ClearOptions();
            for (int i = 0; i < GlobalData.Instance.easeData.Count; i++)
            {
                ease.options.Add(new($"{GlobalData.Instance.easeData[i].easeType}"));
            }
        }
    }
}