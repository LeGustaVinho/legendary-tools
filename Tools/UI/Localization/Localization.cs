using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegendaryTools.UI
{
    public static class Localization
    {
        public delegate void OnLocalizationLanguageChangeEventHandler(string oldLang, string newLang);

        private static readonly string CURRENT_LANGUAGE = "CURRENT_LANGUAGE";
        private static readonly string LOCALIZATION_CONFIG_FILE_NAME = "LocalizationConfig";

        public static Dictionary<string, Dictionary<string, string>> LocalizationData =
            new Dictionary<string, Dictionary<string, string>>();

        private static CSVFileReader csvFileReader;
        private static LocalizationConfig config;

        public static bool IsInit;

        public static string CurrentLanguage
        {
            get => PlayerPrefs.GetString(CURRENT_LANGUAGE, Languages.Length > 0 ? Languages[0] : string.Empty);
            set
            {
                if (IsInit && LocalizationData.ContainsKey(value))
                {
                    string oldLang = CurrentLanguage;
                    PlayerPrefs.SetString(CURRENT_LANGUAGE, value);
                    OnLocalizationLanguageChange?.Invoke(oldLang, value);
                }
            }
        }

        public static string[] Languages
        {
            get
            {
                if (LocalizationData != null && LocalizationData.Keys.Count > 0)
                {
                    return LocalizationData.Keys.ToArray();
                }

                return null;
            }
        }

        public static string[] Keys
        {
            get
            {
                if (LocalizationData != null && LocalizationData.Keys.Count > 0)
                {
                    return LocalizationData[LocalizationData.Keys.First()].Keys.ToArray();
                }

                return null;
            }
        }

        public static event OnLocalizationLanguageChangeEventHandler OnLocalizationLanguageChange;

        public static void Init()
        {
            if (!IsInit)
            {
                LocalizationConfig config = Resources.Load<LocalizationConfig>(LOCALIZATION_CONFIG_FILE_NAME);

                if (config == null)
                {
                    Debug.LogError("Cannot find LocalizationConfig at resources folder.");
                }
                else
                {
                    Init(config);
                }
            }
        }

        public static void Init(LocalizationConfig config)
        {
            Localization.config = config;
            csvFileReader = new CSVFileReader(config.LocalizationData, config.FieldDelimiter, config.TextDelimiter);

            try
            {
                LocalizationData = csvFileReader.ReadAllCSV();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            IsInit = true;
        }

        public static string Get(string key)
        {
            Init();

            if (LocalizationData.ContainsKey(CurrentLanguage))
            {
                if (LocalizationData[CurrentLanguage].ContainsKey(key))
                {
                    return LocalizationData[CurrentLanguage][key];
                }
            }

            Debug.LogError("Key " + key + " was not found in language " + CurrentLanguage);

            return string.Empty;
        }

        public static string GetAndFormat(string key, params string[] args)
        {
            string localized = Get(key);
            if (string.IsNullOrEmpty(localized))
            {
                return string.Empty;
            }

            return string.Format(localized, args);
        }

        public static bool Contains(string key)
        {
            Init();

            if (LocalizationData.ContainsKey(CurrentLanguage))
            {
                return LocalizationData[CurrentLanguage].ContainsKey(key);
            }

            return false;
        }
    }
}