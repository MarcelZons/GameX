using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    [System.Serializable]
    public class PIDController
    {
        public float proportionalCoefficient = 0.01f;
        public float integralCoefficient;
        public float derivativeCoefficient;

        protected float proportionalValue;
        protected float integralValue;
        protected float derivativeValue;

        public float integralInfluence = 1;

        public void SetError(float error, float errorChangeRate)
        {

            // Proportional
            proportionalValue = proportionalCoefficient * error;

            // Integral
            integralValue += integralInfluence * (integralCoefficient * error);
            integralValue = Mathf.Clamp(integralValue, -1, 1);

            // Derivative
            derivativeValue = derivativeCoefficient * errorChangeRate;

        }

        public void SetIntegralInfluence(float influence)
        {
            this.integralInfluence = influence;
        }

        public float GetControlValue()
        {
            return proportionalValue + (integralInfluence * integralValue) + derivativeValue;
        }
    }
}
