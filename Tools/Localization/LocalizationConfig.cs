using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.UI
{
    public class LocalizationConfig : ScriptableObject
    {
        public TextAsset LocalizationData;
        public char FieldDelimiter = ',';
        public char TextDelimiter = '"';
    }
}