using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LegendaryTools.UI
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
        public bool autoDisableSelectable;

        public event OnPanelAlphaChangeEventHandler OnPanelAlphaChange;

        protected virtual void Awake()
        {
            Init();
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

                    if (autoDisableSelectable && CanvasGroup.alpha == 0)
                    {
                        SetAllSelectable(false);
                    }
                    else if (CanvasGroup.alpha > 0 && canvasAlpha == 0)
                    {
                        SetAllSelectable(true);
                    }

                    OnPanelAlphaChange?.Invoke(canvasAlpha, CanvasGroup.alpha);

                    canvasAlpha = CanvasGroup.alpha;
                }
            }
        }
        
        protected virtual void Reset()
        {
            Init();
        }

        private void Init()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            if (CanvasGroup != null) canvasAlpha = CanvasGroup.alpha;
        }
        
        public void SetAllSelectable(bool mode)
        {
            Selectable[] allSelectable = GetComponentsInChildren<Selectable>();
            for (int i = 0; i < allSelectable.Length; i++)
            {
                if(allSelectable[i] != null)
                    allSelectable[i].enabled = mode;
            }
        }
    }
}