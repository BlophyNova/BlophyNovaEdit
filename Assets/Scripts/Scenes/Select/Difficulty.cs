using Scenes.DontDestroyOnLoad;
using System;
using TMPro;
using UnityEngine;

namespace Scenes.Select
{
    public class Difficulty : MonoBehaviour
    {
        public TMP_Dropdown difficulty;

        private void Start()
        {
            difficulty.onValueChanged.AddListener(value =>
            {
                GlobalData.Instance.currentHard = value switch
                {
                    0 => Data.Enumerate.Hard.Easy,
                    1 => Data.Enumerate.Hard.Normal,
                    2 => Data.Enumerate.Hard.Hard,
                    3 => Data.Enumerate.Hard.Ultra,
                    4 => Data.Enumerate.Hard.Special,
                    _ => throw new Exception("没找到你想要创建的难度")
                };
            });
        }
    }
}