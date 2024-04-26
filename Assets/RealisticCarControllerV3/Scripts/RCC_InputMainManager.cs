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
using UnityEngine.UI;

public class RCC_InputMainManager : MonoBehaviour{

	private static RCC_InputsData inputsData = new RCC_InputsData();

	private enum InputState { None, Pressed, Held, Released };

	public static RCC_InputsData GetInputsData(){

		switch (RCC_SettingsData.InstanceR.selectedControllerTypeR) {

		case RCC_SettingsData.ControllerType.Keyboard:

			inputsData.throttleInputValue = Mathf.Clamp01(Input.GetAxis (RCC_SettingsData.InstanceR.verticalInputValue));
			inputsData.brakeInputValue = Mathf.Abs(Mathf.Clamp(Input.GetAxis (RCC_SettingsData.InstanceR.verticalInputValue), -1f, 0f));
			inputsData.steerInputValue = Mathf.Clamp(Input.GetAxis (RCC_SettingsData.InstanceR.horizontalInputValue), -1f, 1f);
			inputsData.handbrakeInputValue = Mathf.Clamp01(Input.GetKey (RCC_SettingsData.InstanceR.handbrakeKBCode) ? 1f : 0f);
			inputsData.boostInputValue = Mathf.Clamp01(Input.GetKey (RCC_SettingsData.InstanceR.boostKBCode) ? 1f : 0f);

			break;

		case RCC_SettingsData.ControllerType.XBox360One:

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.Xbox_triggerRightInputValue))
				inputsData.throttleInputValue = Input.GetAxis (RCC_SettingsData.InstanceR.Xbox_triggerRightInputValue);

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.Xbox_triggerLeftInputValue))
				inputsData.brakeInputValue = Input.GetAxis (RCC_SettingsData.InstanceR.Xbox_triggerLeftInputValue);

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.Xbox_horizontalInputValue))
				inputsData.steerInputValue = Input.GetAxis (RCC_SettingsData.InstanceR.Xbox_horizontalInputValue);

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.Xbox_handbrakeKBValue))
				inputsData.handbrakeInputValue = Input.GetButton (RCC_SettingsData.InstanceR.Xbox_handbrakeKBValue) ? 1f : 0f;

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.Xbox_boostKBValue))
				inputsData.boostInputValue = Input.GetButton(RCC_SettingsData.InstanceR.Xbox_boostKBValue) ? 1f : 0f;

			break;

		case RCC_SettingsData.ControllerType.PS4:

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.PS4_triggerRightInputValue))
				inputsData.throttleInputValue = Mathf.Clamp01(Input.GetAxis(RCC_SettingsData.InstanceR.PS4_triggerRightInputValue));

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.PS4_triggerLeftInputValue))
				inputsData.brakeInputValue = Input.GetAxis(RCC_SettingsData.InstanceR.PS4_triggerLeftInputValue);

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.PS4_horizontalInputValue))
				inputsData.steerInputValue = Input.GetAxis(RCC_SettingsData.InstanceR.PS4_horizontalInputValue);

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.PS4_handbrakeKBValue))
				inputsData.handbrakeInputValue = Input.GetButton(RCC_SettingsData.InstanceR.PS4_handbrakeKBValue) ? 1f : 0f;

			if(!string.IsNullOrEmpty(RCC_SettingsData.InstanceR.PS4_boostKBValue))
				inputsData.boostInputValue = Input.GetButton(RCC_SettingsData.InstanceR.PS4_boostKBValue) ? 1f : 0f;

			break;

			case RCC_SettingsData.ControllerType.Mobile:

			RCC_MobileButtonsController mobileInput = RCC_MobileButtonsController.InstanceR;

			if (mobileInput) {
				
				inputsData.throttleInputValue = RCC_MobileButtonsController.InstanceR.inputsR.throttleInputValue;
				inputsData.brakeInputValue = RCC_MobileButtonsController.InstanceR.inputsR.brakeInputValue;
				inputsData.steerInputValue = RCC_MobileButtonsController.InstanceR.inputsR.steerInputValue;
				inputsData.handbrakeInputValue = RCC_MobileButtonsController.InstanceR.inputsR.handbrakeInputValue;
				inputsData.boostInputValue = RCC_MobileButtonsController.InstanceR.inputsR.boostInputValue;

			}

			break;

		case RCC_SettingsData.ControllerType.LogitechSteeringWheel:

			#if BCG_LOGITECH
			RCC_LogitechSteeringWheel log = RCC_LogitechSteeringWheel.Instance;

			if (log) {

				inputs.throttleInput = log.inputs.throttleInput;
				inputs.brakeInput = log.inputs.brakeInput;
				inputs.steerInput = log.inputs.steerInput;
				inputs.clutchInput = log.inputs.clutchInput;
				inputs.handbrakeInput = log.inputs.handbrakeInput;

			}
			#endif

			break;

		case RCC_SettingsData.ControllerType.Custom:

			// You can use your own inputs with Custom controller type here.

//			inputs.throttleInput = "yourValue";
//			inputs.brakeInput = "yourValue";
//			inputs.steerInput = "yourValue";
//			inputs.boostInput = "yourValue";
//			inputs.clutchInput = "yourValue";
//			inputs.handbrakeInput = "yourValue";

			break;

		}

		return inputsData;

	}

	public static bool GetKeyDownFlag(KeyCode keyCode){

		if (Input.GetKeyDown (keyCode))
			return true;

		return false;

	}

	public static bool GetKeyUpFlag(KeyCode keyCode){

		if (Input.GetKeyUp (keyCode))
			return true;

		return false;

	}

	public static bool GetKeyFlag(KeyCode keyCode){

		if (Input.GetKey (keyCode))
			return true;

		return false;

	}

	public static bool GetButtonDownFlag(string buttonCode){

		if (Input.GetButtonDown (buttonCode))
			return true;

		return false;

	}

	public static bool GetButtonUpFlag(string buttonCode){

		if (Input.GetButtonUp (buttonCode))
			return true;

		return false;

	}

	public static bool GetButtonFlag(string buttonCode){

		if (Input.GetButton (buttonCode))
			return true;

		return false;

	}

}
