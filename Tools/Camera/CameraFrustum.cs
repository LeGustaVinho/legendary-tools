using UnityEngine;

namespace LegendaryTools.Tools
{
    public class CameraFrustum : MonoBehaviour
    {
        public Camera Camera;
        public GameObject Object;

        [Range(0, 1)] 
        public float DepthValue;

        [Range(0, 1)] 
        public float XObject;

        [Range(0, 1)] 
        public float YObject;

        private readonly Vector3[] Near = new Vector3[4];
        private readonly Vector3[] Far = new Vector3[4];
        private readonly Vector3[] Depth = new Vector3[4];

        private void UpdatePoints()
        {
            calcPlanePoints(Near, calcFrustumSize(Camera, Camera.nearClipPlane), Camera.nearClipPlane, Camera.transform);
            calcPlanePoints(Far, calcFrustumSize(Camera, Camera.farClipPlane), Camera.farClipPlane, Camera.transform);
            
            Depth[0] = Vector3.Lerp(Near[0], Far[0], DepthValue);
            Depth[1] = Vector3.Lerp(Near[1], Far[1], DepthValue);
            Depth[2] = Vector3.Lerp(Near[2], Far[2], DepthValue);
            Depth[3] = Vector3.Lerp(Near[3], Far[3], DepthValue);
            
            if (Object != null)
            {
                //Bilinear interpolation
                Object.transform.position = Vector3.Lerp(Vector3.Lerp(Depth[0], Depth[1], YObject), Vector3.Lerp(Depth[2], Depth[3], YObject), XObject);
                Object.transform.rotation = Camera.transform.rotation;
            }
        }

        private void calcPlanePoints(Vector3[] points, Vector2 frustumSize, float clipPlane, Transform parentTransform)
        {
            points[0].Set(-(frustumSize.x * 0.5f), -(frustumSize.y * 0.5f), clipPlane);
            points[1].Set(+(frustumSize.x * 0.5f), -(frustumSize.y * 0.5f), clipPlane);
            points[2].Set(-(frustumSize.x * 0.5f), +(frustumSize.y * 0.5f), clipPlane);
            points[3].Set(+(frustumSize.x * 0.5f), +(frustumSize.y * 0.5f), clipPlane);
            
            transformPoint(parentTransform, points);
        }

        //http://docs.unity3d.com/Documentation/Manual/FrustumSizeAtDistance.html
        private Vector2 calcFrustumSize(Camera camera, float plane)
        {
            float height = 2.0f * plane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad); 
            return new Vector2(height * Camera.aspect, height);
        }
        
        private void transformPoint(Transform parentTransform, Vector3[] points)
        {
            if (parentTransform != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    //Rotate the point and translate to camera position
                    points[i] = parentTransform.rotation * points[i] + parentTransform.position;
                }
            }
        }

        private void GizmosDrawQuad(Vector3[] points)
        {
            Gizmos.DrawLine(points[0], points[1]);
            Gizmos.DrawLine(points[1], points[3]);
            Gizmos.DrawLine(points[3], points[2]);
            Gizmos.DrawLine(points[2], points[0]);
        }
        
        private void OnDrawGizmos()
        {
            UpdatePoints();
            
            Gizmos.color = Color.red;
            GizmosDrawQuad(Near);

            GizmosDrawQuad(Far);

            Gizmos.DrawLine(Near[0], Far[0]);
            Gizmos.DrawLine(Near[1], Far[1]);
            Gizmos.DrawLine(Near[2], Far[2]);
            Gizmos.DrawLine(Near[3], Far[3]);

            Gizmos.color = Color.yellow;
            GizmosDrawQuad(Depth);
        }

        private void Update()
        {
            UpdatePoints();
        }
    }
}