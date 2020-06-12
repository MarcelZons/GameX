using UnityEditor;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    [InitializeOnLoad]
    public class HighlightPlayerShip : MonoBehaviour
    {
        static HighlightPlayerShip()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        // Called for each gameobject shown in the hierarchy
        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {

            Object o = EditorUtility.InstanceIDToObject(instanceID);
            if (o != null)
            {
                GameObject g = o as GameObject;

                if (g.GetComponent<Vehicle>() != null)
                {
                    Vehicle vehicle = g.GetComponent<Vehicle>();
                    foreach (GameAgent occupant in vehicle.Occupants)
                    {
                        if (occupant.IsPlayer)
                        {
                            Color fontColor = Color.red;

                            Vector2 offset = new Vector2(0, 2);
                            Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);

                            EditorGUI.LabelField(offsetRect, o.name, new GUIStyle() { normal = new GUIStyleState() { textColor = fontColor }, fontStyle = FontStyle.Normal });
                        }
                    }
                }
            }
        }
    }
}
