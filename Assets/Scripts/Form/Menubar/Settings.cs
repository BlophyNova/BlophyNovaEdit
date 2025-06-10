using Scenes.PublicScripts;
using UnityEngine;

namespace Form.Menubar
{
    public class Settings : PublicButton
    {
        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(()=>Scenes.Edit.Settings.Settings.Instance.gameObject.SetActive(true));
            Scenes.Edit.Settings.Settings.Instance.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
