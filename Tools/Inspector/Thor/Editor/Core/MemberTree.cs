using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

namespace Legendary.Thor.Editor
{
    public class MemberTree
    {
        private System.Object[] Instances;
        private SerializedObject SerializedObject;
        public Type Type;
        public List<MemberNode> Nodes;

        internal static List<MemberNode> GenerateMemberNodes(MemberTree tree, System.Object[] parentInstances, Type type)
        {
            List<MemberNode> nodes = new List<MemberNode>();

            FieldInfo[] fieldInfos = type.GetFields();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (parentInstances != null)
                {
                    System.Object[] fieldInstances = new object[parentInstances.Length];

                    for (int j = 0; j < fieldInstances.Length; j++)
                    {
                        fieldInstances[j] = fieldInfos[i].GetValue(parentInstances[j]);
                    }
                }

                //nodes.Add(new MemberNode(tree, null, fieldInfos[i], fieldInstances));
            }

            return nodes;
        }

        public MemberTree(System.Object[] instances, SerializedObject serializedObject = null)
        {
            Instances = instances;
            SerializedObject = serializedObject;
            Type = Instances[0].GetType();

            Nodes = GenerateMemberNodes(this, Instances, Type);
        }

        public void Debug_DrawMemberPaths()
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Debug_DrawMemberPaths();
        }

        public void Draw()
        {
            Debug_DrawMemberPaths();
        }
    }
}