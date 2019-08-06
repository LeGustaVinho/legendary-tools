using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using LegendaryTools.Inspector;

namespace LegendaryTools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseLocalize), true)]
    public class BaseLocalizeEditor : UnityEditor.Editor
    {
        List<string> mKeys;

        void OnEnable()
        {
            Dictionary<string, string[]> dict = LocalizationManager.dictionary;

            if (dict.Count > 0)
            {
                mKeys = new List<string>();

                foreach (KeyValuePair<string, string[]> pair in dict)
                {
                    if (pair.Key == "KEY") continue;
                    mKeys.Add(pair.Key);
                }
                mKeys.Sort(delegate (string left, string right) { return left.CompareTo(right); });
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(6f);
            ActionDelegateEditor.SetLabelWidth(80f);

            GUILayout.BeginHorizontal();
            // Key not found in the localization file -- draw it as a text field
            SerializedProperty sp = ActionDelegateEditor.DrawProperty("Key", serializedObject, "key");

            string myKey = sp.stringValue;
            bool isPresent = (mKeys != null) && mKeys.Contains(myKey);
            GUI.color = isPresent ? Color.green : Color.red;
            GUILayout.BeginVertical(GUILayout.Width(22f));
            GUILayout.Space(2f);
            //GUILayout.Label(isPresent ? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
            GUILayout.Label(isPresent ? "\u2714" : "\u2718", GUILayout.Height(20f));
            GUILayout.EndVertical();
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (isPresent)
            {
                if (ActionDelegateEditor.DrawHeader("Preview"))
                {
                    ActionDelegateEditor.BeginContents();

                    string[] keys = LocalizationManager.knownLanguages;
                    string[] values;

                    if (LocalizationManager.dictionary.TryGetValue(myKey, out values))
                    {
                        if (keys.Length != values.Length)
                        {
                            EditorGUILayout.HelpBox("Number of keys doesn't match the number of values! Did you modify the dictionaries by hand at some point?", UnityEditor.MessageType.Error);
                        }
                        else
                        {
                            for (int i = 0; i < keys.Length; ++i)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(keys[i], GUILayout.Width(66f));

                                #if UNITY_4_7 || UNITY_5_5 || UNITY_5_6
	                            if (GUILayout.Button(values[i], "AS TextArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 110f)))
                                #else
                                if (GUILayout.Button(values[i], "TextArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 110f)))
                                #endif
                                {
                                    (target as TextLocalize).value = values[i];
                                    GUIUtility.hotControl = 0;
                                    GUIUtility.keyboardControl = 0;
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    else GUILayout.Label("No preview available");

                    ActionDelegateEditor.EndContents();
                }
            }
            else if (mKeys != null && !string.IsNullOrEmpty(myKey))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80f);
                GUILayout.BeginVertical();
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.35f);

                int matches = 0;

                for (int i = 0, imax = mKeys.Count; i < imax; ++i)
                {
                    if (mKeys[i].StartsWith(myKey, System.StringComparison.OrdinalIgnoreCase) || mKeys[i].Contains(myKey))
                    {
                        if (GUILayout.Button(mKeys[i] + " \u25B2", "CN CountBadge"))
                        {
                            sp.stringValue = mKeys[i];
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

            GUILayout.BeginVertical();
            if (GUILayout.Button("Reload Localization File"))
            {
                LocalizationManager.Reload();
                OnEnable();
            }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}