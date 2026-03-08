using UnityEngine;

namespace Dessert
{
    public class DessertController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _dessertSprite;
        [SerializeField] private DessertConfig _dessertConfig;

        private bool _isInteractable = true;
        private DessertData _dessertData;

        public void Init(DessertData dessertData)
        {
            _dessertData = dessertData;

            _dessertSprite.sprite = _dessertConfig.DessertSprites[(int)_dessertData.DessertType];
        }

        public void MoveToActionBar(Transform newPosition)
        {
            _isInteractable = false;

            var gameObject = this.gameObject;

            gameObject.transform.position = newPosition.position;
            gameObject.transform.parent = newPosition;
            Destroy(this.gameObject.GetComponent<Rigidbody2D>());
        }

        
    }
}