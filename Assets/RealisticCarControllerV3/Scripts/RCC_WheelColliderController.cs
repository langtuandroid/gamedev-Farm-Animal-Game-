//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------


using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Based on Unity's WheelCollider. Modifies few curves, settings in order to get stable and realistic physics depends on selected behavior in RCC Settings.
/// </summary>
[RequireComponent (typeof(WheelCollider))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Wheel Collider")]
public class RCC_WheelColliderController : RCC_CoreMain {
	
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

	// Getting an Instance of Ground Materials.
	#region RCC_GroundMaterials Instance

	private RCC_GroundMaterialsData RCCGroundMaterialsInstanceR;
	private RCC_GroundMaterialsData RCCGroundMaterialsInstance {
		get {
			if (RCCGroundMaterialsInstanceR == null) {
				RCCGroundMaterialsInstanceR = RCC_GroundMaterialsData.InstanceR;
			}
			return RCCGroundMaterialsInstanceR;
		}
	}

	#endregion

	// WheelCollider.
	private WheelCollider _wheelColliderObject;
	public WheelCollider wheelColliderObject{
		get{
			if(_wheelColliderObject == null)
				_wheelColliderObject = GetComponent<WheelCollider>();
			return _wheelColliderObject;
		}
	}

	// Car controller.
	private RCC_CarMainControllerV3 _carMainController;
	public RCC_CarMainControllerV3 carMainController{
		get{
			if(_carMainController == null)
				_carMainController = GetComponentInParent<RCC_CarMainControllerV3>();
			return _carMainController;
		}
	}

	// Rigidbody of the vehicle.
	private Rigidbody _rigidbody;
	public Rigidbody rigidbd{
		get{
			if(_rigidbody == null)
				_rigidbody = carMainController.gameObject.GetComponent<Rigidbody>();
			return _rigidbody;
		}
	}

	private List <RCC_WheelColliderController> allWheelCollidersList = new List<RCC_WheelColliderController>() ;		// All wheelcolliders attached to this vehicle.
	[FormerlySerializedAs("wheelModel")] public Transform wheelModelTransform;		// Wheel model for animating and aligning.

	[FormerlySerializedAs("wheelHit")] public WheelHit wheelHitData;				//	Wheel Hit data.
	[FormerlySerializedAs("isGrounded")] public bool isGroundedFlag = false;		//	Is wheel grounded?

	[FormerlySerializedAs("canPower")] [Space()]
	public bool canPowerFlag = false;		//	Can this wheel power?
	[FormerlySerializedAs("powerMultiplier")] [Range(-1f, 1f)]public float powerMultiplierValue = 1f;
	[FormerlySerializedAs("canSteer")] public bool canSteerFlag = false;		//	Can this wheel steer?
	[FormerlySerializedAs("steeringMultiplier")] [Range(-1f, 1f)]public float steeringMultiplierValue = 1f;
	[FormerlySerializedAs("canBrake")] public bool canBrakeFlag = false;		//	Can this wheel brake?
	[FormerlySerializedAs("brakingMultiplier")] [Range(0f, 1f)]public float brakingMultiplierValue = 1f;
	[FormerlySerializedAs("canHandbrake")] public bool canHandbrakeFlag = false;		//	Can this wheel handbrake?
	[FormerlySerializedAs("handbrakeMultiplier")] [Range(0f, 1f)]public float handbrakeMultiplierValue = 1f;

	[FormerlySerializedAs("width")] [Space()]
	public float widthValue = .275f;	//	Width.
	[FormerlySerializedAs("offset")] public float offsetValue = .05f;		// Offset by X axis.

	internal float wheelRPM2SpeedValue = 0f;     // Wheel RPM to Speed.

	[FormerlySerializedAs("camber")] [Range(-5f, 5f)]public float camberValue = 0f;		// Camber angle.
	[FormerlySerializedAs("caster")] [Range(-5f, 5f)]public float casterValue = 0f;		// Caster angle.
	[FormerlySerializedAs("toe")] [Range(-5f, 5f)]public float toeValue = 0f;          	// Toe angle.

	internal float damagedCamberValue = 0f;			// Damaged camber angle.
	internal float damagedCasterValue = 0f;             // Damaged caster angle.
	internal float damagedToeValue = 0f;                 // Damaged toe angle.

	private RCC_GroundMaterialsData physicsGroundMaterials{get{return RCCGroundMaterialsInstance;}}		// Getting instance of Configurable Ground Materials.
	private RCC_GroundMaterialsData.GroundMaterialFrictionsData[] physicsGroundFrictions{get{return RCCGroundMaterialsInstance.frictionsMass;}}

	//	Skidmarks
	private RCC_SkidmarksGroundManager skidmarksManagerMain;		// Main Skidmark Managers class.
	private int lastSkidmarkValue = -1;

	//	Slips
	private float wheelSlipAmountForwardValue = 0f;		// Forward slip.
	private float wheelSlipAmountSidewaysValue = 0f;	// Sideways slip.
	private float totalSlipValue = 0f;								// Total amount of forward and sideways slips.

	//	WheelFriction Curves and Stiffness.
	private WheelFrictionCurve forwardFrictionCurveWheel;		//	Forward friction curve.
	private WheelFrictionCurve sidewaysFrictionCurveWheel;	//	Sideways friction curve.

	//	Audio
	private AudioSource audioSourceObject;		// Audiosource for tire skid SFX.
	private AudioClip audioClipValue;					// Audioclip for tire skid SFX.
	private float audioVolumeValue = 1f;			//	Maximum volume for tire skid SFX.

	private int groundIndex = 0;		// Current ground physic material index.

	// List for all particle systems.
	internal List<ParticleSystem> allWheelParticlesList = new List<ParticleSystem>();
	internal ParticleSystem.EmissionModule emissionModule;

	//	Tractions used for smooth drifting.
	internal float tractionHelpedSidewaysStiffnessValue = 1f;
	private float minForwardStiffnessValue = .75f;
	private float maxForwardStiffnessValue  = 1f;
	private float minSidewaysStiffnessValue = .75f;
	private float maxSidewaysStiffnessValue = 1f;

	//	Terrain data.
	private TerrainData mTerrainDataValue;
	private int alphamapWidthValue;
	private int alphamapHeightValue;

	private float[,,] mSplatmapDataValue;
	private float mNumTexturesValue;

	// Getting bump force.
	[FormerlySerializedAs("bumpForce")] public float bumpForceValue;
	[FormerlySerializedAs("oldForce")] public float oldForceValue;

	private void Start (){

		// Getting all WheelColliders attached to this vehicle (Except this).
		allWheelCollidersList = carMainController.GetComponentsInChildren<RCC_WheelColliderController>().ToList();

		GetCurrTerrainData ();		//	Getting terrain datas on scene.
		CheckBehaviorWheel ();		//	Checks selected behavior in RCC Settings.

		// Are we going to use skidmarks? If we do, get or create SkidmarksManager on scene.
		if (!RCCSettingsInstance.dontUseSkidmarksFlag) {

			if (GameObject.FindObjectOfType<RCC_SkidmarksGroundManager> ())
				skidmarksManagerMain = GameObject.FindObjectOfType<RCC_SkidmarksGroundManager> ();
			else
				skidmarksManagerMain = GameObject.Instantiate (RCCSettingsInstance.skidmarksManagerR, Vector3.zero, Quaternion.identity);

		}

		// Increasing WheelCollider mass for avoiding unstable behavior.
		if (RCCSettingsInstance.useFixedWheelCollidersFlag)
			wheelColliderObject.mass = rigidbd.mass / 15f;

		// Creating audiosource for skid SFX.
		audioSourceObject = CreateNewAudioSource(RCCSettingsInstance.audioMixerValue, carMainController.gameObject, "Skid Sound AudioSource", 5f, 50f, 0f, audioClipValue, true, true, false);
		audioSourceObject.transform.position = transform.position;

		// Creating all ground particles, and adding them to list.
		if (!RCCSettingsInstance.dontUseAnyParticleEffectsFlag) {

			for (int i = 0; i < RCCGroundMaterialsInstance.frictionsMass.Length; i++) {

				GameObject ps = (GameObject)Instantiate (RCCGroundMaterialsInstance.frictionsMass [i].groundParticlesObject, transform.position, transform.rotation) as GameObject;
				emissionModule = ps.GetComponent<ParticleSystem> ().emission;
				emissionModule.enabled = false;
				ps.transform.SetParent (transform, false);
				ps.transform.localPosition = Vector3.zero;
				ps.transform.localRotation = Quaternion.identity;
				allWheelParticlesList.Add (ps.GetComponent<ParticleSystem> ());

			}

		}

		//	Creating pivot position of the wheel at correct position and rotation.
		GameObject newPivot = new GameObject ("Pivot_" + wheelModelTransform.transform.name);
		newPivot.transform.position = RCC_GetBoundsSize.GetBoundsCenterVector (wheelModelTransform.transform);
		newPivot.transform.rotation = transform.rotation;
		newPivot.transform.SetParent (wheelModelTransform.transform.parent, true);

		//	Settings offsets.
		if (newPivot.transform.localPosition.x > 0)
			wheelModelTransform.transform.position += transform.right * offsetValue;
		else
			wheelModelTransform.transform.position -= transform.right * offsetValue;

		//	Assigning temporary created wheel to actual wheel.
		wheelModelTransform.SetParent (newPivot.transform, true);
		wheelModelTransform = newPivot.transform;

		// Override wheels automatically if enabled.
		if (carMainController.overrideWheels) {

			// Overriding canPower, canSteer, canBrake, canHandbrake.
			if (this == carMainController.FrontLeftWheelCollider || this == carMainController.FrontRightWheelCollider) {

				canSteerFlag = true;
				canBrakeFlag = true;
				brakingMultiplierValue = 1f;

			}

			if (this == carMainController.RearLeftWheelCollider || this == carMainController.RearRightWheelCollider) {

				canHandbrakeFlag = true;
				canBrakeFlag = true;
				brakingMultiplierValue = .5f;

			}

		}

	}

	private void OnEnable(){

		// Listening an event when main behavior changed.
		RCC_SceneManager.OnBehaviorChanged += CheckBehaviorWheel;

	}

	private void CheckBehaviorWheel(){

		// Getting friction curves.
		forwardFrictionCurveWheel = wheelColliderObject.forwardFriction;
		sidewaysFrictionCurveWheel = wheelColliderObject.sidewaysFriction;

		//	Getting behavior if selected.
		RCC_SettingsData.BehaviorTypeData behavior = RCCSettingsInstance.selectedBehaviorTypeR;

		//	If there is a selected behavior, override friction curves.
		if (behavior != null) {

			forwardFrictionCurveWheel = SetFrictionCurvesWheel (forwardFrictionCurveWheel, behavior.forwardExtremumSlipValue, behavior.forwardExtremumValueR, behavior.forwardAsymptoteSlipValue, behavior.forwardAsymptoteValueR);
			sidewaysFrictionCurveWheel = SetFrictionCurvesWheel (sidewaysFrictionCurveWheel, behavior.sidewaysExtremumSlipValue, behavior.sidewaysExtremumValueR, behavior.sidewaysAsymptoteSlipValue, behavior.sidewaysAsymptoteValueR);

		}
			
		// Assigning new frictons.
		wheelColliderObject.forwardFriction = forwardFrictionCurveWheel;
		wheelColliderObject.sidewaysFriction = sidewaysFrictionCurveWheel;

	}

	private void Update(){

		// Return if RCC is disabled.
		if (!carMainController.enabled)
			return;

		// Setting position and rotation of the wheel model.
		WheelAlignModel ();

	}
	
	private void FixedUpdate (){

		isGroundedFlag = wheelColliderObject.GetGroundHit (out wheelHitData);
		groundIndex = GetGroundMaterialIndexR ();

		float circumFerence = 2.0f * 3.14f * wheelColliderObject.radius; // Finding circumFerence 2 Pi R.
		wheelRPM2SpeedValue = (circumFerence * wheelColliderObject.rpm)*60; // Finding KMH.
		wheelRPM2SpeedValue = Mathf.Clamp(wheelRPM2SpeedValue / 1000f, 0f, Mathf.Infinity);

		// Setting power state of the wheels depending on drivetrain mode. Only overrides them if overrideWheels is enabled for the vehicle.
		if (carMainController.overrideWheels) {

			switch (carMainController.wheelTypeChoise) {

			case RCC_CarMainControllerV3.WheelType.AWD:
				canPowerFlag = true;

				break;

			case RCC_CarMainControllerV3.WheelType.BIASED:
				canPowerFlag = true;

				break;

			case RCC_CarMainControllerV3.WheelType.FWD:

				if (this == carMainController.FrontLeftWheelCollider || this == carMainController.FrontRightWheelCollider)
					canPowerFlag = true;
				else
					canPowerFlag = false;

				break;

			case RCC_CarMainControllerV3.WheelType.RWD:

				if (this == carMainController.RearLeftWheelCollider || this == carMainController.RearRightWheelCollider)
					canPowerFlag = true;
				else
					canPowerFlag = false;

				break;

			}

		}

		FrictionsSet ();
		SkidMarksCalculate ();
		AudioSet ();
		ParticlesEnable ();

		// Return if RCC is disabled.
		if (!carMainController.enabled)
			return;

		#region ESP.

		// ESP System. All wheels have individual brakes. In case of loosing control of the vehicle, corresponding wheel will brake for gaining the control again.
		if (carMainController.ESP && carMainController.brakeInput < .5f) {

			if (carMainController.handbrakeInput < .5f) {

				if (carMainController.underSteering) {

					if (this == carMainController.FrontLeftWheelCollider)
						ApplyBrakeTorqueValue ((carMainController.brakeTorque * carMainController.ESPStrength) * Mathf.Clamp (-carMainController.rearSlip, 0f, Mathf.Infinity));

					if (this == carMainController.FrontRightWheelCollider)
						ApplyBrakeTorqueValue ((carMainController.brakeTorque * carMainController.ESPStrength) * Mathf.Clamp (carMainController.rearSlip, 0f, Mathf.Infinity));

				}

				if (carMainController.overSteering) {

					if (this == carMainController.RearLeftWheelCollider)
						ApplyBrakeTorqueValue ((carMainController.brakeTorque * carMainController.ESPStrength) * Mathf.Clamp (-carMainController.frontSlip, 0f, Mathf.Infinity));

					if (this == carMainController.RearRightWheelCollider)
						ApplyBrakeTorqueValue ((carMainController.brakeTorque * carMainController.ESPStrength) * Mathf.Clamp (carMainController.frontSlip, 0f, Mathf.Infinity));

				}

			}

		}

		#endregion

	}

	// Aligning wheel model position and rotation.
	private void WheelAlignModel (){
		
		// Return if no wheel model selected.
		if(!wheelModelTransform){
			
			Debug.LogError(transform.name + " wheel of the " + carMainController.transform.name + " is missing wheel model. This wheel is disabled");
			enabled = false;
			return;

		}

		// Locating correct position and rotation for the wheel.
		Vector3 wheelPosition = Vector3.zero;
		Quaternion wheelRotation = Quaternion.identity;
		wheelColliderObject.GetWorldPose (out wheelPosition, out wheelRotation);

		//	Assigning position and rotation to the wheel model.
		wheelModelTransform.transform.position = wheelPosition;
		wheelModelTransform.transform.rotation = wheelRotation;

		//	Adjusting offset by X axis.
		if (transform.localPosition.x < 0f)
			wheelModelTransform.transform.position += transform.right * offsetValue;
		else
			wheelModelTransform.transform.position -= transform.right * offsetValue;

        // Adjusting camber angle by Z axis.
        if (transform.localPosition.x < 0f)
            wheelModelTransform.transform.RotateAround(wheelModelTransform.transform.position, transform.forward, -camberValue - damagedCamberValue);
        else
            wheelModelTransform.transform.RotateAround(wheelModelTransform.transform.position, transform.forward, camberValue + damagedCamberValue);

		// Adjusting caster angle by X axis.
		if (transform.localPosition.x < 0f)
			wheelModelTransform.transform.RotateAround(wheelModelTransform.transform.position, transform.right, -casterValue - damagedCasterValue);
		else
			wheelModelTransform.transform.RotateAround(wheelModelTransform.transform.position, transform.right, casterValue + damagedCasterValue);

		//transform.rotation = carController.transform.rotation * Quaternion.Euler(caster, toe, camber);

	}

	/// <summary>
	/// Skidmarks.
	/// </summary>
	private void SkidMarksCalculate(){

		// Forward, sideways, and total slips.
		if (isGroundedFlag) {
			wheelSlipAmountForwardValue = Mathf.Abs (wheelHitData.forwardSlip);
			wheelSlipAmountSidewaysValue = Mathf.Abs (wheelHitData.sidewaysSlip);
		} else {
			wheelSlipAmountForwardValue = 0f;
			wheelSlipAmountSidewaysValue = 0f;
		}	
			
		totalSlipValue = Mathf.Lerp(totalSlipValue, ((wheelSlipAmountSidewaysValue + wheelSlipAmountForwardValue) / 2f), Time.fixedDeltaTime * 5f);

		// If scene has skidmarks manager...
		if(!RCCSettingsInstance.dontUseSkidmarksFlag){

			// If slips are bigger than target value...
			if (totalSlipValue > physicsGroundFrictions [groundIndex].slipValue){

				Vector3 skidPoint = wheelHitData.point + 2f * (rigidbd.velocity) * Time.deltaTime;

				if (rigidbd.velocity.magnitude > 1f)
					lastSkidmarkValue = skidmarksManagerMain.AddGroundSkidMark (skidPoint, wheelHitData.normal, totalSlipValue - physicsGroundFrictions [groundIndex].slipValue, lastSkidmarkValue, groundIndex, widthValue);
				else
					lastSkidmarkValue = -1;

			}else{
				
				lastSkidmarkValue = -1;

			}

		}

	}

	/// <summary>
	/// Sets forward and sideways frictions.
	/// </summary>
	private void FrictionsSet(){

		// Handbrake input clamped 0f - 1f.
		float hbInput = carMainController.handbrakeInput;

		if ((this == carMainController.RearLeftWheelCollider || this == carMainController.RearRightWheelCollider) && hbInput > .75f)
			hbInput = .5f;
		else
			hbInput = 1;

		// Setting wheel stiffness to ground physic material stiffness.
		forwardFrictionCurveWheel.stiffness = physicsGroundFrictions[groundIndex].forwardStiffnessValue;
		sidewaysFrictionCurveWheel.stiffness = (physicsGroundFrictions[groundIndex].sidewaysStiffnessValue * hbInput * tractionHelpedSidewaysStiffnessValue);

		// If drift mode is selected, apply specific frictions.
		if(RCCSettingsInstance.selectedBehaviorTypeR != null && RCCSettingsInstance.selectedBehaviorTypeR.applyExternalWheelFrictionsFlag)
			DriftCalculate();

		// Setting new friction curves to wheels.
		wheelColliderObject.forwardFriction = forwardFrictionCurveWheel;
		wheelColliderObject.sidewaysFriction = sidewaysFrictionCurveWheel;

		// Also damp too.
		wheelColliderObject.wheelDampingRate = physicsGroundFrictions[groundIndex].dampValue;

		// Set audioclip to ground physic material sound.
		audioClipValue = physicsGroundFrictions[groundIndex].groundAudioSound;
		audioVolumeValue = physicsGroundFrictions [groundIndex].volumeValue;

	}

	/// <summary>
	/// Particles.
	/// </summary>
	private void ParticlesEnable(){

		// If wheel slip is bigger than ground physic material slip, enable particles. Otherwise, disable particles.
		if (!RCCSettingsInstance.dontUseAnyParticleEffectsFlag) {

			for (int i = 0; i < allWheelParticlesList.Count; i++) {

				if (totalSlipValue > physicsGroundFrictions [groundIndex].slipValue) {

					if (i != groundIndex) {

						ParticleSystem.EmissionModule em;

						em = allWheelParticlesList [i].emission;
						em.enabled = false;

					} else {

						ParticleSystem.EmissionModule em;

						em = allWheelParticlesList [i].emission;
						em.enabled = true;

					}

				} else {

					ParticleSystem.EmissionModule em;

					em = allWheelParticlesList [i].emission;
					em.enabled = false;

				}

			}

		}

	}

	/// <summary>
	/// Drift.
	/// </summary>
	private void DriftCalculate(){

		Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbd.velocity);

		float sqrVel = (relativeVelocity.x * relativeVelocity.x) / 10f;
		sqrVel += (Mathf.Abs(wheelHitData.forwardSlip * wheelHitData.forwardSlip) * 1f);

		// Forward
		if(wheelColliderObject == carMainController.FrontLeftWheelCollider.wheelColliderObject || wheelColliderObject == carMainController.FrontRightWheelCollider.wheelColliderObject){
			forwardFrictionCurveWheel.extremumValue = Mathf.Clamp(1f - sqrVel, .5f, maxForwardStiffnessValue);
			forwardFrictionCurveWheel.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), .5f, minForwardStiffnessValue);
		}else{
			forwardFrictionCurveWheel.extremumValue = Mathf.Clamp(1f - sqrVel, 1f, maxForwardStiffnessValue);
			forwardFrictionCurveWheel.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), 1.2f, minForwardStiffnessValue);
		}

		// Sideways
		if(wheelColliderObject == carMainController.FrontLeftWheelCollider.wheelColliderObject || wheelColliderObject == carMainController.FrontRightWheelCollider.wheelColliderObject){
			sidewaysFrictionCurveWheel.extremumValue = Mathf.Clamp(1f - sqrVel, .4f, maxSidewaysStiffnessValue);
			sidewaysFrictionCurveWheel.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), .4f, minSidewaysStiffnessValue);
		}else{
			sidewaysFrictionCurveWheel.extremumValue = Mathf.Clamp(1f - sqrVel, .375f, maxSidewaysStiffnessValue);
			sidewaysFrictionCurveWheel.asymptoteValue = Mathf.Clamp(.75f - (sqrVel / 2f), .375f, minSidewaysStiffnessValue);
		}

	}

	/// <summary>
	/// Audio.
	/// </summary>
	private void AudioSet(){

		// If total slip is high enough...
		if(totalSlipValue > physicsGroundFrictions [groundIndex].slipValue){

			// Assigning corresponding audio clip.
			if(audioSourceObject.clip != audioClipValue)
				audioSourceObject.clip = audioClipValue;

			// Playing it.
			if(!audioSourceObject.isPlaying)
				audioSourceObject.Play();

			// If vehicle is moving, set volume and pitch. Otherwise set them to 0.
			if(rigidbd.velocity.magnitude > 1f){
				
				audioSourceObject.volume = Mathf.Lerp(0f, audioVolumeValue, totalSlipValue - 0);
				audioSourceObject.pitch = Mathf.Lerp(1f, .8f, audioSourceObject.volume);

			}else{
				
				audioSourceObject.volume = 0f;

			}
			
		}else{
			
			audioSourceObject.volume = 0f;

			// If volume is minimal and audio is still playing, stop.
			if(audioSourceObject.volume <= .05f && audioSourceObject.isPlaying)
				audioSourceObject.Stop();
			
		}

		// Calculating bump force.
		bumpForceValue = wheelHitData.force - oldForceValue;

		//	If bump force is high enough, play bump SFX.
		if ((bumpForceValue) >= 5000f) {

			// Creating and playing audiosource for bump SFX.
			AudioSource bumpSound = CreateNewAudioSource(RCCSettingsInstance.audioMixerValue, carMainController.gameObject, "Bump Sound AudioSource", 5f, 50f, (bumpForceValue - 5000f) / 3000f, RCCSettingsInstanceR.bumpClipValue, false, true, true);
			bumpSound.pitch = Random.Range (.9f, 1.1f);

		}

		oldForceValue = wheelHitData.force;

	}

	/// <summary>
	/// Returns true if one of the wheel is slipping.
	/// </summary>
	/// <returns><c>true</c>, if skidding was ised, <c>false</c> otherwise.</returns>
	private bool isSkiddingFlag(){

		for (int i = 0; i < allWheelCollidersList.Count; i++) {

			if(allWheelCollidersList[i].totalSlipValue > physicsGroundFrictions [groundIndex].slipValue)
				return true;

		}

		return false;

	}

	/// <summary>
	/// Applies the motor torque.
	/// </summary>
	/// <param name="torque">Torque.</param>
	public void ApplyMotorTorqueValue(float torque){

		//	If TCS is enabled, checks forward slip. If wheel is losing traction, don't apply torque.
		if(carMainController.TCS){

			if(Mathf.Abs(wheelColliderObject.rpm) >= 100){
				
				if(wheelHitData.forwardSlip > physicsGroundFrictions [groundIndex].slipValue){
					
					carMainController.TCSAct = true;
					torque -= Mathf.Clamp(torque * (wheelHitData.forwardSlip) * carMainController.TCSStrength, 0f, Mathf.Infinity);

				}else{
					
					carMainController.TCSAct = false;
					torque += Mathf.Clamp(torque * (wheelHitData.forwardSlip) * carMainController.TCSStrength, -Mathf.Infinity, 0f);

				}

			}else{
				
				carMainController.TCSAct = false;

			}

		}
			
		if(OverTorqueFlag())
			torque = 0;

		if(Mathf.Abs(torque) > 1f)
			wheelColliderObject.motorTorque = torque;
		else
			wheelColliderObject.motorTorque = 0f;

	}

	/// <summary>
	/// Applies the steering.
	/// </summary>
	/// <param name="steerInput">Steer input.</param>
	/// <param name="angle">Angle.</param>
	public void ApplySteeringInput(float steerInput, float angle){

		//	Ackerman steering formula.
		if (steerInput > 0f) {

			if (transform.localPosition.x < 0)
				wheelColliderObject.steerAngle = (Mathf.Deg2Rad * angle * 2.55f) * (Mathf.Rad2Deg * Mathf.Atan (2.55f / (6 + (1.5f / 2))) * steerInput);
			else
				wheelColliderObject.steerAngle = (Mathf.Deg2Rad * angle * 2.55f) * (Mathf.Rad2Deg * Mathf.Atan (2.55f  / (6 - (1.5f / 2))) * steerInput);

		} else if (steerInput < 0f) {

			if (transform.localPosition.x < 0)
				wheelColliderObject.steerAngle = (Mathf.Deg2Rad * angle * 2.55f) * (Mathf.Rad2Deg * Mathf.Atan (2.55f / (6 - (1.5f / 2))) * steerInput);
			else
				wheelColliderObject.steerAngle = (Mathf.Deg2Rad * angle * 2.55f) * (Mathf.Rad2Deg * Mathf.Atan (2.55f  / (6 + (1.5f / 2))) * steerInput);

		} else {

			wheelColliderObject.steerAngle = 0f;

		}

		if (transform.localPosition.x < 0)
			wheelColliderObject.steerAngle += toeValue + damagedToeValue;
		else
			wheelColliderObject.steerAngle -= toeValue + damagedToeValue;

	}

	/// <summary>
	/// Applies the brake torque.
	/// </summary>
	/// <param name="torque">Torque.</param>
	public void ApplyBrakeTorqueValue(float torque){

		//	If ABS is enabled, checks forward slip. If wheel is losing traction, don't apply torque.
		if(carMainController.ABS && carMainController.handbrakeInput <= .1f){

			if((Mathf.Abs(wheelHitData.forwardSlip) * Mathf.Clamp01(torque)) >= carMainController.ABSThreshold){
				
				carMainController.ABSAct = true;
				torque = 0;

			}else{
				
				carMainController.ABSAct = false;

			}

		}

		if(Mathf.Abs(torque) > 1f)
			wheelColliderObject.brakeTorque = torque;
		else
			wheelColliderObject.brakeTorque = 0f;

	}

	/// <summary>
	/// Checks if overtorque applying.
	/// </summary>
	/// <returns><c>true</c>, if torque was overed, <c>false</c> otherwise.</returns>
	private bool OverTorqueFlag(){

		if(carMainController.speed > carMainController.maxspeed || !carMainController.engineRunning)
			return true;

		return false;

	}

	/// <summary>
	/// Gets the terrain data.
	/// </summary>
	private void GetCurrTerrainData(){

		if (!Terrain.activeTerrain)
			return;
		
		mTerrainDataValue = Terrain.activeTerrain.terrainData;
		alphamapWidthValue = mTerrainDataValue.alphamapWidth;
		alphamapHeightValue = mTerrainDataValue.alphamapHeight;

		mSplatmapDataValue = mTerrainDataValue.GetAlphamaps(0, 0, alphamapWidthValue, alphamapHeightValue);
		mNumTexturesValue = mSplatmapDataValue.Length / (alphamapWidthValue * alphamapHeightValue);

	}

	/// <summary>
	/// Converts to splat map coordinate.
	/// </summary>
	/// <returns>The to splat map coordinate.</returns>
	/// <param name="playerPos">Player position.</param>
	private Vector3 ConvertToSplatMapCoordinatePos(Vector3 playerPos){
		
		Vector3 vecRet = new Vector3();
		Terrain ter = Terrain.activeTerrain;
		Vector3 terPosition = ter.transform.position;
		vecRet.x = ((playerPos.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
		vecRet.z = ((playerPos.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
		return vecRet;

	}

	/// <summary>
	/// Gets the index of the ground material.
	/// </summary>
	/// <returns>The ground material index.</returns>
	private int GetGroundMaterialIndexR(){

		// Contacted any physic material in Configurable Ground Materials yet?
		bool contacted = false;

		if (wheelHitData.point == Vector3.zero)
			return 0;

		int ret = 0;
		
		for (int i = 0; i < physicsGroundFrictions.Length; i++) {

			if (wheelHitData.collider.sharedMaterial == physicsGroundFrictions [i].groundMaterialValue) {

				contacted = true;
				ret = i;

			}

		}

		// If ground pyhsic material is not one of the ground material in Configurable Ground Materials, check if we are on terrain collider...
		if(!contacted){

			for (int i = 0; i < RCCGroundMaterialsInstance.terrainFrictionsMass.Length; i++) {
				
				if (wheelHitData.collider.sharedMaterial == RCCGroundMaterialsInstance.terrainFrictionsMass [i].groundPhysicMaterial) {

					Vector3 playerPos = transform.position;
					Vector3 TerrainCord = ConvertToSplatMapCoordinatePos(playerPos);
					float comp = 0f;

					for (int k = 0; k < mNumTexturesValue; k++){

						if (comp < mSplatmapDataValue[(int)TerrainCord.z, (int)TerrainCord.x, k])
							ret = k;

					}

					ret = RCCGroundMaterialsInstanceR.terrainFrictionsMass [i].splatmapIndexesMass [ret].indexValue;

				}
					
			}

		}

		return ret;

	}

	/// <summary>
	/// Sets a new friction to WheelCollider.
	/// </summary>
	/// <returns>The friction curves.</returns>
	/// <param name="curve">Curve.</param>
	/// <param name="extremumSlip">Extremum slip.</param>
	/// <param name="extremumValue">Extremum value.</param>
	/// <param name="asymptoteSlip">Asymptote slip.</param>
	/// <param name="asymptoteValue">Asymptote value.</param>
	public WheelFrictionCurve SetFrictionCurvesWheel(WheelFrictionCurve curve, float extremumSlip, float extremumValue, float asymptoteSlip, float asymptoteValue){

		WheelFrictionCurve newCurve = curve;

		newCurve.extremumSlip = extremumSlip;
		newCurve.extremumValue = extremumValue;
		newCurve.asymptoteSlip = asymptoteSlip;
		newCurve.asymptoteValue = asymptoteValue;

		return newCurve;

	}

	private void OnDisable(){

		RCC_SceneManager.OnBehaviorChanged -= CheckBehaviorWheel;

	}

	/// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	private void OnDrawGizmos(){

		#if UNITY_EDITOR
		if(Application.isPlaying){

			WheelHit hit;
			wheelColliderObject.GetGroundHit (out hit);

			// Drawing gizmos for wheel forces and slips.
			float extension = (-wheelColliderObject.transform.InverseTransformPoint(hit.point).y - (wheelColliderObject.radius * transform.lossyScale.y)) / wheelColliderObject.suspensionDistance;
			Debug.DrawLine(hit.point, hit.point + transform.up * (hit.force / rigidbd.mass), extension <= 0.0 ? Color.magenta : Color.white);
			Debug.DrawLine(hit.point, hit.point - transform.forward * hit.forwardSlip * 2f, Color.green);
			Debug.DrawLine(hit.point, hit.point - transform.right * hit.sidewaysSlip * 2f, Color.red);

		}
		#endif

	}

}