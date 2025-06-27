using System;
using System.IO;
using System.Text;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine.InputSystem;

namespace ShortcutKey.Events
{
    public class Save : ShortcutKeyEventBase
    {
        private void Start()
        {
            Init();
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);
            AutoSave.Instance.Save();
        }
    }
}