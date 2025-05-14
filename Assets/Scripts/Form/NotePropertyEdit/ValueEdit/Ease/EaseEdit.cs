using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class EaseEdit : MonoBehaviour
    {
        public TMP_Dropdown easeStyle;

        #region 预设缓动
        public TMP_InputField easeIndex;
        public TMP_Dropdown easeIO;
        public TMP_Dropdown easeOption;
        #endregion

        #region 自定义缓动
        public TextMeshProUGUI customEaseNameText;
        public TMP_InputField customEaseName;
        public TMP_Dropdown customEaseOption;
        #endregion

        public delegate void OnValueChanged(int value);
        public event OnValueChanged onValueChanged= value => { };
        
        private void Start()
        {
            easeStyle.onValueChanged.AddListener(value =>
            {
                if (value == 0)
                {
                    customEaseNameText.gameObject.SetActive(false);
                    customEaseName.gameObject.SetActive(false);
                    customEaseOption.gameObject.SetActive(false);
                    easeIndex.interactable = true;
                    easeIO.gameObject.SetActive(true);
                    easeOption.gameObject.SetActive(true);
                }
                else
                {
                    customEaseNameText.gameObject.SetActive(true);
                    customEaseName.gameObject.SetActive(true);
                    customEaseOption.gameObject.SetActive(true);
                    easeIndex.interactable = false;
                    easeIO.gameObject.SetActive(false);
                    easeOption.gameObject.SetActive(false);
                }
            });
        }
    }
}