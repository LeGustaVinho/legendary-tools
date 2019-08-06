using UnityEngine;

namespace LegendaryTools.Tools
{
    public enum CameraRotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public enum CameraMoveHeuristic { Hard = 0, Towards = 1, Lerp = 2, Smooth = 3 }
    public enum CameraRotationHeuristic { Hard = 0, Towards = 1, Lerp = 2 }
    public enum CameraFollowRotation { TargetPosition = 0, TargetDirection = 1, TargetRotation = 2 }

    public class MultiPurposeCamera : MonoBehaviour
    {
        public Transform Target;

        [Header("FreeLook")]
        public bool CanFreeLook;
        public bool FreeLookForceTargetRotation;
        public CameraRotationHeuristic FreeLookForceTargetRotationHeuristic = CameraRotationHeuristic.Hard;
        public float FreeLookForceTargetRotationDamping = 1;
        public CameraRotationAxes FreeLookAxis = CameraRotationAxes.MouseXAndY;
        public Vector2 FreeLookSensitivity = new Vector2(5, 5);
        public Vector2 FreeLookMin = new Vector2(-360, -60);
        public Vector2 FreeLookMax = new Vector2(360, 60);

        public Vector2 freeLookRotation = new Vector2(0, 0);

        public bool CanOrbit;
        public Vector2 OrbitSensivity = new Vector2(5, 5); //Camera x rotation sensivity
        public Vector2 OrbitYLimit = new Vector2(-360, 360);

        public Vector2 OrbitCurrentRotation = new Vector2(0, 0); //Current camera x rotation

        public bool CanFollow;
        public CameraFollowRotation FollowRotation;
        public CameraRotationHeuristic FollowLookTargetHeuristic = CameraRotationHeuristic.Hard;
        public float FollowLookDamping = 1;
        public bool FollowUseCameraRotation;
        public CameraMoveHeuristic FollowHeuristic = CameraMoveHeuristic.Hard;
        public float FollowDamping = 1;
        public float FollowHeight = 1;

        public Vector3 followSmoothSpeed;
        public Vector3 followTargetPosition;
        public Quaternion followTargetRotation;

        public bool CanZoom;
        public float ZoomSensivity = 5; //Camera zoom sensivity
        public Vector2 ZoomMinMax = new Vector2(1, 30); //Camera min (x) and max (y) value 
        public float ZoomDistance = 10; //Current camera distance

        [Header("Input")]
        public bool UseDefaultInput;
        public Vector2 PointerDelta;
        public float ScrollDelta;

        void Start()
        {
            Update();
        }

        void Update()
        {
            DefaultInput();

            Zoom();
            Orbit();
            FreeLook();
            Follow();

            //OrbitCurrentRotation = new Vector2(transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.x);
        }

        void DefaultInput()
        {
            if (!UseDefaultInput) return;

            PointerDelta = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            ScrollDelta = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        }

