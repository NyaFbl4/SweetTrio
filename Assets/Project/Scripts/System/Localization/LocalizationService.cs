using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using YG;

namespace Project.Scripts.System.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private const string ResourcePath = "Localization/localization_table";
        private const string DefaultLanguage = "ru";
        private const string EnglishLanguage = "en";
        private const string LanguagePrefsKey = "project.localization.language";

        private readonly Dictionary<string, LocalizationEntryData> _entries = new(StringComparer.Ordinal);

        private string _defaultLanguageCode = DefaultLanguage;
        private string _currentLanguageCode = DefaultLanguage;

        public string CurrentLanguageCode => _currentLanguageCode;

        public LocalizationService()
        {
            Debug.Log("init LocalizationService");
            YG2.onCorrectLang += OnChangeLang;
            LoadEntries();

            var detectedLanguage = TryDetectStartupLanguage();
            if (!string.IsNullOrWhiteSpace(detectedLanguage) && SetLanguage(detectedLanguage))
            {
                return;
            }

            var savedLanguage = PlayerPrefs.GetString(LanguagePrefsKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(savedLanguage))
            {
                SetLanguage(savedLanguage);
            }
        }
        public void InitLanguage()
        {
            
        }

        public static void OnChangeLang(string language)
        {
            if (language != "ru" && language != "en")
            {
                YG2.lang = "ru";
            }
        }

        public bool SetLanguage(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return false;

            var normalized = NormalizeLanguageCode(languageCode);
            if (normalized != DefaultLanguage && normalized != EnglishLanguage)
                return false;

            _currentLanguageCode = normalized;
            PlayerPrefs.SetString(LanguagePrefsKey, _currentLanguageCode);
            return true;
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            if (!_entries.TryGetValue(key, out var entry))
                return key;

            var localized = _currentLanguageCode == EnglishLanguage
                ? entry.English
                : entry.Russian;

            if (!string.IsNullOrWhiteSpace(localized))
                return localized;

            var fallback = _currentLanguageCode == EnglishLanguage
                ? entry.Russian
                : entry.English;

            return string.IsNullOrWhiteSpace(fallback) ? key : fallback;
        }

        public string Format(string key, params object[] args)
        {
            var template = Get(key);
            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                Debug.LogWarning($"LocalizationService: wrong format arguments for key '{key}'.");
                return template;
            }
        }

        private void LoadEntries()
        {
            _entries.Clear();

            var tableAsset = Resources.Load<TextAsset>(ResourcePath);
            if (tableAsset == null)
            {
                Debug.LogWarning("LocalizationService: localization table not found in Resources, using fallback values.");
                LoadFallbackEntries();
                return;
            }

            var tableData = JsonUtility.FromJson<LocalizationTableData>(tableAsset.text);
            if (tableData == null || tableData.entries == null || tableData.entries.Length == 0)
            {
                Debug.LogWarning("LocalizationService: localization table is empty, using fallback values.");
                LoadFallbackEntries();
                return;
            }

            _defaultLanguageCode = NormalizeLanguageCode(tableData.defaultLanguage);
            _currentLanguageCode = _defaultLanguageCode;

            for (var i = 0; i < tableData.entries.Length; i++)
            {
                var entry = tableData.entries[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.key))
                    continue;

                _entries[entry.key] = new LocalizationEntryData(entry.ru, entry.en);
            }

            if (_entries.Count == 0)
            {
                Debug.LogWarning("LocalizationService: localization table has no valid entries, using fallback values.");
                LoadFallbackEntries();
            }
        }

        private void LoadFallbackEntries()
        {
            _defaultLanguageCode = DefaultLanguage;
            _currentLanguageCode = DefaultLanguage;
            _entries.Clear();

            AddEntry(LocalizationKeys.MainMenuTitle, "ВЫБОР УРОВНЯ", "LEVEL SELECT");
            AddEntry(LocalizationKeys.MainMenuCurrentLevelFormat, "Текущий уровень: {0}", "Current level: {0}");
            AddEntry(LocalizationKeys.MainMenuChooseLevel, "Выбрать уровень", "Choose level");
            AddEntry(LocalizationKeys.MainMenuEmptyLevels, "Добавьте конфиги уровней в каталог уровней", "Add level configs to level catalog");
            AddEntry(LocalizationKeys.MainMenuNoLevelsConfigured, "Нет уровней", "No levels");

            AddEntry(LocalizationKeys.EndGameTitleWin, "Победа", "Victory");
            AddEntry(LocalizationKeys.EndGameTitleLose, "Поражение", "Defeat");
            AddEntry(LocalizationKeys.EndGameScoreFormat, "Очки: {0}", "Score: {0}");
            AddEntry(LocalizationKeys.EndGameMenuButton, "В меню", "Menu");

            AddEntry(LocalizationKeys.PauseTitle, "ПАУЗА", "PAUSE");

            AddEntry(LocalizationKeys.SettingsTitle, "НАСТРОЙКИ", "SETTINGS");
            AddEntry(LocalizationKeys.SettingsMusicLabel, "Музыка", "Music");
            AddEntry(LocalizationKeys.SettingsSoundLabel, "Звуки", "Sounds");
            AddEntry(LocalizationKeys.SettingsToggleOn, "ВКЛ", "ON");
            AddEntry(LocalizationKeys.SettingsToggleOff, "ВЫКЛ", "OFF");

            AddEntry(LocalizationKeys.RulesTitle, "ПРАВИЛА", "RULES");
            AddEntry(LocalizationKeys.RulesLevelTitle, "Правила уровня", "Level rules");
            AddEntry(
                LocalizationKeys.RulesTextTemplate,
                "Собирай одинаковые десерты в ряд по 3 и больше, чтобы получать очки.\n\n" +
                "Чем длиннее комбинация, тем больше награда.\n\n" +
                "Собирай десерты быстро и делай комбо подряд, чтобы получить бонусные очки.\n\n" +
                "Оставшееся в конце уровня время превращается в дополнительные очки.\n\n" +
                "Звезды за результат:\n" +
                "1 звезда — от {0} очков\n" +
                "2 звезды — от {1} очков\n" +
                "3 звезды — от {2} очков",
                "Match identical desserts in lines of 3 or more to earn points.\n\n" +
                "The longer the combo, the higher the reward.\n\n" +
                "Make matches quickly and chain combos to get bonus points.\n\n" +
                "Time left at the end of a level is converted into extra points.\n\n" +
                "Stars for the result:\n" +
                "1 star — from {0} points\n" +
                "2 stars — from {1} points\n" +
                "3 stars — from {2} points");

            AddEntry(LocalizationKeys.HudScoreFormat, "Очки: {0}", "Score: {0}");
            AddEntry(LocalizationKeys.HudDessertsLabel, "Десерты", "Desserts");
            AddEntry(LocalizationKeys.HudDessertsFormat, "Десерты: {0}", "Desserts: {0}");
            AddEntry(LocalizationKeys.HudShuffleButton, "Перемешать", "Shuffle");
            AddEntry(LocalizationKeys.HudMenuButton, "В меню", "Menu");

            AddEntry(LocalizationKeys.GameStatusNotEnoughToPassFormat, "Не хватило {0} очков до прохождения", "Need {0} more points to pass");
            AddEntry(LocalizationKeys.GameStatusLevelNotPassed, "Уровень не пройден", "Level failed");
            AddEntry(LocalizationKeys.GameStatusMaxResultFormat, "Максимальный результат: {0}/{1} звезд", "Best result: {0}/{1} stars");
            AddEntry(LocalizationKeys.GameStatusPassedStarsFormat, "Пройдено на {0}/{1} звезд", "Completed with {0}/{1} stars");
            AddEntry(LocalizationKeys.GameStatusToNextStarFormat, "Пройдено на {0}/{1}. До {2}-й звезды: {3}", "Completed with {0}/{1}. To star {2}: {3}");
        }

        private void AddEntry(string key, string russian, string english)
        {
            _entries[key] = new LocalizationEntryData(russian, english);
        }

        private static string NormalizeLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return DefaultLanguage;

            var normalized = languageCode.Trim().ToLowerInvariant().Replace('_', '-');
            if (normalized.StartsWith("ru", StringComparison.Ordinal))
                return DefaultLanguage;
            if (normalized.StartsWith("en", StringComparison.Ordinal))
                return EnglishLanguage;

            return EnglishLanguage;
        }

        private static string TryDetectStartupLanguage()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                var languageFromWeb = ProjectLanguageBridge.GetAutoLanguageCode();
                if (!string.IsNullOrWhiteSpace(languageFromWeb))
                    return languageFromWeb;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"LocalizationService: failed to read language from WebGL bridge. {ex.Message}");
            }
#endif

            return Application.systemLanguage switch
            {
                SystemLanguage.Russian => DefaultLanguage,
                SystemLanguage.English => EnglishLanguage,
                _ => EnglishLanguage
            };
        }

        [Serializable]
        private class LocalizationTableData
        {
            public string defaultLanguage = DefaultLanguage;
            public LocalizationEntry[] entries;
        }

        [Serializable]
        private class LocalizationEntry
        {
            public string key;
            public string ru;
            public string en;
        }

        private readonly struct LocalizationEntryData
        {
            public LocalizationEntryData(string russian, string english)
            {
                Russian = russian;
                English = english;
            }

            public string Russian { get; }
            public string English { get; }
        }

        private static class ProjectLanguageBridge
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern string Project_GetAutoLanguage();
#endif

            public static string GetAutoLanguageCode()
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return Project_GetAutoLanguage();
#else
                return string.Empty;
#endif
            }
        }
    }
}
