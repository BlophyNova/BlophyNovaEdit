using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Singleton;

namespace Scenes.Edit
{
    public class ShortcutKeyManager : MonoBehaviourSingleton<ShortcutKeyManager>
    {
        public InputActionAsset inputActionsAsset;
        public PlayerInput playerInput;

        public List<InputAction> EnabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(true);
        public List<InputAction> DisabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(false);
        public List<Action> started = new();
        public List<Action> performed = new();
        public List<Action> canceled = new();


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
        public void RegisterEvents(string actionNameOrId,Action<InputAction.CallbackContext> started,Action<InputAction.CallbackContext> performed,Action<InputAction.CallbackContext> canceled)
        {
            InputAction inputAction = playerInput.actions[actionNameOrId];
            inputAction.started += callbackContext => this.started.Add(() => started(callbackContext));
            inputAction.performed += callbackContext => this.performed.Add(()=>performed(callbackContext));
            inputAction.canceled += callbackContext => this.canceled.Add(()=>canceled(callbackContext)); 
            inputAction.Enable();
        }
        private void LateUpdate()
        {
            ExecuteEvents(started);
            ExecuteEvents(performed);
            ExecuteEvents(canceled);
        }
        void ExecuteEvents(List<Action> actions)
        {
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                actions[i]();
                actions.RemoveAt(i);
            }
        }
    }
}