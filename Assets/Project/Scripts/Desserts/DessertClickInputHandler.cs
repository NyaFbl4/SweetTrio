using VContainer.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using Project.Scripts.System.Audio;
using Project.System;


namespace Assets.Project.Scripts.Desserts
{
    public class DessertClickInputHandler : ITickable
    {
        private readonly IActionBar _actionBar;
        private readonly ISoundManager _soundManager;

        public DessertClickInputHandler(IActionBar actionBar, ISoundManager soundManager)
        {
            _actionBar = actionBar;
            _soundManager = soundManager;
        }

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

            if (dessert == null)
                return;

            _soundManager.PlayTapPick();
            _actionBar.TryAddDessert(dessert);
        }
    }
}
