using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Singleton;
using static UnityEditor.Progress;

public class LabelWindowsManager : MonoBehaviourSingleton<LabelWindowsManager>
{
    public List<LabelWindow> windows;
    public List<Color> colors;
    public bool[] usedColors;
    public void SetFocusWindow(LabelWindow window)
    {
        SetAllWindowFocusAsFalse();
        Color.RGBToHSV(window.labelColor.color, out float H, out _, out float V);
        float S = 1;
        window.labelColor.color = Color.HSVToRGB(H, S, V);
        window.focus = true;
    }

    private void SetAllWindowFocusAsFalse()
    {
        foreach (var item in windows)
        {
            Color.RGBToHSV(item.labelColor.color, out float H, out _, out float V);
            float S = .3f;
            item.labelColor.color = Color.HSVToRGB(H, S, V);
            item.focus = false;
        }
    }
}
