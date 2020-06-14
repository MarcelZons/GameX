using UnityEngine;

namespace WG.GameX.Enemy
{
    [RequireComponent(typeof(BoxCollider))]
    public class EnemyWeakPoint : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        public LayerMask LayerMask => _layer;
    }
}