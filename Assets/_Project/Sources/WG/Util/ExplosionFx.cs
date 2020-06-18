using UnityEngine;
using VSX.Pooling;

namespace WG.GameX.Util
{
    public class ExplosionFx: MonoBehaviour
    {
        [SerializeField] private GameObject _mediamExplosion,_largeExoplosion;

        public void PlayMediumExplosion(Vector3 position)
        {
            PoolManager.Instance.Get(_mediamExplosion,position);
        }
        public void PlaySmallExplosion(Vector3 position)
        {
            PoolManager.Instance.Get(_mediamExplosion,position);
        }
    }
}