using Unity.Entities;

namespace WG.GameX.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct Waypoint : IComponentData
    {
        public Entity NextWaypoint;
    }
}