//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Used for holding a list for waypoints, and drawing gizmos for all of them.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/AI/RCC AI Waypoints Container")]
public class RCC_AIWaypointsDrawingContainer : MonoBehaviour {

	[FormerlySerializedAs("waypoints")] public List<RCC_WaypointR> waypointsList = new List<RCC_WaypointR>();

	// Used for drawing gizmos on Editor.
	private void OnDrawGizmos() {

		if (waypointsList == null)
			return;
		
		for(int i = 0; i < waypointsList.Count; i ++){

			if (waypointsList [i] == null)
				return;
			
			Gizmos.color = new Color(0.0f, 1.0f, 1.0f, 0.3f);
			Gizmos.DrawSphere (waypointsList[i].transform.position, 2);
			Gizmos.DrawWireSphere (waypointsList[i].transform.position, 20f);
			
			if(i < waypointsList.Count - 1){
				
				if(waypointsList[i] && waypointsList[i+1]){
					
					if (waypointsList.Count > 0) {
						
						Gizmos.color = Color.green;

						if(i < waypointsList.Count - 1)
							Gizmos.DrawLine(waypointsList[i].transform.position, waypointsList[i+1].transform.position); 
						if(i < waypointsList.Count - 2)
							Gizmos.DrawLine(waypointsList[waypointsList.Count - 1].transform.position, waypointsList[0].transform.position); 
						
					}

				}

			}

		}
		
	}
	
}
