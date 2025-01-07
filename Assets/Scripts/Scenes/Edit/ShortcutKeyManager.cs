using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Singleton;
using Application = UnityEngine.Application;

namespace Scenes.Edit
{
    public class ShortcutKeyManager : MonoBehaviourSingleton<ShortcutKeyManager>
    {
        public InputActionAsset inputActionsAsset;
        public PlayerInput playerInput;

        public List<InputAction> EnabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(true);
        public List<InputAction> DisabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(false);
        public KeyValueList<Action, InputAction.CallbackContext> started = new();
        public KeyValueList<Action, InputAction.CallbackContext> performed = new();
        public KeyValueList<Action, InputAction.CallbackContext> canceled = new();


        // Start is called before the first frame update
        private void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                playerInput.actions = inputActionsAsset;
                Debug.Log($"{inputActionsAsset.ToJson()}");
                File.WriteAllText($"{Application.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED",
                    inputActionsAsset.ToJson());
            }
            else
            {
                playerInput.actions = InputActionAsset.FromJson(
                    File.ReadAllText($"{Application.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED"));
            }
        }

        private List<InputAction> GetAssignStateShortcutKeyEvents(bool state)
        {
            List<InputAction> assignStateActions = new();
            InputActionAsset events = playerInput.actions;
            foreach (InputAction @event in events)
            {
                if (@event.enabled == state)
                {
                    assignStateActions.Add(@event);
                }
            }

            return assignStateActions;
        }
        public void RegisterEvents(string actionNameOrId, Action<InputAction.CallbackContext> started, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
        {
            InputAction inputAction = playerInput.actions[actionNameOrId];
            inputAction.started += callbackContext => this.started.Add(() => started(callbackContext), callbackContext);
            inputAction.performed += callbackContext => this.performed.Add(() => performed(callbackContext), callbackContext);
            inputAction.canceled += callbackContext => this.canceled.Add(() => canceled(callbackContext), callbackContext);
            inputAction.Enable();
        }
        private void LateUpdate()
        {
            ExecuteEvents(started);
            ExecuteEvents(performed);
            ExecuteEvents(canceled);
        }
        void ExecuteEvents(KeyValueList<Action, InputAction.CallbackContext> keyValueList)
        {
            for (int i = keyValueList.Count - 1; i >= 0; i--)
            {
                //这里放在众多被触发的快捷键中选择一个的逻辑，但为了暂时不因为报错导致无法继续运行，所以先放个try
                try
                {
                    keyValueList[i]();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
                }
                keyValueList.RemoveAt(i);
            }
        }
    }
}