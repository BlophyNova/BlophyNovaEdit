using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPMList : LabelWindowContent
{
    public BPMItem bpmItemPrefabs;
    public List<BPMItem> bpmItems;
    public GridLayoutGroup gridLayoutGroup;
    private void Start()
    {
        for (int i = 0; i <GlobalData.Instance.chartEditData.bpmList.Count; i++)
        {
            BPMItem newItem = Instantiate(bpmItemPrefabs, gridLayoutGroup.transform);
            newItem.myBPM = GlobalData.Instance.chartEditData.bpmList[i];
            newItem.bpmValue.text = newItem.myBPM.currentBPM.ToString();
            newItem.startBeats.text= $"{newItem.myBPM.integer}:{newItem.myBPM.molecule}/{newItem.myBPM.denominator}";
            bpmItems.Add(newItem);
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
