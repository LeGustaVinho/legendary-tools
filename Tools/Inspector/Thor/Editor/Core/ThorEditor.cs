using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Legendary.Thor.Editor
{
    public class ThorEditor : UnityEditor.Editor
    {
        private bool IsInit;
        private MemberTree MemberTree;

        private void Init()
        {
            if (MemberTree == null)
            {
                MemberTree = new MemberTree(targets, serializedObject);
            }
        }

        public void DrawThorInspector()
        {
            Init();

            MemberTree.Draw();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Targets count: " + targets.Length);

            DrawThorInspector();
        }
    }
}