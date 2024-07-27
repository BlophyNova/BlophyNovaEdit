using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AssemblySystem
{
    public static void CallAllMethodWithInterfaceName(string interfaceName,string methodName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] exportedTypes = assembly.ExportedTypes.ToArray();
        Type targetInterface = null;
        for (int i = 0; i < exportedTypes.Length; i++)
        {
            Type item = exportedTypes[i];
            if (item.FullName == interfaceName && item.IsInterface)
            {
                targetInterface = item;
            }
        }
        for (int i = 0; i < exportedTypes.Length; i++)
        {
            Type item = exportedTypes[i];
            if(targetInterface.IsAssignableFrom(item))
            {
                item.GetMethod(methodName).Invoke(null,null);
            }
        }
    }
    public static List<T> FindAllInterfaceByTypes<T>()
    {
        List<T> interfaces = new();

        IEnumerable<T> types = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<T>();

        foreach (T t in types)
        {
            interfaces.Add(t);
        }

        return interfaces;
    }
    public static void Exe<T>(List<T> values,Action<T> action)
    {
        foreach (T item in values)
        {
            action?.Invoke(item);
        }
    }
}
