using System;
using System.Collections;
using System.Collections.Generic;
using Data.ChartEdit;
using Data.Interface;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;

namespace Form.PropertyEdit
{
    public class Father : MonoBehaviour,IRefreshEdit
    {

        public int currentBoxID;
        public int currentLineID;

        public List<Box> boxes => GlobalData.Instance.chartEditData.boxes;
        
        
        public TMP_Dropdown father;
        private void Start()
        {
            father.onValueChanged.AddListener(FatherValueChanged);
            UpdateFatherList();
        }

        private void FatherValueChanged(int value)
        {
            boxes[currentBoxID].parentId = boxes[value].id;
            boxes[value].childrenIds.Add(boxes[currentBoxID].id);
        }

        private void UpdateFatherList()
        {
            father.options.Clear();
            father.AddOptions(new List<string>(){"不继承"});
            for (int index = 0; index < boxes.Count; index++)
            {
                Box box = boxes[index];
                if (box.parentId==string.Empty)
                {
                    if(index==currentBoxID)continue;
                    father.AddOptions(new List<string>(){$"{index}"});
                }
            }

            if (boxes[currentBoxID].parentId == string.Empty)
            {
                father.SetValueWithoutNotify(0);
            }
            else
            {
                for (int i = 0; i < boxes.Count; i++)
                {
                    Box box = boxes[i];
                    if (boxes[currentBoxID].parentId == box.id)
                    {
                        father.SetValueWithoutNotify(i);
                        break;
                    }
                }
            }
        }

        public void RefreshEdit(int lineID, int boxID)
        {
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID;
            UpdateFatherList();
        }
    }
}
