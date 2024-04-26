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
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// UI Steering Wheel controller.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Steering Wheel")]
public class RCC_UISteeringCarWheelController : MonoBehaviour {

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

	private GameObject steeringWheelGameObjectValue;
	private Image steeringWheelTextureValue;

	[FormerlySerializedAs("input")] public float inputValue = 0f;
	[FormerlySerializedAs("steeringWheelAngle")] public float steeringWheelAngleValue = 0f;
	[FormerlySerializedAs("steeringWheelMaximumsteerAngle")] public float steeringWheelMaximumsteerAngleValue = 270f;
	[FormerlySerializedAs("steeringWheelResetPosSpeed")] public float steeringWheelResetPosSpeedValue = 20f;	
	[FormerlySerializedAs("steeringWheelCenterDeadZoneRadius")] public float steeringWheelCenterDeadZoneRadiusValue = 5f;

	private RectTransform steeringWheelRectTransform;
	private CanvasGroup steeringWheelCanvasGroupObject;

	private float steeringWheelTempAngleValue, steeringWheelNewAngleValue;
	private bool steeringWheelPressedFlag;

	private Vector2 steeringWheelCenterVector, steeringWheelTouchPosVector;

	private EventTrigger eventTriggerValue;

	private void Awake(){

		steeringWheelTextureValue = GetComponent<Image>();

	}

	private void Update () {

		if(RCCSettingsInstance.mobileControllerObject != RCC_SettingsData.MobileController.SteeringWheel)
			return;

		SteeringWheelInit();
		SteeringWheelControl();
		inputValue = GetSteeringCarWheelInput();

	}

	private void SteeringWheelInit(){

		if (steeringWheelRectTransform && !steeringWheelTextureValue)
			return;

		steeringWheelGameObjectValue = steeringWheelTextureValue.gameObject;
		steeringWheelRectTransform = steeringWheelTextureValue.rectTransform;
		steeringWheelCanvasGroupObject = steeringWheelTextureValue.GetComponent<CanvasGroup> ();
		steeringWheelCenterVector = steeringWheelRectTransform.position;
		
		SteeringWheelEventsInit ();

	}

	//Events Initialization For Steering Wheel.
	private void SteeringWheelEventsInit(){

		eventTriggerValue = steeringWheelGameObjectValue.GetComponent<EventTrigger>();
		
		var a = new EventTrigger.TriggerEvent();
		a.AddListener( data => 
		              {
			var evData = (PointerEventData)data;
			data.Use();
			
			steeringWheelPressedFlag = true;
			steeringWheelTouchPosVector = evData.position;
			steeringWheelTempAngleValue = Vector2.Angle(Vector2.up, evData.position - steeringWheelCenterVector);
		});
		
		eventTriggerValue.triggers.Add(new EventTrigger.Entry{callback = a, eventID = EventTriggerType.PointerDown});
		
		
		var b = new EventTrigger.TriggerEvent();
		b.AddListener( data => 
		              {
			var evData = (PointerEventData)data;
			data.Use();
			steeringWheelTouchPosVector = evData.position;
		});
		
		eventTriggerValue.triggers.Add(new EventTrigger.Entry{callback = b, eventID = EventTriggerType.Drag});
		
		
		var c = new EventTrigger.TriggerEvent();
		c.AddListener( data => 
		              {
			steeringWheelPressedFlag = false;
		});
		
		eventTriggerValue.triggers.Add(new EventTrigger.Entry{callback = c, eventID = EventTriggerType.EndDrag});

	}

	private float GetSteeringCarWheelInput(){

		return Mathf.Round(steeringWheelAngleValue / steeringWheelMaximumsteerAngleValue * 100) / 100;

	}

	public bool isSteeringWheelPressedFlag(){

		return steeringWheelPressedFlag;

	}

	private void SteeringWheelControl (){

		if(!steeringWheelCanvasGroupObject || !steeringWheelRectTransform || RCCSettingsInstance.mobileControllerObject != RCC_SettingsData.MobileController.SteeringWheel){
			
			if(steeringWheelGameObjectValue)
				steeringWheelGameObjectValue.SetActive(false);
			
			return;

		}

		if(!steeringWheelGameObjectValue.activeSelf)
			steeringWheelGameObjectValue.SetActive(true);

		if(steeringWheelPressedFlag){

			steeringWheelNewAngleValue = Vector2.Angle(Vector2.up, steeringWheelTouchPosVector - steeringWheelCenterVector);

			if(Vector2.Distance( steeringWheelTouchPosVector, steeringWheelCenterVector ) > steeringWheelCenterDeadZoneRadiusValue){

				if(steeringWheelTouchPosVector.x > steeringWheelCenterVector.x)
					steeringWheelAngleValue += steeringWheelNewAngleValue - steeringWheelTempAngleValue;
				else
					steeringWheelAngleValue -= steeringWheelNewAngleValue - steeringWheelTempAngleValue;

			}

			if(steeringWheelAngleValue > steeringWheelMaximumsteerAngleValue)
				steeringWheelAngleValue = steeringWheelMaximumsteerAngleValue;
			else if(steeringWheelAngleValue < -steeringWheelMaximumsteerAngleValue)
				steeringWheelAngleValue = -steeringWheelMaximumsteerAngleValue;
			
			steeringWheelTempAngleValue = steeringWheelNewAngleValue;

		}else{

			if(!Mathf.Approximately(0f, steeringWheelAngleValue)){

				float deltaAngle = steeringWheelResetPosSpeedValue;
				
				if(Mathf.Abs(deltaAngle) > Mathf.Abs(steeringWheelAngleValue)){
					steeringWheelAngleValue = 0f;
					return;
				}
				
				if(steeringWheelAngleValue > 0f)
					steeringWheelAngleValue -= deltaAngle;
				else
					steeringWheelAngleValue += deltaAngle;

			}

		}

		steeringWheelRectTransform.eulerAngles = new Vector3 (0f, 0f, -steeringWheelAngleValue);
		
	}

	private void OnDisable(){
		
		steeringWheelPressedFlag = false;
		inputValue = 0f;

	}

}
