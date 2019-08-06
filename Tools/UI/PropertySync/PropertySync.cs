using UnityEngine;

namespace LegendaryTools
{
    /// <summary>
    /// Property binding lets you bind two fields or properties so that changing one will update the other.
    /// </summary>

    [ExecuteInEditMode]
    [AddComponentMenu("UI/Binding/Property Sync")]
    public class PropertySync : MonoBehaviour
    {
        public enum UpdateCondition
        {
            OnStart,
            OnUpdate,
            OnLateUpdate,
            OnFixedUpdate,
        }

        public enum Direction
        {
            SourceUpdatesTarget,
            TargetUpdatesSource,
            BiDirectional,
        }

        /// <summary>
        /// First property reference.
        /// </summary>

        public LegendaryTools.Inspector.PropertyBindingReference source;

        /// <summary>
        /// Second property reference.
        /// </summary>

        public LegendaryTools.Inspector.PropertyBindingReference target;

        /// <summary>
        /// Direction of updates.
        /// </summary>

        public Direction direction = Direction.SourceUpdatesTarget;

        /// <summary>
        /// When the property update will occur.
        /// </summary>

        public UpdateCondition update = UpdateCondition.OnUpdate;

        /// <summary>
        /// Whether the values will update while in edit mode.
        /// </summary>

        public bool editMode = true;
		
		public bool invertBool = false;

        // Cached value from the last update, used to see which property changes for bi-directional updates.
        object mLastValue = null;

        void Start()
        {
            UpdateTarget();
            if (update == UpdateCondition.OnStart) enabled = false;
        }

        void Update()
        {
#if UNITY_EDITOR
            if (!editMode && !Application.isPlaying) return;
#endif
            if (update == UpdateCondition.OnUpdate) UpdateTarget();
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            if (!editMode && !Application.isPlaying) return;
#endif
            if (update == UpdateCondition.OnLateUpdate) UpdateTarget();
        }

        void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!editMode && !Application.isPlaying) return;
#endif
            if (update == UpdateCondition.OnFixedUpdate) UpdateTarget();
        }

        void OnValidate()
        {
            if (source != null) source.Reset();
            if (target != null) target.Reset();
        }

        /// <summary>
        /// Immediately update the bound data.
        /// </summary>

        [ContextMenu("Update Now")]
        public void UpdateTarget()
        {
            if (source != null && target != null && source.isValid && target.isValid)
            {
                if (direction == Direction.SourceUpdatesTarget)
                {
                    if(source.GetPropertyType() == typeof(bool) && target.GetPropertyType() == typeof(bool) && invertBool)
                        target.Set(!(bool)source.Get());
                    else
                        target.Set(source.Get());
                }
                else if (direction == Direction.TargetUpdatesSource)
                {
                    if (source.GetPropertyType() == typeof(bool) && target.GetPropertyType() == typeof(bool) && invertBool)
                        source.Set(!(bool)target.Get());
                    else
                        source.Set(target.Get());
                }
                else if (source.GetPropertyType() == target.GetPropertyType())
                {
                    object current = source.Get();

                    if (mLastValue == null || !mLastValue.Equals(current))
                    {
                        if (source.GetPropertyType() == typeof(bool) && target.GetPropertyType() == typeof(bool) && invertBool)
                        {
                            mLastValue = !(bool)current;
                            target.Set(!(bool)current);
                        }
                        else
                        {
                            mLastValue = current;
                            target.Set(current);
                        }
                    }
                    else
                    {
                        current = target.Get();

                        if (!mLastValue.Equals(current))
                        {
                            if (source.GetPropertyType() == typeof(bool) && target.GetPropertyType() == typeof(bool) && invertBool)
                            {
                                mLastValue = !(bool)current;
                                source.Set(!(bool)current);
                            }
                            else
                            {
                                mLastValue = current;
                                source.Set(current);
                            }
                        }
                    }
                }
            }
        }
    }
}