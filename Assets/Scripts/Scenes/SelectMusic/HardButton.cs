using Scenes.DontDestoryOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Scenes.SelectMusic
{
    public class HardButton : MonoBehaviour
    {
        public Button thisButton;
        public HardButton[] otherButton;
        public TextMeshProUGUI thisText;
        public TextMeshProUGUI[] otherText;
        public Image texture;
        public Color selectedColor;
        public Color unselectColor;
        public string hardLevel;
        public bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                switch (isSelected)
                {
                    case true:
                        ColorBlock selectedColor = new();
                        selectedColor.normalColor =
                            selectedColor.highlightedColor =
                                selectedColor.pressedColor =
                                    selectedColor.disabledColor =
                                        selectedColor.selectedColor = this.selectedColor;
                        selectedColor.colorMultiplier = 1;
                        selectedColor.fadeDuration = .1f;
                        thisButton.colors = selectedColor;
                        thisText.color = Color.black;
                        for (int i = 0; i < otherButton.Length; i++)
                        {
                            otherButton[i].IsSelected = false;
                            otherText[i].color = Color.white;
                        }
                        GlobalData.Instance.currentHard = hardLevel;
                        break;
                    case false:
                        ColorBlock unselectColor = new();
                        unselectColor.normalColor =
                            unselectColor.highlightedColor =
                                unselectColor.pressedColor =
                                    unselectColor.disabledColor =
                                        unselectColor.selectedColor = this.unselectColor;
                        unselectColor.colorMultiplier = 1;
                        unselectColor.fadeDuration = .1f;
                        thisButton.colors = unselectColor;
                        break;
                }
            }
        }
        private void Start()
        {
            IsSelected = isSelected;
            thisButton.onClick.AddListener(() =>
            {
                if (isSelected) return;
                IsSelected = !IsSelected;
            });
        }
    }
}