        void Follow()
        {
            if (!CanFollow) return;
            if (CanOrbit) return; //prevent conflict with orbit

            if (!CanFreeLook)
            {
                switch (FollowLookTargetHeuristic)
                {
                    case CameraRotationHeuristic.Hard:
                        switch (FollowRotation)
                        {
                            case CameraFollowRotation.TargetPosition: followTargetRotation = Quaternion.LookRotation(Target.transform.position - transform.position); break;
                            case CameraFollowRotation.TargetDirection: followTargetRotation = Quaternion.LookRotation(Target.transform.forward); break;
                            case CameraFollowRotation.TargetRotation: followTargetRotation = Target.transform.rotation; break;
                        }
                        break;
                    case CameraRotationHeuristic.Towards:
                        switch (FollowRotation)
                        {
                            case CameraFollowRotation.TargetPosition: followTargetRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Target.transform.position - transform.position), FollowLookDamping); break;
                            case CameraFollowRotation.TargetDirection: followTargetRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Target.transform.forward), FollowLookDamping); break;
                            case CameraFollowRotation.TargetRotation: followTargetRotation = Quaternion.RotateTowards(transform.rotation, Target.transform.rotation, FollowLookDamping); break;
                        }
                        break;
                    case CameraRotationHeuristic.Lerp:
                        switch (FollowRotation)
                        {
                            case CameraFollowRotation.TargetPosition: followTargetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Target.transform.position - transform.position), Time.deltaTime * FollowLookDamping); break;
                            case CameraFollowRotation.TargetDirection: followTargetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Target.transform.forward), Time.deltaTime * FollowLookDamping); break;
                            case CameraFollowRotation.TargetRotation: followTargetRotation = Quaternion.Lerp(transform.rotation, Target.transform.rotation, Time.deltaTime * FollowLookDamping); break;
                        }
                        break;
                }

                transform.rotation = followTargetRotation;
            }

            if (FreeLookForceTargetRotation)
            {
                switch (FollowHeuristic)
                {
                    case CameraMoveHeuristic.Hard: followTargetPosition = (FollowUseCameraRotation ? transform.rotation : Quaternion.identity) * new Vector3(0, FollowHeight, -ZoomDistance) + Target.transform.position; break;
                    case CameraMoveHeuristic.Towards: followTargetPosition = Vector3.MoveTowards(transform.position, (FollowUseCameraRotation ? transform.rotation : Quaternion.identity) * new Vector3(0, FollowHeight, -ZoomDistance) + Target.transform.position, Time.deltaTime * FollowDamping); break;
                    case CameraMoveHeuristic.Lerp: followTargetPosition = Vector3.Lerp(transform.position, (FollowUseCameraRotation ? transform.rotation : Quaternion.identity) * new Vector3(0, FollowHeight, -ZoomDistance) + Target.transform.position, Time.deltaTime * FollowDamping); break;
                    case CameraMoveHeuristic.Smooth: followTargetPosition = Vector3.SmoothDamp(transform.position, (FollowUseCameraRotation ? transform.rotation : Quaternion.identity) * new Vector3(0, FollowHeight, -ZoomDistance) + Target.transform.position, ref followSmoothSpeed, FollowDamping); break;
                }
            }

            transform.position = followTargetPosition;
        }

        void Orbit()
        {
            if (!CanOrbit) return;
            if (CanFollow) return; //prevent conflict with follow

            OrbitCurrentRotation = new Vector2(OrbitCurrentRotation.x + (PointerDelta.x * OrbitSensivity.x), ClampAngle(OrbitCurrentRotation.y - (PointerDelta.y * OrbitSensivity.y), OrbitYLimit.x, OrbitYLimit.y));
            transform.rotation = Quaternion.Euler(OrbitCurrentRotation.y, OrbitCurrentRotation.x, 0);
            transform.position = transform.rotation * new Vector3(0, FollowHeight, -ZoomDistance) + Target.transform.position; //hardfollow
        }

        void FreeLook()
        {
            if (!CanFreeLook) return;

            if (FreeLookAxis == CameraRotationAxes.MouseXAndY)
            {
                freeLookRotation = new Vector2(transform.localEulerAngles.y + PointerDelta.x * FreeLookSensitivity.x,
                    Mathf.Clamp(freeLookRotation.y + (PointerDelta.y * FreeLookSensitivity.y), FreeLookMin.y, FreeLookMax.y));

                transform.localEulerAngles = new Vector3(-freeLookRotation.y, freeLookRotation.x, 0);
            }
            else if (FreeLookAxis == CameraRotationAxes.MouseX)
                transform.Rotate(0, PointerDelta.x * FreeLookSensitivity.x, 0);
            else
            {
                freeLookRotation = new Vector2(freeLookRotation.x,
                    Mathf.Clamp(freeLookRotation.y + (PointerDelta.y * FreeLookSensitivity.y), FreeLookMin.y, FreeLookMax.y));

                transform.localEulerAngles = new Vector3(-freeLookRotation.y, transform.localEulerAngles.y, 0);
            }

            switch (FreeLookForceTargetRotationHeuristic)
            {
                case CameraRotationHeuristic.Hard: Target.transform.rotation = transform.rotation; break;
                case CameraRotationHeuristic.Towards: Target.transform.rotation = Quaternion.RotateTowards(Target.transform.rotation, transform.rotation, FreeLookForceTargetRotationDamping); break;
                case CameraRotationHeuristic.Lerp: Target.transform.rotation = Quaternion.Lerp(Target.transform.rotation, transform.rotation, Time.deltaTime * FreeLookForceTargetRotationDamping); break;
            }
        }

        void Zoom()
        {
            if (!CanZoom) return;

            ZoomDistance = Mathf.Clamp(-ScrollDelta * ZoomSensivity + ZoomDistance, ZoomMinMax.x, ZoomMinMax.y);
        }

        float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }
    }
}