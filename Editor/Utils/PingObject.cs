using UnityEngine;
using System.Collections;
using UnityEditor;

namespace LegendaryTools.EditorTime
{
    public class PingObject : MonoBehaviour
    {
        [MenuItem("GameObject/Ping Selected")]
        public static void Ping()
        {
            if (!Selection.activeObject)
            {
                Debug.LogError("Select an object to ping");
                return;
            }
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}