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
using UnityEngine.EventSystems;

///<summary>
/// Main Customization Class For RCC.
///</summary>
public class RCC_Customization : MonoBehaviour {

	/// <summary>
	/// Set Customization Mode. This will enable / disable controlling the vehicle, and enable / disable orbit camera mode.
	/// </summary>
	public static void SetCustomizationMode(RCC_CarMainControllerV3 vehicle, bool state){

		if (!vehicle) {
			
			Debug.LogError ("Player vehicle is not selected for customization! Use RCC_Customization.SetCustomizationMode(playerVehicle, true/false); for enabling / disabling customization mode for player vehicle.");
			return;

		}

		RCC_CameraController cam = RCC_SceneManager.Instance.activePlayerCamera;
		RCC_UIDashboardDisplayController UI = RCC_SceneManager.Instance.activePlayerCanvas;

		if (state) {

			vehicle.SetCanControl (false);

			if (cam)
				cam.ChangeCameraObject (RCC_CameraController.CameraMode.TPS);

			if (UI)
				UI.SetDisplayTypeValue(RCC_UIDashboardDisplayController.DisplayType.Customization);

		} else {

			SetSmokeParticle (vehicle, false);
			SetExhaustFlame (vehicle, false);
			vehicle.SetCanControl (true);

			if (cam)
				cam.ChangeCameraObject (RCC_CameraController.CameraMode.TPS);

			if (UI)
				UI.SetDisplayTypeValue(RCC_UIDashboardDisplayController.DisplayType.Full);

		}

	}

	/// <summary>
	///	 Updates RCC while vehicle is inactive.
	/// </summary>
	public static void OverrideRCC (RCC_CarMainControllerV3 vehicle) {

		if (!CheckVehicle (vehicle))
			return;

	}

	/// <summary>
	///	 Enable / Disable Smoke Particles. You can use it for previewing current wheel smokes.
	/// </summary>
	public static void SetSmokeParticle (RCC_CarMainControllerV3 vehicle, bool state) {

		if (!CheckVehicle (vehicle))
			return;

		vehicle.PreviewSmokeParticle (state);

	}

	/// <summary>
	/// Set Smoke Color.
	/// </summary>
	public static void SetSmokeColor (RCC_CarMainControllerV3 vehicle, int indexOfGroundMaterial, Color color) {

		if (!CheckVehicle (vehicle))
			return;

		RCC_WheelColliderController[] wheels = vehicle.GetComponentsInChildren<RCC_WheelColliderController> ();

		foreach(RCC_WheelColliderController wheel in wheels){

			for (int i = 0; i < wheel.allWheelParticlesList.Count; i++) {

				ParticleSystem ps = wheel.allWheelParticlesList[i];
				ParticleSystem.MainModule psmain = ps.main;
				color.a = psmain.startColor.color.a;
				psmain.startColor = color;

			}

		}

	}

	/// <summary>
	/// Set Headlights Color.
	/// </summary>
	public static void SetHeadlightsColor (RCC_CarMainControllerV3 vehicle, Color color) {

		if (!CheckVehicle (vehicle))
			return;

		vehicle.lowBeamHeadLightsOn = true;
		RCC_LightController[] lights = vehicle.GetComponentsInChildren<RCC_LightController> ();

		foreach(RCC_LightController l in lights){

			if(l.lightTypeValue == RCC_LightController.LightType.HeadLight || l.lightTypeValue == RCC_LightController.LightType.HighBeamHeadLight || l.lightTypeValue == RCC_LightController.LightType.ParkLight)
				l.GetComponent<Light>().color = color;

		}

	}

	/// <summary>
	/// Enable / Disable Exhaust Flame Particles.
	/// </summary>
	public static void SetExhaustFlame (RCC_CarMainControllerV3 vehicle, bool state) {

		if (!CheckVehicle (vehicle))
			return;

		RCC_Exhaust[] exhausts = vehicle.GetComponentsInChildren<RCC_Exhaust> ();

		foreach (RCC_Exhaust exhaust in exhausts)
			exhaust.previewFlames = state;

	}

