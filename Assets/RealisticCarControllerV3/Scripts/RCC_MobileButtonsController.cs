//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// Receiving inputs from UI buttons, and feeds active vehicles on your scene.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Mobile Buttons")]
public class RCC_MobileButtonsController : RCC_CoreMain {

	#region Singleton
	private static RCC_MobileButtonsController instanceR;
	public static RCC_MobileButtonsController InstanceR{	get{if(instanceR == null) instanceR = GameObject.FindObjectOfType<RCC_MobileButtonsController>(); return instanceR;}}
	#endregion

	// Getting an Instance of Main Shared RCC Settings.
	#region RCC Settings Instance

	private RCC_SettingsData RCCSettingsInstanceR;
	private RCC_SettingsData RCCSettingsR {
		get {
			if (RCCSettingsInstanceR == null) {
				RCCSettingsInstanceR = RCC_SettingsData.InstanceR;
				return RCCSettingsInstanceR;
			}
			return RCCSettingsInstanceR;
		}
	}

	#endregion

	[FormerlySerializedAs("gasButton")] public RCC_UIInputController gasButtonUI;
	[FormerlySerializedAs("gradualGasButton")] public RCC_UIInputController gradualGasButtonUI;
	[FormerlySerializedAs("brakeButton")] public RCC_UIInputController brakeButtonUI;
	[FormerlySerializedAs("leftButton")] public RCC_UIInputController leftButtonUI;
	[FormerlySerializedAs("rightButton")] public RCC_UIInputController rightButtonUI;
	[FormerlySerializedAs("steeringWheel")] public RCC_UISteeringCarWheelController steeringWheelUI;
	[FormerlySerializedAs("handbrakeButton")] public RCC_UIInputController handbrakeButtonUI;
	[FormerlySerializedAs("NOSButton")] public RCC_UIInputController NOSButtonUI;
	[FormerlySerializedAs("NOSButtonSteeringWheel")] public RCC_UIInputController NOSButtonSteeringWheelUI;
	[FormerlySerializedAs("gearButton")] public GameObject gearButtonUI;

	[FormerlySerializedAs("joystick")] public RCC_UIJoystickInput joystickR;

	[FormerlySerializedAs("inputs")] public RCC_InputsData inputsR = new RCC_InputsData ();

	private float throttleInputValue = 0f;
	private float brakeInputValue = 0f;
	private float leftInputValue = 0f;
	private float rightInputValue = 0f;
	private float steeringWheelInputValue = 0f;
	private float handbrakeInputValue = 0f;
	private float boostInputValue = 1f;
	private float gyroInputValue = 0f;
	private float joystickInputValue = 0f;
	private float horizontalInputValue;
	private float verticalInputValue;
	private bool canUseNosFlag = false;

	private Vector3 orgBrakeButtonPosVector;

	private void Start(){

		if(brakeButtonUI)
			orgBrakeButtonPosVector = brakeButtonUI.transform.position;

		CheckPlayerController ();

	}

	private void OnEnable(){

		RCC_SceneManager.OnControllerChanged += CheckPlayerController;
		RCC_SceneManager.OnVehicleChanged += CheckPlayerController;

	}

	private void CheckPlayerController(){

		if (!RCC_SceneManager.Instance.activePlayerVehicle)
			return;

		if (RCCSettingsR.selectedControllerTypeR == RCC_SettingsData.ControllerType.Mobile) {

			EnableButtonsUI ();
			return;

		} else {

			DisableButtonsUI ();
			return;

		}

	}

	void DisableButtonsUI(){

		if (gasButtonUI)
			gasButtonUI.gameObject.SetActive (false);
		if (gradualGasButtonUI)
			gradualGasButtonUI.gameObject.SetActive (false);
		if (leftButtonUI)
			leftButtonUI.gameObject.SetActive (false);
		if (rightButtonUI)
			rightButtonUI.gameObject.SetActive (false);
		if (brakeButtonUI)
			brakeButtonUI.gameObject.SetActive (false);
		if (steeringWheelUI)
			steeringWheelUI.gameObject.SetActive (false);
		if (handbrakeButtonUI)
			handbrakeButtonUI.gameObject.SetActive (false);
		if (NOSButtonUI)
			NOSButtonUI.gameObject.SetActive (false);
		if (NOSButtonSteeringWheelUI)
			NOSButtonSteeringWheelUI.gameObject.SetActive (false);
		if (gearButtonUI)
			gearButtonUI.gameObject.SetActive (false);
		if (joystickR)
			joystickR.gameObject.SetActive (false);

	}

	void EnableButtonsUI(){

		if (gasButtonUI)
			gasButtonUI.gameObject.SetActive (true);
		//			if (gradualGasButton)
		//				gradualGasButton.gameObject.SetActive (true);
		if (leftButtonUI)
			leftButtonUI.gameObject.SetActive (true);
		if (rightButtonUI)
			rightButtonUI.gameObject.SetActive (true);
		if (brakeButtonUI)
			brakeButtonUI.gameObject.SetActive (true);
		if (steeringWheelUI)
			steeringWheelUI.gameObject.SetActive (true);
		if (handbrakeButtonUI)
			handbrakeButtonUI.gameObject.SetActive (true);

		if (canUseNosFlag) {

			if (NOSButtonUI)
				NOSButtonUI.gameObject.SetActive (true);
			if (NOSButtonSteeringWheelUI)
				NOSButtonSteeringWheelUI.gameObject.SetActive (true);

		}

		if (joystickR)
			joystickR.gameObject.SetActive (true);
		
	}

