using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

namespace WG.GameX.ECS.Helpers
{
    // not the best way to pass data to ECS but for now the only one I currently know from the course and can use quickly
    public class ECSDataManager : MonoBehaviour
    {
        [System.Serializable]
        public struct PrefabGameObjects
        {
            public GameObject NPCBullet;
        }

        public PrefabGameObjects Prefabs;

        private EntityManager EntityManager;
        private BlobAssetStore Store;


        void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Store = new BlobAssetStore();

            ConvertPrefabs();
        }

        void OnDestroy()
        {
            Store.Dispose();
        }

        void ConvertPrefabs()
        {
            ECSData.NPCBullet = Prefabs.NPCBullet;
        }
    }
}