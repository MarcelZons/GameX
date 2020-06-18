using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace WG.GameX.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct NPCShip : IComponentData
    {
        public enum NavigationType
        {
            None, // don't do anything
            Approach,
            Avoid
        }
        public float Acceleration;
        public float MaximumSpeed;
        public float RotationSpeed;
        public float CountersteerScale;

        public float AttackRadius;

        public NavigationType navigationType;
        public float3 navigationTarget;
    }
}
