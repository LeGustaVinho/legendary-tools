using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LegendaryTools.DesignPatters
{
    public class PoolObject<T>
    {
        public static PoolObject<T> Instance = null;

        protected List<T> Instances = new List<T>();
        protected List<T> InactiveInstances = new List<T>();
        protected List<T> ActiveInstances = new List<T>();

        public int ActiveCount
        {
            get { return ActiveInstances.Count; }
        }

        public int InactiveCount
        {
            get { return InactiveInstances.Count; }
        }

        public int TotalCount
        {
            get { return ActiveCount + InactiveCount; }
        }

        public PoolObject()
        {
            Instance = this;
        }

        public virtual T Create()
        {
            T newObject = default(T);

            if(InactiveInstances.Count > 0)
            {
                newObject = InactiveInstances[0];
                InactiveInstances.RemoveAt(0);
            }
            else
            {
                newObject = NewObject();
                Instances.Add(newObject);

                if (newObject is IPoolable)
                    (newObject as IPoolable).OnConstruct();
            }

            ActiveInstances.Add(newObject);

            if (newObject is IPoolable)
                (newObject as IPoolable).OnCreate();

            return newObject;
        }

        public virtual void Recycle(T instance)
        {
            //Debug.Log("instance.type = " + instance.GetType().ToString());

            //for (int i = 0; i < ActiveInstances.Count; i++)
            //{
            //    Debug.Log("ActiveInstance ["+i + "].type = " + ActiveInstances[i].GetType().ToString());
            //}

            if (instance != null)
            {
                if (ActiveInstances.Contains(instance))
                {
                    ActiveInstances.Remove(instance);
                    InactiveInstances.Add(instance);

                    if (instance is IPoolable)
                        (instance as IPoolable).OnRecycle();
                }
                else
                    Debug.LogWarning("[PoolObject:Recycle()] -> ActiveInstance does not contains the instance.");
            }
            else
                Debug.LogWarning("[PoolObject:Recycle()] -> Instance cannot be null.");
        }

        protected virtual T NewObject()
        {
            return Activator.CreateInstance<T>();
        }

        public virtual void Clear()
        {
            InactiveInstances.Clear();
            ActiveInstances.Clear();
        }

        public static T CreateObject()
        {
            if(Instance == null)
                Instance = new PoolObject<T>();

            return Instance.Create();
        }

        public static void RecycleObject(T instance)
        {
            if (Instance != null)
                Instance.Recycle(instance);
        }
    }
}