	private void Update(){

		if (RCCSettingsR.selectedControllerTypeR != RCC_SettingsData.ControllerType.Mobile)
			return;

		switch (RCCSettingsR.mobileControllerObject) {

		case RCC_SettingsData.MobileController.TouchScreen:

			gyroInputValue = 0f;

			if(steeringWheelUI && steeringWheelUI.gameObject.activeInHierarchy)
				steeringWheelUI.gameObject.SetActive(false);

			if(NOSButtonUI && NOSButtonUI.gameObject.activeInHierarchy != canUseNosFlag)
				NOSButtonUI.gameObject.SetActive(canUseNosFlag);

			if(joystickR && joystickR.gameObject.activeInHierarchy)
				joystickR.gameObject.SetActive(false);

			if(!leftButtonUI.gameObject.activeInHierarchy){

				brakeButtonUI.transform.position = orgBrakeButtonPosVector;
				leftButtonUI.gameObject.SetActive(true);

			}

			if(!rightButtonUI.gameObject.activeInHierarchy)
				rightButtonUI.gameObject.SetActive(true);

			break;

		case RCC_SettingsData.MobileController.Gyro:

			gyroInputValue = Mathf.Lerp(gyroInputValue, Input.acceleration.x * RCCSettingsR.gyroSensitivityValue, Time.deltaTime * 5f);
			brakeButtonUI.transform.position = leftButtonUI.transform.position;

			if(steeringWheelUI && steeringWheelUI.gameObject.activeInHierarchy)
				steeringWheelUI.gameObject.SetActive(false);

			if(NOSButtonUI && NOSButtonUI.gameObject.activeInHierarchy != canUseNosFlag)
				NOSButtonUI.gameObject.SetActive(canUseNosFlag);

			if(joystickR && joystickR.gameObject.activeInHierarchy)
				joystickR.gameObject.SetActive(false);
			
			if(leftButtonUI.gameObject.activeInHierarchy)
				leftButtonUI.gameObject.SetActive(false);

			if(rightButtonUI.gameObject.activeInHierarchy)
				rightButtonUI.gameObject.SetActive(false);

			break;

		case RCC_SettingsData.MobileController.SteeringWheel:

			gyroInputValue = 0f;

			if(steeringWheelUI && !steeringWheelUI.gameObject.activeInHierarchy){
				steeringWheelUI.gameObject.SetActive(true);
				brakeButtonUI.transform.position = orgBrakeButtonPosVector;
			}

			if(NOSButtonUI && NOSButtonUI.gameObject.activeInHierarchy)
				NOSButtonUI.gameObject.SetActive(false);

			if(NOSButtonSteeringWheelUI && NOSButtonSteeringWheelUI.gameObject.activeInHierarchy != canUseNosFlag)
				NOSButtonSteeringWheelUI.gameObject.SetActive(canUseNosFlag);

			if(joystickR && joystickR.gameObject.activeInHierarchy)
				joystickR.gameObject.SetActive(false);
			
			if(leftButtonUI.gameObject.activeInHierarchy)
				leftButtonUI.gameObject.SetActive(false);
			if(rightButtonUI.gameObject.activeInHierarchy)
				rightButtonUI.gameObject.SetActive(false);

			break;

		case RCC_SettingsData.MobileController.Joystick:

			gyroInputValue = 0f;

			if (steeringWheelUI && steeringWheelUI.gameObject.activeInHierarchy)
				steeringWheelUI.gameObject.SetActive (false);

			if (NOSButtonUI && NOSButtonUI.gameObject.activeInHierarchy != canUseNosFlag)
				NOSButtonUI.gameObject.SetActive (canUseNosFlag);

			if (joystickR && !joystickR.gameObject.activeInHierarchy) {
				joystickR.gameObject.SetActive (true);
				brakeButtonUI.transform.position = orgBrakeButtonPosVector;
			}
			
			if(leftButtonUI.gameObject.activeInHierarchy)
				leftButtonUI.gameObject.SetActive(false);

			if(rightButtonUI.gameObject.activeInHierarchy)
				rightButtonUI.gameObject.SetActive(false);

			break;

		}

		throttleInputValue = GetInputR(gasButtonUI) + GetInputR(gradualGasButtonUI);
		brakeInputValue = GetInputR (brakeButtonUI);
		leftInputValue = GetInputR(leftButtonUI);
		rightInputValue = GetInputR(rightButtonUI);
		handbrakeInputValue = GetInputR(handbrakeButtonUI);
		boostInputValue = Mathf.Clamp((GetInputR(NOSButtonUI) + GetInputR(NOSButtonSteeringWheelUI)), 0f, 1f);

		if(steeringWheelUI)
			steeringWheelInputValue = steeringWheelUI.inputValue;

		if(joystickR)
			joystickInputValue = joystickR.inputHorizontalValue;
		
		FeedRCCR ();

	}

	private void FeedRCCR(){

		if (!RCC_SceneManager.Instance.activePlayerVehicle)
			return;

		canUseNosFlag = RCC_SceneManager.Instance.activePlayerVehicle.useNOS;

		inputsR.throttleInputValue = throttleInputValue;
		inputsR.brakeInputValue = brakeInputValue;
		inputsR.steerInputValue = -leftInputValue + rightInputValue + steeringWheelInputValue + gyroInputValue + joystickInputValue;
		inputsR.handbrakeInputValue = handbrakeInputValue;
		inputsR.boostInputValue = boostInputValue;

	}

	// Gets input from button.
	float GetInputR(RCC_UIInputController button){

		if(button == null)
			return 0f;

		return(button.inputValue);

	}

	// Gets input from joystick.
	Vector2 GetInputR(RCC_UIJoystickInput joystick){

		if(joystick == null)
			return Vector2.zero;

		return(joystick.inputVectorValue);

	}

	private void OnDisable(){

		RCC_SceneManager.OnControllerChanged -= CheckPlayerController;
		RCC_SceneManager.OnVehicleChanged -= CheckPlayerController;

	}

}
