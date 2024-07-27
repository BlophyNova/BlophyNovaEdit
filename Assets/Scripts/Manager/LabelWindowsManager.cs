using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Singleton;

public class LabelWindowsManager : MonoBehaviourSingleton<LabelWindowsManager>
{
    public List<LabelWindow> windows;
    public List<Color> colors;
    public bool[] usedColors;
    public LabelWindow currentFocusWindow;
    public LabelWindow labelWindowPrefab;
    public void SetFocusWindow(LabelWindow window)
    {
        SetAllWindowFocusAsFalse();
        Color.RGBToHSV(window.labelColor.color, out float H, out _, out float V);
        float S = 1;
        window.labelColor.color = Color.HSVToRGB(H, S, V);
        window.focus = true;
        window.labelWindowRect.anchoredPosition3D = new(window.labelWindowRect.anchoredPosition3D.x, window.labelWindowRect.anchoredPosition3D.y,-1);
        currentFocusWindow = window;
    }
    public void NewLabelWindow()
    {
        int unusedColorIndex= GetUnusedColor();
        if (unusedColorIndex < 0) 
        { 
            Alert.EnableAlert("已经达到了窗口上限，请关闭掉不需要的窗口后创建"); 
            return;
        }
        LabelWindow newItem=Instantiate(labelWindowPrefab,transform);
        newItem.labelColorIndex = unusedColorIndex;
        newItem.labelColor.color=GetColorWithIndex(newItem.labelColorIndex);
        newItem.labelWindowRect.anchorMin = Vector2.up;
        newItem.labelWindowRect.anchorMax = Vector2.up;
        newItem.labelWindowRect.pivot = Vector2.up;
        windows.Add(newItem);
        SetFocusWindow(newItem);
    }
    public int GetUnusedColor()
    {
        int res = -1;
        for (int i = 0; i < usedColors.Length; i++)
        {
            if (!usedColors[i])
            {
                res=i;
                usedColors[i]=true;
                break;
            }
        }
        return res;
    }
    public Color GetColorWithIndex(int index)
    {
        return colors[index];
    }
    public void SetUsedColor2Unused(int index)
    {
        usedColors[index] = false;
    }
    private void SetAllWindowFocusAsFalse()
    {
        foreach (var item in windows)
        {
            Color.RGBToHSV(item.labelColor.color, out float H, out _, out float V);
            float S = .3f;
            item.labelColor.color = Color.HSVToRGB(H, S, V);
            item.focus = false;
            item.labelWindowRect.anchoredPosition3D = new(item.labelWindowRect.anchoredPosition3D.x, item.labelWindowRect.anchoredPosition3D.y,0);
        }
    }
}
