using System;
using System.Collections.Generic;
using Scenes.DontDestroyOnLoad;
using TMPro;

namespace Scenes.PublicScripts
{
    public class Alert : PublicButton
    {
        private static readonly List<string> textList = new();
        private static readonly List<Action> actions = new();
        public TMP_Text content;

        private void Start()
        {
            thisButton.onClick.AddListener(() => { DisableAlert(); });
        }

        public static void EnableAlert(string text, Action action = null)
        {
            textList.Add(text);
            actions.Add(action);
            InstWindow(text);
        }

        private static void InstWindow(string text)
        {
            if (textList.Count == 1)
            {
                Alert a = Instantiate(GlobalData.Instance.alert);
                a.content.text = text;
            }
        }

        private static void EnableAlertWithoutAdd2List(string text)
        {
            InstWindow(text);
        }

        private void DisableAlert()
        {
            actions[0]?.Invoke();
            textList.RemoveAt(0);
            actions.RemoveAt(0);
            if (textList.Count != 0)
            {
                EnableAlertWithoutAdd2List(textList[0]);
            }

            Destroy(gameObject);
        }
    }
}