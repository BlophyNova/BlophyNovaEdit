using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class BoxList : LabelWindowContent,IRefresh
{

    public BoxListItem boxListItemPrefabs;
    public List<BoxListItem> boxListItems;
    public GridLayoutGroup gridLayoutGroup;
    private void Start()
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
            newItem.boxIDText.text = $"{i}号方框";
            newItem.boxList = this;
            newItem.thisBox = GlobalData.Instance.chartEditData.boxes[i];
            boxListItems.Add(newItem);
        }
    }
    private void OnEnable()
    {
        UpdateAera();
    }
    public override void WindowSizeChanged()
    {
        base.WindowSizeChanged();
        UpdateAera();
    }
    public void UpdateAera()
    {

        gridLayoutGroup.cellSize = new Vector2(labelWindow.labelWindowRect.sizeDelta.x * .8f, gridLayoutGroup.cellSize.y);
    }
}
