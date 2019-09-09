using UnityEngine;

namespace LegendaryTools
{
    internal class PoolAssetObject<T> : PoolObject<T> where T : Object
    {
        private T Original;

        public PoolAssetObject(T original)
        {
            Original = original;
            Instance = this;
        }

        public T CreateUnityObject()
        {
            T newObject = Create();

            if (Pool.Verbose)
            {
                Debug.Log("Creating. " + newObject.name + " Type: " + typeof(T) + " | T: " + TotalCount + "/A:" +
                          ActiveCount + "/I:" + InactiveCount);
            }

            if (newObject is GameObject)
            {
                (newObject as GameObject).SetActive(true);
                (newObject as GameObject).name = Original.name + " #" + Instances.IndexOf(newObject);
            }

            return newObject;
        }

        public T CreateUnityObject(Transform parent)
        {
            if (Original is GameObject)
            {
                return CreateUnityObject((Original as GameObject).transform.position,
                    (Original as GameObject).transform.rotation, parent);
            }

            return CreateUnityObject();
        }

        public T CreateUnityObject(Vector3 position, Quaternion rotation)
        {
            return CreateUnityObject(position, rotation, null);
        }

        public T CreateUnityObject(Vector3 position, Quaternion rotation, Transform parent)
        {
            T newObject = CreateUnityObject();

            if (newObject is GameObject)
            {
                (newObject as GameObject).transform.SetParent(parent);
                (newObject as GameObject).transform.position = position;
                (newObject as GameObject).transform.rotation = rotation;

                NotifyOnCreate(newObject as GameObject);
            }

            return newObject;
        }

        public void RecycleUnityObject(T instance)
        {
            if (instance is GameObject)
            {
                (instance as GameObject).SetActive(false);
                (instance as GameObject).transform.SetParent(null);
                NotifyOnRecycle(instance as GameObject);
            }

            base.Recycle(instance);

            if (Pool.Verbose)
            {
                Debug.Log("Recycle. Type: " + typeof(T) + " | T: " + TotalCount + "/A:" + ActiveCount + "/I:" +
                          InactiveCount);
            }
        }

        protected override T NewObject()
        {
            T obj = Object.Instantiate(Original);

            if (obj is GameObject)
            {
                NotifyOnConstruct(obj as GameObject);
            }

            return obj;
        }

        public override void Clear()
        {
            for (int i = 0; i < ActiveInstances.Count; i++)
            {
                if (ActiveInstances[i] != null)
                {
                    if (ActiveInstances[i] is GameObject)
                    {
                        Object.Destroy(ActiveInstances[i] as GameObject);
                    }
                    else
#if UNITY_EDITOR
                    {
                        Object.DestroyImmediate(ActiveInstances[i]);
                    }
#else
                        Object.Destroy(ActiveInstances[i]);
#endif
                }
            }

            for (int i = 0; i < InactiveInstances.Count; i++)
            {
                if (ActiveInstances[i] != null)
                {
                    if (InactiveInstances[i] is GameObject)
                    {
                        Object.Destroy(InactiveInstances[i] as GameObject);
                    }
                    else
#if UNITY_EDITOR
                    {
                        Object.DestroyImmediate(InactiveInstances[i]);
                    }
#else
                        Object.Destroy(ActiveInstances[i]);
#endif
                }
            }

            base.Clear();
        }

        private GameObject GetGameObject(T obj)
        {
            if (obj is GameObject)
            {
                return obj as GameObject;
            }

            if (obj is Component)
            {
                return (obj as Component).gameObject;
            }

            return null;
        }

        private void NotifyOnConstruct(GameObject obj)
        {
            Component[] comps = obj.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] is IPoolable)
                {
                    (comps[i] as IPoolable).OnConstruct();
                }
            }
        }

        private void NotifyOnCreate(GameObject obj)
        {
            Component[] comps = obj.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] is IPoolable)
                {
                    (comps[i] as IPoolable).OnCreate();
                }
            }
        }

        private void NotifyOnRecycle(GameObject obj)
        {
            Component[] comps = obj.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] is IPoolable)
                {
                    (comps[i] as IPoolable).OnRecycle();
                }
            }
        }
    }
}