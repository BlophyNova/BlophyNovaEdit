using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShortcutKeyManager : MonoBehaviour
{
    public InputActionAsset inputActionsAsset;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"{inputActionsAsset.ToJson()}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
