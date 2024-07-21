using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class AddLabel : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Transform contentParentTransform;
    public Transform labelParentTransform;
    private void Start()
    {
        dropdown.ClearOptions();
        List<string> labelWindowNames = new();
        labelWindowNames.Add("请选择您想添加的标签窗口");
        foreach (LabelWindowContent item in GlobalData.Instance.labelWindowContents)
        {
            labelWindowNames.Add(item.labelWindowName);
        }
        dropdown.AddOptions(labelWindowNames);

        dropdown.onValueChanged.AddListener((index) => 
        {
            LabelItem newItem= Instantiate(GlobalData.Instance.labelItemPrefab, labelParentTransform);
            newItem.labelName.text = GlobalData.Instance.labelWindowContents[index-1].labelWindowName;
            LabelWindowContent newContent = Instantiate(GlobalData.Instance.labelWindowContents[index - 1], contentParentTransform);
            newItem.labelWindowContent = newContent;
            dropdown.SetValueWithoutNotify(0);
        });
    }
}
