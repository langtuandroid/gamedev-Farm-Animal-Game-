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

public class RCC_VehicleSpawner : MonoBehaviour {

	// Use this for initialization
	private void Start () {

		int selectedIndex = PlayerPrefs.GetInt ("SelectedRCCVehicle", 0);

		RCC_Manager.SpawnRCCVehicle (RCC_DemoVehiclesData.InstanceR.vehiclesMass [selectedIndex], transform.position, transform.rotation, true, true, true);
		
	}

}
