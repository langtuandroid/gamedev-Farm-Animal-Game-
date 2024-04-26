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

/// <summary>
/// Truck trailer has additional wheelcolliders. This script handles center of mass of the trailer, wheelcolliders, and antiroll.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Truck Trailer")]
[RequireComponent (typeof(Rigidbody))]
public class RCC_TruckTrailerController : MonoBehaviour {

	private RCC_CarMainControllerV3 carControllerR;
	private Rigidbody rigidb;
	private ConfigurableJoint jointObject;

	[FormerlySerializedAs("COM")] public Transform COMTransform;
	private bool isSleepingFlag = false;

	[System.Serializable]
	public class TrailerWheelData{

		[FormerlySerializedAs("wheelCollider")] public WheelCollider wheelColliderObject;
		[FormerlySerializedAs("wheelModel")] public Transform wheelModelTransform;

		public void TorqueSet(float torque){

			wheelColliderObject.motorTorque = torque;

		}

		public void BrakeSet(float torque){

			wheelColliderObject.brakeTorque = torque;

		}

	}

	//Extra Wheels.
	[FormerlySerializedAs("trailerWheels")] public TrailerWheelData[] trailerWheelsMass;

	[FormerlySerializedAs("attached")] public bool attachedFlag = false;

	public class JointConfigRestrictions{

		public ConfigurableJointMotion motionXValue;
		public ConfigurableJointMotion motionYValue;
		public ConfigurableJointMotion motionZValue;

		public ConfigurableJointMotion angularMotionXValue;
		public ConfigurableJointMotion angularMotionYValue;
		public ConfigurableJointMotion angularMotionZValue;

		public void GetJoint(ConfigurableJoint configurableJoint){

			motionXValue = configurableJoint.xMotion;
			motionYValue = configurableJoint.yMotion;
			motionZValue = configurableJoint.zMotion;

			angularMotionXValue = configurableJoint.angularXMotion;
			angularMotionYValue = configurableJoint.angularYMotion;
			angularMotionZValue = configurableJoint.angularZMotion;

		}

		public void SetJoint(ConfigurableJoint configurableJoint){

			configurableJoint.xMotion = motionXValue;
			configurableJoint.yMotion = motionYValue;
			configurableJoint.zMotion = motionZValue;

			configurableJoint.angularXMotion = angularMotionXValue;
			configurableJoint.angularYMotion = angularMotionYValue;
			configurableJoint.angularZMotion = angularMotionZValue;

		}

		public void ResetJoint(ConfigurableJoint configurableJoint){

			configurableJoint.xMotion = ConfigurableJointMotion.Free;
			configurableJoint.yMotion = ConfigurableJointMotion.Free;
			configurableJoint.zMotion = ConfigurableJointMotion.Free;

			configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Free;

		}

	}

	[FormerlySerializedAs("jointRestrictions")] public JointConfigRestrictions jointConfigRestrictions = new JointConfigRestrictions();

	private void Start () {

		rigidb = GetComponent<Rigidbody>();
		jointObject = GetComponentInParent<ConfigurableJoint> ();
		jointConfigRestrictions.GetJoint (jointObject);

		rigidb.interpolation = RigidbodyInterpolation.None;
		rigidb.interpolation = RigidbodyInterpolation.Interpolate;
		jointObject.configuredInWorldSpace = true;

		if (jointObject.connectedBody) {
			
			AttachTrailerCar (jointObject.connectedBody.gameObject.GetComponent<RCC_CarMainControllerV3> ());

		} else {
			
			carControllerR = null;
			jointObject.connectedBody = null;
			jointConfigRestrictions.ResetJoint (jointObject);

		}

	}

	private void FixedUpdate(){

		attachedFlag = jointObject.connectedBody;
		
		rigidb.centerOfMass = transform.InverseTransformPoint(COMTransform.transform.position);

		if (!carControllerR)
			return;

		for (int i = 0; i < trailerWheelsMass.Length; i++) {
			
			trailerWheelsMass [i].TorqueSet (carControllerR.throttleInput * (attachedFlag ? 1f : 0f));
			trailerWheelsMass [i].BrakeSet ((attachedFlag ? 0f : 5000f));

		}

	}

	private void Update(){

		if(rigidb.velocity.magnitude < .01f && Mathf.Abs(rigidb.angularVelocity.magnitude) < .01f)
			isSleepingFlag = true;
		else
			isSleepingFlag = false;
		for (int i = 0; i < trailerWheelsMass.Length; i++) {

			trailerWheelsMass [i].TorqueSet (carControllerR.throttleInput * (attachedFlag ? 1f : 0f));
			trailerWheelsMass [i].BrakeSet ((attachedFlag ? 0f : 5000f));

		}
		WheelAlignPos ();

	}

	// Aligning wheel model position and rotation.
	private void WheelAlignPos (){
		
		if (isSleepingFlag)
			return;

		for (int i = 0; i < trailerWheelsMass.Length; i++) {

			// Return if no wheel model selected.
			if(!trailerWheelsMass[i].wheelModelTransform){

				Debug.LogError(transform.name + " wheel of the " + transform.name + " is missing wheel model. This wheel is disabled");
				enabled = false;
				return;

			}

			// Locating correct position and rotation for the wheel.
			Vector3 wheelPosition = Vector3.zero;
			Quaternion wheelRotation = Quaternion.identity;
			trailerWheelsMass[i].wheelColliderObject.GetWorldPose (out wheelPosition, out wheelRotation);

			//	Assigning position and rotation to the wheel model.
			trailerWheelsMass[i].wheelModelTransform.transform.position = wheelPosition;
			trailerWheelsMass[i].wheelModelTransform.transform.rotation = wheelRotation;

		}

	}

	public void DetachTrailerCamera(){

		carControllerR = null;
		jointObject.connectedBody = null;
		jointConfigRestrictions.ResetJoint (jointObject);

		if (RCC_SceneManager.Instance.activePlayerCamera)
			StartCoroutine(RCC_SceneManager.Instance.activePlayerCamera.AutoFocusCoroutine ());

	}

	public void AttachTrailerCar(RCC_CarMainControllerV3 vehicle){

		carControllerR = vehicle;

		jointObject.connectedBody = vehicle.rigid;
		jointConfigRestrictions.SetJoint (jointObject);

		vehicle.attachedTrailer = this;

		if (RCC_SceneManager.Instance.activePlayerCamera)
			StartCoroutine(RCC_SceneManager.Instance.activePlayerCamera.AutoFocusCoroutine (transform, carControllerR.transform));

	}

	void OnTriggerEnter(Collider col){

//		RCC_TrailerAttachPoint attacher = col.gameObject.GetComponent<RCC_TrailerAttachPoint> ();
//
//		if (!attacher)
//			return;
//
//		RCC_CarControllerV3 vehicle = attacher.gameObject.GetComponentInParent<RCC_CarControllerV3> ();
//
//		if (!vehicle || !attacher)
//			return;
//		
//		AttachTrailer (vehicle, attacher);

	}

}
