using Controller;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UtilityCode.ObjectPool
{
    [Serializable]
    public abstract class ObjectPoolBase<T> where T : MonoBehaviour
    {
        protected Transform Parent;
        protected T PoolObject;
        protected int SortSeed;

        protected ObjectPoolBase(T @object, int poolLength, int sortSeed, Transform parent = null)
        {
            Parent = parent;
            SortSeed = sortSeed;
            PoolObject = @object;
        }

        protected ObjectPoolBase(T @object, int poolLength, Transform parent = null)
        {
            Parent = parent;
            PoolObject = @object;
        }

        protected T CreateNote()
        {
            T obj = Object.Instantiate(PoolObject, Vector3.zero, Quaternion.identity,
                Parent == null ? PoolObject.transform : Parent);
            NoteController note = obj.GetComponent<NoteController>();
            for (int i = 0; i < note.renderOrder.Count; i++)
            {
                foreach (SpriteRenderer item in note.renderOrder[i].tierCount)
                {
                    item.sortingOrder = SortSeed + 1 + i;
                }
            }

            obj.gameObject.SetActive(false);
            return obj;
        }

        protected virtual T GetNote()
        {
            return null;
        }

        public virtual void ReturnNote(T obj)
        {
        }

        protected virtual T GetObject()
        {
            return null;
        }

        public virtual void ReturnObject(T obj)
        {
        }

        protected T CreateObject()
        {
            T obj = Object.Instantiate(PoolObject, Vector3.zero, Quaternion.identity,
                !Parent ? PoolObject.transform : Parent);
            obj.gameObject.SetActive(false);
            return obj;
        }
    }
}