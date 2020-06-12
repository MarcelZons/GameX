using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Links/unlinks damageable modules mounted at a module mount with damage receivers.
    /// </summary>
    public class DamageableModuleLinker : MonoBehaviour
    {

        [SerializeField]
        protected ModuleMount moduleMount;

        [SerializeField]
        protected List<DamageReceiver> damageReceivers = new List<DamageReceiver>();

        [Header("Events")]

        // Damageable damaged event
        public OnDamageableDamagedEventHandler onDamageableModuleDamaged;

        // Damageable healed event
        public OnDamageableHealedEventHandler onDamageableModuleHealed;

        // Damageable destroyed event
        public OnDamageableDestroyedEventHandler onDamageableModuleDestroyed;

        // Damageable restored event
        public OnDamageableRestoredEventHandler onDamageableModuleRestored;


        protected virtual void Reset()
        {
            moduleMount = GetComponent<ModuleMount>();
        }

        protected virtual void Awake()
        {
            moduleMount.onModuleMounted.AddListener(OnModuleMounted);
            moduleMount.onModuleUnmounted.AddListener(OnModuleUnmounted);
        }

        // Called when a module is mounted on the module mount
        protected virtual void OnModuleMounted(Module module)
        {
            Damageable damageable = module.GetComponent<Damageable>();
            if (damageable != null)
            {
                // Link this
                damageable.onDamaged.AddListener(OnDamageableModuleDamaged);
                damageable.onHealed.AddListener(OnDamageableModuleHealed);
                damageable.onDestroyed.AddListener(OnDamageableModuleDestroyed);
                damageable.onRestored.AddListener(OnDamageableModuleRestored);

                for (int i = 0; i < damageReceivers.Count; ++i)
                {
                    // Link damage receivers
                    damageReceivers[i].onDamaged.AddListener(damageable.Damage);
                    damageReceivers[i].onHealed.AddListener(damageable.Heal);
                    damageReceivers[i].onCollision.AddListener(damageable.OnCollision);

                    DamageReceiver damageReceiver = damageReceivers[i];     // Can't create lambda expressions from a list element without errors
                    damageable.onDestroyed.AddListener(delegate { damageReceiver.SetActivation(false); });
                    damageable.onRestored.AddListener(delegate { damageReceiver.SetActivation(true); });
                }
            }
        }


        // Called when a module is unmounted on the module mount
        protected virtual void OnModuleUnmounted(Module module)
        {
            Damageable damageable = module.GetComponent<Damageable>();
            if (damageable != null)
            {

                // Unlink this
                damageable.onDamaged.RemoveListener(OnDamageableModuleDamaged);
                damageable.onHealed.RemoveListener(OnDamageableModuleHealed);
                damageable.onDestroyed.RemoveListener(OnDamageableModuleDestroyed);
                damageable.onRestored.RemoveListener(OnDamageableModuleRestored);

                for (int i = 0; i < damageReceivers.Count; ++i)
                {
                    damageReceivers[i].onDamaged.RemoveListener(damageable.Damage);
                    damageReceivers[i].onHealed.RemoveListener(damageable.Heal);
                    damageReceivers[i].onCollision.RemoveListener(damageable.OnCollision);

                    DamageReceiver damageReceiver = damageReceivers[i];     // Can't create lambda expressions from a list element without errors
                    damageable.onDestroyed.RemoveListener(delegate { damageReceiver.SetActivation(false); });
                    damageable.onRestored.RemoveListener(delegate { damageReceiver.SetActivation(true); });
                }
            }
        }

        protected virtual void OnDamageableModuleDamaged(float damage, Vector3 hitPoint, DamageSourceType damageSourceType, Transform damageSourceRootTransform)
        {
            onDamageableModuleDamaged.Invoke(damage, hitPoint, damageSourceType, damageSourceRootTransform);
        }


        protected virtual void OnDamageableModuleHealed(float healing, Vector3 hitPoint, Transform damageSourceRootTransform)
        {
            onDamageableModuleHealed.Invoke(healing, hitPoint, damageSourceRootTransform);
        }

        protected virtual void OnDamageableModuleDestroyed()
        {
            onDamageableModuleDestroyed.Invoke();
        }

        protected virtual void OnDamageableModuleRestored()
        {
            onDamageableModuleRestored.Invoke();
        }
    }
}
