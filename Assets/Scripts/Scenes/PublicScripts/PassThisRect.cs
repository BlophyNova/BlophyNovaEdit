using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThisRect : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsPass;
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) => IsPass;
}
