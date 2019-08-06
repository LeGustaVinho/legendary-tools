using UnityEngine;
using UnityEngine.UI;

namespace LegendaryTools
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("UI/Localize Text")]
    public class TextLocalize : BaseLocalize
    {
        private Text text;

        public override void OnLocalize()
        {
            // If no localization key has been specified, use the label's text as the key
            if (string.IsNullOrEmpty(key))
            {
                if(text == null) text = GetComponent<Text>();
                if (text != null) key = text.text;
            }

            // If we still don't have a key, leave the value as blank
            if (!string.IsNullOrEmpty(key)) value = LocalizationManager.Get(key);
        }
    }
}