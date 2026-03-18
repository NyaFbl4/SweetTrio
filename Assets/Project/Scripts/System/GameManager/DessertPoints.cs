using System;
using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Dessert Points Config", fileName = "DessertPointsConfig")]
    public class DessertPoints : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            [SerializeField] private EDessertType _dessertType;
            [SerializeField, Min(0)] private int _points = 100;

            public EDessertType DessertType => _dessertType;
            public int Points => _points;
        }

        [SerializeField] private List<Entry> _entries = new();

        public IReadOnlyList<Entry> Entries => _entries;

        public int GetPointsForDessert(EDessertType dessertType, int defaultPoints)
        {
            if (_entries == null)
                return Mathf.Max(0, defaultPoints);

            for (var i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (entry == null || entry.DessertType != dessertType)
                    continue;

                return Mathf.Max(0, entry.Points);
            }

            return Mathf.Max(0, defaultPoints);
        }
    }
}
