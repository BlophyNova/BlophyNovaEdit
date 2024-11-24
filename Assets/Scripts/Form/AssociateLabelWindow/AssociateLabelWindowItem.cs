using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssociateLabelWindowItem : MonoBehaviour
{
    public Image colorImage;
    public TMP_Dropdown dropdown;
    public TMP_Text text;
    public LabelWindow labelWindow;
    private void Start()
    {
        dropdown.onValueChanged.AddListener((index) =>
        {
            for (int i = 0; i < LabelWindowsManager.Instance.windows.Count; i++)
            {
                if (LabelWindowsManager.Instance.windows[i].labelColorIndex == index)
                {
                    labelWindow.associateLabelWindow = LabelWindowsManager.Instance.windows[i];
                }
            }

        });
    }
    private void OnEnable()
    {
        dropdown.options.Clear();
        for (int i = 0; i < LabelWindowsManager.Instance.windows.Count; i++)
        {
            if (LabelWindowsManager.Instance.usedColors[LabelWindowsManager.Instance.windows[i].labelColorIndex])
            {
                dropdown.options.Add(new($"窗口{LabelWindowsManager.Instance.windows[i].labelColorIndex}"));
            }
        }
        dropdown.options.Add(new("选择窗口"));
        dropdown.SetValueWithoutNotify(dropdown.options.Count - 1);
        if (labelWindow.associateLabelWindow != null)
        {
            for (int i = 0; i < LabelWindowsManager.Instance.windows.Count; i++)
            {
                LabelWindow item = LabelWindowsManager.Instance.windows[i];
                if (item == labelWindow.associateLabelWindow)
                {
                    dropdown.SetValueWithoutNotify(i);
                }
            }
        }
    }
}
