using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace LegendaryTools.Actor
{
    public abstract class Actor : IActor
    {
        protected static readonly Dictionary<Type, List<Actor>> allActorsByType = new Dictionary<Type, List<Actor>>();
        protected MonoBehaviourBridge bridge;

        public Actor()
        {
            GameObject newGO = CreateGameObject();
            Init(newGO);
        }

        public Actor(Object prefab = null, string name = "")
        {
            GameObject newGO = CreateGameObject(name, prefab);
            Init(newGO);
        }

        private void Init(GameObject gameObject)
        {
            bridge = AddBridge(gameObject);
            bridge.BindActor(this);

            RegisterActor();
            RegisterBridgeEvents();
        }

        public static void Destroy(Actor actor)
        {
            actor.Dispose();
        }

        public static object FindObjectOfType(Type type)
        {
            return null;
        }

        public static T FindObjectOfType<T>() where T : Actor<T>
        {
            return null;
        }

        public static object[] FindObjectsOfType(Type type)
        {
            return null;
        }

        public static T[] FindObjectsOfType<T>() where T : Actor<T>
        {
            return null;
        }

        protected void RegisterBridgeEvents()
        {
            bridge.WhenAwake += Awake;
            bridge.WhenStart += Start;
            bridge.WhenUpdate += Update;
            bridge.WhenDestroy += InternalOnDestroy;
            bridge.WhenEnable += OnEnable;
            bridge.WhenDisable += OnDisable;
            bridge.WhenTriggerEnter += OnTriggerEnter;
            bridge.WhenTriggerExit += OnTriggerExit;
            bridge.WhenCollisionEnter += OnCollisionEnter;
            bridge.WhenCollisionExit += OnCollisionExit;
        }

        protected void UnRegisterBridgeEvents()
        {
            bridge.WhenAwake -= Awake;
            bridge.WhenStart -= Start;
            bridge.WhenUpdate -= Update;
            bridge.WhenDestroy -= InternalOnDestroy;
            bridge.WhenEnable -= OnEnable;
            bridge.WhenDisable -= OnDisable;
            bridge.WhenTriggerEnter -= OnTriggerEnter;
            bridge.WhenTriggerExit -= OnTriggerExit;
            bridge.WhenCollisionEnter -= OnCollisionEnter;
            bridge.WhenCollisionExit -= OnCollisionExit;
        }

        protected abstract void RegisterActor();

        protected abstract void UnRegisterActor();

        protected virtual GameObject CreateGameObject(string name = "", Object prefab = null)
        {
            if (prefab == null)
            {
                return new GameObject(name);
            }

            return Object.Instantiate(prefab) as GameObject;
        }

        protected virtual void DestroySelf()
        {
            Object.Destroy(bridge.GameObject);
        }

#if UNITY_EDITOR
        protected virtual void EditorDestroySelf()
        {
            Object.DestroyImmediate(bridge.GameObject);
        }
#endif

        protected virtual MonoBehaviourBridge AddBridge(GameObject gameObject)
        {
            return gameObject.AddComponent<MonoBehaviourBridge>();
        }

        #region MonoBehaviour calls

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        private void InternalOnDestroy()
        {
            OnDestroy();
            UnRegisterBridgeEvents();
            UnRegisterActor();
            bridge.BindActor(null);
            bridge = null;
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
        }

        protected virtual void OnTriggerExit(Collider other)
        {
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
        }

        protected virtual void OnCollisionExit(Collision other)
        {
        }

        #endregion

        #region Interfaces Implementations

        public string Tag
        {
            get => bridge.GameObject.tag;
            set => bridge.GameObject.tag = value;
        }

        public HideFlags HideFlags
        {
            get => bridge.GameObject.hideFlags;
            set => bridge.GameObject.hideFlags = value;
        }

        public int GetInstanceID()
        {
            return bridge.GameObject.GetInstanceID();
        }

        public T GetComponent<T>() where T : Component
        {
            return bridge.GameObject.GetComponent<T>();
        }

        public Component GetComponent(Type type)
        {
            return bridge.GameObject.GetComponent(type);
        }

        public Component GetComponent(string type)
        {
            return bridge.GameObject.GetComponent(type);
        }

        public Component GetComponentInChildren(Type t)
        {
            return bridge.GameObject.GetComponentInChildren(t);
        }

        public T GetComponentInChildren<T>() where T : Component
        {
            return bridge.GameObject.GetComponentInChildren<T>();
        }

        public Component GetComponentInParent(Type t)
        {
            return bridge.GameObject.GetComponentInParent(t);
        }

        public T GetComponentInParent<T>() where T : Component
        {
            return bridge.GameObject.GetComponentInParent<T>();
        }

        public Component[] GetComponents(Type type)
        {
            return bridge.GameObject.GetComponents(type);
        }

        public T[] GetComponents<T>() where T : Component
        {
            return bridge.GameObject.GetComponents<T>();
        }

        public Component[] GetComponentsInChildren(Type t, bool includeInactive)
        {
            return bridge.GameObject.GetComponentsInChildren(t, includeInactive);
        }

        public T[] GetComponentsInChildren<T>(bool includeInactive) where T : Component
        {
            return bridge.GameObject.GetComponentsInChildren<T>(includeInactive);
        }

        public Component[] GetComponentsInParent(Type t, bool includeInactive = false)
        {
            return bridge.GameObject.GetComponentsInParent(t, includeInactive);
        }

        public T[] GetComponentsInParent<T>(bool includeInactive = false) where T : Component
        {
            return bridge.GameObject.GetComponentsInParent<T>(includeInactive);
        }

        public bool Enabled
        {
            get => bridge.enabled;
            set => bridge.enabled = true;
        }

        public bool IsActiveAndEnabled => bridge.isActiveAndEnabled;

        public void CancelInvoke()
        {
            bridge.CancelInvoke();
        }

        public void CancelInvoke(string methodName)
        {
            bridge.CancelInvoke(methodName);
        }

        public void Invoke(string methodName, float time)
        {
            bridge.Invoke(methodName, time);
        }

        public void InvokeRepeating(string methodName, float time, float repeatRate)
        {
            bridge.InvokeRepeating(methodName, time, repeatRate);
        }

        public bool IsInvoking(string methodName)
        {
            return bridge.IsInvoking(methodName);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return bridge.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(string methodName, object value = null)
        {
            return bridge.StartCoroutine(methodName, value);
        }

        public void StopAllCoroutines()
        {
            bridge.StopAllCoroutines();
        }

        public void StopCoroutine(string methodName)
        {
            bridge.StopCoroutine(methodName);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            bridge.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            bridge.StopCoroutine(routine);
        }

        public int ChildCount => bridge.Transform.childCount;

        public Vector3 EulerAngles
        {
            get => bridge.Transform.eulerAngles;
            set => bridge.Transform.eulerAngles = value;
        }

        public Vector3 Forward
        {
            get => bridge.Transform.forward;
            set => bridge.Transform.forward = value;
        }

        public bool HasChanged
        {
            get => bridge.Transform.hasChanged;
            set => bridge.Transform.hasChanged = value;
        }

        public int HierarchyCapacity => bridge.Transform.hierarchyCount;

        public int HierarchyCount => bridge.Transform.hierarchyCount;

        public Vector3 LocalEulerAngles
        {
            get => bridge.Transform.localEulerAngles;
            set => bridge.Transform.localEulerAngles = value;
        }

        public Vector3 LocalPosition
        {
            get => bridge.Transform.localPosition;
            set => bridge.Transform.localPosition = value;
        }

        public Quaternion LocalRotation
        {
            get => bridge.Transform.localRotation;
            set => bridge.Transform.localRotation = value;
        }

        public Vector3 LocalScale
        {
            get => bridge.Transform.localScale;
            set => bridge.Transform.localScale = value;
        }

        public Matrix4x4 LocalToWorldMatrix => bridge.Transform.localToWorldMatrix;
        public Vector3 LossyScale => bridge.Transform.lossyScale;

        public Transform Parent
        {
            get => bridge.Transform.parent;
            set => bridge.Transform.parent = value;
        }

        public Vector3 Position
        {
            get => bridge.Transform.position;
            set => bridge.Transform.position = value;
        }

        public Vector3 Right
        {
            get => bridge.Transform.right;
            set => bridge.Transform.right = value;
        }

        public Transform Root => bridge.Transform.root;

        public Quaternion Rotation
        {
            get => bridge.Transform.rotation;
            set => bridge.Transform.rotation = value;
        }

        public Vector3 Up
        {
            get => bridge.Transform.up;
            set => bridge.Transform.up = value;
        }

        public Matrix4x4 WorldToLocalMatrix => bridge.Transform.worldToLocalMatrix;

        public void DetachChildren()
        {
            bridge.Transform.DetachChildren();
        }

        public Transform Find(string name)
        {
            return bridge.Transform.Find(name);
        }

        public Transform GetChild(int index)
        {
            return bridge.Transform.GetChild(index);
        }

        public int GetSiblingIndex()
        {
            return bridge.Transform.GetSiblingIndex();
        }

        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            return bridge.Transform.InverseTransformDirection(direction);
        }

        public Vector3 InverseTransformPoint(Vector3 position)
        {
            return bridge.Transform.InverseTransformDirection(position);
        }

        public Vector3 InverseTransformVector(Vector3 vector)
        {
            return bridge.Transform.InverseTransformDirection(vector);
        }

        public bool IsChildOf(Transform parent)
        {
            return bridge.Transform.IsChildOf(parent);
        }

        public void LookAt(Transform target)
        {
            bridge.Transform.LookAt(target);
        }

        public void LookAt(Transform target, Vector3 worldUp)
        {
            bridge.Transform.LookAt(target, worldUp);
        }

        public void Rotate(Vector3 eulers, Space relativeTo = Space.Self)
        {
            bridge.Transform.Rotate(eulers, relativeTo);
        }

        public void Rotate(float xAngle, float yAngle, float zAngle, Space relativeTo = Space.Self)
        {
            bridge.Transform.Rotate(xAngle, yAngle, zAngle, relativeTo);
        }

        public void Rotate(Vector3 axis, float angle, Space relativeTo = Space.Self)
        {
            bridge.Transform.Rotate(axis, angle, relativeTo);
        }

        public void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            bridge.Transform.RotateAround(point, axis, angle);
        }

        public void SetAsFirstSibling()
        {
            bridge.Transform.SetAsFirstSibling();
        }

        public void SetAsLastSibling()
        {
            bridge.Transform.SetAsLastSibling();
        }

        public void SetParent(Transform parent)
        {
            bridge.Transform.SetParent(parent);
        }

        public void SetParent(Transform parent, bool worldPositionStays)
        {
            bridge.Transform.SetParent(parent, worldPositionStays);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            bridge.Transform.SetPositionAndRotation(position, rotation);
        }

        public void SetSiblingIndex(int index)
        {
            bridge.Transform.SetSiblingIndex(index);
        }

        public Vector3 TransformDirection(Vector3 direction)
        {
            return bridge.Transform.TransformDirection(direction);
        }

        public Vector3 TransformPoint(Vector3 position)
        {
            return bridge.Transform.TransformDirection(position);
        }

        public Vector3 TransformVector(Vector3 vector)
        {
            return bridge.Transform.TransformDirection(vector);
        }

        public void Translate(Vector3 translation)
        {
            bridge.Transform.Translate(translation);
        }

        public void Translate(Vector3 translation, Space relativeTo = Space.Self)
        {
            bridge.Transform.Translate(translation, relativeTo);
        }

        public bool activeInHierarchy { get; set; }
        public bool activeSelf { get; set; }
        public int layer { get; set; }
        public Scene scene { get; set; }
        public string tag { get; set; }

        public void SetActive(bool value)
        {
            bridge.GameObject.SetActive(value);
        }

        public Component AddComponent(Type componentType)
        {
            return bridge.GameObject.AddComponent(componentType);
        }

        public T AddComponent<T>() where T : Component
        {
            return bridge.GameObject.AddComponent<T>();
        }

        public virtual void Dispose()
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(bridge.GameObject);
#else
            Object.Destroy(bridge.GameObject);
#endif
        }

        public Transform Transform => bridge.Transform;
        public RectTransform RectTransform => bridge.RectTransform;
        public GameObject GameObject => bridge.GameObject;

        #endregion
    }

    public class Actor<TClass> : Actor
    {
        public Actor()
        {
        }

        public Actor(Object prefab = null, string name = "") : base(prefab, name)
        {
        }

        protected override void RegisterActor()
        {
            Type type = typeof(TClass);
            if (!allActorsByType.ContainsKey(type))
            {
                allActorsByType.Add(type, new List<Actor>());
            }

            if (!allActorsByType[type].Contains(this))
            {
                allActorsByType[type].Add(this);
            }
        }

        protected override void UnRegisterActor()
        {
            Type type = typeof(TClass);
            if (allActorsByType.ContainsKey(type))
            {
                if (allActorsByType[type].Contains(this))
                {
                    allActorsByType[type].Remove(this);
                }
            }
        }
    }

    public class Actor<TClass, TBehaviour> : Actor<TClass>
        where TBehaviour : MonoBehaviourBridge
    {
        public TBehaviour BaseBehaviour { get; private set; }

        public Actor()
        {
            BaseBehaviour = bridge as TBehaviour;
        }

        public Actor(Object prefab = null, string name = "") : base(prefab, name)
        {
            BaseBehaviour = bridge as TBehaviour;
        }

        protected override MonoBehaviourBridge AddBridge(GameObject gameObject)
        {
            return gameObject.AddComponent<TBehaviour>();
        }

        protected sealed override void RegisterActor()
        {
        }
    }
}