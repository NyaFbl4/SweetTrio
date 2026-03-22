using System;
using System.Collections.Generic;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuView : LayoutViewBase, IMainMenuView
    {
        private const string ChooseLevelButtonName = "main-menu-choose-level-button";
        private const string LegacyStartButtonName = "main-menu-start-button";
        private const string LevelsTabName = "main-menu-levels-tab";
        private const string LevelsListName = "main-menu-levels-list";
        private const string SelectedLevelLabelName = "main-menu-selected-level-label";
        private const string EmptyLevelsLabelName = "main-menu-empty-levels-label";
        private const string CenterColumnName = "main-menu-center-column";

        private static readonly Color CardBackground = new(0.86f, 0.2f, 0.2f, 1f);
        private static readonly Color CardSelectedBackground = new(0.93f, 0.29f, 0.25f, 1f);
        private static readonly Color CardBorder = new(0.62f, 0.07f, 0.11f, 1f);
        private static readonly Color StarEmptyFill = new(1f, 1f, 1f, 0f);
        private static readonly Color StarEmptyBorder = new(1f, 1f, 1f, 1f);
        private static readonly Color StarEarnedFill = new(1f, 0.87f, 0.24f, 1f);
        private static readonly Color StarEarnedBorder = new(1f, 0.87f, 0.24f, 1f);

        private Button _chooseLevelButton;
        private VisualElement _levelsTab;
        private ScrollView _levelsList;
        private Label _selectedLevelLabel;
        private Label _emptyLevelsLabel;
        private VisualElement _centerColumn;
        private readonly List<Button> _levelButtons = new();

        private LevelConfig _selectedLevel;

        public event Action ChooseLevelClicked;
        public event Action<LevelConfig> LevelSelected;

        public override void Awake()
        {
            base.Awake();

            _chooseLevelButton = _root.Q<Button>(ChooseLevelButtonName) ?? _root.Q<Button>(LegacyStartButtonName);
            _levelsTab = _root.Q<VisualElement>(LevelsTabName);
            _levelsList = _root.Q<ScrollView>(LevelsListName);
            _selectedLevelLabel = _root.Q<Label>(SelectedLevelLabelName);
            _emptyLevelsLabel = _root.Q<Label>(EmptyLevelsLabelName);
            _centerColumn = _root.Q<VisualElement>(CenterColumnName);

            EnsureRequiredElements();

            if (_chooseLevelButton != null)
            {
                _chooseLevelButton.clicked += HandleChooseLevelClicked;
            }

            SetLevelsTabVisible(true);
            SetSelectedLevel(null);
        }

        private void OnDestroy()
        {
            if (_chooseLevelButton != null)
            {
                _chooseLevelButton.clicked -= HandleChooseLevelClicked;
            }

            _levelButtons.Clear();
        }

        public void SetLevels(IReadOnlyList<LevelConfig> levels, LevelConfig selectedLevel, IReadOnlyList<int> savedStars)
        {
            if (_levelsList == null)
                return;

            _levelsList.Clear();
            _levelButtons.Clear();

            var hasLevels = false;
            if (levels != null)
            {
                for (var i = 0; i < levels.Count; i++)
                {
                    var levelConfig = levels[i];
                    if (levelConfig == null)
                        continue;

                    hasLevels = true;
                    var levelNumber = i + 1;
                    var levelCopy = levelConfig;
                    var earnedStars = 0;
                    if (savedStars != null && i < savedStars.Count)
                    {
                        earnedStars = Mathf.Clamp(savedStars[i], 0, LevelConfig.TotalStarsCount);
                    }

                    var levelButton = CreateLevelButton(levelCopy, levelNumber, earnedStars);
                    _levelsList.Add(levelButton);
                    _levelButtons.Add(levelButton);
                }
            }

            if (_emptyLevelsLabel != null)
            {
                _emptyLevelsLabel.style.display = hasLevels ? DisplayStyle.None : DisplayStyle.Flex;
            }

            SetSelectedLevel(selectedLevel);
        }

        public void SetLevelsTabVisible(bool visible)
        {
            if (_levelsTab == null)
                return;

            _levelsTab.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetChooseLevelButtonText(string text)
        {
            if (_chooseLevelButton == null)
                return;

            _chooseLevelButton.text = text;
        }

        public void SetSelectedLevel(LevelConfig selectedLevel)
        {
            _selectedLevel = selectedLevel;

            if (_selectedLevelLabel != null)
            {
                var title = _selectedLevel != null ? _selectedLevel.name : "";
                _selectedLevelLabel.text = string.IsNullOrWhiteSpace(title) ? "" : $"Текущий уровень: {title}";
            }

            RefreshLevelButtonSelection();
        }

        private Button CreateLevelButton(LevelConfig levelConfig, int levelNumber, int earnedStarsCount)
        {
            var button = new Button(() => HandleLevelSelected(levelConfig))
            {
                text = string.Empty,
                userData = levelConfig
            };

            button.style.width = 164f;
            button.style.height = 164f;
            button.style.marginTop = 8f;
            button.style.marginBottom = 8f;
            button.style.marginLeft = 8f;
            button.style.marginRight = 8f;
            button.style.paddingTop = 6f;
            button.style.paddingBottom = 8f;
            button.style.paddingLeft = 8f;
            button.style.paddingRight = 8f;
            button.style.flexDirection = FlexDirection.Column;
            button.style.alignItems = Align.Center;
            button.style.justifyContent = Justify.SpaceBetween;
            button.style.backgroundColor = CardBackground;
            button.style.borderTopLeftRadius = 20f;
            button.style.borderTopRightRadius = 20f;
            button.style.borderBottomLeftRadius = 20f;
            button.style.borderBottomRightRadius = 20f;
            button.style.borderLeftWidth = 3f;
            button.style.borderRightWidth = 3f;
            button.style.borderTopWidth = 3f;
            button.style.borderBottomWidth = 3f;
            button.style.borderLeftColor = CardBorder;
            button.style.borderRightColor = CardBorder;
            button.style.borderTopColor = CardBorder;
            button.style.borderBottomColor = CardBorder;

            var starsRow = new VisualElement();
            starsRow.style.flexDirection = FlexDirection.Row;
            starsRow.style.alignItems = Align.Center;
            starsRow.style.justifyContent = Justify.Center;
            starsRow.style.marginTop = 2f;
            button.Add(starsRow);
            for (var i = 0; i < 3; i++)
            {
                var starCircle = new VisualElement();
                starCircle.style.width = 20f;
                starCircle.style.height = 20f;
                starCircle.style.marginLeft = 3f;
                starCircle.style.marginRight = 3f;
                starCircle.style.borderTopLeftRadius = 10f;
                starCircle.style.borderTopRightRadius = 10f;
                starCircle.style.borderBottomLeftRadius = 10f;
                starCircle.style.borderBottomRightRadius = 10f;
                starCircle.style.borderLeftWidth = 2f;
                starCircle.style.borderRightWidth = 2f;
                starCircle.style.borderTopWidth = 2f;
                starCircle.style.borderBottomWidth = 2f;
                starCircle.style.borderLeftColor = StarEmptyBorder;
                starCircle.style.borderRightColor = StarEmptyBorder;
                starCircle.style.borderTopColor = StarEmptyBorder;
                starCircle.style.borderBottomColor = StarEmptyBorder;
                var isEarned = i < earnedStarsCount;
                starCircle.style.borderLeftColor = isEarned ? StarEarnedBorder : StarEmptyBorder;
                starCircle.style.borderRightColor = isEarned ? StarEarnedBorder : StarEmptyBorder;
                starCircle.style.borderTopColor = isEarned ? StarEarnedBorder : StarEmptyBorder;
                starCircle.style.borderBottomColor = isEarned ? StarEarnedBorder : StarEmptyBorder;
                starCircle.style.backgroundColor = isEarned ? StarEarnedFill : StarEmptyFill;
                starsRow.Add(starCircle);
            }

            var plate = new VisualElement();
            plate.style.width = 120f;
            plate.style.height = 102f;
            plate.style.backgroundColor = new Color(0.36f, 0.53f, 0.86f, 1f);
            plate.style.borderTopLeftRadius = 16f;
            plate.style.borderTopRightRadius = 16f;
            plate.style.borderBottomLeftRadius = 16f;
            plate.style.borderBottomRightRadius = 16f;
            plate.style.borderLeftWidth = 3f;
            plate.style.borderRightWidth = 3f;
            plate.style.borderTopWidth = 3f;
            plate.style.borderBottomWidth = 3f;
            plate.style.borderLeftColor = new Color(0.26f, 0.36f, 0.68f, 1f);
            plate.style.borderRightColor = new Color(0.26f, 0.36f, 0.68f, 1f);
            plate.style.borderTopColor = new Color(0.26f, 0.36f, 0.68f, 1f);
            plate.style.borderBottomColor = new Color(0.26f, 0.36f, 0.68f, 1f);
            plate.style.alignItems = Align.Center;
            plate.style.justifyContent = Justify.Center;
            plate.style.marginBottom = 6f;

            var numberCircle = new VisualElement();
            numberCircle.style.width = 68f;
            numberCircle.style.height = 68f;
            numberCircle.style.borderTopLeftRadius = 34f;
            numberCircle.style.borderTopRightRadius = 34f;
            numberCircle.style.borderBottomLeftRadius = 34f;
            numberCircle.style.borderBottomRightRadius = 34f;
            numberCircle.style.backgroundColor = new Color(0.18f, 0.33f, 0.67f, 1f);
            numberCircle.style.alignItems = Align.Center;
            numberCircle.style.justifyContent = Justify.Center;

            var levelNumberLabel = new Label(levelNumber.ToString());
            levelNumberLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            levelNumberLabel.style.fontSize = 34f;
            levelNumberLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            levelNumberLabel.style.color = Color.white;
            numberCircle.Add(levelNumberLabel);

            plate.Add(numberCircle);
            button.Add(plate);

            return button;
        }

        private void RefreshLevelButtonSelection()
        {
            for (var i = 0; i < _levelButtons.Count; i++)
            {
                var levelButton = _levelButtons[i];
                var buttonLevel = levelButton.userData as LevelConfig;
                var isSelected = buttonLevel != null && buttonLevel == _selectedLevel;

                levelButton.style.backgroundColor = isSelected ? CardSelectedBackground : CardBackground;
                levelButton.style.scale = isSelected ? new Scale(new Vector3(1.03f, 1.03f, 1f)) : new Scale(Vector3.one);
            }
        }
        private void HandleChooseLevelClicked()
        {
            ChooseLevelClicked?.Invoke();
        }

        private void HandleLevelSelected(LevelConfig levelConfig)
        {
            LevelSelected?.Invoke(levelConfig);
        }

        private void EnsureRequiredElements()
        {
            if (_root == null)
                return;

            if (_centerColumn == null)
            {
                _centerColumn = new VisualElement
                {
                    name = CenterColumnName
                };
                _centerColumn.style.width = Length.Percent(96f);
                _centerColumn.style.maxWidth = 900f;
                _centerColumn.style.alignItems = Align.Center;
                _centerColumn.style.justifyContent = Justify.FlexStart;
                _root.Add(_centerColumn);
            }

            if (_chooseLevelButton == null)
            {
                _chooseLevelButton = new Button
                {
                    name = ChooseLevelButtonName,
                    text = "Выбрать уровень"
                };
                _chooseLevelButton.style.display = DisplayStyle.None;
                _centerColumn.Add(_chooseLevelButton);
            }

            if (_selectedLevelLabel == null)
            {
                _selectedLevelLabel = new Label
                {
                    name = SelectedLevelLabelName
                };
                _selectedLevelLabel.style.display = DisplayStyle.None;
                _centerColumn.Add(_selectedLevelLabel);
            }

            if (_levelsTab == null)
            {
                _levelsTab = new VisualElement
                {
                    name = LevelsTabName
                };
                _levelsTab.style.display = DisplayStyle.Flex;
                _levelsTab.style.flexDirection = FlexDirection.Column;
                _levelsTab.style.width = Length.Percent(100f);
                _levelsTab.style.maxWidth = 760f;
                _levelsTab.style.maxHeight = 560f;
                _centerColumn.Add(_levelsTab);
            }

            if (_emptyLevelsLabel == null)
            {
                _emptyLevelsLabel = new Label("Добавьте конфиги уровней в каталог уровней")
                {
                    name = EmptyLevelsLabelName
                };
                _emptyLevelsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                _emptyLevelsLabel.style.fontSize = 17f;
                _emptyLevelsLabel.style.color = new Color(0.91f, 0.94f, 1f, 1f);
                _levelsTab.Add(_emptyLevelsLabel);
            }

            if (_levelsList == null)
            {
                _levelsList = new ScrollView
                {
                    name = LevelsListName
                };
                _levelsList.style.flexGrow = 1f;
                _levelsList.style.minHeight = 240f;
                _levelsTab.Add(_levelsList);
            }

            ConfigureLevelsListLayout();
        }

        private void ConfigureLevelsListLayout()
        {
            if (_levelsList == null)
                return;

            _levelsList.style.paddingLeft = 4f;
            _levelsList.style.paddingRight = 4f;

            var content = _levelsList.contentContainer;
            content.style.flexDirection = FlexDirection.Row;
            content.style.flexWrap = Wrap.Wrap;
            content.style.justifyContent = Justify.Center;
            content.style.alignItems = Align.FlexStart;
        }
    }
}





