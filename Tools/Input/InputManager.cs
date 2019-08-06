using System.Collections.Generic;
using UnityEngine;
using LegendaryTools.DesignPatters;

namespace LegendaryTools.Input
{
    public class InputManager : SingletonBehaviour<InputManager>
    {
        public delegate void OnMouseInputEventHandler(MouseButton mouseCode, MouseEvent eventType);
        
        private bool IsInitialized;
        public bool CanInput = true;
        public float mouseClickThreshold = 5;

        public Dictionary<string, KeyBinding> KeyBindings = new Dictionary<string, KeyBinding>();

        private readonly Vector2[] lastPressedMousePosition = new Vector2[3];

        public event OnMouseInputEventHandler OnMouseInput;
        
        protected override void Awake()
        {
            base.Awake();

            IsInitialized = true;
        }

        private void Update()
        {
            if (!IsInitialized) return;

            if (!CanInput) return;

            #region Click Detection

            for (int i = 0; i < 3; i++)
            {
                if (UnityEngine.Input.GetMouseButtonDown(i))
                {
                    lastPressedMousePosition[i] = UnityEngine.Input.mousePosition;
                    OnMouseInput?.Invoke((MouseButton)i, MouseEvent.Press);
                }

                if (UnityEngine.Input.GetMouseButtonUp(i))
                {
                    OnMouseInput?.Invoke((MouseButton)i, MouseEvent.Release);

                    if (Vector2.Distance(lastPressedMousePosition[i], UnityEngine.Input.mousePosition) < mouseClickThreshold) //determine whether a click is valid
                        OnMouseInput?.Invoke((MouseButton)i, MouseEvent.Click);
                }

                if (UnityEngine.Input.GetMouseButton(i))
                    OnMouseInput?.Invoke((MouseButton)i, MouseEvent.Hold);
            }

            #endregion

            #region Inputs axis calcs

            foreach (KeyValuePair<string, KeyBinding> keyBinding in KeyBindings)
            {
                if (UnityEngine.Input.GetKey(keyBinding.Value.PositiveKey) || UnityEngine.Input.GetKey(keyBinding.Value.AltPositiveKey))
                    keyBinding.Value.RaiseValue();
                else if (UnityEngine.Input.GetKey(keyBinding.Value.NegativeKey) || UnityEngine.Input.GetKey(keyBinding.Value.AltNegativeKey))
                    keyBinding.Value.LowerValue();
                else
                    keyBinding.Value.MoveToNeutral();
            }

            #endregion
        }

        private void LateUpdate()
        {
            foreach (KeyValuePair<string, KeyBinding> keyBinding in KeyBindings)
                keyBinding.Value.ResetFrameKeys();
        }

        public float GetAxis(string keyBindingName)
        {
            if (KeyBindings.ContainsKey(keyBindingName))
                return KeyBindings[keyBindingName].Value;
            else
            {
                Debug.LogError("KeyBinding name " + keyBindingName + " not found !");
                return 0;
            }
        }

        public KeyBinding GetKeyBinding(string keyBindingName)
        {
            if (KeyBindings.ContainsKey(keyBindingName))
                return KeyBindings[keyBindingName];
            else
            {
                Debug.LogError("KeyBinding name " + keyBindingName + " not found !");
                return null;
            }
        }

        public bool GetButton(string keyBindingName)
        {
            if (KeyBindings.ContainsKey(keyBindingName))
                return KeyBindings[keyBindingName].Value > 0 ? true : false;
            else
            {
                Debug.LogError("KeyBinding name " + keyBindingName + " not found !");
                return false;
            }
        }

        public bool GetButtonUp(string keyBindingName)
        {
            if (KeyBindings.ContainsKey(keyBindingName))
                return KeyBindings[keyBindingName].FrameKeyUp;
            else
            {
                Debug.LogError("KeyBinding name " + keyBindingName + " not found !");
                return false;
            }
        }

        public bool GetButtonDown(string keyBindingName)
        {
            if (KeyBindings.ContainsKey(keyBindingName))
                return KeyBindings[keyBindingName].FrameKeyDown;
            else
            {
                Debug.LogError("KeyBinding name " + keyBindingName + " not found !");
                return false;
            }
        }
    }
}