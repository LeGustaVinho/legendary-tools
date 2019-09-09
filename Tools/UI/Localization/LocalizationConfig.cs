using UnityEngine;

namespace LegendaryTools.UI
{
    public class LocalizationConfig : ScriptableObject
    {
        public char FieldDelimiter = ',';
        public TextAsset LocalizationData;
        public char TextDelimiter = '"';
    }
}