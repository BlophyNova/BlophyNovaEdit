using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssociateLabelWindow : LabelWindowContent
{
    public AssociateLabelWindowItem associateLabelWindowItem;
    public GridLayoutGroup gridLayoutGroup;
    public List<AssociateLabelWindowItem> associateLabels;
    private void OnEnable()
    {
        Refresh();
    }
    public void Refresh()
    {
        foreach (AssociateLabelWindowItem item in associateLabels)
        {
            Destroy(item.gameObject);
        }
        associateLabels.Clear();
        for (int i = 0; i < LabelWindowsManager.Instance.windows.Count; i++)
        {
            AssociateLabelWindowItem newAssociateLabelWindowItem = Instantiate(associateLabelWindowItem, gridLayoutGroup.transform);
            newAssociateLabelWindowItem.colorImage.color = LabelWindowsManager.Instance.GetColorWithIndex(LabelWindowsManager.Instance.windows[i].labelColorIndex);
            newAssociateLabelWindowItem.text.text = $"窗口{LabelWindowsManager.Instance.windows[i].labelColorIndex}：指向->";
            newAssociateLabelWindowItem.labelWindow = LabelWindowsManager.Instance.windows[i];
            newAssociateLabelWindowItem.gameObject.SetActive(true);
            associateLabels.Add(newAssociateLabelWindowItem);
        }
    }
}
