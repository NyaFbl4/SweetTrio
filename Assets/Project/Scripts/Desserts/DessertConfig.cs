using System.Collections.Generic;
using UnityEngine;

namespace Dessert
{
    [CreateAssetMenu(menuName = "Game/DessertConfig", fileName = "DessertConfig ")]
    public class DessertConfig : ScriptableObject
    {
        [SerializeField] private Sprite[] _dessertSprites;
        
        public Sprite[] DessertSprites => _dessertSprites;
    }
}