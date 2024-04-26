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
using UnityEngine.UI;

public class RCC_TelemetryR : MonoBehaviour{

	private RCC_CarMainControllerV3 carMainController;
	[FormerlySerializedAs("mainPanel")] public GameObject mainPanelObject;
	
	[FormerlySerializedAs("RPM_WheelFL")] public Text RPM_WheelFLText;
	[FormerlySerializedAs("RPM_WheelFR")] public Text RPM_WheelFRText;
	[FormerlySerializedAs("RPM_WheelRL")] public Text RPM_WheelRLText;
	[FormerlySerializedAs("RPM_WheelRR")] public Text RPM_WheelRRText;

	[FormerlySerializedAs("Torque_WheelFL")] public Text Torque_WheelFLText;
	[FormerlySerializedAs("Torque_WheelFR")] public Text Torque_WheelFRText;
	[FormerlySerializedAs("Torque_WheelRL")] public Text Torque_WheelRLText;
	[FormerlySerializedAs("Torque_WheelRR")] public Text Torque_WheelRRText;

	[FormerlySerializedAs("Brake_WheelFL")] public Text Brake_WheelFLText;
	[FormerlySerializedAs("Brake_WheelFR")] public Text Brake_WheelFRText;
	[FormerlySerializedAs("Brake_WheelRL")] public Text Brake_WheelRLText;
	[FormerlySerializedAs("Brake_WheelRR")] public Text Brake_WheelRRText;

	[FormerlySerializedAs("Force_WheelFL")] public Text Force_WheelFLText;
	[FormerlySerializedAs("Force_WheelFR")] public Text Force_WheelFRText;
	[FormerlySerializedAs("Force_WheelRL")] public Text Force_WheelRLText;
	[FormerlySerializedAs("Force_WheelRR")] public Text Force_WheelRRText;

	[FormerlySerializedAs("Angle_WheelFL")] public Text Angle_WheelFLText;
	[FormerlySerializedAs("Angle_WheelFR")] public Text Angle_WheelFRText;
	[FormerlySerializedAs("Angle_WheelRL")] public Text Angle_WheelRLText;
	[FormerlySerializedAs("Angle_WheelRR")] public Text Angle_WheelRRText;

	[FormerlySerializedAs("Sideways_WheelFL")] public Text Sideways_WheelFLText;
	[FormerlySerializedAs("Sideways_WheelFR")] public Text Sideways_WheelFRText;
	[FormerlySerializedAs("Sideways_WheelRL")] public Text Sideways_WheelRLText;
	[FormerlySerializedAs("Sideways_WheelRR")] public Text Sideways_WheelRRText;

	[FormerlySerializedAs("Forward_WheelFL")] public Text Forward_WheelFLText;
	[FormerlySerializedAs("Forward_WheelFR")] public Text Forward_WheelFRText;
	[FormerlySerializedAs("Forward_WheelRL")] public Text Forward_WheelRLText;
	[FormerlySerializedAs("Forward_WheelRR")] public Text Forward_WheelRRText;

	[FormerlySerializedAs("ABS")] public Text ABSText;
	[FormerlySerializedAs("ESP")] public Text ESPText;
	[FormerlySerializedAs("TCS")] public Text TCSText;

	[FormerlySerializedAs("GroundHit_WheelFL")] public Text GroundHit_WheelFLText;
	[FormerlySerializedAs("GroundHit_WheelFR")] public Text GroundHit_WheelFRText;
	[FormerlySerializedAs("GroundHit_WheelRL")] public Text GroundHit_WheelRLText;
	[FormerlySerializedAs("GroundHit_WheelRR")] public Text GroundHit_WheelRRText;

	[FormerlySerializedAs("speed")] public Text speedText;
	[FormerlySerializedAs("engineRPM")] public Text engineRPMText;
	[FormerlySerializedAs("gear")] public Text gearText;
	[FormerlySerializedAs("finalTorque")] public Text finalTorqueText;
	[FormerlySerializedAs("drivetrain")] public Text drivetrainText;
	[FormerlySerializedAs("angularVelocity")] public Text angularVelocityText;
	[FormerlySerializedAs("controllable")] public Text controllableText;

	[FormerlySerializedAs("throttle")] public Text throttleText;
	[FormerlySerializedAs("steer")] public Text steerText;
	[FormerlySerializedAs("brake")] public Text brakeText;
	[FormerlySerializedAs("handbrake")] public Text handbrakeText;
	[FormerlySerializedAs("clutch")] public Text clutchText;

