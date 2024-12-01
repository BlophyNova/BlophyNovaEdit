using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityCode.ObjectPool
{
    /// <summary> 使用Queue队列的GameObject对象池 </summary>
    [Serializable]
    public class ObjectPoolQueue<T> : ObjectPoolBase<T> where T : MonoBehaviour
    {
        private readonly Queue<T> pool;

        public ObjectPoolQueue(T @object, int poolLength, int sortSeed, Transform parent = null) : base(@object,
            poolLength, sortSeed, parent)
        {
            pool = new Queue<T>();
            for (int i = 0; i < poolLength; i++)
            {
                T obj = CreateNote();
                pool.Enqueue(obj);
            }
        }

        public ObjectPoolQueue(T @object, int poolLength, Transform parent = null) : base(@object, poolLength, parent)
        {
            pool = new Queue<T>();
            for (int i = 0; i < poolLength; i++)
            {
                T obj = CreateObject();
                pool.Enqueue(obj);
            }
        }

        public int PoolLength => pool.Count;

        protected override T GetNote()
        {
            return pool.Count > 0 ? pool.Dequeue() : CreateNote();
            // 如果池子空了就重新创建物体
        }

        public T PrepareNote() // 取出物体
        {
            T obj = GetNote();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public override void ReturnNote(T obj) // 回收物体
        {
            obj.gameObject.SetActive(false);
            if (obj)
            {
                pool.Enqueue(obj);
            }
        }

        protected override T GetObject()
        {
            return pool.Count > 0 ? pool.Dequeue() : CreateObject();
            // 如果池子空了就重新创建物体
        }

        public T PrepareObject() // 取出物体
        {
            T obj = GetObject();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public override void ReturnObject(T obj) // 回收物体
        {
            obj.gameObject.SetActive(false);
            if (obj)
            {
                pool.Enqueue(obj);
            }
        }
    }
}