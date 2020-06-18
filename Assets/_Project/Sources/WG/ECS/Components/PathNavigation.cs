using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace WG.GameX.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct PathNavigation : IComponentData
    {
        public Entity CurrentWaypoint;
        public bool IsActive;
    }
}
