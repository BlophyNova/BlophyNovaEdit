using Scenes.PublicScripts;
using UnityEngine;

namespace Scenes.Edit.Settings.Option
{
    public class Return : PublicButton
    {

        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(()=>Settings.Instance.gameObject.SetActive(false));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}