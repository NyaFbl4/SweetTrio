using UnityEngine;

namespace Assets.Project.Scripts.Desserts
{
    public class DessertController : MonoBehaviour
    {
        // [SerializeField] private SpriteRenderer _dessertSprite;
        [SerializeField] private EDessertType _dessertType;
        // [SerializeField] private DessertConfig _dessertConfig;

        private bool _isInteractable = true;
        // private DessertData _dessertData;

        public EDessertType DessertType => _dessertType;
        // public event Action<DessertController> Clicked;

        // public void Init(DessertData dessertData)
        // {
        //     _dessertData = dessertData;

        // }

        public void MoveToActionBar(Transform newPosition)
        {
            _isInteractable = false;

            var gameObject = this.gameObject;

            gameObject.transform.position = newPosition.position;
            gameObject.transform.parent = newPosition;
            Destroy(this.gameObject.GetComponent<Rigidbody2D>());
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
        }

        private void OnMouseDown()
        {
            Debug.Log("Figure clicked: " + _dessertType);
            if (!_isInteractable)
                return;

            // Clicked?.Invoke(this);
        }
    }
}
