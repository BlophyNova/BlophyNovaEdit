using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Singleton;
using UnityEngine.InputSystem.Utilities;

public class ShortcutKeyManager : MonoBehaviourSingleton<ShortcutKeyManager>
{
    public InputActionAsset inputActionsAsset;
    public PlayerInput playerInput;

    public List<InputAction> EnabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(true);
    public List<InputAction> DisabledShortcurKeyEvents => GetAssignStateShortcutKeyEvents(false);

    // Start is called before the first frame update
    void Start()
    {

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            playerInput.actions = inputActionsAsset;
            Debug.Log($"{inputActionsAsset.ToJson()}");
            File.WriteAllText($"{Application.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED", inputActionsAsset.ToJson());
        }
        else
        {
            playerInput.actions = InputActionAsset.FromJson(File.ReadAllText($"{Application.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED"));
        }
    }
    private List<InputAction> GetAssignStateShortcutKeyEvents(bool state)
    {
        List<InputAction> assignStateActions = new();
        InputActionAsset @events = playerInput.actions;
        foreach (InputAction @event in @events)
            if (@event.enabled== state)
                assignStateActions.Add(@event);
        return assignStateActions;
    }
}
