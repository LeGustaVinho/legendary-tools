using UnityEngine;

namespace LegendaryTools
{
	[ExecuteInEditMode]
    public class FollowTransform : MonoBehaviour
    {
        public enum FollowTransformLoopMode
        {
            Update,
            LateUpdate
        }

        public Transform Target;
        public FollowTransformLoopMode LoopMode;

        public bool FollowPosition;
        public Vector3 PositionOffset;
		public Space PositionOffsetRelativeTo;
        public float DeltaPositionThreshold;
        public bool SmoothPosition;
        public float SmoothPositionFactor = 1;
        public float CurrentDistance;

        public bool FollowRotation;
		public Vector3 RotationOffset;
		public Space RotationOffsetRelativeTo;
        public float DeltaAngleThreshold;
        public bool SmoothRotation;
        public float SmoothRotationFactor = 1;
        public float CurrentAngle;

        private Vector3 lastTargetPosition;
        private Quaternion lastTargetRotation;
		[HideInInspector]
        public Transform Transform;

		void Awake()
		{
			Init ();
			Follow ();
		}

		void Init()
		{
			Transform = transform;

			if (Target != null)
			{
				lastTargetPosition = Target.position + PositionOffset;
				lastTargetRotation = Target.rotation;
			}
		}

        public void Follow()
        {
			if (Target == null || Transform == null)
				Init ();

            if (Target != null)
            {
                if (FollowPosition)
                {
                    CurrentDistance = Vector3.Distance(Transform.position, Target.position);
                    if (CurrentDistance >= DeltaPositionThreshold - PositionOffset.magnitude)
                        lastTargetPosition = Target.position;

					if (SmoothPosition)
						Transform.position = Vector3.Lerp (Transform.position, lastTargetPosition, SmoothPositionFactor * Time.deltaTime);
					else
						Transform.position = lastTargetPosition;

					Transform.Translate (PositionOffset, PositionOffsetRelativeTo);
                }

                if (FollowRotation)
                {
                    CurrentAngle = Quaternion.Angle(Transform.rotation, Target.rotation);
                    if (CurrentAngle >= DeltaAngleThreshold)
                        lastTargetRotation = Target.rotation;

					if (SmoothRotation)
						Transform.rotation = Quaternion.Lerp (Transform.rotation, lastTargetRotation, SmoothRotationFactor * Time.deltaTime);
					else
						Transform.rotation = lastTargetRotation;
					
					Transform.Rotate (RotationOffset, RotationOffsetRelativeTo);
                }
            }
        }

        void Update()
        {
            if (LoopMode == FollowTransformLoopMode.Update)
            {
                Follow();
            }
        }

        void LateUpdate()
        {
            if (LoopMode == FollowTransformLoopMode.LateUpdate)
            {
                Follow();
            }
        }

        void Reset()
        {
			Init();
        }
    }
}