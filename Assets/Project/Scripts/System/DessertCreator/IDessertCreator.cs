using System.Collections.Generic;
using Assets.Project.Scripts.Desserts;
using UnityEngine;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public interface IDessertCreator
    {
        void SpawnDessert(Transform parent, DessertController gamePbject);
    }
}
