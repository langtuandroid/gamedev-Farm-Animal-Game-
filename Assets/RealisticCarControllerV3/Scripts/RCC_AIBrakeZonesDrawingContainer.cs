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
/// Used for holding a list for brake zones, and drawing gizmos for all of them.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/AI/RCC AI Brake Zones Container")]
public class RCC_AIBrakeZonesDrawingContainer : MonoBehaviour {
	
	[FormerlySerializedAs("brakeZones")] public List<Transform> brakeZonesList = new List<Transform>();		// Brake Zones list.

    private void Awake() {

		// Changing all layers to ignore raycasts to prevent lens flare occlusion.
        foreach (var item in brakeZonesList)
            item.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

    }

    // Used for drawing gizmos on Editor.
    private void OnDrawGizmos() {
		
		for(int i = 0; i < brakeZonesList.Count; i ++){

			Gizmos.matrix = brakeZonesList[i].transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 0f, 0f, .25f);
			Vector3 colliderBounds = brakeZonesList[i].GetComponent<BoxCollider>().size;

			Gizmos.DrawCube(Vector3.zero, colliderBounds);

		}
		
	}
	
}
