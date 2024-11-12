using Fleck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeConvert
{
    public static bool TryConvert<T>(object obj, out T result) where T : class
    {
        result = default;
        if (obj is not T) return false;
        result = obj as T;
        return true;
    }
}
