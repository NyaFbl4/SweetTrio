using VContainer.Unity;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Assets.Project.Scripts.Desserts
{
    public class DessertClickInputHandler : ITickable
    {
        public void Tick()
        {
            if (Mouse.current == null)
                return;

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            var camera = Camera.main;
            if (camera == null)
                return;

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos = camera.ScreenToWorldPoint(screenPos);
            var hit = Physics2D.Raycast(new Vector2(worldPos.x, worldPos.y), Vector2.zero);

            if (!hit.collider)
                return;

            var dessert = hit.collider.GetComponent<DessertController>()
                        ?? hit.collider.GetComponentInParent<DessertController>();

            // dessert?.HandleClick();
        }
    }
}