using UnityEngine;
using UnityEngine.UI;

namespace LegendaryTools.UI
{
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Localize Text")]
    public class TextLocalize : BaseLocalize
    {
        private Text Text;

        protected override void Start()
        {
            base.Start();

            Localize(Localization.Get(key));
        }

        public override void Localize(string value)
        {
            if (string.IsNullOrEmpty(key)) return;
            
            if (Text == null)
                Text = GetComponent<Text>();

            if (Text == null) return;
            Text.text = value;
                    
#if UNITY_EDITOR
            if(!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}