using System;
using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.System
{
    public class ActionBar : MonoBehaviour, IActionBar
    {
        [SerializeField] private Transform _actionBarContainer;
        [SerializeField] private int _maxCount = 7;
        [SerializeField] private int _baseSortingOrder = 100;
        private readonly List<DessertController> _desserts = new();

        public event Action<DessertController> DessertAdded;
        public int CurrentCount => _desserts.Count;

        public bool TryAddDessert(DessertController dessert)
        {
            if (dessert == null || _actionBarContainer == null)
                return false;

            if (_desserts.Count >= _maxCount)
                return false;

            var slot = _actionBarContainer;

            dessert.MoveToActionBar(slot);
            ApplyRenderOrder(dessert, _baseSortingOrder + _desserts.Count);
            _desserts.Add(dessert);
            DessertAdded?.Invoke(dessert);
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

            for (var i = 0; i < desserts.Count; i++)
            {
                var dessert = desserts[i];
                if (dessert == null)
                    continue;

                if (_desserts.Remove(dessert))
                {
                    Destroy(dessert.gameObject);
                }
            }
        }

        public void ClearField()
        {
            for (var i = 0; i < _desserts.Count; i++)
            {
                if (_desserts[i] != null)
                {
                    Destroy(_desserts[i].gameObject);
                }
            }

            _desserts.Clear();
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
