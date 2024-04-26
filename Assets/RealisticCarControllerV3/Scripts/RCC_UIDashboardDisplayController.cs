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
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Handles RCC Canvas dashboard elements.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Dashboard Displayer")]
[RequireComponent (typeof(RCC_DashboardInputs))]
public class RCC_UIDashboardDisplayController : MonoBehaviour {

	// Getting an Instance of Main Shared RCC Settings.
	#region RCC Settings Instance

	private RCC_SettingsData RCCSettingsInstanceR;
	private RCC_SettingsData RCCSettingsInstance {
		get {
			if (RCCSettingsInstanceR == null) {
				RCCSettingsInstanceR = RCC_SettingsData.InstanceR;
				return RCCSettingsInstanceR;
			}
			return RCCSettingsInstanceR;
		}
	}

	#endregion

	private RCC_DashboardInputs inputsR;

	[FormerlySerializedAs("displayType")] public DisplayType displayTypeData;
	public enum DisplayType{Full, Customization, TopButtonsOnly, Off}

	[FormerlySerializedAs("topButtons")] public GameObject topButtonsObject;
	[FormerlySerializedAs("controllerButtons")] public GameObject controllerButtonsObject;
	[FormerlySerializedAs("gauges")] public GameObject gaugesObject;
	[FormerlySerializedAs("customizationMenu")] public GameObject customizationMenuObject;
	
	[FormerlySerializedAs("RPMLabel")] public Text RPMLabelText;
	[FormerlySerializedAs("KMHLabel")] public Text KMHLabelText;
	[FormerlySerializedAs("GearLabel")] public Text GearLabelText;
	[FormerlySerializedAs("recordingLabel")] public Text recordingLabelText;

	[FormerlySerializedAs("ABS")] public Image ABSImage;
	[FormerlySerializedAs("ESP")] public Image ESPImage;
	[FormerlySerializedAs("Park")] public Image ParkImage;
	[FormerlySerializedAs("Headlights")] public Image HeadlightsImage;
	[FormerlySerializedAs("leftIndicator")] public Image leftIndicatorImage;
	[FormerlySerializedAs("rightIndicator")] public Image rightIndicatorImage;
	[FormerlySerializedAs("heatIndicator")] public Image heatIndicatorImage;
	[FormerlySerializedAs("fuelIndicator")] public Image fuelIndicatorImage;
	[FormerlySerializedAs("rpmIndicator")] public Image rpmIndicatorImage;

	[FormerlySerializedAs("mobileControllers")] public Dropdown mobileControllersDropdown;

	private void Awake(){

		inputsR = GetComponent<RCC_DashboardInputs>();

		if (!inputsR) {

			enabled = false;
			return;

		}

	}
	
	private void Start () {
		
		CheckControllerType ();
		
	}

	private void OnEnable(){

		RCC_SceneManager.OnControllerChanged += CheckControllerType;

	}

	private void CheckControllerType(){

		if (RCCSettingsInstance.selectedControllerTypeR == RCC_SettingsData.ControllerType.Keyboard || RCCSettingsInstance.selectedControllerTypeR == RCC_SettingsData.ControllerType.XBox360One || RCCSettingsInstance.selectedControllerTypeR == RCC_SettingsData.ControllerType.PS4 || RCCSettingsInstance.selectedControllerTypeR == RCC_SettingsData.ControllerType.LogitechSteeringWheel) {

			if(mobileControllersDropdown)
				mobileControllersDropdown.interactable = false;
			
		}

		if (RCCSettingsInstance.selectedControllerTypeR == RCC_SettingsData.ControllerType.Mobile) {

			if(mobileControllersDropdown)
				mobileControllersDropdown.interactable = true;
			
		}

	}

	private void Update(){

		switch (displayTypeData) {

		case DisplayType.Full:

			if(topButtonsObject && !topButtonsObject.activeInHierarchy)
				topButtonsObject.SetActive(true);

			if(controllerButtonsObject && !controllerButtonsObject.activeInHierarchy)
				controllerButtonsObject.SetActive(true);

			if(gaugesObject && !gaugesObject.activeInHierarchy)
				gaugesObject.SetActive(true);

			if(customizationMenuObject && customizationMenuObject.activeInHierarchy)
				customizationMenuObject.SetActive(false);

			break;

		case DisplayType.Customization:

			if(topButtonsObject && topButtonsObject.activeInHierarchy)
				topButtonsObject.SetActive(false);

			if(controllerButtonsObject && controllerButtonsObject.activeInHierarchy)
				controllerButtonsObject.SetActive(false);

			if(gaugesObject && gaugesObject.activeInHierarchy)
				gaugesObject.SetActive(false);

			if(customizationMenuObject && !customizationMenuObject.activeInHierarchy)
				customizationMenuObject.SetActive(true);

			break;

		case DisplayType.TopButtonsOnly:

			if(!topButtonsObject.activeInHierarchy)
				topButtonsObject.SetActive(true);

			if(controllerButtonsObject.activeInHierarchy)
				controllerButtonsObject.SetActive(false);

			if(gaugesObject.activeInHierarchy)
				gaugesObject.SetActive(false);

			if(customizationMenuObject.activeInHierarchy)
				customizationMenuObject.SetActive(false);

			break;

		case DisplayType.Off:

			if(topButtonsObject &&topButtonsObject.activeInHierarchy)
				topButtonsObject.SetActive(false);

			if(controllerButtonsObject &&controllerButtonsObject.activeInHierarchy)
				controllerButtonsObject.SetActive(false);

			if(gaugesObject &&gaugesObject.activeInHierarchy)
				gaugesObject.SetActive(false);

			if(customizationMenuObject &&customizationMenuObject.activeInHierarchy)
				customizationMenuObject.SetActive(false);

			break;

		}

	}
	
