using System;
using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;

namespace Project.System
{
    public interface IActionBar
    {
        event Action<DessertController> DessertAdded;
        int CurrentCount { get; }

        bool TryAddDessert(DessertController dessert);
        IReadOnlyList<DessertController> GetDesserts();
        void RemoveDesserts(IReadOnlyList<DessertController> desserts);
        void ClearField();
        bool TryReturnDessertsToPool();
    }
}
