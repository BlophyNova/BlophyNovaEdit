using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class EventEdit
{
    void SelectBoxDown()
    {
        selectBox.isPressing = true;
        Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
    }
    void SelectBoxUp()
    {
        selectBox.isPressing = false;
        Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
    }
}
