using UnityEngine;

namespace LegendaryTools.Input
{
    public enum MouseButton
    {
        None = -1,
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public enum MouseEvent { Press, Release, Click, Hold };

    [System.Serializable]
    public class KeyBinding
    {
        public string Name;

        public KeyCode PositiveKey = KeyCode.None;
        public KeyCode NegativeKey = KeyCode.None;

        public KeyCode AltPositiveKey = KeyCode.None;
        public KeyCode AltNegativeKey = KeyCode.None;

        public bool Snap;
        public bool Invert;
        public float Sensivity = 1;
        public float Gravity = 1;
        
        private float m_Value = 0;

        public float Value
        {
            get
            {
                if (Invert)
                    return m_Value * -1;
                else
                    return m_Value;
            }
        }

        [System.NonSerialized]
        public bool FrameKeyUp = false;

        [System.NonSerialized]
        public bool FrameKeyDown = false;

        public KeyBinding()
        { }

        public KeyBinding(string name, KeyCode negativeKey, KeyCode positiveKey, float sensivity, float gravity, bool invert = false)
        {
            this.Name = name;
            this.NegativeKey = negativeKey;
            this.PositiveKey = positiveKey;
            this.Sensivity = sensivity;
            this.Gravity = gravity;
            this.Invert = invert;
        }

        public KeyBinding(string name, KeyCode negativeKey, KeyCode positiveKey, KeyCode altNegativeKey, KeyCode altPositiveKey, float sensivity, float gravity, bool invert = false)
        {
            this.Name = name;
            this.NegativeKey = negativeKey;
            this.PositiveKey = positiveKey;
            this.AltNegativeKey = altNegativeKey;
            this.AltPositiveKey = altPositiveKey;
            this.Sensivity = sensivity;
            this.Gravity = gravity;
            this.Invert = invert;
        }

        public void RaiseValue()
        {
            if (m_Value < 0 && Snap) m_Value = 0;

            if (m_Value == 0) FrameKeyDown = true;

            m_Value = Mathf.Clamp(m_Value + (Sensivity * Time.deltaTime), -1, 1);
        }

        public void LowerValue()
        {
            if (m_Value > 0 && Snap) m_Value = 0;

            m_Value = Mathf.Clamp(m_Value - (Gravity * Time.deltaTime), -1, 1);
        }

        public void MoveToNeutral()
        {
            if (Snap)
            {
                if (m_Value != 0) FrameKeyUp = true;
                m_Value = 0;
                return;
            }

            if (Value > 0)
            {
                if ((Value - Sensivity) < 0)
                {
                    m_Value = 0;
                    FrameKeyUp = true;
                }
                else
                    LowerValue();
            }
            else if (Value < 0)
            {
                if ((Value + Sensivity) > 0)
                {
                    m_Value = 0;
                    FrameKeyUp = true;
                }
                else
                    RaiseValue();
            }
        }

        public static MouseButton ToMouseButton(KeyCode key)
        {
            return (MouseButton)Mathf.Clamp((int)key - (int)KeyCode.Mouse0, (int)MouseButton.None, (int)MouseButton.Middle);
        }

        public static KeyCode ToKeyCode(MouseButton mouseButton)
        {
            return (KeyCode)Mathf.Clamp((int)mouseButton + (int)KeyCode.Mouse0, (int)KeyCode.Mouse0, (int)KeyCode.Mouse6);
        }

        public MouseButton PositiveKeyToMouseButton()
        {
            return ToMouseButton(PositiveKey);
        }

        public MouseButton NegativeKeyToMouseButton()
        {
            return ToMouseButton(NegativeKey);
        }

        public MouseButton AltPositiveKeyToMouseButton()
        {
            return ToMouseButton(AltPositiveKey);
        }

        public MouseButton AltNegativeKeyToMouseButton()
        {
            return ToMouseButton(AltNegativeKey);
        }

        public void ResetFrameKeys()
        {
            FrameKeyUp = false;
            FrameKeyDown = false;
        }
    }
}