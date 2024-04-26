//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RCC_TeleporterR : MonoBehaviour{

	[FormerlySerializedAs("spawnPoint")] public Transform spawnPointTransform;
	
	void OnTriggerEnter(Collider col){

		if (col.isTrigger)
			return;

		RCC_CarMainControllerV3 carController = col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ();

		if (!carController)
			return;

		RCC_Manager.TransportPlayer (carController, spawnPointTransform.position, spawnPointTransform.rotation);
		
	}

}
