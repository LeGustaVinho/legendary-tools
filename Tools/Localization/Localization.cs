using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegendaryTools.UI
{
    public interface ILocalize
    {
        
    }
    
    public static class Localization
    {
        public static readonly string CURRENT_LANGUAGE = "CURRENT_LANGUAGE";
        public static readonly string LOCALIZATION_CONFIG_FILE_NAME = "LocalizationConfig";
        
        public static string currentLanguage
        {
            get { return PlayerPrefs.GetString(CURRENT_LANGUAGE); }
            set { PlayerPrefs.SetString(CURRENT_LANGUAGE, value); }
        }
        
        public static string[] Languages
        {
            get
            {
                if (LocalizationData != null && LocalizationData.Keys.Count > 0)
                    return LocalizationData.Keys.ToArray();
                
                return null;
            }
        }
        
        public static string[] Keys
        {
            get
            {
                if (LocalizationData != null && LocalizationData.Keys.Count > 0)
                    return LocalizationData[LocalizationData.Keys.First()].Keys.ToArray();

                return null;
            }
        }
        
        public static Dictionary<string, Dictionary<string, string>> LocalizationData;
        private static CSVFileReader csvFileReader;
        private static LocalizationConfig config;

        public static bool IsInit = false;

        public static void Init()
        {
            if (!IsInit)
            {
                LocalizationConfig config = Resources.Load<LocalizationConfig>(LOCALIZATION_CONFIG_FILE_NAME);

                if (config == null)
                    Debug.LogError("Cannot find LocalizationConfig at resources folder.");
                else
                    Init(config);
            }
        }
        
        public static void Init(LocalizationConfig config)
        {
            Localization.config = config;
            csvFileReader = new CSVFileReader(config.LocalizationData, config.FieldDelimiter, config.TextDelimiter);

            LocalizationData = csvFileReader.ReadAllCSV();
            
            IsInit = true;
        }
        
        public static string Get(string key)
        {
            if (LocalizationData.ContainsKey(currentLanguage))
            {
                if (LocalizationData[currentLanguage].ContainsKey(key))
                {
                    return LocalizationData[currentLanguage][key];
                }
            }
            
            Debug.LogError("Key " + key + " was not found in language " + currentLanguage);
            
            return string.Empty;
        }
    }
}