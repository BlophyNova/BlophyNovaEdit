using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log($"{inputActionsAsset.ToJson()}");
        playerInput.actions = inputActionsAsset;

        //InputAction hit = playerInput.actions["Fire"];
        //InputAction move = playerInput.actions["Move"];

        // 手动注册回调函数
        //hit.started += OnFireStarted;
        //hit.performed += OnFirePerformed;
        //hit.canceled += OnFireCanceled;
        //move.started += OnMoveStarted;
        //move.performed += OnMovePerformed;
        //move.canceled += OnMoveCanceled;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
