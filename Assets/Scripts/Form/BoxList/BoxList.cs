using Data.Interface;
using Form.LabelWindow;
using Scenes.DontDestroyOnLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Form.BoxList
{
    public class BoxList : LabelWindowContent, IRefresh
    {
        public BoxListItem boxListItemPrefabs;
        public List<BoxListItem> boxListItems;
        public GridLayoutGroup gridLayoutGroup;

        private void Start()
        {
            Refresh();
        }

        private void OnEnable()
        {
            UpdateAera();
        }
        public void Refresh()
        {
            foreach (BoxListItem item in boxListItems)
            {
                Destroy(item.gameObject);
            }

            boxListItems.Clear();
            for (int i = 0; i < GlobalData.Instance.chartEditData.boxes.Count; i++)
            {
                BoxListItem newItem = Instantiate(boxListItemPrefabs, gridLayoutGroup.transform);
                newItem.boxIDText.text = $"{i}号方框";
                newItem.boxList = this;
                newItem.thisBox = GlobalData.Instance.chartEditData.boxes[i];
                boxListItems.Add(newItem);
            }
        }

        public override void WindowSizeChanged()
        {
            base.WindowSizeChanged();
            UpdateAera();
        }

        public void UpdateAera()
        {
            gridLayoutGroup.cellSize =
                new Vector2(labelWindow.labelWindowRect.sizeDelta.x * .8f, gridLayoutGroup.cellSize.y);
        }

    }
}