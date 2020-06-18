using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using WG.GameX.ECS.Components;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;

namespace WG.GameX.ECS.Systems
{
    [UpdateAfter(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class NPCShipMovement : JobComponentSystem
    {
        private static void Move(
            float deltaTime,
            ref PhysicsVelocity velocity, 
            ref Rotation rotation,
            ref Translation position,
            ref PathNavigation navigation,
            ref NPCShip ship
        )
        {
            if (ship.navigationType != NPCShip.NavigationType.Approach) return;

            var targetLocation = ship.navigationTarget;
            var heading = math.normalize(targetLocation - position.Value);
        
            // if the ship has velocity, calculate a course that will counteract it to move straight to the target
            if (math.length(velocity.Linear) >= 0.1f)
            {
                var velocityDirection = math.normalize(velocity.Linear);
                var countersteerVector = math.reflect(-velocityDirection, heading);
                if (math.dot(velocityDirection, heading) <= 0)
                {
                    countersteerVector = -velocityDirection;
                }
                heading = math.normalize(heading + countersteerVector * ship.CountersteerScale);
            }

            var targetDirection = quaternion.LookRotation(heading, math.up());

            rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * ship.RotationSpeed);
            velocity.Linear += deltaTime * ship.Acceleration * math.forward(rotation.Value);
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float deltaTime = Time.DeltaTime;

            var jobHandle = Entities
                .WithName("NPCShipMovement")
                .ForEach((
                    ref Translation position,
                    ref PhysicsVelocity velocity,
                    ref Rotation rotation,
                    ref NPCShip ship,
                    ref PathNavigation navigation
                ) =>
                {
                    Move(deltaTime, ref velocity, ref rotation, ref position, ref navigation, ref ship);
                })
                .Schedule(inputDependencies);

            jobHandle.Complete();

            return jobHandle;
        }
    }
}