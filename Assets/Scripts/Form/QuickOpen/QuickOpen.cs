using System.Collections.Generic;
using Form.LabelWindow;
using Scenes.Edit.Settings;
using UnityEngine.UI;

namespace Form.QuickOpen
{
    public class QuickOpen : LabelWindowContent
    {
        public QuickOpenItem quickOpenItemPrefab;
        public List<QuickOpenItem> quickOpenItems = new();
        public UIAdaptation content;
        private void Start()
        {
            labelItem.onLabelGetFocus += UpdateContent;
            UpdateContent();
        }

        private void UpdateContent()
        {
            for (int i = quickOpenItems.Count - 1; i >= 0; i--)
            {
                Destroy(quickOpenItems[i].gameObject);
            }
            quickOpenItems.Clear();
            foreach (LabelItem labelItem in labelWindow.labels)
            {
                if (labelItem == base.labelItem)
                {
                    continue;
                }

                QuickOpenItem newInst= Instantiate(quickOpenItemPrefab, content.transform);
                LabelWindowContent labelWindowContent = labelItem.labelWindowContent;
                newInst.text.text = labelWindowContent.labelWindowName;
                newInst.thisButton.onClick.AddListener(() =>
                {
                    labelWindowContent.labelItem.labelButton.ThisLabelGetFocus();
                });
                quickOpenItems.Add(newInst);
            }
        }

        private T GetT<T>() where T : LabelWindowContent
        {
            foreach (LabelItem labelItem in labelWindow.labels)
            {
                if (labelItem.labelWindowContent is T content)
                {
                    return content;
                }
            }

            return null;
        }
    }
}