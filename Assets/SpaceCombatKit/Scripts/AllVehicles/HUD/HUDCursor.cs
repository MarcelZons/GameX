using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    public class HUDCursor : MonoBehaviour
    {

        [SerializeField]
        protected HUDComponent parentHUDComponent;

        public float distanceFromCamera = 0.5f;

        [SerializeField]
        protected RectTransform cursorRectTransform;

        [SerializeField]
        protected RectTransform lineRectTransform;
        
        private void LateUpdate()
        {

            if (!parentHUDComponent.Activated) return;

            if (Mathf.Approximately(Time.timeScale, 0)) return;

            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = distanceFromCamera;

            if (cursorRectTransform != null)
            {
                cursorRectTransform.position = parentHUDComponent.HUDCamera.ScreenToWorldPoint(mouseScreenPos);
                cursorRectTransform.position = parentHUDComponent.HUDCamera.transform.position + (cursorRectTransform.position - parentHUDComponent.HUDCamera.transform.position).normalized * distanceFromCamera;
                cursorRectTransform.LookAt(parentHUDComponent.HUDCamera.transform);
            }
            
            if (lineRectTransform != null)
            {
                Vector3 centerPos = parentHUDComponent.HUDCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, distanceFromCamera));
                centerPos = parentHUDComponent.HUDCamera.transform.position + (centerPos - parentHUDComponent.HUDCamera.transform.position).normalized * distanceFromCamera;


                lineRectTransform.position = 0.5f * centerPos + 0.5f * cursorRectTransform.position;
                lineRectTransform.LookAt(parentHUDComponent.HUDCamera.transform, 
                                            Vector3.Cross(parentHUDComponent.HUDCamera.transform.forward, (cursorRectTransform.position - lineRectTransform.position).normalized));

                lineRectTransform.sizeDelta = new Vector2((cursorRectTransform.position - lineRectTransform.position).magnitude * 2 * (1 / lineRectTransform.localScale.x), 
                                                            lineRectTransform.sizeDelta.y);
            }
        }
    }
}