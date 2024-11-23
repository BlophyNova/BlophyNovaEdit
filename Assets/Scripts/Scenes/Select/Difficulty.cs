using Data.Enumerate;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
    public TMP_Dropdown difficulty;
    private void Start()
    {
        difficulty.onValueChanged.AddListener((value) =>
        {
            GlobalData.Instance.currentHard = value switch
            {
                0 => Hard.Easy,
                1 => Hard.Normal,
                2 => Hard.Hard,
                3 => Hard.Ultra,
                4 => Hard.Special,
                _ => throw new System.Exception("没找到你想要创建的难度")
            };
        });
    }
}
