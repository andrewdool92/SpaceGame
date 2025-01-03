using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame.Utils
{
    public class ObjectPool<T>
    {
        private Queue<T> pool;

        public ObjectPool()
        {
            pool = new Queue<T>();
        }

        public void Add(T item)
        {
            pool.Enqueue(item);
        }

        public T Get()
        {
            T obj = pool.Dequeue();
            pool.Enqueue(obj);
            return obj;
        }
    }
}