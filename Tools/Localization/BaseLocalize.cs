using UnityEngine;
using UnityEngine.UI;

namespace LegendaryTools.UI
{
    public abstract class BaseLocalize : MonoBehaviour
    {
        /// <summary>
        /// Localization key.
        /// </summary>

        public string key;

        /// <summary>
        /// Manually change the value of whatever the localization component is attached to.
        /// </summary>

        public string value
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Text lbl = GetComponent<Text>();

                    if (lbl != null)
                    {
                        // If this is a label used by input, we should localize its default value instead
                        InputField input = FindInParents<InputField>(lbl.gameObject);

                        if (input != null)
                        {
                            if (input.placeholder != null)
                                (input.placeholder as Text).text = value;
                        }
                        else
                            lbl.text = value;

#if UNITY_EDITOR
                        if (!Application.isPlaying) SetDirty(lbl);
#endif
                    }
                }
            }
        }

        bool mStarted = false;

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (mStarted) OnLocalize();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            mStarted = true;
            OnLocalize();
        }

        public abstract void OnLocalize();

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;

            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                Transform t = go.transform.parent;

                while (t != null && comp == null)
                {
                    comp = t.gameObject.GetComponent<T>();
                    t = t.parent;
                }
            }

            return comp;
        }

        static public T FindInParents<T>(Transform trans) where T : Component
        {
            if (trans == null) return null;

            return trans.GetComponentInParent<T>();
        }

        static public string GetHierarchy(GameObject obj)
        {
            if (obj == null) return "";
            string path = obj.name;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "\\" + path;
            }
            return path;
        }

        static public void SetDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if (obj)
            {
                UnityEditor.EditorUtility.SetDirty(obj);
            }
#endif
        }
    }
 }