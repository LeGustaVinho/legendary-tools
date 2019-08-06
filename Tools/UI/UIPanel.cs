using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LegendaryTools
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("UI/Layout/Panel")]
    public class UIPanel : MonoBehaviour
    {
        public delegate void OnPanelAlphaChangeEventHandler(float oldValue, float newValue);

        protected CanvasGroup CanvasGroup;
        private float canvasAlpha;

        public bool blocksRaycasts = true;
        public bool ignoreParentGroups;
        public bool interactable = true;
        public List<Selectable> allSelectable = new List<Selectable>();

        public bool AutoDisableSelectable
        {
            get { return autoDisableSelectable; }
            set
            {
                autoDisableSelectable = value;
                UpdateSelectableList();
                checkSelectable();
            }
        }
        private bool autoDisableSelectable = true;

        public event OnPanelAlphaChangeEventHandler OnPanelAlphaChange;

        protected virtual void Awake()
        {
            Init();
        }

        void Init()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            if (CanvasGroup != null) canvasAlpha = CanvasGroup.alpha;

            allSelectable.Clear();
            allSelectable.AddRange(GetComponentsInChildren<Selectable>());

            UpdateSelectableList();

            checkSelectable();
        }

        protected virtual void Update()
        {
            if (CanvasGroup != null)
            {
                if (canvasAlpha != CanvasGroup.alpha)
                {
                    CanvasGroup.blocksRaycasts = CanvasGroup.alpha > 0 && blocksRaycasts;
                    CanvasGroup.interactable = CanvasGroup.alpha > 0 && interactable;
                    CanvasGroup.ignoreParentGroups = ignoreParentGroups;

                    checkSelectable();

                    if (OnPanelAlphaChange != null)
                        OnPanelAlphaChange.Invoke(canvasAlpha, CanvasGroup.alpha);

                    canvasAlpha = CanvasGroup.alpha;
                }
            }
        }

        public void SetAllSelectable(bool mode)
        {
            for (int i = 0; i < allSelectable.Count; i++)
            {
                if(allSelectable[i] != null)
                    allSelectable[i].enabled = mode;
            }
        }

        public void UpdateSelectableList()
        {
            allSelectable.Clear();
            allSelectable.AddRange(GetComponentsInChildren<Selectable>());
        }

        protected virtual void Reset()
        {
            Init();
        }

        private void checkSelectable()
        {
            if (AutoDisableSelectable)
                SetAllSelectable(CanvasGroup.alpha > 0);
        }
    }
}