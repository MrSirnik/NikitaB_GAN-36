using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace FPS
{
    public class WeaponSwitcher : MonoBehaviour
    {
        [SerializeField] private WeaponController[] _weapons;

        private int _activeIndex;

        public void SetWeapons(WeaponController[] weapons) => _weapons = weapons;

        private void Start()
        {
            SetActive(0);
        }

        private void Update()
        {
            HandleSwitchInput();
            HandleFireInput();
        }

        private void HandleSwitchInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            for (int i = 0; i < _weapons.Length && i < 9; i++)
            {
                KeyControl key = keyboard[(Key)((int)Key.Digit1 + i)];
                if (key.wasPressedThisFrame)
                {
                    SetActive(i);
                }
            }
        }

        private void HandleFireInput()
        {
            if (Mouse.current == null || _weapons.Length == 0) return;

            if (Mouse.current.leftButton.isPressed)
            {
                _weapons[_activeIndex].TryFire();
            }
        }

        private void SetActive(int index)
        {
            if (index < 0 || index >= _weapons.Length) return;

            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i].gameObject.SetActive(i == index);
            }

            _activeIndex = index;
        }
    }
}
