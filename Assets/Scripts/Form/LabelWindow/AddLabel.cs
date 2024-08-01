using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AddLabel : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Transform contentParentTransform;
    public Transform labelParentTransform;
    public LabelWindow labelWindow;
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
            foreach (LabelItem item in labelWindow.labels)
            {
                if(1<<(index-1)  == ((int)item.labelWindowContent.labelWindowContentType))
                {
                    dropdown.SetValueWithoutNotify(0);
                    Alert.EnableAlert("这个窗口已经有一个相同的标签了捏~");
                    return;
                }
            }

            LabelItem newItem= Instantiate(GlobalData.Instance.labelItemPrefab, labelParentTransform);
            newItem.labelName.text = GlobalData.Instance.labelWindowContents[index-1].labelWindowName;
            LabelWindowContent newContent = Instantiate(GlobalData.Instance.labelWindowContents[index - 1], contentParentTransform);
            newContent.labelWindow = labelWindow;
            labelWindow.currentLabelWindow = newContent;
            newItem.labelWindowContent = newContent;
            newItem.labelWindow = labelWindow;
            newItem.closeThisLabel.labelWindow = labelWindow;
            newItem.labelWindowContent.gameObject.SetActive(true);
            foreach (var item in labelWindow.labels)
            {
                item.labelWindowContent.gameObject.SetActive(false);
            }
            labelWindow.labels.Add(newItem);
            dropdown.SetValueWithoutNotify(0);
        });
    }
}
