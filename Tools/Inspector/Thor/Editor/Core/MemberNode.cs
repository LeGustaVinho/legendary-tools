using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;

namespace LegendaryTools.Inspector.Thor.Editor
{
    public enum MemberNodeType
    {
        Field,
        Method,
        Property,
    }

    public class MemberNode
    {
        public MemberTree Tree;
        public MemberNode Parent;
        public Type Type;
        public MemberNodeType NodeType;
        public List<MemberNode> Nodes = new List<MemberNode>();
        public System.Object[] Instances;

        public List<string> Path
        {
            get
            {
                List<string> path = new List<string>();
                for (MemberNode currentNode = this; currentNode != null; currentNode = currentNode.Parent)
                {
                    switch (NodeType)
                    {
                        case MemberNodeType.Field: path.Add(fieldInfo.Name); break;
                        case MemberNodeType.Property: path.Add(propertyInfo.Name); break;
                        case MemberNodeType.Method: path.Add(methodInfo.Name); break;
                    }
                }

                path.Reverse();

                return path;
            }
        }

        private FieldInfo fieldInfo;
        private MethodInfo methodInfo;
        private PropertyInfo propertyInfo;

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public System.Object Value
        {
            get; set;
        }

        public MemberNode(MemberTree tree, MemberNode parent, System.Object[] instances)
        {
            Tree = tree;
            Parent = parent;
        }

        public MemberNode(MemberTree tree, MemberNode parent, FieldInfo fieldInfo, System.Object[] instances) : this(tree, parent, instances)
        {
            this.fieldInfo = fieldInfo;
            NodeType = MemberNodeType.Field;
            Type = fieldInfo.FieldType;

            Nodes = MemberTree.GenerateMemberNodes(tree, Instances, Type);
        }

        public MemberNode(MemberTree tree, MemberNode parent, PropertyInfo propertyInfo, System.Object[] instances) : this(tree, parent, instances)
        {
            this.propertyInfo = propertyInfo;
            NodeType = MemberNodeType.Property;
            Type = propertyInfo.GetType();

            Nodes = MemberTree.GenerateMemberNodes(tree, Instances, Type);
        }

        public MemberNode(MemberTree tree, MemberNode parent, MethodInfo methodInfo, System.Object[] instances) : this(tree, parent, instances)
        {
            this.methodInfo = methodInfo;
            NodeType = MemberNodeType.Method;
            Type = methodInfo.GetType();

            Nodes = MemberTree.GenerateMemberNodes(tree, Instances, Type);
        }

        public void Debug_DrawMemberPaths()
        {
            EditorGUILayout.LabelField(String.Join("/", Path));

            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Debug_DrawMemberPaths();
        }

        public void Draw()
        {

        }
    }
}