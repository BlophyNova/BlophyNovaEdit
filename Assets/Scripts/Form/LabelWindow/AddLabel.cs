using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Form.LabelWindow
{
    public class AddLabel : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        public Transform contentParentTransform;
        public Transform labelParentTransform;
        public LabelWindow labelWindow;

        private void Start()
        {
            dropdown.ClearOptions();
            List<string> labelWindowNames = new()
            {
                "请选择您想添加的标签窗口"
            };
            foreach (LabelWindowContent item in GlobalData.Instance.labelWindowContents)
            {
                labelWindowNames.Add(item.labelWindowName);
            }

            dropdown.AddOptions(labelWindowNames);

            dropdown.onValueChanged.AddListener(index =>
            {
                foreach (LabelItem item in labelWindow.labels)
                {
                    if (1 << (index - 1) == (int)item.labelWindowContent.labelWindowContentType)
                    {
                        dropdown.SetValueWithoutNotify(0);
                        Alert.EnableAlert("这个窗口已经有一个相同的标签了捏~");
                        return;
                    }
                }

                LabelItem newItem = Instantiate(GlobalData.Instance.labelItemPrefab, labelParentTransform);
                newItem.labelName.text = GlobalData.Instance.labelWindowContents[index - 1].labelWindowName;
                LabelWindowContent newContent = Instantiate(GlobalData.Instance.labelWindowContents[index - 1],
                    contentParentTransform);
                newContent.labelWindow = labelWindow;
                newContent.labelItem = newItem;
                labelWindow.currentLabelItem = newItem;
                newItem.labelWindowContent = newContent;
                newItem.labelWindow = labelWindow;
                newItem.closeThisLabel.labelWindow = labelWindow;
                labelWindow.labels.Add(newItem);
                labelWindow.labels[^1].labelButton.ThisLabelGetFocus();
                dropdown.SetValueWithoutNotify(0);
            });
        }
    }
}