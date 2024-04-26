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
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// UI input (float) receiver from UI Button. 
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Controller Button")]
public class RCC_UIInputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

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

	private Button buttonObject;
	private Slider sliderObject;

	internal float inputValue;
	private float sensitivityValue{get{return RCCSettingsInstance.UIButtonSensitivityValue;}}
	private float gravityValue{get{return RCCSettingsInstance.UIButtonGravityValue;}}
	[FormerlySerializedAs("pressing")] public bool pressingFlag;

	private void Awake(){

		buttonObject = GetComponent<Button> ();
		sliderObject = GetComponent<Slider> ();

	}

	public void OnPointerDown(PointerEventData eventData){
		
		pressingFlag = true;

	}

	public void OnPointerUp(PointerEventData eventData){
		 
		pressingFlag = false;
		
	}

	void OnPress (bool isPressed){

		if(isPressed)
			pressingFlag = true;
		else
			pressingFlag = false;

	}

	private void Update(){

		if (buttonObject && !buttonObject.interactable) {
			
			pressingFlag = false;
			inputValue = 0f;
			return;

		}

		if (sliderObject && !sliderObject.interactable) {

			pressingFlag = false;
			inputValue = 0f;
			sliderObject.value = 0f;
			return;

		}

		if (sliderObject) {

			if(pressingFlag)
				inputValue = sliderObject.value;
			else
				inputValue = 0f;

			sliderObject.value = inputValue;

		} else {

			if (pressingFlag)
				inputValue += Time.deltaTime * sensitivityValue;
			else
				inputValue -= Time.deltaTime * gravityValue;

		}

		if(inputValue < 0f)
			inputValue = 0f;
		
		if(inputValue > 1f)
			inputValue = 1f;
		
	}

	private void OnDisable(){

		inputValue = 0f;
		pressingFlag = false;

	}

}
