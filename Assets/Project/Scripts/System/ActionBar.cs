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
using Cysharp.Threading.Tasks;

namespace Project.System
{
    public class ActionBar : MonoBehaviour, IActionBar
    {
        private const int MatchCount = 3;
        private static readonly Vector3 DessertSlotLocalOffset = new(0f, 0.2f, 0f);

        [SerializeField] private Transform _actionBarContainer;
        [SerializeField] private int _maxCount = 7;
        [SerializeField] private int _baseSortingOrder = 100;
        [SerializeField] private List<Transform> _slots = new();
        [SerializeField, Min(0.1f)] private float _autoSlotSpacing = 0.9f;
        [Inject] private readonly IDessertSpawner _dessertSpawner;
        [Inject] private readonly ITimerCountdownUseCase _timerCountdownUseCase;
        [Inject] private readonly ILevelSelectionService _levelSelectionService;
        [Inject] private readonly IPublisher<DessertCountsDto> _dessertCountsPublisher;
        private readonly List<DessertController> _desserts = new();
        
        [SerializeField, Min(0.05f)] private float _dessertFlyDuration = 0.28f;
        [SerializeField, Min(0f)] private float _dessertFlyArcHeight = 0.8f;
        
        private bool _isDessertFlyingToActionBar;

        public event Action<DessertController> DessertAdded;
        public int CurrentCount => _desserts.Count;

        private void Awake()
        {
            EnsureSlotsReady();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                EnsureSlotsReady();
            }
        }

        public bool TryAddDessert(DessertController dessert)
        {
            if (dessert == null || _actionBarContainer == null)
                return false;

            EnsureSlotsReady();

            if (dessert.IsInActionBar)
                return false;

            if (_desserts.Count >= _maxCount && !WillCreateMatch(dessert))
            {
                HandleActionBarOverflow(dessert);
                return false;
            }

            if (_isDessertFlyingToActionBar)
                return false;

            var slot = GetSlotByIndex(_desserts.Count);
            _isDessertFlyingToActionBar = true;

            AddDessertAnimatedAsync(dessert, slot).Forget();
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
                RebuildLayout();
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

        private async UniTaskVoid AddDessertAnimatedAsync(DessertController dessert, Transform slot)
        {
            dessert.BeginMoveToActionBar();
            ApplyRenderOrder(dessert, _baseSortingOrder + _desserts.Count);

            var startPosition = dessert.transform.position;
            var targetPosition = slot.TransformPoint(DessertSlotLocalOffset);

            var startScale = dessert.transform.localScale;
            var targetScale = Vector3.one * 0.85f;

            var elapsed = 0f;
            while (elapsed < _dessertFlyDuration)
            {
                elapsed += Time.deltaTime;

                var t = Mathf.Clamp01(elapsed / _dessertFlyDuration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);

                var position = Vector3.Lerp(startPosition, targetPosition, eased);
                position.y += Mathf.Sin(t * Mathf.PI) * _dessertFlyArcHeight;

                dessert.transform.position = position;
                dessert.transform.localScale = Vector3.Lerp(startScale, targetScale, eased);

                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
            }

            dessert.MoveToActionBar(slot);
            dessert.transform.localPosition = DessertSlotLocalOffset;

            _desserts.Add(dessert);
            RebuildLayout();

            _isDessertFlyingToActionBar = false;

            DessertAdded?.Invoke(dessert);
            PublishCountsChanged();
        }
        
        private void HandleActionBarOverflow(DessertController overflowDessert)
        {
            var dessertsToReturn = new List<DessertController>(_desserts.Count + 1);

            if (_desserts.Count > 0)
            {
                dessertsToReturn.AddRange(_desserts);
                _desserts.Clear();
            }

            if (overflowDessert != null && !overflowDessert.IsInActionBar)
            {
                overflowDessert.MoveToActionBar(_actionBarContainer);
            }

            if (overflowDessert != null)
            {
                dessertsToReturn.Add(overflowDessert);
            }

            if (dessertsToReturn.Count > 0)
            {
                _dessertSpawner.ReturnDessertsToPool(dessertsToReturn);
            }

            RebuildLayout();
            PublishCountsChanged();

            var levelConfig = _levelSelectionService.CurrentLevel;
            if (_timerCountdownUseCase != null && levelConfig != null)
            {
                _timerCountdownUseCase.SubtractSeconds(levelConfig.ActionBarOverflowPenaltySeconds);
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

        private void RebuildLayout()
        {
            EnsureSlotsReady();

            if (_actionBarContainer == null)
                return;

            for (var i = 0; i < _desserts.Count; i++)
            {
                var dessert = _desserts[i];
                if (dessert == null)
                    continue;

                var slot = GetSlotByIndex(i);
                dessert.transform.SetParent(slot, worldPositionStays: false);
                dessert.transform.localPosition = DessertSlotLocalOffset;
                dessert.transform.localRotation = Quaternion.identity;
                dessert.transform.localScale = Vector3.one * 0.85f;
                ApplyRenderOrder(dessert, _baseSortingOrder + i);
            }
        }

        [ContextMenu("Use Container Children As Slots")]
        private void UseContainerChildrenAsSlots()
        {
            if (_actionBarContainer == null)
                return;

            _slots.Clear();
            for (var i = 0; i < _actionBarContainer.childCount; i++)
            {
                _slots.Add(_actionBarContainer.GetChild(i));
            }
        }

        private void EnsureSlotsReady()
        {
            if (_actionBarContainer == null)
                return;

            if (_slots == null)
            {
                _slots = new List<Transform>();
            }

            RemoveNullSlots();
            if (_slots.Count == 0)
            {
                for (var i = 0; i < _actionBarContainer.childCount; i++)
                {
                    _slots.Add(_actionBarContainer.GetChild(i));
                }
            }

            if (_slots.Count < _maxCount)
            {
                for (var i = _slots.Count; i < _maxCount; i++)
                {
                    var slotObject = new GameObject($"Slot_{i + 1}");
                    var slotTransform = slotObject.transform;
                    slotTransform.SetParent(_actionBarContainer, worldPositionStays: false);
                    slotTransform.localPosition = new Vector3((i - (_maxCount - 1) * 0.5f) * _autoSlotSpacing, 0f, 0f);
                    slotTransform.localRotation = Quaternion.identity;
                    slotTransform.localScale = Vector3.one;
                    _slots.Add(slotTransform);
                }
            }
        }

        private Transform GetSlotByIndex(int index)
        {
            if (_slots != null && index >= 0 && index < _slots.Count && _slots[index] != null)
                return _slots[index];

            return _actionBarContainer;
        }

        private void RemoveNullSlots()
        {
            for (var i = _slots.Count - 1; i >= 0; i--)
            {
                if (_slots[i] == null)
                {
                    _slots.RemoveAt(i);
                }
            }
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
