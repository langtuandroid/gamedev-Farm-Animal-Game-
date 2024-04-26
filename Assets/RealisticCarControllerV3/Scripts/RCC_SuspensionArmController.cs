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
using UnityEngine.Serialization;

/// <summary>
/// Rotates and moves suspension arms based on wheelcollider suspension distance.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Visual Axle (Suspension Distance Based)")]
public class RCC_SuspensionArmController : MonoBehaviour {

	[FormerlySerializedAs("wheelcollider")] public RCC_WheelColliderController currWheelcollider;

	public SuspensionType suspensionType;
	public enum SuspensionType{Position, Rotation}

	[FormerlySerializedAs("axis")] public Axis axisValue;
	public enum Axis {X, Y, Z}

	private Vector3 orgPosVector;
	private Vector3 orgRotVector;

	private float totalSuspensionDistanceValue = 0;

	[FormerlySerializedAs("offsetAngle")] [SerializeField] private float offsetAngleValue = 30;
	[FormerlySerializedAs("angleFactor")] [SerializeField] private float angleFactorValue = 150;
	
	private void Start () {

		orgPosVector = transform.localPosition;
		orgRotVector = transform.localEulerAngles;

		totalSuspensionDistanceValue = GetSuspensionDistanceValue ();

	}

	private void Update () {
		
		float suspensionCourse = GetSuspensionDistanceValue () - totalSuspensionDistanceValue;

		transform.localPosition = orgPosVector;
		transform.localEulerAngles = orgRotVector;

		switch (suspensionType) {

		case SuspensionType.Position:

			switch(axisValue){

			case Axis.X:
				transform.position += transform.right * suspensionCourse;
				break;
			case Axis.Y:
				transform.position += transform.up * suspensionCourse;
				break;
			case Axis.Z:
				transform.position += transform.forward * suspensionCourse;
				break;

			}

			break;

		case SuspensionType.Rotation:

			switch (axisValue) {

			case Axis.X:
				transform.Rotate (Vector3.right, suspensionCourse * angleFactorValue - offsetAngleValue, Space.Self);
				break;
			case Axis.Y:
				transform.Rotate (Vector3.up, suspensionCourse * angleFactorValue - offsetAngleValue, Space.Self);
				break;
			case Axis.Z:
				transform.Rotate (Vector3.forward, suspensionCourse * angleFactorValue - offsetAngleValue, Space.Self);
				break;

			}

			break;

		}

	}
		
	private float GetSuspensionDistanceValue() {
		
		Quaternion quat;
		Vector3 position;
		currWheelcollider.wheelColliderObject.GetWorldPose(out position, out quat);
		Vector3 local = currWheelcollider.transform.InverseTransformPoint (position);
		return local.y;

	}

}
