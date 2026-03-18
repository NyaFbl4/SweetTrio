using System;
using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using Assets.Project.Scripts.System.DessertCreator;
using MessagePipe;
using Project.Scripts.Systems.UI.Dtos;
using Project.Scripts.UI.LevelUI;
using Project.Scripts.UI.UseCases;
using Project.System;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.GameManager
{
    public class GameRulesManager : IInitializable, ITickable, IDisposable, IGameStartListener, IGameFinishListener
    {
        private const int MatchCount = 3;
        private const int DefaultPointsPerDessert = 100;
        private const float MinBonusMultiplier = 1.5f;
        private const float MaxBonusMultiplier = 3f;

        private readonly IActionBar _actionBar;
        private readonly IDessertSpawner _dessertSpawner;
        private readonly ILevelCounterUseCase _levelCounterUseCase;
        private readonly IPublisher<GameStatusCommandDto> _gameStatusPublisher;
        private readonly LevelConfig _levelConfig;
        private readonly ILevelUIPresenter _levelUIPresenter;

        private bool _isGameFinished;
        private bool _isRoundActive;
        private bool _hasBonusDessert;
        private EDessertType _bonusDessertType;
        private float _bonusMultiplier;

        public GameRulesManager(
            IActionBar actionBar,
            IDessertSpawner dessertSpawner,
            ILevelCounterUseCase levelCounterUseCase,
            IPublisher<GameStatusCommandDto> gameStatusPublisher,
            LevelConfig levelConfig,
            ILevelUIPresenter levelUIPresenter)
        {
            _actionBar = actionBar;
            _dessertSpawner = dessertSpawner;
            _levelCounterUseCase = levelCounterUseCase;
            _gameStatusPublisher = gameStatusPublisher;
            _levelConfig = levelConfig;
            _levelUIPresenter = levelUIPresenter;
        }

        public void Initialize()
        {
            _actionBar.DessertAdded += HandleDessertAdded;
            IGameListener.Register(this);
            _isGameFinished = false;
            _isRoundActive = false;
        }

        public void Dispose()
        {
            _actionBar.DessertAdded -= HandleDessertAdded;
            IGameListener.Unregister(this);
        }

        public void OnStartGame()
        {
            _isGameFinished = false;
            _isRoundActive = true;
            _levelCounterUseCase.Reset();
            InitializeBonusWidget();
        }

        public void OnFinishGame()
        {
            _isGameFinished = true;
            _isRoundActive = false;
        }

        private void HandleDessertAdded(DessertController _)
        {
            if (_isGameFinished || !_isRoundActive)
                return;

            RemoveMatches();
            EvaluateGameState();
        }

        public void Tick()
        {
            if (_isGameFinished || !_isRoundActive)
                return;

            EvaluateGameState();
        }

        private void EvaluateGameState()
        {
            if (_dessertSpawner.RemainingDessertsCount == 0 && _dessertSpawner.ActiveDessertsCount == 0)
            {
                _gameStatusPublisher.Publish(new GameStatusCommandDto
                {
                    Command = EGameStatusCommand.ShowWinAndFinish
                });
                _isGameFinished = true;
            }
        }

        private void RemoveMatches()
        {
            while (TryGetThreeOfKind(out var matchedDesserts))
            {
                _actionBar.RemoveDesserts(matchedDesserts);
                var matchPoints = GetPointsForMatchedDesserts(matchedDesserts);
                if (matchPoints > 0)
                {
                    _levelCounterUseCase.Increment(matchPoints);
                }

                RerollBonusWidget();
            }
        }

        private int GetPointsForMatchedDesserts(IReadOnlyList<DessertController> matchedDesserts)
        {
            if (matchedDesserts == null || matchedDesserts.Count == 0)
                return 0;

            var totalPoints = 0;
            EDessertType? matchedType = null;

            for (var i = 0; i < matchedDesserts.Count; i++)
            {
                var dessert = matchedDesserts[i];
                if (dessert == null)
                    continue;

                matchedType ??= dessert.DessertType;

                var points = _levelConfig != null
                    ? _levelConfig.GetPointsForDessert(dessert.DessertType, DefaultPointsPerDessert)
                    : DefaultPointsPerDessert;
                totalPoints += points;
            }

            if (_hasBonusDessert && matchedType.HasValue && matchedType.Value == _bonusDessertType)
            {
                totalPoints = Mathf.RoundToInt(totalPoints * _bonusMultiplier);
            }

            return totalPoints;
        }

        private bool TryGetThreeOfKind(out List<DessertController> matchedDesserts)
        {
            matchedDesserts = null;
            var desserts = _actionBar.GetDesserts();

            var dessertsByType = new Dictionary<EDessertType, List<DessertController>>();
            for (var i = 0; i < desserts.Count; i++)
            {
                var dessert = desserts[i];
                if (dessert == null)
                    continue;

                var type = dessert.DessertType;
                if (!dessertsByType.TryGetValue(type, out var bucket))
                {
                    bucket = new List<DessertController>();
                    dessertsByType[type] = bucket;
                }

                bucket.Add(dessert);
                if (bucket.Count >= MatchCount)
                {
                    matchedDesserts = bucket.GetRange(0, MatchCount);
                    return true;
                }
            }

            return false;
        }

        private void InitializeBonusWidget()
        {
            RerollBonusWidget();
        }

        private void RerollBonusWidget()
        {
            var dessertPrefab = GetRandomDessertPrefab();
            _hasBonusDessert = dessertPrefab != null;
            _bonusDessertType = dessertPrefab != null ? dessertPrefab.DessertType : default;
            _bonusMultiplier = Mathf.Round(UnityEngine.Random.Range(MinBonusMultiplier, MaxBonusMultiplier + 0.001f) * 100f) / 100f;

            var sprite = GetDessertSprite(dessertPrefab);
            _levelUIPresenter.SetBonusDessertSprite(sprite);
            _levelUIPresenter.SetBonusMultiplierText($"x{_bonusMultiplier:0.00}");
        }

        private DessertController GetRandomDessertPrefab()
        {
            var dessertPrefabs = _levelConfig?.DessertPool?.DessertPrefabs;
            if (dessertPrefabs == null || dessertPrefabs.Count == 0)
                return null;

            var validPrefabs = new List<DessertController>(dessertPrefabs.Count);
            for (var i = 0; i < dessertPrefabs.Count; i++)
            {
                if (dessertPrefabs[i] != null)
                {
                    validPrefabs.Add(dessertPrefabs[i]);
                }
            }

            if (validPrefabs.Count == 0)
                return null;

            var index = UnityEngine.Random.Range(0, validPrefabs.Count);
            return validPrefabs[index];
        }

        private static Sprite GetDessertSprite(DessertController dessertPrefab)
        {
            if (dessertPrefab == null)
                return null;

            var renderer = dessertPrefab.GetComponentInChildren<SpriteRenderer>(true);
            return renderer != null ? renderer.sprite : null;
        }
    }
}
