using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// This class provides a way to create customised input that can be modified in the inspector.
    /// </summary>
    [System.Serializable]
    public class CustomInput
    {
        // Group

        public string group;

        // Action

        public string action;

        // Input type

        public InputType inputType;

        // Key

        public KeyCode key;

        // Mouse button

        public int mouseButton;

        // Axis

        public bool getAxisRaw;

        public string axis;


        public CustomInput (string group, string action, KeyCode key)
        {
            this.group = group;
            this.action = action;
            this.inputType = InputType.Key;
            this.key = key;
        }

        public CustomInput(string group, string action, int mouseButton)
        {
            this.group = group;
            this.action = action;
            this.inputType = InputType.MouseButton;
            this.mouseButton = mouseButton;
        }

        public CustomInput(string group, string action, string axis)
        {
            this.group = group;
            this.action = action;
            this.inputType = InputType.Axis;
            this.axis = axis;
        }


        public virtual bool Down ()
        {
            switch (inputType)
            {
                case InputType.Key:

                    return Input.GetKeyDown(key);
                    
                case InputType.MouseButton:

                    return Input.GetMouseButtonDown(mouseButton);                    

                case InputType.Axis:

                    return Input.GetAxis(axis) > 0.5f;

                default:

                    return false;
            }
        }


        public virtual bool Up ()
        {

            switch (inputType)
            {
                case InputType.Key:

                    return Input.GetKeyUp(key);

                case InputType.MouseButton:

                    return Input.GetMouseButtonUp(mouseButton);

                case InputType.Axis:

                    return Input.GetAxis(axis) < 0.5f;

                default:

                    return false;
            }
        }

        public virtual bool Pressed()
        {

            switch (inputType)
            {
                case InputType.Key:

                    return Input.GetKey(key);

                case InputType.MouseButton:

                    return Input.GetMouseButton(mouseButton);

                case InputType.Axis:

                    return Input.GetAxis(axis) < 0.5f;

                default:

                    return false;
            }
        }

        public virtual float FloatValue()
        {
            switch (inputType)
            {
                case InputType.Key:

                    return Input.GetKey(key) ? 1 : 0;

                case InputType.MouseButton:

                    return Input.GetMouseButton(mouseButton) ? 1 : 0;

                case InputType.Axis:

                    if (getAxisRaw)
                    {
                        return Input.GetAxisRaw(axis);
                    }
                    else
                    {
                        return Input.GetAxis(axis);
                    }

                default:

                    return 0;
            }
        }

        public string GetInputAsString()
        {
            switch (inputType)
            {
                case InputType.Key:
                    return key.ToString();
                case InputType.MouseButton:
                    return "Mouse " + mouseButton.ToString();
                default:
                    return (axis.ToString() + " Axis");
            }
        }
    }
}