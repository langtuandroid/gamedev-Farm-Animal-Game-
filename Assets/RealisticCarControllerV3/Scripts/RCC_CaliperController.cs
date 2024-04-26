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

/// <summary>
/// Rotates the brake caliper.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Visual Brake Caliper")]
public class RCC_CaliperController : MonoBehaviour {

	[FormerlySerializedAs("wheelCollider")] public RCC_WheelColliderController currentWheelCollider;		//	Actual WheelCollider.
	private GameObject newPivotObject;						//	Creating new center pivot for correct position.
	private Quaternion defLocalRotationQuaternion;			//	Default rotation.

	private void Start () {

		//	No need to go further if no wheelcollider found.
		if (!currentWheelCollider){

			Debug.LogError ("WheelCollider is not selected for this caliper named " + transform.name);
			enabled = false;
			return;

		}

		//	Creating new center pivot for correct position.
		newPivotObject = new GameObject ("Pivot_" + transform.name);
		newPivotObject.transform.SetParent (currentWheelCollider.wheelColliderObject.transform, false);
		transform.SetParent (newPivotObject.transform, true);

		//	Assigning default rotation.
		defLocalRotationQuaternion = newPivotObject.transform.localRotation;
		
	}

	private void Update () {

		//	No need to go further if no wheelcollider found.
		if (!currentWheelCollider.wheelModelTransform || !currentWheelCollider.wheelColliderObject)
			return;

		//	Re-positioning camber pivot.
		newPivotObject.transform.position = new Vector3 (currentWheelCollider.wheelModelTransform.transform.position.x, currentWheelCollider.wheelModelTransform.transform.position.y, currentWheelCollider.wheelModelTransform.transform.position.z);
		//	Re-rotationing camber pivot.
		newPivotObject.transform.localRotation = defLocalRotationQuaternion * Quaternion.AngleAxis (currentWheelCollider.wheelColliderObject.steerAngle, Vector3.up);
		
	}

}
