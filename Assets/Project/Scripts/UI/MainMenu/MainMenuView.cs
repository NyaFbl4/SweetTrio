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

        private static readonly Color DefaultButtonBackground = new(1f, 1f, 1f, 0.96f);
        private static readonly Color SelectedButtonBackground = new(0.16f, 0.42f, 0.78f, 0.96f);
        private static readonly Color DefaultButtonText = new(0.07f, 0.1f, 0.14f, 1f);

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

            if (_chooseLevelButton == null)
            {
                Debug.LogError("MainMenuView: failed to create fallback choose-level button.");
            }
            else
            {
                _chooseLevelButton.clicked += HandleChooseLevelClicked;
            }

            SetLevelsTabVisible(false);
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

        public void SetLevels(IReadOnlyList<LevelConfig> levels, LevelConfig selectedLevel)
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
                    var levelCopy = levelConfig;
                    var levelButton = CreateLevelButton(levelCopy);
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
                var title = _selectedLevel != null ? _selectedLevel.LevelTitle : "Уровень не выбран";
                _selectedLevelLabel.text = $"Текущий уровень: {title}";
            }

            RefreshLevelButtonSelection();
        }

        private Button CreateLevelButton(LevelConfig levelConfig)
        {
            var button = new Button(() => HandleLevelSelected(levelConfig))
            {
                text = $"{levelConfig.LevelTitle}\n{levelConfig.LevelDescription}",
                userData = levelConfig
            };

            button.style.height = 74f;
            button.style.marginTop = 6f;
            button.style.marginBottom = 6f;
            button.style.paddingLeft = 14f;
            button.style.paddingRight = 14f;
            button.style.whiteSpace = WhiteSpace.Normal;
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.style.fontSize = 16f;
            button.style.borderTopLeftRadius = 10f;
            button.style.borderTopRightRadius = 10f;
            button.style.borderBottomLeftRadius = 10f;
            button.style.borderBottomRightRadius = 10f;
            button.style.backgroundColor = DefaultButtonBackground;
            button.style.color = DefaultButtonText;

            return button;
        }

        private void RefreshLevelButtonSelection()
        {
            for (var i = 0; i < _levelButtons.Count; i++)
            {
                var levelButton = _levelButtons[i];
                var buttonLevel = levelButton.userData as LevelConfig;
                var isSelected = buttonLevel != null && buttonLevel == _selectedLevel;

                levelButton.style.backgroundColor = isSelected ? SelectedButtonBackground : DefaultButtonBackground;
                levelButton.style.color = isSelected ? Color.white : DefaultButtonText;
                levelButton.style.unityFontStyleAndWeight = isSelected ? FontStyle.Bold : FontStyle.Normal;
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
                _centerColumn.style.width = Length.Percent(92f);
                _centerColumn.style.maxWidth = 860f;
                _centerColumn.style.alignItems = Align.Center;
                _centerColumn.style.justifyContent = Justify.Center;
                _root.Add(_centerColumn);
            }

            if (_chooseLevelButton == null)
            {
                _chooseLevelButton = new Button
                {
                    name = ChooseLevelButtonName,
                    text = "Выбрать уровень"
                };
                _chooseLevelButton.style.width = 320f;
                _chooseLevelButton.style.height = 72f;
                _chooseLevelButton.style.marginTop = 14f;
                _chooseLevelButton.style.alignSelf = Align.Center;
                _chooseLevelButton.style.backgroundColor = new Color(0.13f, 0.37f, 0.77f, 0.98f);
                _chooseLevelButton.style.color = Color.white;
                _centerColumn.Add(_chooseLevelButton);
            }

            if (_selectedLevelLabel == null)
            {
                _selectedLevelLabel = new Label
                {
                    name = SelectedLevelLabelName
                };
                _selectedLevelLabel.style.marginTop = 10f;
                _selectedLevelLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                _selectedLevelLabel.style.fontSize = 20f;
                _selectedLevelLabel.style.color = new Color(0.2f, 0.24f, 0.3f, 1f);
                _centerColumn.Add(_selectedLevelLabel);
            }

            if (_levelsTab == null)
            {
                _levelsTab = new VisualElement
                {
                    name = LevelsTabName
                };
                _levelsTab.style.display = DisplayStyle.None;
                _levelsTab.style.flexDirection = FlexDirection.Column;
                _levelsTab.style.marginTop = 14f;
                _levelsTab.style.paddingTop = 12f;
                _levelsTab.style.paddingBottom = 12f;
                _levelsTab.style.paddingLeft = 12f;
                _levelsTab.style.paddingRight = 12f;
                _levelsTab.style.backgroundColor = new Color(0.97f, 0.98f, 0.99f, 1f);
                _levelsTab.style.borderTopLeftRadius = 12f;
                _levelsTab.style.borderTopRightRadius = 12f;
                _levelsTab.style.borderBottomLeftRadius = 12f;
                _levelsTab.style.borderBottomRightRadius = 12f;
                _levelsTab.style.maxWidth = 760f;
                _levelsTab.style.alignSelf = Align.Center;
                _levelsTab.style.width = Length.Percent(100f);
                _levelsTab.style.maxHeight = 420f;
                _centerColumn.Add(_levelsTab);
            }

            if (_emptyLevelsLabel == null)
            {
                _emptyLevelsLabel = new Label("Добавьте конфиги уровней в каталог уровней")
                {
                    name = EmptyLevelsLabelName
                };
                _emptyLevelsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                _emptyLevelsLabel.style.fontSize = 14f;
                _emptyLevelsLabel.style.color = new Color(0.35f, 0.38f, 0.45f, 1f);
                _emptyLevelsLabel.style.marginBottom = 8f;
                _levelsTab.Add(_emptyLevelsLabel);
            }

            if (_levelsList == null)
            {
                _levelsList = new ScrollView
                {
                    name = LevelsListName
                };
                _levelsList.style.maxHeight = 340f;
                _levelsList.style.minHeight = 180f;
                _levelsTab.Add(_levelsList);
            }
        }
    }
}
