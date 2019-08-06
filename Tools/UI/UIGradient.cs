using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LegendaryTools.UI
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class UIGradient : BaseMeshEffect
    {
        public Color TopLeft = Color.white;
        public Color TopRight = Color.white;
        public Color BottonLeft = Color.black;
        public Color BottonRight = Color.black;

        private UIPanel Panel;

        private float xMin = 0;
        private float xMax = 0;
        private float yMin = 0;
        private float yMax = 0;

        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive() || helper.currentVertCount == 0)
                return;

            UIVertex v = new UIVertex();
            xMin = xMin = yMin = yMax = 0;
            for (int i = 0; i < helper.currentVertCount; i++)
            {
                helper.PopulateUIVertex(ref v, i);

                if (v.position.y <= yMin)
                    yMin = v.position.y;

                if (v.position.y >= yMax)
                    yMax = v.position.y;

                if (v.position.x <= xMin)
                    xMin = v.position.x;

                if (v.position.x >= xMax)
                    xMax = v.position.x;
            }

            for (int i = 0; i < helper.currentVertCount; i++)
            {
                helper.PopulateUIVertex(ref v, i);

                v.color = BilinearColor(BottonLeft, BottonRight, TopLeft, TopRight, v.position.x.Remap(xMin, xMax, 0, 1), v.position.y.Remap(yMin, yMax, 0, 1));

                helper.SetUIVertex(v, i);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Panel = GetComponentInParent<UIPanel>();

            if (Panel != null)
                Panel.OnPanelAlphaChange += OnPanelAlphaChange;
        }

        private void OnPanelAlphaChange(float oldValue, float newValue)
        {
            enabled = newValue > 0;
        }

        public Color BilinearColor(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight, float x, float y)
        {
            return Color.Lerp(Color.Lerp(topLeft, topRight, Mathf.Clamp01(x)), Color.Lerp(bottomLeft, bottomRight, Mathf.Clamp01(x)), Mathf.Clamp01(y));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Panel != null)
                Panel.OnPanelAlphaChange -= OnPanelAlphaChange;
        }
    }
}