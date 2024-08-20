using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SelectBox : MonoBehaviour
{
    public NoteEdit noteEdit;
    public EventEdit eventEdit;
    public bool isPressing=false;
    public ISelectBox selectBoxObjects => noteEdit == null ? noteEdit : eventEdit;
    private void Start()
    {
        Debug.Log($"这里有问题，isPressing的正确性和窗口切换焦点事件被抢，导致对不上号");
    }
    private void Update()
    {
        if (isPressing)
        {
            Debug.Log($"SelectBox被按下，noteEdit={noteEdit}，eventEdit={eventEdit}");
        }
    }
}
