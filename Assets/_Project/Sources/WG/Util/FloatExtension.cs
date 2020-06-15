using UnityEngine;

namespace WG.GameX.Util
{
    public static class FloatExtension
    {
        public static float GetSmoothDamping(this float current, float target, float dampSpeed)
        {
            current = Mathf.Lerp(current, target, Time.deltaTime * dampSpeed);
            return current;
        }

        public static Vector2 GetSmoothDamping(this Vector2 current, Vector2 target, float dampSpeed)
        {
            current = Vector2.Lerp(current, target, Time.deltaTime * dampSpeed);
            return current;
        }

        public static float Clamp(this float current, float min, float max)
        {
            return Mathf.Clamp(current, min, max);
        }
        
        public static float Remap (this float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}