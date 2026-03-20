using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.Systems.UI;
using Project.Scripts.Systems.UI.Dtos;
using VContainer;

namespace Project.Scripts.UI.MainScreen
{
    public class MainMenuPresenter : LayoutPresenterBase<IMainMenuView>, IMainMenuPresenter
    {
        private const string ChooseLevelButtonText = "Выбрать уровень";
        private const string HideLevelsButtonText = "Скрыть уровни";
        private const string LevelsNotConfiguredText = "Нет уровней";

        [Inject] private readonly IGameManagerService _gameManagerService;
        [Inject] private readonly ILevelSelectionService _levelSelectionService;
        [Inject] private readonly IPublisher<HidePopupDto> _hidePopUpPublisher;

        private bool _isLevelsTabVisible;

        public override void Initialize()
        {
            base.Initialize();

            _layoutView.ChooseLevelClicked += HandleChooseLevelClicked;
            _layoutView.LevelSelected += HandleLevelSelected;

            RefreshLevels();
            _isLevelsTabVisible = false;
            ApplyLevelsTabState();
            _layoutView.Show();
        }

        public override async UniTask ActivateAsync()
        {
            RefreshLevels();
            _isLevelsTabVisible = false;
            ApplyLevelsTabState();
            await base.ActivateAsync();
        }

        public override void Dispose()
        {
            _layoutView.ChooseLevelClicked -= HandleChooseLevelClicked;
            _layoutView.LevelSelected -= HandleLevelSelected;
            base.Dispose();
        }

        private void HandleChooseLevelClicked()
        {
            if (!_levelSelectionService.HasAnyLevel)
                return;

            _isLevelsTabVisible = !_isLevelsTabVisible;
            ApplyLevelsTabState();
        }

        private void HandleLevelSelected(LevelConfig levelConfig)
        {
            _levelSelectionService.SelectLevel(levelConfig);
            if (_levelSelectionService.CurrentLevel == null)
                return;

            _layoutView.SetSelectedLevel(_levelSelectionService.CurrentLevel);

            _isLevelsTabVisible = false;
            ApplyLevelsTabState();

            _hidePopUpPublisher.Publish(new HidePopupDto
            {
                TargetPopUpType = typeof(IMainMenuPresenter)
            });

            _gameManagerService.StartGame();
        }

        private void RefreshLevels()
        {
            var levels = _levelSelectionService.AvailableLevels;
            var selectedLevel = _levelSelectionService.CurrentLevel;
            _layoutView.SetLevels(levels, selectedLevel);
            _layoutView.SetSelectedLevel(selectedLevel);
        }

        private void ApplyLevelsTabState()
        {
            _layoutView.SetLevelsTabVisible(_isLevelsTabVisible);

            if (_levelSelectionService.HasAnyLevel)
            {
                _layoutView.SetChooseLevelButtonText(_isLevelsTabVisible ? HideLevelsButtonText : ChooseLevelButtonText);
            }
            else
            {
                _layoutView.SetChooseLevelButtonText(LevelsNotConfiguredText);
            }
        }
    }
}
