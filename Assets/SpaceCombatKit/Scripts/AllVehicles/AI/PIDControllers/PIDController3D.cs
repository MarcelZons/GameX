using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.UniversalVehicleCombat
{
    [System.Serializable]
    public class PIDController3D
    {
        public PIDController controllerXAxis = new PIDController();
        public PIDController controllerYAxis = new PIDController();
        public PIDController controllerZAxis = new PIDController();

        public enum Axis
        {
            X,
            Y,
            Z
        }

        public void SetError(Axis axis, float error, float errorChangeRate)
        {
            switch (axis)
            {
                case Axis.X:
                    controllerXAxis.SetError(error, errorChangeRate);
                    break;
                case Axis.Y:
                    controllerYAxis.SetError(error, errorChangeRate);
                    break;
                case Axis.Z:
                    controllerZAxis.SetError(error, errorChangeRate);
                    break;
            }
        }

        public void SetIntegralInfluence(PIDController3D.Axis axis, float influence)
        {
            switch (axis)
            {
                case Axis.X:
                    controllerXAxis.SetIntegralInfluence(influence);
                    break;
                case Axis.Y:
                    controllerYAxis.SetIntegralInfluence(influence);
                    break;
                case Axis.Z:
                    controllerZAxis.SetIntegralInfluence(influence);
                    break;
            }
        }

        public void SetIntegralInfluence(float influence)
        {
            controllerXAxis.SetIntegralInfluence(influence);
            controllerYAxis.SetIntegralInfluence(influence);
            controllerZAxis.SetIntegralInfluence(influence);
        }

        public float GetControlValue(Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return controllerXAxis.GetControlValue();
                case Axis.Y:
                    return controllerYAxis.GetControlValue();
                default:    // Z
                    return controllerZAxis.GetControlValue();
            }
        }

        public Vector3 GetControlValues()
        {
            return new Vector3(controllerXAxis.GetControlValue(), controllerYAxis.GetControlValue(), controllerZAxis.GetControlValue());
        }
    }
}