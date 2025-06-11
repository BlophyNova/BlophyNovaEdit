using Data.Enumerate;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Edit.Settings.Option
{
    public class Back2Select : PublicButton
    {
        // Start is called before the first frame update
        void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.currentHard = Hard.Hard;
                SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            });
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