	/// <summary>
	/// Set Front Wheel Cambers.
	/// </summary>
	public static void SetFrontCambers(RCC_CarMainControllerV3 vehicle, float camberAngle){

		if (!CheckVehicle (vehicle))
			return;

		RCC_WheelColliderController[] wc = vehicle.GetComponentsInChildren<RCC_WheelColliderController> ();

		foreach (RCC_WheelColliderController w in wc) {
			
			if (w == vehicle.FrontLeftWheelCollider || w == vehicle.FrontRightWheelCollider)
				w.camberValue = camberAngle;
			
		}

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Rear Wheel Cambers.
	/// </summary>
	public static void SetRearCambers(RCC_CarMainControllerV3 vehicle, float camberAngle){

		if (!CheckVehicle (vehicle))
			return;

		RCC_WheelColliderController[] wc = vehicle.GetComponentsInChildren<RCC_WheelColliderController> ();

		foreach (RCC_WheelColliderController w in wc) {
			
			if (w != vehicle.FrontLeftWheelCollider && w != vehicle.FrontRightWheelCollider)
				w.camberValue = camberAngle;
			
		}

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Change Wheel Models. You can find your wheel models array in Tools --> BCG --> RCC --> Configure Changable Wheels.
	/// </summary>
	public static void ChangeWheels(RCC_CarMainControllerV3 vehicle, GameObject wheel, bool applyRadius){

		if (!CheckVehicle (vehicle))
			return;

		for (int i = 0; i < vehicle.allWheelColliders.Length; i++) {

			if (vehicle.allWheelColliders [i].wheelModelTransform.GetComponent<MeshRenderer> ()) 
				vehicle.allWheelColliders [i].wheelModelTransform.GetComponent<MeshRenderer> ().enabled = false;

			foreach (Transform t in vehicle.allWheelColliders [i].wheelModelTransform.GetComponentInChildren<Transform> ())
				t.gameObject.SetActive (false);

			GameObject newWheel = (GameObject)Instantiate (wheel, vehicle.allWheelColliders[i].wheelModelTransform.position, vehicle.allWheelColliders[i].wheelModelTransform.rotation, vehicle.allWheelColliders[i].wheelModelTransform);

			if (vehicle.allWheelColliders [i].wheelModelTransform.localPosition.x > 0f)
				newWheel.transform.localScale = new Vector3 (newWheel.transform.localScale.x * -1f, newWheel.transform.localScale.y, newWheel.transform.localScale.z);

			if (applyRadius)
				vehicle.allWheelColliders [i].wheelColliderObject.radius = RCC_GetBoundsSize.MaxBoundsExtentValue (wheel.transform);

		}

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Front Suspension targetPositions. It changes targetPosition of the front WheelColliders.
	/// </summary>
	public static void SetFrontSuspensionsTargetPos(RCC_CarMainControllerV3 vehicle, float targetPosition){

		if (!CheckVehicle (vehicle))
			return;

		targetPosition = Mathf.Clamp01(targetPosition);

		JointSpring spring1 = vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring;
		spring1.targetPosition = 1f - targetPosition;

		vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring = spring1;

		JointSpring spring2 = vehicle.FrontRightWheelCollider.wheelColliderObject.suspensionSpring;
		spring2.targetPosition = 1f - targetPosition;

		vehicle.FrontRightWheelCollider.wheelColliderObject.suspensionSpring = spring2;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Rear Suspension targetPositions. It changes targetPosition of the rear WheelColliders.
	/// </summary>
	public static void SetRearSuspensionsTargetPos(RCC_CarMainControllerV3 vehicle, float targetPosition){

		if (!CheckVehicle (vehicle))
			return;

		targetPosition = Mathf.Clamp01(targetPosition);

		JointSpring spring1 = vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring;
		spring1.targetPosition = 1f - targetPosition;

		vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring = spring1;

		JointSpring spring2 = vehicle.RearRightWheelCollider.wheelColliderObject.suspensionSpring;
		spring2.targetPosition = 1f - targetPosition;

		vehicle.RearRightWheelCollider.wheelColliderObject.suspensionSpring = spring2;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set All Suspension targetPositions. It changes targetPosition of the all WheelColliders.
	/// </summary>
	public static void SetAllSuspensionsTargetPos(RCC_CarMainControllerV3 vehicle, float targetPosition){

		if (!CheckVehicle (vehicle))
			return;

		targetPosition = Mathf.Clamp01(targetPosition);

		JointSpring spring1 = vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring;
		spring1.targetPosition = 1f - targetPosition;

		vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring = spring1;

		JointSpring spring2 = vehicle.RearRightWheelCollider.wheelColliderObject.suspensionSpring;
		spring2.targetPosition = 1f - targetPosition;

		vehicle.RearRightWheelCollider.wheelColliderObject.suspensionSpring = spring2;

		JointSpring spring3 = vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring;
		spring3.targetPosition = 1f - targetPosition;

		vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring = spring3;

		JointSpring spring4 = vehicle.FrontRightWheelCollider.wheelColliderObject.suspensionSpring;
		spring4.targetPosition = 1f - targetPosition;

		vehicle.FrontRightWheelCollider.wheelColliderObject.suspensionSpring = spring4;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Front Suspension Distances.
	/// </summary>
	public static void SetFrontSuspensionsDistances(RCC_CarMainControllerV3 vehicle, float distance){

		if (!CheckVehicle (vehicle))
			return;

		if (distance <= 0)
			distance = .05f;

		vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionDistance = distance;
		vehicle.FrontRightWheelCollider.wheelColliderObject.suspensionDistance = distance;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Rear Suspension Distances.
	/// </summary>
	public static void SetRearSuspensionsDistances(RCC_CarMainControllerV3 vehicle, float distance){

		if (!CheckVehicle (vehicle))
			return;

		if (distance <= 0)
			distance = .05f;

		vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionDistance = distance;
		vehicle.RearRightWheelCollider.wheelColliderObject.suspensionDistance = distance;

		if (vehicle.ExtraRearWheelsCollider != null && vehicle.ExtraRearWheelsCollider.Length > 0) {
			
			foreach (RCC_WheelColliderController wc in vehicle.ExtraRearWheelsCollider)
				wc.wheelColliderObject.suspensionDistance = distance;
			
		}

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Drivetrain Mode.
	/// </summary>
	public static void SetDrivetrainMode(RCC_CarMainControllerV3 vehicle, RCC_CarMainControllerV3.WheelType mode){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.wheelTypeChoise = mode;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Gear Shifting Threshold. Automatic gear will shift up at earlier rpm on lower values. Automatic gear will shift up at later rpm on higher values. 
	/// </summary>
	public static void SetGearShiftingThreshold(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.gearShiftingThreshold = targetValue;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Clutch Threshold. Automatic gear will shift up at earlier rpm on lower values. Automatic gear will shift up at later rpm on higher values. 
	/// </summary>
	public static void SetClutchThreshold(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.clutchInertia = targetValue;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Enable / Disable Counter Steering while vehicle is drifting. Useful for avoid spinning.
	/// </summary>
	public static void SetCounterSteering(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.useCounterSteering = state;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Enable / Disable NOS.
	/// </summary>
	public static void SetNOS(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.useNOS = state;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Enable / Disable Turbo.
	/// </summary>
	public static void SetTurbo(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.useTurbo = state;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Enable / Disable Exhaust Flames.
	/// </summary>
	public static void SetUseExhaustFlame(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.useExhaustFlame = state;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Enable / Disable Rev Limiter.
	/// </summary>
	public static void SetRevLimiter(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.useRevLimiter = state;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Front Suspension Spring Force.
	/// </summary>
	public static void SetFrontSuspensionsSpringForce(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		JointSpring spring = vehicle.FrontLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring;
		spring.spring = targetValue;
		vehicle.FrontLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;
		vehicle.FrontRightWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Rear Suspension Spring Force.
	/// </summary>
	public static void SetRearSuspensionsSpringForce(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		JointSpring spring = vehicle.RearLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring;
		spring.spring = targetValue;
		vehicle.RearLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;
		vehicle.RearRightWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Front Suspension Spring Damper.
	/// </summary>
	public static void SetFrontSuspensionsSpringDamper(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		JointSpring spring = vehicle.FrontLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring;
		spring.damper = targetValue;
		vehicle.FrontLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;
		vehicle.FrontRightWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Rear Suspension Spring Damper.
	/// </summary>
	public static void SetRearSuspensionsSpringDamper(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		JointSpring spring = vehicle.RearLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring;
		spring.damper = targetValue;
		vehicle.RearLeftWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;
		vehicle.RearRightWheelCollider.GetComponent<WheelCollider> ().suspensionSpring = spring;

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Maximum Speed of the vehicle.
	/// </summary>
	public static void SetMaximumSpeed(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.maxspeed = Mathf.Clamp(targetValue, 10f, 400f);

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Maximum Engine Torque of the vehicle.
	/// </summary>
	public static void SetMaximumTorque(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.maxEngineTorque = Mathf.Clamp(targetValue, 50f, 50000f);

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Set Maximum Brake of the vehicle.
	/// </summary>
	public static void SetMaximumBrake(RCC_CarMainControllerV3 vehicle, float targetValue){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.brakeTorque = Mathf.Clamp(targetValue, 0f, 50000f);

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Repair vehicle.
	/// </summary>
	public static void Repair(RCC_CarMainControllerV3 vehicle){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.repairNow = true;

	}

	/// <summary>
	/// Enable / Disable ESP.
	/// </summary>
	public static void SetESP(RCC_CarMainControllerV3 vehicle, bool state){
		
		if (!CheckVehicle (vehicle))
			return;

		vehicle.ESP = state;

	}

	/// <summary>
	/// Enable / Disable ABS.
	/// </summary>
	public static void SetABS(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.ABS = state;

	}

	/// <summary>
	/// Enable / Disable TCS.
	/// </summary>
	public static void SetTCS(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.TCS = state;

	}

	/// <summary>
	/// Enable / Disable Steering Helper.
	/// </summary>
	public static void SetSH(RCC_CarMainControllerV3 vehicle, bool state){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.steeringHelper = state;

	}

	/// <summary>
	/// Set Steering Helper strength.
	/// </summary>
	public static void SetSHStrength(RCC_CarMainControllerV3 vehicle, float value){

		if (!CheckVehicle (vehicle))
			return;

		vehicle.steeringHelper = true;
		vehicle.steerHelperLinearVelStrength = value;
		vehicle.steerHelperAngularVelStrength = value;

	}

	/// <summary>
	/// Set Transmission of the vehicle.
	/// </summary>
	public static void SetTransmission(bool automatic){

		RCC_SettingsData.InstanceR.useAutomaticGearFlag = automatic;

	}

	/// <summary>
	/// Save all stats with PlayerPrefs.
	/// </summary>
	public static void SaveStats(RCC_CarMainControllerV3 vehicle){

		if (!CheckVehicle (vehicle))
			return;

		PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontCamber", vehicle.FrontLeftWheelCollider.camberValue);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_RearCamber", vehicle.RearLeftWheelCollider.camberValue);

		PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontSuspensionsDistance", vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionDistance);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_RearSuspensionsDistance", vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionDistance);

		PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontSuspensionsSpring", vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring.spring);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_RearSuspensionsSpring", vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring.spring);

		PlayerPrefs.SetFloat(vehicle.transform.name + "_FrontSuspensionsDamper", vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring.damper);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_RearSuspensionsDamper", vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring.damper);

		PlayerPrefs.SetFloat(vehicle.transform.name + "_MaximumSpeed", vehicle.maxspeed);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_MaximumBrake", vehicle.brakeTorque);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_MaximumTorque", vehicle.maxEngineTorque);

		PlayerPrefs.SetString(vehicle.transform.name + "_DrivetrainMode", vehicle.wheelTypeChoise.ToString());

		PlayerPrefs.SetFloat(vehicle.transform.name + "_GearShiftingThreshold", vehicle.gearShiftingThreshold);
		PlayerPrefs.SetFloat(vehicle.transform.name + "_ClutchingThreshold", vehicle.clutchInertia);

		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "_CounterSteering", vehicle.useCounterSteering);

		foreach(RCC_LightController _light in vehicle.GetComponentsInChildren<RCC_LightController>()){
			
			if (_light.lightTypeValue == RCC_LightController.LightType.HeadLight) {
				
				RCC_PlayerPrefsX.SetColor(vehicle.transform.name + "_HeadlightsColor", _light.GetComponentInChildren<Light>().color);
				break;

			}

		}

		ParticleSystem ps = vehicle.RearLeftWheelCollider.allWheelParticlesList[0];
		ParticleSystem.MainModule psmain = ps.main;

		RCC_PlayerPrefsX.SetColor(vehicle.transform.name + "_WheelsSmokeColor", psmain.startColor.color);

		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "_ABS", vehicle.ABS);
		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "_ESP", vehicle.ESP);
		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "_TCS", vehicle.TCS);
		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "_SH", vehicle.steeringHelper);

		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "NOS", vehicle.useNOS);
		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "Turbo", vehicle.useTurbo);
		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "ExhaustFlame", vehicle.useExhaustFlame);
		RCC_PlayerPrefsX.SetBool(vehicle.transform.name + "RevLimiter", vehicle.useRevLimiter);

	}

	/// <summary>
	/// Load all stats with PlayerPrefs.
	/// </summary>
	public static void LoadStats(RCC_CarMainControllerV3 vehicle){

		if (!CheckVehicle (vehicle))
			return;

		SetFrontCambers (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontCamber", vehicle.FrontLeftWheelCollider.camberValue));
		SetRearCambers (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearCamber", vehicle.RearLeftWheelCollider.camberValue));

		SetFrontSuspensionsDistances (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontSuspensionsDistance", vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionDistance));
		SetRearSuspensionsDistances (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearSuspensionsDistance", vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionDistance));

		SetFrontSuspensionsSpringForce (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontSuspensionsSpring", vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring.spring));
		SetRearSuspensionsSpringForce (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearSuspensionsSpring", vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring.spring));

		SetFrontSuspensionsSpringDamper (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_FrontSuspensionsDamper", vehicle.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring.damper));
		SetRearSuspensionsSpringDamper (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_RearSuspensionsDamper", vehicle.RearLeftWheelCollider.wheelColliderObject.suspensionSpring.damper));

		SetMaximumSpeed (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_MaximumSpeed", vehicle.maxspeed));
		SetMaximumBrake (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_MaximumBrake", vehicle.brakeTorque));
		SetMaximumTorque (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_MaximumTorque", vehicle.maxEngineTorque));

		string drvtrn = PlayerPrefs.GetString(vehicle.transform.name + "_DrivetrainMode", vehicle.wheelTypeChoise.ToString());

		switch (drvtrn) {

		case "FWD":
			vehicle.wheelTypeChoise = RCC_CarMainControllerV3.WheelType.FWD;
			break;

		case "RWD":
			vehicle.wheelTypeChoise = RCC_CarMainControllerV3.WheelType.RWD;
			break;

		case "AWD":
			vehicle.wheelTypeChoise = RCC_CarMainControllerV3.WheelType.AWD;
			break;

		}

		SetGearShiftingThreshold (vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_GearShiftingThreshold", vehicle.gearShiftingThreshold));
		SetClutchThreshold(vehicle, PlayerPrefs.GetFloat(vehicle.transform.name + "_ClutchingThreshold", vehicle.clutchInertia));

		SetCounterSteering (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "_CounterSteering", vehicle.useCounterSteering));

		SetABS (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "_ABS", vehicle.ABS));
		SetESP (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "_ESP", vehicle.ESP));
		SetTCS (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "_TCS", vehicle.TCS));
		SetSH (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "_SH", vehicle.steeringHelper));

		SetNOS (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "NOS", vehicle.useNOS));
		SetTurbo (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "Turbo", vehicle.useTurbo));
		SetUseExhaustFlame (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "ExhaustFlame", vehicle.useExhaustFlame));
		SetRevLimiter (vehicle, RCC_PlayerPrefsX.GetBool(vehicle.transform.name + "RevLimiter", vehicle.useRevLimiter));

		if(PlayerPrefs.HasKey(vehicle.transform.name + "_WheelsSmokeColor"))
			SetSmokeColor (vehicle, 0, RCC_PlayerPrefsX.GetColor(vehicle.transform.name + "_WheelsSmokeColor"));

		if(PlayerPrefs.HasKey(vehicle.transform.name + "_HeadlightsColor"))
			SetHeadlightsColor (vehicle, RCC_PlayerPrefsX.GetColor(vehicle.transform.name + "_HeadlightsColor"));

		OverrideRCC (vehicle);

	}

	/// <summary>
	/// Resets all stats and saves default values with PlayerPrefs.
	/// </summary>
	public static void ResetStats(RCC_CarMainControllerV3 vehicle, RCC_CarMainControllerV3 defaultCar){

		if (!CheckVehicle (vehicle))
			return;

		if (!CheckVehicle (defaultCar))
			return;

		SetFrontCambers (vehicle, defaultCar.FrontLeftWheelCollider.camberValue);
		SetRearCambers (vehicle, defaultCar.RearLeftWheelCollider.camberValue);

		SetFrontSuspensionsDistances (vehicle, defaultCar.FrontLeftWheelCollider.wheelColliderObject.suspensionDistance);
		SetRearSuspensionsDistances (vehicle, defaultCar.RearLeftWheelCollider.wheelColliderObject.suspensionDistance);

		SetFrontSuspensionsSpringForce (vehicle, defaultCar.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring.spring);
		SetRearSuspensionsSpringForce (vehicle, defaultCar.RearLeftWheelCollider.wheelColliderObject.suspensionSpring.spring);

		SetFrontSuspensionsSpringDamper (vehicle, defaultCar.FrontLeftWheelCollider.wheelColliderObject.suspensionSpring.damper);
		SetRearSuspensionsSpringDamper (vehicle, defaultCar.RearLeftWheelCollider.wheelColliderObject.suspensionSpring.damper);

		SetMaximumSpeed (vehicle, defaultCar.maxspeed);
		SetMaximumBrake (vehicle, defaultCar.brakeTorque);
		SetMaximumTorque (vehicle, defaultCar.maxEngineTorque);

		string drvtrn = defaultCar.wheelTypeChoise.ToString();

		switch (drvtrn) {

		case "FWD":
			vehicle.wheelTypeChoise = RCC_CarMainControllerV3.WheelType.FWD;
			break;

		case "RWD":
			vehicle.wheelTypeChoise = RCC_CarMainControllerV3.WheelType.RWD;
			break;

		case "AWD":
			vehicle.wheelTypeChoise = RCC_CarMainControllerV3.WheelType.AWD;
			break;

		}

		SetGearShiftingThreshold (vehicle, defaultCar.gearShiftingThreshold);
		SetClutchThreshold(vehicle, defaultCar.clutchInertia);

		SetCounterSteering (vehicle, defaultCar.useCounterSteering);

		SetABS (vehicle, defaultCar.ABS);
		SetESP (vehicle, defaultCar.ESP);
		SetTCS (vehicle, defaultCar.TCS);
		SetSH (vehicle, defaultCar.steeringHelper);

		SetNOS (vehicle, defaultCar.useNOS);
		SetTurbo (vehicle, defaultCar.useTurbo);
		SetUseExhaustFlame (vehicle, defaultCar.useExhaustFlame);
		SetRevLimiter (vehicle, defaultCar.useRevLimiter);

		SetSmokeColor (vehicle, 0, Color.white);
		SetHeadlightsColor (vehicle, Color.white);

		SaveStats (vehicle);

		OverrideRCC (vehicle);

	}

	public static bool CheckVehicle(RCC_CarMainControllerV3 vehicle){

		if (!vehicle) {

			Debug.LogError ("Vehicle is missing!");
			return false;

		}

		return true;

	}

}
