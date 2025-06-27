using Scenes.PublicScripts;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Edit.Settings.Option
{
    public class Return : PublicButton
    {

        public List<GameObject> canvas;
        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(() => 
            {
                foreach (var item in canvas) 
                {
                    item.SetActive(false);
                }
                Settings.Instance.gameObject.SetActive(false);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}