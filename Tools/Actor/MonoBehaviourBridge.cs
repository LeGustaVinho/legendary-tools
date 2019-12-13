using System;
using UnityEngine;

namespace LegendaryTools.Actor
{
    public class MonoBehaviourBridge : MonoBehaviour
    {
        private Actor Actor;

        public Transform Transform;
        public RectTransform RectTransform;
        public GameObject GameObject;

        #region MonoBehaviour Events

        public event Action WhenAwake;
        public event Action WhenStart;
        public event Action WhenUpdate;
        public event Action WhenDestroy;
        public event Action WhenEnable;
        public event Action WhenDisable;
        public event Action<Collider> WhenTriggerEnter;
        public event Action<Collider> WhenTriggerExit;
        public event Action<Collision> WhenCollisionEnter;
        public event Action<Collision> WhenCollisionExit;

        #endregion

        public void BindActor(Actor actor)
        {
            Actor = actor;

            Transform = GetComponent<Transform>();
            RectTransform = GetComponent<RectTransform>();
            GameObject = GetComponent<GameObject>();
        }

        protected virtual void Awake()
        {
            Transform = GetComponent<Transform>();
            RectTransform = GetComponent<RectTransform>();
            GameObject = GetComponent<GameObject>();

            WhenAwake?.Invoke();
        }

        protected virtual void Start()
        {
            WhenStart?.Invoke();
        }

        protected virtual void Update()
        {
            WhenUpdate?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            WhenDestroy?.Invoke();
        }

        protected virtual void OnEnable()
        {
            WhenEnable?.Invoke();
        }

        protected virtual void OnDisable()
        {
            WhenDisable?.Invoke();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            WhenTriggerEnter?.Invoke(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            WhenTriggerExit?.Invoke(other);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            WhenCollisionEnter?.Invoke(collision);
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            WhenCollisionExit?.Invoke(collision);
        }
    }
}