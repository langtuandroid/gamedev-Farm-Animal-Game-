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

public class RCC_FuelStationController : MonoBehaviour {

	private RCC_CarMainControllerV3 targetVehicleController;
	[FormerlySerializedAs("refillSpeed")] public float refillSpeedValue = 1f;

	private void OnTriggerStay (Collider col) {

		if (targetVehicleController == null) {

			if (col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ())
				targetVehicleController = col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ();

		}

		if(targetVehicleController)
			targetVehicleController.fuelTank += refillSpeedValue * Time.deltaTime;
		
	}

	private void OnTriggerExit (Collider col) {

		if (col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ())
			targetVehicleController = null;

	}

}
