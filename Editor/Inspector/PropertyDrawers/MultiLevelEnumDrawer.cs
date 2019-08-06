using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendaryTools
{
    [CustomPropertyDrawer(typeof(MultiLevelEnumAttribute))]
    public class MultiLevelEnumDrawer : PropertyDrawer
    {
        private bool init = false;
        private GUIContent[] displayedOptions;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                if (!init)
                {
                    displayedOptions = EnumGetNames(property);
                    init = true;
                }

                property.enumValueIndex = EditorGUI.Popup(position, label, property.enumValueIndex, displayedOptions);
            }
            else
                EditorGUI.LabelField(position, label.text + " not Supported type: " + property.propertyType.ToString());
        }

        GUIContent[] EnumGetNames(SerializedProperty property)
        {
            GUIContent[] result = new GUIContent[property.enumNames.Length];

            for (int i = 0; i < property.enumNames.Length; i++)
                result[i] = new GUIContent(property.enumNames[i].Replace('_', '/'));

            return result;
        }
    }
}