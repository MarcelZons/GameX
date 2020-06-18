using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace WG.GameX.ECS.Helpers
{
    // from ecs course
    public class EntityTracker : MonoBehaviour
    {
        private EntityManager EntityManager;
        public Entity EntityToTrack ;


        void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        void LateUpdate()
        {
            if (EntityToTrack != Entity.Null)
            {
                try
                {
                    transform.position = EntityManager.GetComponentData<Translation>(EntityToTrack).Value;
                    transform.rotation = EntityManager.GetComponentData<Rotation>(EntityToTrack).Value;
                }
                catch
                {
                    EntityToTrack = Entity.Null;
                }
            }
        }
    }
}