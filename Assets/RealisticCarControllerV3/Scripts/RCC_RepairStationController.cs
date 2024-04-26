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

public class RCC_RepairStationController : MonoBehaviour {

	private RCC_CarMainControllerV3 targetVehicleController;

	private void OnTriggerStay (Collider col) {

		if (targetVehicleController == null) {

			if (col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ())
				targetVehicleController = col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ();

		}

		if (targetVehicleController)
			targetVehicleController.repairNow = true;

	}

	private void OnTriggerExit (Collider col) {

		if (col.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ())
			targetVehicleController = null;

	}

}
