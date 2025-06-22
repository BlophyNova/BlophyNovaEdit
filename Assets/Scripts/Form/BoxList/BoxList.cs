using System.Collections.Generic;
using Data.Interface;
using Form.LabelWindow;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UnityEngine.UI;

namespace Form.BoxList
{
    public class BoxList : LabelWindowContent, IRefreshUI
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
            UpdateArea();
        }

        public void RefreshUI()
        {
            Refresh();
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
                newItem.currentBoxIndex = i;
                newItem.thisBox = GlobalData.Instance.chartEditData.boxes[i];
                newItem.boxIndexText.text = $"{i}号方框";
                newItem.boxId.text = newItem.thisBox.id;
                newItem.boxList = this;
                boxListItems.Add(newItem);
            }
            Destroy(boxListItems[0].up.gameObject);
            Destroy(boxListItems[^1].down.gameObject);
        }

        public override void WindowSizeChanged()
        {
            base.WindowSizeChanged();
            UpdateArea();
        }

        public void UpdateArea()
        {
            gridLayoutGroup.cellSize =
                new Vector2(labelWindow.labelWindowRect.sizeDelta.x * .8f, gridLayoutGroup.cellSize.y);
        }
    }
}