    // Update is called once per frame
    private void Update(){

		mainPanelObject.SetActive (RCC_SettingsData.InstanceR.useTelemetryObject);

		carMainController = RCC_SceneManager.Instance.activePlayerVehicle;

		if (!carMainController)
			return;

		RPM_WheelFLText.text = carMainController.FrontLeftWheelCollider.wheelColliderObject.rpm.ToString ("F0");
		RPM_WheelFRText.text = carMainController.FrontRightWheelCollider.wheelColliderObject.rpm.ToString ("F0");
		RPM_WheelRLText.text = carMainController.RearLeftWheelCollider.wheelColliderObject.rpm.ToString ("F0");
		RPM_WheelRRText.text = carMainController.RearRightWheelCollider.wheelColliderObject.rpm.ToString ("F0");

		Torque_WheelFLText.text = carMainController.FrontLeftWheelCollider.wheelColliderObject.motorTorque.ToString ("F0");
		Torque_WheelFRText.text = carMainController.FrontRightWheelCollider.wheelColliderObject.motorTorque.ToString ("F0");
		Torque_WheelRLText.text = carMainController.RearLeftWheelCollider.wheelColliderObject.motorTorque.ToString ("F0");
		Torque_WheelRRText.text = carMainController.RearRightWheelCollider.wheelColliderObject.motorTorque.ToString ("F0");

		Brake_WheelFLText.text = carMainController.FrontLeftWheelCollider.wheelColliderObject.brakeTorque.ToString ("F0");
		Brake_WheelFRText.text = carMainController.FrontRightWheelCollider.wheelColliderObject.brakeTorque.ToString ("F0");
		Brake_WheelRLText.text = carMainController.RearLeftWheelCollider.wheelColliderObject.brakeTorque.ToString ("F0");
		Brake_WheelRRText.text = carMainController.RearRightWheelCollider.wheelColliderObject.brakeTorque.ToString ("F0");

		Force_WheelFLText.text = carMainController.FrontLeftWheelCollider.oldForceValue.ToString ("F0");
		Force_WheelFRText.text = carMainController.FrontRightWheelCollider.oldForceValue.ToString ("F0");
		Force_WheelRLText.text = carMainController.RearLeftWheelCollider.oldForceValue.ToString ("F0");
		Force_WheelRRText.text = carMainController.RearRightWheelCollider.oldForceValue.ToString ("F0");

		Angle_WheelFLText.text = carMainController.FrontLeftWheelCollider.wheelColliderObject.steerAngle.ToString ("F0");
		Angle_WheelFRText.text = carMainController.FrontRightWheelCollider.wheelColliderObject.steerAngle.ToString ("F0");
		Angle_WheelRLText.text = carMainController.RearLeftWheelCollider.wheelColliderObject.steerAngle.ToString ("F0");
		Angle_WheelRRText.text = carMainController.RearRightWheelCollider.wheelColliderObject.steerAngle.ToString ("F0");

		Sideways_WheelFLText.text = carMainController.FrontLeftWheelCollider.wheelHitData.sidewaysSlip.ToString ("F");
		Sideways_WheelFRText.text = carMainController.FrontRightWheelCollider.wheelHitData.sidewaysSlip.ToString ("F");
		Sideways_WheelRLText.text = carMainController.RearLeftWheelCollider.wheelHitData.sidewaysSlip.ToString ("F");
		Sideways_WheelRRText.text = carMainController.RearRightWheelCollider.wheelHitData.sidewaysSlip.ToString ("F");

		Forward_WheelFLText.text = carMainController.FrontLeftWheelCollider.wheelHitData.forwardSlip.ToString ("F");
		Forward_WheelFRText.text = carMainController.FrontRightWheelCollider.wheelHitData.forwardSlip.ToString ("F");
		Forward_WheelRLText.text = carMainController.RearLeftWheelCollider.wheelHitData.forwardSlip.ToString ("F");
		Forward_WheelRRText.text = carMainController.RearRightWheelCollider.wheelHitData.forwardSlip.ToString ("F");

		ABSText.text = carMainController.ABSAct ? "Engaged" : "Not Engaged";
		ESPText.text = carMainController.ESPAct ? "Engaged" : "Not Engaged";
		TCSText.text = carMainController.TCSAct ? "Engaged" : "Not Engaged";

		GroundHit_WheelFLText.text = carMainController.FrontLeftWheelCollider.isGroundedFlag ? carMainController.FrontLeftWheelCollider.wheelHitData.collider.name : "";
		GroundHit_WheelFRText.text = carMainController.FrontRightWheelCollider.isGroundedFlag ? carMainController.FrontRightWheelCollider.wheelHitData.collider.name : "";
		GroundHit_WheelRLText.text = carMainController.RearLeftWheelCollider.isGroundedFlag ? carMainController.RearLeftWheelCollider.wheelHitData.collider.name : "";
		GroundHit_WheelRRText.text = carMainController.RearRightWheelCollider.isGroundedFlag ? carMainController.RearRightWheelCollider.wheelHitData.collider.name : "";

		speedText.text = carMainController.speed.ToString ("F0");
		engineRPMText.text = carMainController.engineRPM.ToString ("F0");
		gearText.text = carMainController.currentGear.ToString ("F0");

		switch(carMainController.wheelTypeChoise){

		case RCC_CarMainControllerV3.WheelType.FWD:

			drivetrainText.text = "FWD";
			break;

		case RCC_CarMainControllerV3.WheelType.RWD:

			drivetrainText.text = "RWD";
			break;

		case RCC_CarMainControllerV3.WheelType.AWD:

			drivetrainText.text = "AWD";
			break;

		}

		angularVelocityText.text = carMainController.rigid.angularVelocity.ToString ();
		controllableText.text = carMainController.canControl ? "True" : "False";

		throttleText.text = carMainController.throttleInput.ToString ("F");
		steerText.text = carMainController.steerInput.ToString ("F");
		brakeText.text = carMainController.brakeInput.ToString ("F");
		handbrakeText.text = carMainController.handbrakeInput.ToString ("F");
		clutchText.text = carMainController.clutchInput.ToString ("F");
        
    }

}
