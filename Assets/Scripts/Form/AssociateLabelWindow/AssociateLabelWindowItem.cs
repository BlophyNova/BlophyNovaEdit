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
    private void OnEnable()
    {
        dropdown.options.Clear();
        for (int i = 0; i < LabelWindowsManager.Instance.usedColors.Length; i++)
        {
            if (LabelWindowsManager.Instance.usedColors[i])
            {
                dropdown.options.Add(new($"窗口{i}"));
            }
        }
    }
}
