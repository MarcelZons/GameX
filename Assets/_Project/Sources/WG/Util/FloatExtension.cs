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
    }
}