	private void LateUpdate () {

		if (RCC_SceneManager.Instance.activePlayerVehicle) {
	
			if (RPMLabelText)
				RPMLabelText.text = inputsR.RPM.ToString ("0");
		
			if (KMHLabelText) {
			
				if (RCCSettingsInstance.unitsR == RCC_SettingsData.Units.KMH)
					KMHLabelText.text = inputsR.KMH.ToString ("0") + "\nKMH";
				else
					KMHLabelText.text = (inputsR.KMH * 0.62f).ToString ("0") + "\nMPH";
			
			}

			if (GearLabelText) {
			
				if (!inputsR.NGear && !inputsR.changingGear)
					GearLabelText.text = inputsR.direction == 1 ? (inputsR.Gear + 1).ToString ("0") : "R";
				else
					GearLabelText.text = "N";
			
			}

			if (recordingLabelText) {

				switch (RCC_SceneManager.Instance.recordMode) {

				case RCC_SceneManager.RecordMode.Neutral:

					if (recordingLabelText.gameObject.activeInHierarchy)
						recordingLabelText.gameObject.SetActive (false);

					recordingLabelText.text = "";

					break;

				case RCC_SceneManager.RecordMode.Play:

					if (!recordingLabelText.gameObject.activeInHierarchy)
						recordingLabelText.gameObject.SetActive (true);

					recordingLabelText.text = "Playing";
					recordingLabelText.color = Color.green;

					break;

				case RCC_SceneManager.RecordMode.Record:

					if (!recordingLabelText.gameObject.activeInHierarchy)
						recordingLabelText.gameObject.SetActive (true);

					recordingLabelText.text = "Recording";
					recordingLabelText.color = Color.red;

					break;

				}

			}

			if (ABSImage)
				ABSImage.color = inputsR.ABS == true ? Color.yellow : Color.white;
			if (ESPImage)
				ESPImage.color = inputsR.ESP == true ? Color.yellow : Color.white;
			if (ParkImage)
				ParkImage.color = inputsR.Park == true ? Color.red : Color.white;
			if (HeadlightsImage)
				HeadlightsImage.color = inputsR.Headlights == true ? Color.green : Color.white;
			if (heatIndicatorImage)
				heatIndicatorImage.color = RCC_SceneManager.Instance.activePlayerVehicle.engineHeat >= 100f ? Color.red : new Color (.1f, 0f, 0f);
			if (fuelIndicatorImage)
				fuelIndicatorImage.color = RCC_SceneManager.Instance.activePlayerVehicle.fuelTank < 10f ? Color.red : new Color (.1f, 0f, 0f); 
			if (rpmIndicatorImage)
				rpmIndicatorImage.color = RCC_SceneManager.Instance.activePlayerVehicle.engineRPM >= RCC_SceneManager.Instance.activePlayerVehicle.maxEngineRPM - 500f ? Color.red : new Color (.1f, 0f, 0f); 
		
			if (leftIndicatorImage && rightIndicatorImage) {

				switch (inputsR.indicators) {

				case RCC_CarMainControllerV3.IndicatorsOn.Left:
					leftIndicatorImage.color = new Color (1f, .5f, 0f);
					rightIndicatorImage.color = new Color (.5f, .25f, 0f);
					break;
				case RCC_CarMainControllerV3.IndicatorsOn.Right:
					leftIndicatorImage.color = new Color (.5f, .25f, 0f);
					rightIndicatorImage.color = new Color (1f, .5f, 0f);
					break;
				case RCC_CarMainControllerV3.IndicatorsOn.All:
					leftIndicatorImage.color = new Color (1f, .5f, 0f);
					rightIndicatorImage.color = new Color (1f, .5f, 0f);
					break;
				case RCC_CarMainControllerV3.IndicatorsOn.Off:
					leftIndicatorImage.color = new Color (.5f, .25f, 0f);
					rightIndicatorImage.color = new Color (.5f, .25f, 0f);
					break;

				}

			}

		}

	}

	public void SetDisplayTypeValue(DisplayType _displayType){

		displayTypeData = _displayType;

	}

	private void OnDisable(){

		RCC_SceneManager.OnControllerChanged -= CheckControllerType;

	}

}
