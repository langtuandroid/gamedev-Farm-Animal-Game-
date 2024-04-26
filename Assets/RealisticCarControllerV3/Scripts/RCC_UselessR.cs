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

public class RCC_UselessR : MonoBehaviour {

	[FormerlySerializedAs("useless")] public Useless uselessR;
	public enum Useless{MainController, MobileControllers, Behavior, Graphics}

	// Use this for initialization
	private void Awake () {

		int type = 0;

		if(uselessR == Useless.Behavior){

			type = RCC_SettingsData.InstanceR.behaviorSelectedIndexValue;

		}if(uselessR == Useless.MainController){

			type = RCC_SettingsData.InstanceR.controllerSelectedIndexValue;

		}if(uselessR == Useless.MobileControllers){

			switch (RCC_SettingsData.InstanceR.mobileControllerObject) {

			case RCC_SettingsData.MobileController.TouchScreen:

				type = 0;

				break;

			case RCC_SettingsData.MobileController.Gyro:

				type = 1;

				break;

			case RCC_SettingsData.MobileController.SteeringWheel:

				type = 2;

				break;

			case RCC_SettingsData.MobileController.Joystick:

				type = 3;

				break;

			}

		}if(uselessR == Useless.Graphics){

			type = QualitySettings.GetQualityLevel ();

		}

		GetComponent<Dropdown>().value = type;
		GetComponent<Dropdown>().RefreshShownValue();
	
	}

}
