using System;
using System.Collections.Generic;
using Project.Scripts.GameManager;
using Project.Scripts.System.Localization;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuView : LayoutViewBase, IMainMenuView
    {
        private const int LevelsPerPage = 12;
        private const int LevelsPerRow = 3;

        [Inject] private readonly ILocalizationService _localizationService;

        private Button _paginationPrevButton;
        private Button _paginationNextButton;
        private Button _settingsButton;
        private VisualElement _overlay;
        private VisualElement _banner;
        private VisualElement _levelsTab;
        private ScrollView _levelsList;
        private Label _titleLabel;
        private Label _selectedLevelLabel;
        private Label _emptyLevelsLabel;
        private VisualElement _centerColumn;
        private VisualElement _paginationIndicatorsContainer;

        private readonly List<Button> _levelButtons = new();
        private readonly List<LevelEntry> _levels = new();

        private VisualTreeAsset _levelCellTemplate;

        private LevelConfig _selectedLevel;
        private int _currentPageIndex;
        private int _totalPages = 1;

        public event Action<LevelConfig> LevelSelected;
        public event Action SettingsClicked;

        public override void Awake()
        {
            base.Awake();

            _paginationPrevButton = _root.Q<Button>("main-menu-pagination-prev-button");
            _paginationNextButton = _root.Q<Button>("main-menu-pagination-next-button");
            _settingsButton = _root.Q<Button>("main-menu-settings-button");
            _overlay = _root.Q<VisualElement>("main-menu-overlay");
            _banner = _root.Q<VisualElement>("main-menu-banner");
            _titleLabel = _root.Q<Label>("main-menu-title-label");
            _levelsTab = _root.Q<VisualElement>("main-menu-levels-tab");
            _levelsList = _root.Q<ScrollView>("main-menu-levels-list");
            _selectedLevelLabel = _root.Q<Label>("main-menu-selected-level-label");
            _emptyLevelsLabel = _root.Q<Label>("main-menu-empty-levels-label");
            _centerColumn = _root.Q<VisualElement>("main-menu-center-column");
            _paginationIndicatorsContainer = _root.Q<VisualElement>("main-menu-pagination-indicators");

            EnsureRequiredElements();
            LoadTemplates();

            if (_paginationPrevButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_paginationPrevButton);
                _paginationPrevButton.clicked += HandlePaginationPrevClicked;
            }

            if (_paginationNextButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_paginationNextButton);
                _paginationNextButton.clicked += HandlePaginationNextClicked;
            }

            if (_settingsButton != null)
            {
                UIButtonAnimationUtility.EnableDefault(_settingsButton);
                _settingsButton.clicked += HandleSettingsClicked;
            }

            ApplyTexts();
            SetLevelsTabVisible(true);
            SetSelectedLevel(null);
            UpdatePaginationState();
        }

        private void OnDestroy()
        {
            if (_paginationPrevButton != null)
                _paginationPrevButton.clicked -= HandlePaginationPrevClicked;

            if (_paginationNextButton != null)
                _paginationNextButton.clicked -= HandlePaginationNextClicked;

            if (_settingsButton != null)
                _settingsButton.clicked -= HandleSettingsClicked;

            _levelButtons.Clear();
            _levels.Clear();
        }

        public void SetLevels(IReadOnlyList<LevelConfig> levels, LevelConfig selectedLevel, IReadOnlyList<int> savedStars)
        {
            _levels.Clear();

            if (levels != null)
            {
                for (var i = 0; i < levels.Count; i++)
                {
                    var levelConfig = levels[i];
                    if (levelConfig == null)
                        continue;

                    var stars = 0;
                    if (savedStars != null && i < savedStars.Count)
                    {
                        stars = Mathf.Clamp(savedStars[i], 0, LevelConfig.TotalStarsCount);
                    }

                    _levels.Add(new LevelEntry(levelConfig, i + 1, stars));
                }
            }

            _totalPages = Mathf.Max(1, Mathf.CeilToInt(_levels.Count / (float)LevelsPerPage));
            _currentPageIndex = ResolveInitialPageIndex(selectedLevel);

            if (_emptyLevelsLabel != null)
                _emptyLevelsLabel.style.display = _levels.Count > 0 ? DisplayStyle.None : DisplayStyle.Flex;

            RenderCurrentPage();
            SetSelectedLevel(selectedLevel);
        }

        public void SetLevelsTabVisible(bool visible)
        {
            if (_levelsTab == null)
                return;

            _levelsTab.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetSelectedLevel(LevelConfig selectedLevel)
        {
            _selectedLevel = selectedLevel;

            if (_selectedLevelLabel != null)
            {
                var title = _selectedLevel != null ? _selectedLevel.name : string.Empty;
                _selectedLevelLabel.text = string.IsNullOrWhiteSpace(title)
                    ? string.Empty
                    : FormatLocalizedText(LocalizationKeys.MainMenuCurrentLevelFormat, "Current level: {0}", title);
            }

            EnsureSelectedLevelPageVisible();
            RefreshLevelButtonSelection();
        }

        private int ResolveInitialPageIndex(LevelConfig selectedLevel)
        {
            if (selectedLevel == null || _levels.Count == 0)
                return 0;

            for (var i = 0; i < _levels.Count; i++)
            {
                if (_levels[i].LevelConfig == selectedLevel)
                    return i / LevelsPerPage;
            }

            return 0;
        }

        private void EnsureSelectedLevelPageVisible()
        {
            if (_selectedLevel == null || _levels.Count == 0)
                return;

            for (var i = 0; i < _levels.Count; i++)
            {
                if (_levels[i].LevelConfig != _selectedLevel)
                    continue;

                var requiredPage = i / LevelsPerPage;
                if (requiredPage != _currentPageIndex)
                {
                    _currentPageIndex = requiredPage;
                    RenderCurrentPage();
                }

                return;
            }
        }

        private void RenderCurrentPage()
        {
            if (_levelsList == null)
                return;

            _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, Mathf.Max(0, _totalPages - 1));

            _levelsList.Clear();
            _levelButtons.Clear();

            if (_levels.Count > 0)
            {
                var startIndex = _currentPageIndex * LevelsPerPage;
                var endIndex = Mathf.Min(startIndex + LevelsPerPage, _levels.Count);
                VisualElement currentRow = null;
                var rowItemIndex = 0;

                for (var i = startIndex; i < endIndex; i++)
                {
                    if (rowItemIndex % LevelsPerRow == 0)
                    {
                        currentRow = CreateLevelsRowContainer();
                        _levelsList.Add(currentRow);
                    }

                    var entry = _levels[i];
                    var levelButton = CreateLevelButton(entry.LevelConfig, entry.LevelNumber, entry.EarnedStarsCount);
                    currentRow?.Add(levelButton);
                    _levelButtons.Add(levelButton);
                    rowItemIndex++;
                }
            }

            UpdatePaginationState();
            RefreshLevelButtonSelection();
        }

        private Button CreateLevelButton(LevelConfig levelConfig, int levelNumber, int earnedStarsCount)
        {
            var templateButton = CreateLevelButtonFromTemplate(levelConfig, levelNumber, earnedStarsCount);
            return templateButton ?? CreateLevelButtonFallback(levelConfig, levelNumber);
        }

        private Button CreateLevelButtonFromTemplate(LevelConfig levelConfig, int levelNumber, int earnedStarsCount)
        {
            if (_levelCellTemplate == null)
                return null;

            var templateRoot = _levelCellTemplate.CloneTree();
            var button = templateRoot.Q<Button>("main-menu-level-cell-button") ?? templateRoot.Q<Button>();
            if (button == null)
                return null;

            button.text = string.Empty;
            button.userData = levelConfig;
            button.clicked += () => HandleLevelSelected(levelConfig);

            var starsStrip = button.Q<VisualElement>("main-menu-level-cell-stars");
            ApplyStarsState(starsStrip, earnedStarsCount);

            var levelNumberLabel = button.Q<Label>("main-menu-level-cell-number-label");
            if (levelNumberLabel != null)
                levelNumberLabel.text = levelNumber.ToString();

            UIButtonAnimationUtility.EnableDefault(button);
            return button;
        }

        private Button CreateLevelButtonFallback(LevelConfig levelConfig, int levelNumber)
        {
            var button = new Button(() => HandleLevelSelected(levelConfig))
            {
                userData = levelConfig,
                text = levelNumber.ToString()
            };

            UIButtonAnimationUtility.EnableDefault(button);
            return button;
        }

        private static VisualElement CreateLevelsRowContainer()
        {
            var row = new VisualElement();
            row.style.width = Length.Percent(100f);
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.Center;
            row.style.alignItems = Align.FlexStart;
            row.style.marginBottom = 2f;
            return row;
        }

        private void RefreshLevelButtonSelection()
        {
            for (var i = 0; i < _levelButtons.Count; i++)
            {
                var levelButton = _levelButtons[i];
                var buttonLevel = levelButton.userData as LevelConfig;
                var isSelected = buttonLevel != null && buttonLevel == _selectedLevel;

                if (isSelected)
                    levelButton.AddToClassList("main-menu-level-cell-button--selected");
                else
                    levelButton.RemoveFromClassList("main-menu-level-cell-button--selected");

                UIButtonAnimationUtility.SetBaseScale(levelButton, isSelected ? 1.05f : 1f);
            }
        }

        private static void ApplyStarsState(VisualElement starsStrip, int earnedStarsCount)
        {
            if (starsStrip == null)
                return;

            for (var i = 0; i <= LevelConfig.TotalStarsCount; i++)
            {
                starsStrip.RemoveFromClassList("main-menu-level-cell-stars--" + i);
            }

            var clampedStars = Mathf.Clamp(earnedStarsCount, 0, LevelConfig.TotalStarsCount);
            starsStrip.AddToClassList("main-menu-level-cell-stars--" + clampedStars);
        }

        private void UpdatePaginationState()
        {
            if (_paginationPrevButton != null)
            {
                var canGoPrev = _currentPageIndex > 0;
                _paginationPrevButton.SetEnabled(canGoPrev);
                _paginationPrevButton.style.opacity = canGoPrev ? 1f : 0.45f;
            }

            if (_paginationNextButton != null)
            {
                var canGoNext = _currentPageIndex < _totalPages - 1;
                _paginationNextButton.SetEnabled(canGoNext);
                _paginationNextButton.style.opacity = canGoNext ? 1f : 0.45f;
            }

            RebuildPageIndicators();
        }

        private void RebuildPageIndicators()
        {
            if (_paginationIndicatorsContainer == null)
                return;

            _paginationIndicatorsContainer.Clear();
            for (var i = 0; i < _totalPages; i++)
            {
                var outerCircle = new VisualElement();
                outerCircle.AddToClassList("main-menu-page-indicator-outer");

                var innerCircle = new VisualElement();
                innerCircle.AddToClassList("main-menu-page-indicator-inner");
                innerCircle.AddToClassList(i == _currentPageIndex
                    ? "main-menu-page-indicator-inner--active"
                    : "main-menu-page-indicator-inner--inactive");

                outerCircle.Add(innerCircle);
                _paginationIndicatorsContainer.Add(outerCircle);
            }
        }

        private void HandleLevelSelected(LevelConfig levelConfig)
        {
            LevelSelected?.Invoke(levelConfig);
        }

        private void HandlePaginationPrevClicked()
        {
            if (_currentPageIndex <= 0)
                return;

            _currentPageIndex--;
            RenderCurrentPage();
        }

        private void HandleSettingsClicked()
        {
            SettingsClicked?.Invoke();
        }

        private void HandlePaginationNextClicked()
        {
            if (_currentPageIndex >= _totalPages - 1)
                return;

            _currentPageIndex++;
            RenderCurrentPage();
        }

        private void EnsureRequiredElements()
        {
            if (_root == null)
                return;

            _overlay ??= _root.Q<VisualElement>("main-menu-overlay") ?? _root;
            _banner ??= _root.Q<VisualElement>("main-menu-banner");
            _titleLabel ??= _root.Q<Label>("main-menu-title-label");
            _levelsTab ??= _root.Q<VisualElement>("main-menu-levels-tab");
            _levelsList ??= _root.Q<ScrollView>("main-menu-levels-list");
            _selectedLevelLabel ??= _root.Q<Label>("main-menu-selected-level-label");
            _emptyLevelsLabel ??= _root.Q<Label>("main-menu-empty-levels-label");
            _centerColumn ??= _root.Q<VisualElement>("main-menu-center-column");
            _paginationIndicatorsContainer ??= _root.Q<VisualElement>("main-menu-pagination-indicators");

            if (_levelsList != null)
                ConfigureLevelsListLayout();
            if (_levelsTab == null || _levelsList == null || _paginationIndicatorsContainer == null)
            {
                Debug.LogError("MainMenuView: Required elements not found in UXML.");
            }
        }

        private void ConfigureLevelsListLayout()
        {
            if (_levelsList == null)
                return;

            _levelsList.style.backgroundColor = Color.clear;
            _levelsList.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _levelsList.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            _levelsList.style.paddingLeft = 0f;
            _levelsList.style.paddingRight = 0f;

            var content = _levelsList.contentContainer;
            content.style.width = Length.Percent(100f);
            content.style.flexDirection = FlexDirection.Column;
            content.style.flexWrap = Wrap.NoWrap;
            content.style.justifyContent = Justify.FlexStart;
            content.style.alignItems = Align.Stretch;
            content.style.alignContent = Align.Stretch;
        }

        private void LoadTemplates()
        {
            _levelCellTemplate = Resources.Load<VisualTreeAsset>("UIToolkit/MainMenuLevelCell");
            if (_levelCellTemplate == null)
            {
                Debug.LogError("MainMenuView: MainMenuLevelCell template not found in Resources/UIToolkit.");
            }
        }

        private void ApplyTexts()
        {
            if (_titleLabel != null)
                _titleLabel.text = GetLocalizedText(LocalizationKeys.MainMenuTitle, "LEVEL SELECT");

            if (_emptyLevelsLabel != null)
                _emptyLevelsLabel.text = GetLocalizedText(LocalizationKeys.MainMenuEmptyLevels, "Add level configs to level catalog");
        }

        private string GetLocalizedText(string key, string fallback)
        {
            if (_localizationService == null)
                return fallback;

            var text = _localizationService.Get(key);
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private string FormatLocalizedText(string key, string fallbackFormat, params object[] args)
        {
            if (_localizationService != null)
                return _localizationService.Format(key, args);

            try
            {
                return string.Format(fallbackFormat, args);
            }
            catch (FormatException)
            {
                return fallbackFormat;
            }
        }

        private readonly struct LevelEntry
        {
            public LevelEntry(LevelConfig levelConfig, int levelNumber, int earnedStarsCount)
            {
                LevelConfig = levelConfig;
                LevelNumber = levelNumber;
                EarnedStarsCount = earnedStarsCount;
            }

            public LevelConfig LevelConfig { get; }
            public int LevelNumber { get; }
            public int EarnedStarsCount { get; }
        }
    }
}


