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

///<summary>
/// An example script to show how the RCC API works.
///</summary>
public class RCC_APIVehicleExample : MonoBehaviour {

	[FormerlySerializedAs("spawnVehiclePrefab")] public RCC_CarMainControllerV3 spawnVehicleControllerPrefab;			// Vehicle prefab we gonna spawn.
	private RCC_CarMainControllerV3 currentVehicleControllerPrefab;		// Spawned vehicle.
	[FormerlySerializedAs("spawnTransform")] public Transform spawnTransformValue;								// Spawn transform.

	[FormerlySerializedAs("playerVehicle")] public bool playerVehicleFlag;			// Spawn as a player vehicle?
	[FormerlySerializedAs("controllable")] public bool controllableFlag;			// Spawn as controllable vehicle?
	[FormerlySerializedAs("engineRunning")] public bool engineRunningFlag;		// Spawn with running engine?

	public void SpawnVehicle(){

		// Spawning the vehicle with given settings.
		currentVehicleControllerPrefab = RCC_Manager.SpawnRCCVehicle (spawnVehicleControllerPrefab, spawnTransformValue.position, spawnTransformValue.rotation, playerVehicleFlag, controllableFlag, engineRunningFlag);

	}

	public void SetPlayerVehicle(){

		// Registers the vehicle as player vehicle.
		RCC_Manager.RegisterPlayerVehicleController (currentVehicleControllerPrefab);

	}

	public void SetCarControl(bool control){

		// Enables / disables controllable state of the vehicle.
		RCC_Manager.SetCarControl (currentVehicleControllerPrefab, control);

	}

	public void SetEngineState(bool engine){

		// Starts / kills engine of the vehicle.
		RCC_Manager.SetEngineState (currentVehicleControllerPrefab, engine);

	}

	public void DeRegisterPlayerVehicle(){

		// Deregisters the vehicle from as player vehicle.
		RCC_Manager.DeRegisterPlayerVehicleController ();

	}

}
