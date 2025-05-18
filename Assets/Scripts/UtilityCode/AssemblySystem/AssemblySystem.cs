using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UtilityCode.AssemblySystem
{
    public class AssemblySystem
    {
        public static void CallAllMethodWithInterfaceName(string interfaceName, string methodName)
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
                if (targetInterface.IsAssignableFrom(item))
                {
                    item.GetMethod(methodName).Invoke(null, null);
                }
            }
        }

        public static List<T> FindAllInterfaceByTypes<T>()
        {
            List<T> interfaces = new();

            IEnumerable<T> types = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>();

            foreach (T t in types)
            {
                interfaces.Add(t);
            }

            return interfaces;
        }

        public static void Exe<T>(List<T> values, Action<T> action,List<Type> types, bool isBlackList)
        {
            foreach (T item in values)
            {
                if(ExeAllMethod(action, types, item)) continue;
                if (isBlackList)
                {
                    bool isInBlackList = false;
                    foreach (Type type in types)
                    {
                        if (item.GetType() == type)
                        {
                            isInBlackList = true;
                        }
                    }

                    if (isInBlackList)
                    {
                        continue;
                    }
                    else
                    {
                        action?.Invoke(item);
                    }
                }
                else
                {
                    ExeSpecificMethodWithWhiteList(action, types, item);
                }
            }
        }
        private static void ExeSpecificMethodWithWhiteList<T>(Action<T> action, List<Type> types, T item)
        {
            foreach (Type type in types)
            {
                if (item.GetType() == type)
                {
                    action?.Invoke(item);
                    break;

                }
            }
        }

        private static bool ExeAllMethod<T>(Action<T> action, List<Type> types, T item)
        {
            if (types == null)
            {
                action?.Invoke(item);
                return true;
            }
            return false;
        }
    }
}