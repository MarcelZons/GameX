using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace WG.GameX.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct Weapon : IComponentData
    {
        public float BulletVelocity;
        public float RateOfFire;
        public float Cycle;
    }
}
