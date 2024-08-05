using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerticalLineCount : MonoBehaviour
{
    public TMP_Text thisText;
    public Button add;
    public Button subtraction;
    private void Start()
    {
        add.onClick.AddListener(() => 
        {
            GlobalData.Instance.chartEditData.verticalSubdivision++;
            GlobalData.Instance.chartEditData.eventVerticalSubdivision++;
            thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.verticalSubdivision}";

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        });
        subtraction.onClick.AddListener(() => 
        {
            GlobalData.Instance.chartEditData.verticalSubdivision--;
            GlobalData.Instance.chartEditData.eventVerticalSubdivision--;
            thisText.text = $"垂直线：{GlobalData.Instance.chartEditData.verticalSubdivision}";

            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
        });
    }
}
