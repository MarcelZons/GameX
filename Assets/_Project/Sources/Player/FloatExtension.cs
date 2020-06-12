using UnityEngine;

namespace gamex.util
{
    public static class FloatExtension
    {
        public static float GetSmoothDamping(this float current, float target, float dampSpeed)
        {
            current = Mathf.Lerp(current,target , Time.deltaTime * dampSpeed);
            return current;
        }

        public static float Clamp(this float current, float min,  float max)
        {
            return Mathf.Clamp(current, min, max);
        }
    }
}