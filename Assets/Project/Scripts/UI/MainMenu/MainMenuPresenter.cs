using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.RulesUI;
using Project.Scripts.UI.SettingsUI;
using VContainer;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuPresenter : LayoutPresenterBase<IMainMenuView>, IMainMenuPresenter
    {
        [Inject] private readonly ILevelSelectionService _levelSelectionService;
        [Inject] private readonly ILevelProgressService _levelProgressService;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;
        [Inject] private readonly IPublisher<ShowPopupDto> _showPopUpPublisher;
        [Inject] private readonly IRulesUIController _rulesUIController;

        public override void Initialize()
        {
            base.Initialize();

            _layoutView.LevelSelected += HandleLevelSelected;
            _layoutView.SettingsClicked += HandleSettingsClicked;

            RefreshLevels();
            _layoutView.SetLevelsTabVisible(true);
            _layoutView.Show();
        }

        public override async UniTask ActivateAsync()
        {
            RefreshLevels();
            _layoutView.SetLevelsTabVisible(true);
            await base.ActivateAsync();
        }

        public override void Dispose()
        {
            _layoutView.LevelSelected -= HandleLevelSelected;
            _layoutView.SettingsClicked -= HandleSettingsClicked;
            base.Dispose();
        }

        private void HandleLevelSelected(LevelConfig levelConfig)
        {
            if (!_levelProgressService.IsLevelUnlocked(_levelSelectionService.AvailableLevels, levelConfig))
                return;
            
            _levelSelectionService.SelectLevel(levelConfig);
            if (_levelSelectionService.CurrentLevel == null)
                return;

            _layoutView.SetSelectedLevel(_levelSelectionService.CurrentLevel);

            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IMainMenuPresenter)
            });

            _rulesUIController.ShowBeforeLevelStart();
        }

        private void HandleSettingsClicked()
        {
            _showPopUpPublisher.Publish(new ShowPopupDto
            {
                TargetPopUpType = typeof(ISettingsUIPresenter)
            });
        }

        private void RefreshLevels()
        {
            var levels = _levelSelectionService.AvailableLevels;
            var selectedLevel = _levelSelectionService.CurrentLevel;
            var savedStars = BuildSavedStars(levels);
            var unlockedLevels = BuildUnlockedLevels(levels);
            
            _layoutView.SetLevels(levels, selectedLevel, savedStars, unlockedLevels);
            _layoutView.SetSelectedLevel(selectedLevel);
        }

        private IReadOnlyList<bool> BuildUnlockedLevels(IReadOnlyList<LevelConfig> levels)
        {
            var result = new List<bool>();

            if (levels == null)
                return result;

            var maxUnlockedIndex = _levelProgressService.GetMaxUnlockedLevelIndex(levels);

            for (var i = 0; i < levels.Count; i++)
            {
                result.Add(i <= maxUnlockedIndex);
            }

            return result;
        }
        
        private IReadOnlyList<int> BuildSavedStars(IReadOnlyList<LevelConfig> levels)
        {
            if (levels == null || levels.Count == 0)
                return new List<int>();

            var result = new List<int>(levels.Count);
            for (var i = 0; i < levels.Count; i++)
            {
                result.Add(_levelProgressService.GetBestStars(levels[i]));
            }

            return result;
        }
    }
}


