﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LegendaryTools.DesignPatters
{
    public static class Pool
    {
        public static bool Verbose;
        public static List<Type> PoolTypes = new List<Type>();
        private static Dictionary<UnityEngine.Object, PoolAssetObject<GameObject>> GameObjectPools = new Dictionary<UnityEngine.Object, PoolAssetObject<GameObject>>();

        public static T Instantiate<T>(UnityEngine.Object original) where T : UnityEngine.Object
        {
            if (typeof(T).IsSameOrSubclass(typeof(GameObject))) //T is GameObject
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return GameObject
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject() as T;
                else //original is Component and return GameObject
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject() as T;
            }
            else if (typeof(T).IsSameOrSubclass(typeof(Component))) //T is component (like MonoBehaviour, Transform and all others Unity Components)
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return T Component
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject().GetComponent<T>();
                else //original is Component and return T Component
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject().GetComponent<T>();
            }
            else //includes Texture, AudioClip, etc
                return NonGameObjectInstantiate<T>(original);
        }

        public static T Instantiate<T>(UnityEngine.Object original, Transform parent) where T : UnityEngine.Object
        {
            if (typeof(T).IsSameOrSubclass(typeof(GameObject))) //T is GameObject
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return GameObject
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject(parent) as T;
                else //original is Component and return GameObject
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject(parent) as T;
            }
            else if (typeof(T).IsSameOrSubclass(typeof(Component))) //T is component (like MonoBehaviour, Transform and all others Unity Components)
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return T Component
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject(parent).GetComponent<T>();
                else //original is Component and return T Component
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject(parent).GetComponent<T>();
            }
            else //includes Texture, AudioClip, etc
                return NonGameObjectInstantiate<T>(original);
        }

        public static T Instantiate<T>(UnityEngine.Object original, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
        {
            if (typeof(T).IsSameOrSubclass(typeof(GameObject))) //T is GameObject
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return GameObject
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject(position, rotation) as T;
                else //original is Component and return GameObject
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject(position, rotation) as T;
            }
            else if (typeof(T).IsSameOrSubclass(typeof(Component))) //T is component (like MonoBehaviour, Transform and all others Unity Components)
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return T Component
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject(position, rotation).GetComponent<T>();
                else //original is Component and return T Component
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject(position, rotation).GetComponent<T>();
            }
            else //includes Texture, AudioClip, etc
                return NonGameObjectInstantiate<T>(original);
        }

        public static T Instantiate<T>(UnityEngine.Object original, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object
        {
            if (typeof(T).IsSameOrSubclass(typeof(GameObject))) //T is GameObject
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return GameObject
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject(position, rotation, parent) as T;
                else //original is Component and return GameObject
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject(position, rotation, parent) as T;
            }
            else if (typeof(T).IsSameOrSubclass(typeof(Component))) //T is component (like MonoBehaviour, Transform and all others Unity Components)
            {
                if (original.GetType().IsSameOrSubclass(typeof(GameObject))) //original is GameObject and return T Component
                    return GetGameObjectPool<T>((original as GameObject).transform).CreateUnityObject(position, rotation, parent).GetComponent<T>();
                else //original is Component and return T Component
                    return GetGameObjectPool<T>((original as Component).transform).CreateUnityObject(position, rotation, parent).GetComponent<T>();
            }
            else //includes Texture, AudioClip, etc
                return NonGameObjectInstantiate<T>(original);
        }

        private static PoolAssetObject<GameObject> GetGameObjectPool<T>(UnityEngine.Component original) where T : UnityEngine.Object
        {
            if (!GameObjectPools.ContainsKey(original))
            {
                GameObjectPools.Add(original, new PoolAssetObject<GameObject>(original.gameObject));

                if (!PoolTypes.Contains(typeof(GameObject)))
                    PoolTypes.Add(typeof(GameObject));
            }

            return GameObjectPools[original];
        }

        private static T NonGameObjectInstantiate<T>(UnityEngine.Object original) where T : UnityEngine.Object
        {
            PoolAssetObject<T> poolAssetObject = PoolAssetObject<T>.Instance as PoolAssetObject<T>;

            if (PoolAssetObject<T>.Instance == null)
                poolAssetObject = new PoolAssetObject<T>(original as T);

            if (!PoolTypes.Contains(typeof(T)))
                PoolTypes.Add(typeof(T));

            return poolAssetObject.CreateUnityObject();
        }

        public static void Destroy<T>(T instance) where T : UnityEngine.Object
        {
            if (instance.GetType().IsSameOrSubclass(typeof(Component))) //includes MonoBehaviour, Transform and all others components
            {
                PoolAssetObject<GameObject> poolAssetObject = PoolAssetObject<GameObject>.Instance as PoolAssetObject<GameObject>;

                if(poolAssetObject != null)
                    poolAssetObject.RecycleUnityObject((instance as Component).gameObject);
            }
            else if (instance.GetType().IsSameOrSubclass(typeof(GameObject)))
            {
                PoolAssetObject<GameObject> poolAssetObject = PoolAssetObject<GameObject>.Instance as PoolAssetObject<GameObject>;

                if (poolAssetObject != null)
                    poolAssetObject.RecycleUnityObject(instance as GameObject);
            }
            else //includes Texture, AudioClip, etc
            {
                PoolAssetObject<T> poolAssetObject = PoolAssetObject<T>.Instance as PoolAssetObject<T>;

                if (poolAssetObject != null)
                    poolAssetObject.RecycleUnityObject(instance);
            }
        }

        public static T Create<T>()
        {
            return PoolObject<T>.CreateObject();
        }

        public static void Recycle<T>(T instance)
        {
            PoolObject<T>.RecycleObject(instance);
        }

        public static void Clear<T>()
        {
            if (PoolObject<T>.Instance != null)
                PoolObject<T>.Instance.Clear();
        }

        public static int GetTotalInstances<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                if (PoolAssetObject<GameObject>.Instance != null)
                    return PoolAssetObject<GameObject>.Instance.TotalCount;
            }
            else
            {
                if (PoolObject<T>.Instance != null)
                    return PoolObject<T>.Instance.TotalCount;
            }

            return -1;
        }

        public static int GetActiveInstances<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                if (PoolAssetObject<GameObject>.Instance != null)
                    return PoolAssetObject<GameObject>.Instance.ActiveCount;
            }
            else
            {
                if (PoolObject<T>.Instance != null)
                    return PoolObject<T>.Instance.ActiveCount;
            }

            return -1;
        }

        public static int GetInactiveInstances<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                if (PoolAssetObject<GameObject>.Instance != null)
                    return PoolAssetObject<GameObject>.Instance.InactiveCount;
            }
            else
            {
                if (PoolObject<T>.Instance != null)
                    return PoolObject<T>.Instance.InactiveCount;
            }

            return -1;
        }
    }
}