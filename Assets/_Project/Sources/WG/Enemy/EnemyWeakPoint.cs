using Boo.Lang.Runtime.DynamicDispatching;
using UnityEngine;

namespace WG.GameX.Enemy
{
    [RequireComponent(typeof(BoxCollider))]
    public class EnemyWeakPoint : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        public LayerMask LayerMask => _layer;

        public float WeakPointHealth { get; set; }

        public void ReduceHealth()
        {
            WeakPointHealth -= Time.deltaTime;
            if (WeakPointHealth <= 0)
            {
                Debug.LogError($"Weakpoint {gameObject.name} can be destroyed");
            }
        }

    }
}