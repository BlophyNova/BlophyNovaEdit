using System.Collections.Generic;
using Scenes.PublicScripts;
using UnityEngine;

namespace Scenes.Edit.Settings.Option
{
    public class SwitchLabel : PublicButton
    {
        public RectTransform option;

        public List<RectTransform> otherOptions;
        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                option.gameObject.SetActive(true);
                foreach (RectTransform otherOption in otherOptions)
                {
                    otherOption.gameObject.SetActive(false);
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
