using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class CopyTransform : MonoBehaviour
    {

        [SerializeField]
        protected Transform copyTarget;


        [SerializeField]
        protected bool copyPositionX = true;

        [SerializeField]
        protected bool copyPositionY = true;

        [SerializeField]
        protected bool copyPositionZ = true;


        [SerializeField]
        protected bool copyRotation = true;

        [SerializeField]
        protected bool clearRotation = true;



        // Update is called once per frame
        void LateUpdate()
        {

            if (copyTarget != null)
            {
                Vector3 nextPosition = new Vector3(copyPositionX ? copyTarget.position.x : transform.position.x,
                                                copyPositionY ? copyTarget.position.y : transform.position.y,
                                                copyPositionZ ? copyTarget.position.z : transform.position.z);

                transform.position = nextPosition;

                if (copyRotation) transform.rotation = copyTarget.rotation;

            }

            if (clearRotation) transform.rotation = Quaternion.identity;

        }
    }
}