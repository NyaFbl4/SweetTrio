using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts.Desserts
{
    [CreateAssetMenu(menuName = "Configs/Dessert pool", fileName = "DessertConfig ")]
    public class DessertPool : ScriptableObject
    {
        [SerializeField] private List<DessertController> _dessertPrefabs;
        
        public List<DessertController> DessertPrefabs => _dessertPrefabs;
    }
}