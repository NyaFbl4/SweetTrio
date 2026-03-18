using System;
using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using Assets.Project.Scripts.System.DessertCreator;
using Assets.Project.Scripts.System.DessertCreator.Dtos;
using MessagePipe;
using Project.Scripts.GameManager;
using Project.Scripts.UI.UseCases;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

namespace Project.System
{
    public class ActionBar : MonoBehaviour, IActionBar
    {
        private const int MatchCount = 3;

        [SerializeField] private Transform _actionBarContainer;
        [SerializeField] private int _maxCount = 7;
        [SerializeField] private int _baseSortingOrder = 100;
        [Inject] private readonly IDessertSpawner _dessertSpawner;
        [Inject] private readonly ITimerCountdownUseCase _timerCountdownUseCase;
        [Inject] private readonly LevelConfig _levelConfig;
        [Inject] private readonly IPublisher<DessertCountsDto> _dessertCountsPublisher;
        private readonly List<DessertController> _desserts = new();

        public event Action<DessertController> DessertAdded;
        public int CurrentCount => _desserts.Count;

        public bool TryAddDessert(DessertController dessert)
        {
            if (dessert == null || _actionBarContainer == null)
                return false;

            if (dessert.IsInActionBar)
                return false;

            if (_desserts.Count >= _maxCount && !WillCreateMatch(dessert))
            {
                HandleActionBarOverflow();
                return false;
            }

            var slot = _actionBarContainer;

            dessert.MoveToActionBar(slot);
            ApplyRenderOrder(dessert, _baseSortingOrder + _desserts.Count);
            _desserts.Add(dessert);
            DessertAdded?.Invoke(dessert);
            PublishCountsChanged();
            return true;
        }

        public IReadOnlyList<DessertController> GetDesserts()
        {
            return _desserts;
        }

        public void RemoveDesserts(IReadOnlyList<DessertController> desserts)
        {
            if (desserts == null)
                return;

            var hasChanges = false;
            for (var i = 0; i < desserts.Count; i++)
            {
                var dessert = desserts[i];
                if (dessert == null)
                    continue;

                if (_desserts.Remove(dessert))
                {
                    Destroy(dessert.gameObject);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                PublishCountsChanged();
            }
        }

        public void ClearField()
        {
            var hasChanges = _desserts.Count > 0;
            for (var i = 0; i < _desserts.Count; i++)
            {
                if (_desserts[i] != null)
                {
                    Destroy(_desserts[i].gameObject);
                }
            }

            _desserts.Clear();
            if (hasChanges)
            {
                PublishCountsChanged();
            }
        }

        private void HandleActionBarOverflow()
        {
            if (_desserts.Count > 0)
            {
                var dessertsToReturn = new List<DessertController>(_desserts);
                _desserts.Clear();
                _dessertSpawner.ReturnDessertsToPool(dessertsToReturn);
                PublishCountsChanged();
            }

            if (_timerCountdownUseCase != null && _levelConfig != null)
            {
                _timerCountdownUseCase.SubtractSeconds(_levelConfig.ActionBarOverflowPenaltySeconds);
            }
        }

        private bool WillCreateMatch(DessertController dessert)
        {
            if (dessert == null)
                return false;

            var sameTypeCount = 0;
            for (var i = 0; i < _desserts.Count; i++)
            {
                var currentDessert = _desserts[i];
                if (currentDessert == null)
                    continue;

                if (currentDessert.DessertType == dessert.DessertType)
                {
                    sameTypeCount++;
                    if (sameTypeCount >= MatchCount - 1)
                        return true;
                }
            }

            return false;
        }

        private void PublishCountsChanged()
        {
            _dessertCountsPublisher.Publish(new DessertCountsDto(
                _dessertSpawner.TotalDessertsCount,
                _dessertSpawner.RemainingDessertsCount,
                _dessertSpawner.ActiveDessertsCount,
                _dessertSpawner.FieldDessertsCount));
        }

        private void ApplyRenderOrder(DessertController dessert, int order)
        {
            var sortingGroup = dessert.GetComponentInChildren<SortingGroup>(true);
            if (sortingGroup != null)
            {
                sortingGroup.sortingOrder = order;
                return;
            }

            var renderers = dessert.GetComponentsInChildren<SpriteRenderer>(true);
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].sortingOrder = order;
            }
        }
    }
}
