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
    public class Navigation : JobComponentSystem
    {
        private static void Move(
            ref PathNavigation navigation,
            ref NPCShip ship,
            ref Translation position,
            ref Waypoint waypoint,
            float3 waypointPosition
        )
        {
            if (ship.navigationType==NPCShip.NavigationType.None)
            {
                ship.navigationTarget = waypointPosition;
                ship.navigationType = NPCShip.NavigationType.Approach;
            }
            else if (math.distance(waypointPosition, position.Value) < 3)
            {
                navigation.CurrentWaypoint = waypoint.NextWaypoint;
                ship.navigationType = NPCShip.NavigationType.None; // will update ship approach next frame
            }
        }


        // --- Set up chunk job boilerplate to execute the above function for all ships in concurrent job

        private EntityQuery NPCShipGroup;

        protected override void OnCreate()
        {
            NPCShipGroup = GetEntityQuery(
                typeof(PathNavigation),
                typeof(NPCShip),
                ComponentType.ReadOnly<Translation>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new MovementJob()
            {
                PathNavigationType = GetArchetypeChunkComponentType<PathNavigation>(false),
                NPCShipType = GetArchetypeChunkComponentType<NPCShip>(false),
                TranslationType = GetArchetypeChunkComponentType<Translation>(true),

                AllTranslationsType = GetComponentDataFromEntity<Translation>(true), //required to get waypoint location
                AllWaypointsType = GetComponentDataFromEntity<Waypoint>(true),
            };

            return job.Schedule(NPCShipGroup, inputDependencies);
        }

        [BurstCompile]
        struct MovementJob : IJobChunk
        {
            public ArchetypeChunkComponentType<PathNavigation> PathNavigationType;
            public ArchetypeChunkComponentType<NPCShip> NPCShipType;
            [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;

            [ReadOnly] public ComponentDataFromEntity<Translation> AllTranslationsType;
            [ReadOnly] public ComponentDataFromEntity<Waypoint> AllWaypointsType;

            // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeed
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var navigations = chunk.GetNativeArray(PathNavigationType);
                var ships = chunk.GetNativeArray(NPCShipType);
                var translations = chunk.GetNativeArray(TranslationType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    var navigation = navigations[i];
                    var ship = ships[i];
                    var translation = translations[i];
                    var waypoint = AllWaypointsType[navigation.CurrentWaypoint];
                    var waypointPosition = AllTranslationsType[navigation.CurrentWaypoint].Value;

                    Move(
                        ref navigation,
                        ref ship,
                        ref translation,
                        ref waypoint,
                        waypointPosition
                    );

                    navigations[i] = navigation;
                    ships[i] = ship;
                }
            }
        }
    }
}