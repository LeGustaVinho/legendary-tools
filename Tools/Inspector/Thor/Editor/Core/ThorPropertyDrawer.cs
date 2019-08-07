using System.Collections;
using System.Collections.Generic;
using Legendary.Editor.Thor;
using UnityEditor;
using UnityEngine;

namespace LegendaryTools.Editor.Thor

{
    public class ThorPropertyDrawer<T> : PropertyDrawer
        where T : class
    {
        private bool isInit = false;
        public T Instance;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ensureInit(property);
            if (Instance != null)
            {
                OnDraw(position, property, label);
            }
        }

        private void ensureInit(SerializedProperty property)
        {
            Instance = ThorUtil.GetInstance(property) as T;
            if (!isInit && Instance != null)
            {
                isInit = true;
                OnInit(property);
            }
        }

        public virtual void OnInit(SerializedProperty property)
        {
        }

        public virtual void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}