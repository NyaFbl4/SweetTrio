using VContainer.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (!TryGetPointerDownScreenPosition(out var screenPos, out var pointerId))
                return;

            if (IsPointerOverUi(pointerId))
                return;

            var camera = Camera.main;
            if (camera == null)
                return;

            var ray = camera.ScreenPointToRay(screenPos);
            var hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (!hit.collider)
                return;

            var dessert = hit.collider.GetComponent<DessertController>()
                        ?? hit.collider.GetComponentInParent<DessertController>();

            // dessert?.HandleClick();

            if (dessert == null)
                return;

            if (_actionBar.TryAddDessert(dessert))
                _soundManager.PlayTapPick();
        }

        private static bool TryGetPointerDownScreenPosition(out Vector2 screenPos, out int pointerId)
        {
            screenPos = default;
            pointerId = -1;

            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                screenPos = mouse.position.ReadValue();
                return true;
            }

            var touchscreen = Touchscreen.current;
            if (touchscreen == null)
                return false;

            var primaryTouch = touchscreen.primaryTouch;
            if (primaryTouch.press.wasPressedThisFrame)
            {
                screenPos = primaryTouch.position.ReadValue();
                pointerId = primaryTouch.touchId.ReadValue();
                return true;
            }

            var touches = touchscreen.touches;
            for (var i = 0; i < touches.Count; i++)
            {
                var touch = touches[i];
                if (!touch.press.wasPressedThisFrame)
                    continue;

                screenPos = touch.position.ReadValue();
                pointerId = touch.touchId.ReadValue();
                return true;
            }

            return false;
        }

        private static bool IsPointerOverUi(int pointerId)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
                return false;

            if (pointerId >= 0)
                return eventSystem.IsPointerOverGameObject(pointerId);

            return eventSystem.IsPointerOverGameObject();
        }
    }
}
