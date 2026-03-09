using System.Collections.Generic;
using UnityEngine;

namespace Dessert
{
    [CreateAssetMenu(menuName = "Configs/Dessert pool", fileName = "DessertConfig ")]
    public class DessertPool : ScriptableObject
    {
        [SerializeField] private List<DessertController> _dessertPrefabs;
        
        public List<DessertController> DessertPrefabs => _dessertPrefabs;
    }
}