using System;
using System.Collections.Generic;
using UnityEngine;

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
            LoadEntries();

            var savedLanguage = PlayerPrefs.GetString(LanguagePrefsKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(savedLanguage))
            {
                _currentLanguageCode = NormalizeLanguageCode(savedLanguage);
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

            return languageCode.Trim().ToLowerInvariant();
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
    }
}
