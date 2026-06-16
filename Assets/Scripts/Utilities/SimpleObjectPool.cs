using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SpaceGame.Utils
{
    public class SimpleObjectPool<T>
    {
        private Queue<T> pool;

        public SimpleObjectPool()
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