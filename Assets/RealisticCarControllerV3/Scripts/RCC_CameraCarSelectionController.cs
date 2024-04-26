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

public class RCC_CameraCarSelectionController : MonoBehaviour{

	[FormerlySerializedAs("target")] public Transform targetTransform;
	[FormerlySerializedAs("distance")] public float distanceValue= 10.0f;

	[FormerlySerializedAs("xSpeed")] public float xSpeedValue= 250f;
	[FormerlySerializedAs("ySpeed")] public float  ySpeedValue= 120f;

	[FormerlySerializedAs("yMinLimit")] public float yMinLimitValue= -20f;
	[FormerlySerializedAs("yMaxLimit")] public float yMaxLimitValue= 80f;

	private float xValue= 0f;
	private float yValue= 0f;

	private bool selfTurnFlag = true;
	private float selfTurnTimeValue = 0f;

	private void Start (){

		Vector3 angles= transform.eulerAngles;
		xValue = angles.y;
		yValue = angles.x;

	}

	private void LateUpdate (){

		if (targetTransform) {

			if(selfTurnFlag)
				xValue += xSpeedValue / 2f * Time.deltaTime;

			yValue = ClampAngleValue(yValue, yMinLimitValue, yMaxLimitValue);

			Quaternion rotation= Quaternion.Euler(yValue, xValue, 0);
			Vector3 position= rotation * new Vector3(0f, 0f, -distanceValue) + targetTransform.position;

			transform.rotation = rotation;
			transform.position = position;

			if (selfTurnTimeValue <= 1f)
				selfTurnTimeValue += Time.deltaTime;

			if (selfTurnTimeValue >= 1f)
				selfTurnFlag = true;

		}

	}

	static float ClampAngleValue ( float angle ,   float min ,   float max  ){

		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);

	}

	public void OnDragObject(BaseEventData data){
		
		PointerEventData pointerData = data as PointerEventData;

		xValue += pointerData.delta.x * xSpeedValue * 0.02f;
		yValue -= pointerData.delta.y * ySpeedValue * 0.02f;

		yValue = ClampAngleValue(yValue, yMinLimitValue, yMaxLimitValue);

		Quaternion rotation= Quaternion.Euler(yValue, xValue, 0);
		Vector3 position= rotation * new Vector3(0f, 0f, -distanceValue) + targetTransform.position;

		transform.rotation = rotation;
		transform.position = position;

		selfTurnFlag = false;
		selfTurnTimeValue = 0f;

	}

}