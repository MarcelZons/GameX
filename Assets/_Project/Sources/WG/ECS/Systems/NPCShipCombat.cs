using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using WG.GameX.ECS.Components;
using WG.GameX.ECS.Helpers;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;
using VSX.Pooling;
using WG.GameX.Common;


namespace WG.GameX.ECS.Systems
{
    [UpdateAfter(typeof(NPCShipMovement))]
    public class NPCShipCombat : JobComponentSystem
    {
        
        private static void Combat(float deltaTime, float3 targetPosition, ref Translation position, ref NPCShip ship, ref Weapon weapon)
        {
            var direction = math.normalize(targetPosition - position.Value);
            var rotation = quaternion.LookRotation(direction, math.up());

            if (math.distance(targetPosition, position.Value) < ship.AttackRadius)
            {
                weapon.Cycle += deltaTime * weapon.RateOfFire;

                if(weapon.Cycle>=1)
                {
                    weapon.Cycle = 0;
                    var bullet = PoolManager.Instance.Get(ECSData.NPCBullet, position.Value, rotation);
                    var bulletVelocity = weapon.BulletVelocity;
                    bullet.GetComponent<Bullet>().Fire(direction * bulletVelocity);
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null) return inputDependencies;

            float deltaTime = Time.DeltaTime;
            float3 playerPosition = player.GetComponent<Transform>().position;

            Entities
                .WithName("NPCShipCombat")
                .WithoutBurst() // to be able to dirty spawn game object
                .ForEach((ref Translation position, ref NPCShip ship, ref Weapon weapon) =>
                    Combat(deltaTime, playerPosition, ref position, ref ship, ref weapon))
                .Run(); // unfortunately not parallel as gameobject needs to be created

            return inputDependencies;
        }
    }
}