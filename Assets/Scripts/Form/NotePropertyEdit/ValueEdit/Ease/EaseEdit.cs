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

        public VisualEase visualEase;

        public delegate void OnValueChanged(int value);
        public event OnValueChanged onValueChanged= value => { };
        
        private void Start()
        {
            easeStyle.onValueChanged.AddListener(EaseStyleChanged);
            easeIndex.onEndEdit.AddListener(EaseIndexChanged);
            easeIO.onValueChanged.AddListener(EaseIoChanged);
            easeOption.onValueChanged.AddListener(EaseOptionChanged);
        }
        public void SetValueWithoutNotify(int value)
        {
            if (value is < 0 or > 30) return;
            Index2IO(value, out int io, out int option);
            easeIndex.SetTextWithoutNotify($"{value}");
            easeIO.SetValueWithoutNotify(io);
            easeOption.SetValueWithoutNotify(option);
            //onValueChanged(value);
        }
        private void EaseOptionChanged(int value)
        {
            if (value is < 0 or > 30)return;
            IO2Index(easeIO.value,easeOption.value,out int index);
            easeIndex.SetTextWithoutNotify($"{index}");
            onValueChanged(index);
        }

        private void EaseIoChanged(int value)
        {
            if (value is < 0 or > 2)return;
            IO2Index(easeIO.value,easeOption.value,out int index);
            easeIndex.SetTextWithoutNotify($"{index}");
            onValueChanged(index);
        }

        private void EaseIndexChanged(string value)
        {
            if (!int.TryParse(value, out int result))return;
            if (result is < 0 or > 30)return;
            Index2IO(result,out int io,out int option);
            easeIO.SetValueWithoutNotify(io);
            easeOption.SetValueWithoutNotify(option);
            onValueChanged(result);
        }
        /*
        
            0=IN           InSine = 1,          InExpo = 16,
            1=OUT          OutSine = 2,         OutExpo = 17,
            2=I/O          InOutSine = 3,       InOutExpo = 18,
                           InQuad = 4,          InCirc = 19,
                           OutQuad = 5,         OutCirc = 20,
            1=Sine         InOutQuad = 6,       InOutCirc = 21,
            2=Quad         InCubic = 7,         InElastic = 22,
            3=Cubic        OutCubic = 8,        OutElastic = 23,
            4=Quart        InOutCubic = 9,      InOutElastic = 24,
            5=Quint        InQuart = 10,        InBack = 25,
            6=Expo         OutQuart = 11,       OutBack = 26,
            7=Circ         InOutQuart = 12,     InOutBack = 27,
            8=Elastic      InQuint = 13,        InBounce = 28,
            9=Back         OutQuint = 14,       OutBounce = 29,
            10=Bounce      InOutQuint = 15,     InOutBounce = 30
        
        
         */
        void Index2IO(int index,out int io,out int @option)
        {
            io = option = 0;
            switch (index)
            {
                case 0:
                    return;
                case 1:
                    io = 0; option = 1;
                    break;
                case 2:
                    io = 1; option = 1;
                    break;
                case 3:
                    io = 2; option = 1;
                    break;
                case 4:
                    io = 0; option = 2;
                    break;
                case 5:
                    io = 1; option = 2;
                    break;
                case 6:
                    io = 2; option = 2;
                    break;
                case 7:
                    io = 0; option = 3;
                    break;
                case 8:
                    io = 1; option = 3;
                    break;
                case 9:
                    io = 2; option = 3;
                    break;
                case 10:
                    io = 0; option = 4;
                    break;
                case 11:
                    io = 1; option = 4;
                    break;
                case 12:
                    io = 2; option = 4;
                    break;
                case 13:
                    io = 0; option = 5;
                    break;
                case 14:
                    io = 1; option = 5;
                    break;
                case 15:
                    io = 2; option = 5;
                    break;
                case 16:
                    io = 0; option = 6;
                    break;
                case 17:
                    io = 1; option = 6;
                    break;
                case 18:
                    io = 2; option = 6;
                    break;
                case 19:
                    io = 0; option = 7;
                    break;
                case 20:
                    io = 1; option = 7;
                    break;
                case 21:
                    io = 2; option = 7;
                    break;
                case 22:
                    io = 0; option = 8;
                    break;
                case 23:
                    io = 1; option = 8;
                    break;
                case 24:
                    io = 2; option = 8;
                    break;
                case 25:
                    io = 0; option = 9;
                    break;
                case 26:
                    io = 1; option = 9;
                    break;
                case 27:
                    io = 2; option = 9;
                    break;
                case 28:
                    io = 0; option = 10;
                    break;
                case 29:
                    io = 1; option = 10;
                    break;
                case 30:
                    io = 2; option = 10;
                    break;
            }
        }

        void IO2Index(int io,int option,out int index)
        {
            index = 0;
            switch (io)
            {
                case 0 when option == 0:
                    return;
                case 0 when option == 1:
                    index = 1;
                    break;
                case 1 when option == 1:
                    index = 2;
                    break;
                case 2 when option == 1:
                    index = 3;
                    break;
                case 0 when option == 2:
                    index = 4;
                    break;
                case 1 when option == 2:
                    index = 5;
                    break;
                case 2 when option == 2:
                    index = 6;
                    break;
                case 0 when option == 3:
                    index = 7;
                    break;
                case 1 when option == 3:
                    index = 8;
                    break;
                case 2 when option == 3:
                    index = 9;
                    break;
                case 0 when option == 4:
                    index = 10;
                    break;
                case 1 when option == 4:
                    index = 11;
                    break;
                case 2 when option == 4:
                    index = 12;
                    break;
                case 0 when option == 5:
                    index = 13;
                    break;
                case 1 when option == 5:
                    index = 14;
                    break;
                case 2 when option == 5:
                    index = 15;
                    break;
                case 0 when option == 6:
                    index = 16;
                    break;
                case 1 when option == 6:
                    index = 17;
                    break;
                case 2 when option == 6:
                    index = 18;
                    break;
                case 0 when option == 7:
                    index = 19;
                    break;
                case 1 when option == 7:
                    index = 20;
                    break;
                case 2 when option == 7:
                    index = 21;
                    break;
                case 0 when option == 8:
                    index = 22;
                    break;
                case 1 when option == 8:
                    index = 23;
                    break;
                case 2 when option == 8:
                    index = 24;
                    break;
                case 0 when option == 9:
                    index = 25;
                    break;
                case 1 when option == 9:
                    index = 26;
                    break;
                case 2 when option == 9:
                    index = 27;
                    break;
                case 0 when option == 10:
                    index =28;
                    break;
                case 1 when option == 10:
                    index =29;
                    break;
                case 2 when option == 10:
                    index =30;
                    break;
            }
        }
        private void EaseStyleChanged(int value)
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
        }
    }
}