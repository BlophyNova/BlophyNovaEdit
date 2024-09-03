using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Singleton;

public class ShortcutKeyManager : MonoBehaviourSingleton<ShortcutKeyManager>
{
    public InputActionAsset inputActionsAsset;
    public PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log($"{inputActionsAsset.ToJson()}");
        //File.WriteAllText($"{Application.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED", inputActionsAsset.ToJson());

        //playerInput.actions = inputActionsAsset;
        playerInput.actions = InputActionAsset.FromJson(File.ReadAllText($"{Application.streamingAssetsPath}/Config/ShortcutKeyConfig.HuaWaterED"));
    }

}
