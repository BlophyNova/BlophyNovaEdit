using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hook;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.KeyValueList;
using UtilityCode.Singleton;
using Application = UnityEngine.Application;

namespace Scenes.Edit
{
    public class ShortcutKeyManager : MonoBehaviourSingleton<ShortcutKeyManager>
    {
        public InputActionAsset inputActionsAsset;
        public PlayerInput playerInput;
        public KeyValueList<Action, InputAction.CallbackContext> canceled = new();
        public KeyValueList<Action, InputAction.CallbackContext> performed = new();
        public KeyValueList<Action, InputAction.CallbackContext> started = new();

        public List<InputAction> EnabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(true);
        public List<InputAction> DisabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(false);


        // Start is called before the first frame update
        private void Start()
        {
            if (Application.isEditor)
            {
                playerInput.actions = inputActionsAsset;
                Debug.Log($"{inputActionsAsset.ToJson()}");
                File.WriteAllText(
                    new Uri($"{Applicationm.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED").LocalPath,
                    inputActionsAsset.ToJson(), Encoding.UTF8);
            }
            else
            {
                playerInput.actions = InputActionAsset.FromJson(
                    File.ReadAllText(
                        new Uri($"{Applicationm.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED").LocalPath,
                        Encoding.UTF8));
            }
        }

        private void LateUpdate()
        {
            ExecuteEvents(started);
            ExecuteEvents(performed);
            ExecuteEvents(canceled);
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

        public void RegisterEvents(string actionNameOrId, Action<InputAction.CallbackContext> started,
            Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
        {
            InputAction inputAction = playerInput.actions[actionNameOrId];
            inputAction.started += callbackContext => this.started.Add(() => started(callbackContext), callbackContext);
            inputAction.performed += callbackContext =>
                this.performed.Add(() => performed(callbackContext), callbackContext);
            inputAction.canceled += callbackContext =>
                this.canceled.Add(() => canceled(callbackContext), callbackContext);
            inputAction.Enable();
        }

        private void ExecuteEvents(KeyValueList<Action, InputAction.CallbackContext> keyValueList)
        {
            for (int i = keyValueList.Count - 1; i >= 0; i--)
            {
                //��������ڶ౻�����Ŀ�ݼ���ѡ��һ�����߼�����Ϊ����ʱ����Ϊ���������޷��������У������ȷŸ�try
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