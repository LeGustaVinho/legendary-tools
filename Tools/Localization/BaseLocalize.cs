using UnityEngine;

namespace LegendaryTools.UI
{
    public abstract class BaseLocalize : MonoBehaviour
    {
        /// <summary>
        /// Localization key.
        /// </summary>
        public string key;

        protected virtual void Start()
        {
            Localization.OnLocalizationLanguageChange += OnLocalizationLanguageChange;
        }
        
        protected  virtual void OnDestroy()
        {
            Localization.OnLocalizationLanguageChange -= OnLocalizationLanguageChange;
        }

        protected virtual void OnLocalizationLanguageChange(string oldLanguage, string newLanguage)
        {
            Localize(Localization.Get(key));
        }
        public abstract void Localize(string value);
    }
 }