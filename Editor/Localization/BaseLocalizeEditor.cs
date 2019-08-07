using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using LegendaryTools.Inspector;
using LegendaryTools.UI;

namespace LegendaryTools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseLocalize), true)]
    public class BaseLocalizeEditor : UnityEditor.Editor
    {
        private static string[] keys;

        private void Init()
        {
            if (!Localization.IsInit)
                Localization.Init();
            
            if (Localization.LocalizationData != null)
            {
                keys = Localization.Keys;
                Array.Sort(keys, (left, right) => left.CompareTo(right));
            }
        }

        public override void OnInspectorGUI()
        {
            Init();
            
            serializedObject.Update();

            GUILayout.Space(6f);
            ActionDelegateEditor.SetLabelWidth(80f);

            GUILayout.BeginHorizontal();
            
            SerializedProperty sp = ActionDelegateEditor.DrawProperty("Key", serializedObject, "key");

            string currentKey = sp.stringValue;
            bool isPresent = keys != null && Array.Exists(keys, item => item == currentKey);
            GUI.color = isPresent ? Color.green : Color.red;
            GUILayout.BeginVertical(GUILayout.Width(22f));
            GUILayout.Space(2f);
            GUILayout.Label(isPresent ? "\u2714" : "\u2718", GUILayout.Height(20f));
            GUILayout.EndVertical();
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (isPresent)
            {
                if (ActionDelegateEditor.DrawHeader("Preview"))
                {
                    ActionDelegateEditor.BeginContents();

                    foreach (var pair in Localization.LocalizationData)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(pair.Key, GUILayout.Width(66f));

                        if (GUILayout.Button(pair.Value[currentKey], "TextArea", GUILayout.MinWidth(80f),
                            GUILayout.MaxWidth(Screen.width - 110f)))
                        {
                            (target as BaseLocalize).value = pair.Value[currentKey];
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                        }

                        GUILayout.EndHorizontal();
                    }

                    ActionDelegateEditor.EndContents();
                }
            }
            else if (keys != null && !string.IsNullOrEmpty(currentKey))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80f);
                GUILayout.BeginVertical();
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.35f);

                int matches = 0;

                for (int i = 0, imax = keys.Length; i < imax; ++i)
                {
                    if (keys[i].StartsWith(currentKey, System.StringComparison.OrdinalIgnoreCase) || keys[i].Contains(currentKey))
                    {
                        if (GUILayout.Button(keys[i] + " \u25B2", "CN CountBadge"))
                        {
                            sp.stringValue = keys[i];
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                        }

                        if (++matches == 8)
                        {
                            GUILayout.Label("...and more");
                            break;
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndVertical();
                GUILayout.Space(22f);
                GUILayout.EndHorizontal(); 
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}