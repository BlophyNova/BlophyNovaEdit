using Data.Enumerate;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Scenes.Select
{
    public class Hard : PublicButton
    {
        public Data.Enumerate.Hard hard;
        public List<Hard> otherButton = new();
        public Image image;
        public Color selectedColor;
        public Color unselectedColor;
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.currentHard = hard;
                image.color = selectedColor;
                foreach (Hard hard in otherButton)
                {
                    hard.image.color = unselectedColor;
                }
            });
        }
    }
}