using UnityEngine;
using System.Collections;

namespace VSX.UniversalVehicleCombat
{

	/// <summary>
    /// This class spawns a group of objects in the scene (e.g. asteroids).
    /// </summary>
	public class MassObjectSpawner : MonoBehaviour 
	{
	
		Transform player;

		[SerializeField]
		private GameObject prefab;

        [SerializeField]
        private int numX = 20;

		[SerializeField]
		private int numY = 2;

        [SerializeField]
		private int numZ = 20;

		[SerializeField]
		private float spacingX = 300;

		[SerializeField]
		private float spacingY = 300;

        [SerializeField]
		private float spacingZ = 300;


        [SerializeField]
		private float minRandomOffset = 20;

        [SerializeField]
		private float maxRandomOffset = 20;

        [SerializeField]
        private float minRandomScale = 1;

        [SerializeField]
        private float maxRandomScale = 3;

        [SerializeField]
        private float distanceCutoff = 1000;

        [SerializeField]
        private float maxDistMargin = 0;
	
	

		// Use this for initialization
		void Start () 
		{
            CreateObjects();	
		}
	
	    
        /// <summary>
        /// Create the objects in the scene
        /// </summary>
		void CreateObjects()
		{
			for (int i = 0; i < numX; ++i)
			{
				for (int j = 0; j < numY; ++j)
				{
					for (int k = 0; k < numZ; ++k)
					{
	
						Vector3 spawnPos = Vector3.zero;
						
                        // Get a random offset for the position
						Vector3 offsetVector = Random.Range(minRandomOffset, maxRandomOffset) * Random.insideUnitSphere;
						
                        // Calculate the spawn position
						spawnPos.x = transform.position.x - ((numX - 1) * spacingX) / 2 + (i * spacingX);
						spawnPos.y = transform.position.y - ((numY - 1) * spacingY) / 2 + (j * spacingY);
						spawnPos.z = transform.position.z - ((numZ - 1) * spacingZ) / 2 + (k * spacingZ);
	
						spawnPos += offsetVector;
	
                        // Spawn objects within a radius from the center, pulling in those objects that are close to the boundary
						float distFromCenter = Vector3.Distance(spawnPos, transform.position);
						if (distFromCenter > distanceCutoff)
						{
							if (distFromCenter - distanceCutoff < maxDistMargin)
							{
								spawnPos = transform.position + (spawnPos - transform.position).normalized * distanceCutoff;
							}
							else
							{
								continue;
							}
						}
						
                        // Calculate a random rotation
						Quaternion spawnRot = Quaternion.Euler (Random.Range (0, 360), Random.Range (0, 360),
						                                        Random.Range (0, 360));
						
                        // Create the object
						GameObject temp = (GameObject)Instantiate(prefab, spawnPos, spawnRot, transform);
						
                        // Random scale
						float scale = Random.Range (minRandomScale, maxRandomScale);
						temp.transform.localScale = new Vector3(scale, scale, scale);

					}
				}
			}
		}
	}
}
