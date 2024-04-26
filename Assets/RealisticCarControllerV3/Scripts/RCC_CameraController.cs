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
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// Main RCC Camera controller. Includes 6 different camera modes with many customizable settings. It doesn't use different cameras on your scene like *other* assets. Simply it parents the camera to their positions that's all. No need to be Einstein.
/// Also supports collision detection.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Camera/RCC Camera")]
public class RCC_CameraController : MonoBehaviour{

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

	// Currently rendering?
	[FormerlySerializedAs("isRendering")] public bool isRenderingFlag = true;

	// The target we are following transform and rigidbody.
	[FormerlySerializedAs("playerCar")] public RCC_CarMainControllerV3 playerCarController;
	private Rigidbody playerRigidbody;
	private float playerSpeedValue = 0f;
	private Vector3 playerVelocityVector = new Vector3 (0f, 0f, 0f);

	[FormerlySerializedAs("thisCam")] public Camera thisCamera;			// Camera is not attached to this main gameobject. Camera is parented to pivot gameobject. Therefore, we can apply additional position and rotation changes.
	[FormerlySerializedAs("pivot")] public GameObject pivotObject;		// Pivot center of the camera. Used for making offsets and collision movements.

	// Camera Modes.
	[FormerlySerializedAs("cameraMode")] public CameraMode cameraModeValue;
	public enum CameraMode{TPS, FPS, WHEEL, FIXED, CINEMATIC, TOP}
	[FormerlySerializedAs("lastCameraMode")] public CameraMode lastCameraModeValue;

	[FormerlySerializedAs("useTopCameraMode")] public bool useTopCameraModeFlag = false;				// Shall we use top camera mode?
	[FormerlySerializedAs("useHoodCameraMode")] public bool useHoodCameraModeFlag = true;				// Shall we use hood camera mode?
	[FormerlySerializedAs("useOrbitInTPSCameraMode")] public bool useOrbitInTPSCameraModeFlag = true;		// Shall we use orbit control in TPS camera mode?
	[FormerlySerializedAs("useOrbitInHoodCameraMode")] public bool useOrbitInHoodCameraModeFlag = true;	// Shall we use orbit control in hood camera mode?
	[FormerlySerializedAs("useWheelCameraMode")] public bool useWheelCameraModeFlag = true;			// Shall we use wheel camera mode?
	[FormerlySerializedAs("useFixedCameraMode")] public bool useFixedCameraModeFlag = true;				// Shall we use fixed camera mode?
	[FormerlySerializedAs("useCinematicCameraMode")] public bool useCinematicCameraModeFlag = true;		// Shall we use cinematic camera mode?
	[FormerlySerializedAs("useOrthoForTopCamera")] public bool useOrthoForTopCameraFlag = false;			// Shall we use ortho in top camera mode?
	[FormerlySerializedAs("useOcclusion")] public bool useOcclusionFlag = true;							// Shall we use camera occlusion?
	[FormerlySerializedAs("occlusionLayerMask")] public LayerMask occlusionLayerMaskValue = -1;

	[FormerlySerializedAs("useAutoChangeCamera")] public bool useAutoChangeCameraFlag = false;			// Shall we change camera mode by auto?
	private float autoChangeCameraTimerValue = 0f;

	[FormerlySerializedAs("topCameraAngle")] public Vector3 topCameraAngleVector = new Vector3(45f, 45f, 0f);		// If so, we will use this Vector3 angle for top camera mode.

	private float distanceOffsetValue = 0f;	
	[FormerlySerializedAs("maximumZDistanceOffset")] public float maximumZDistanceOffsetValue = 10f;		// Distance offset for top camera mode. Related with vehicle speed. If vehicle speed is higher, camera will move to front of the vehicle.
	[FormerlySerializedAs("topCameraDistance")] public float topCameraDistanceValue = 100f;				// Top camera height / distance.

	// Target position.
	private Vector3 targetPositionVector = Vector3.zero;

	// Used for resetting orbit values when direction of the vehicle has been changed.
	private int directionValue = 1;
	private int lastDirectionValue = 1;

	[FormerlySerializedAs("TPSDistance")] public float TPSDistanceValue = 6f;										// The distance for TPS camera mode.
	[FormerlySerializedAs("TPSHeight")] public float TPSHeightValue = 2f;											// The height we want the camera to be above the target for TPS camera mode.
	[FormerlySerializedAs("TPSHeightDamping")] public float TPSHeightDampingValue = 10f;							// Height movement damper.
	[FormerlySerializedAs("TPSRotationDamping")] public float TPSRotationDampingValue = 5f;							// Rotation movement damper.
	[FormerlySerializedAs("TPSTiltMaximum")] public float TPSTiltMaximumValue = 15f;								// Maximum tilt angle related with rigidbody local velocity.
	[FormerlySerializedAs("TPSTiltMultiplier")] public float TPSTiltMultiplierValue = 2f;								// Tilt angle multiplier.
	private float TPSTiltAngleValue = 0f;									// Current tilt angle.
	[FormerlySerializedAs("TPSYawAngle")] public float TPSYawAngleValue = 0f;									// Yaw angle.
	[FormerlySerializedAs("TPSPitchAngle")] public float TPSPitchAngleValue = 7f;									// Pitch angle.
	[FormerlySerializedAs("TPSOffsetX")] public float TPSOffsetXValue = 0f;										// Offset X.
	[FormerlySerializedAs("TPSOffsetY")] public float TPSOffsetYValue = .5f;										// Offset Y.
	[FormerlySerializedAs("TPSAutoFocus")] public bool TPSAutoFocusFlag = true;								// Auto focus to player vehicle.
	[FormerlySerializedAs("TPSAutoReverse")] public bool TPSAutoReverseFlag = true;								// Auto reverse when player vehicle is at reverse gear.
	[FormerlySerializedAs("TPSCollision")] public bool TPSCollisionFlag = true;									// Collision effect when player vehicle crashes.
	[FormerlySerializedAs("TPSOffset")] public Vector3 TPSOffsetVector = new Vector3(0f, 0f, .25f);   // Camera will look at front wheel distance.
	[FormerlySerializedAs("TPSStartRotation")] public Vector3 TPSStartRotationVector = new Vector3(0f, 0f, 0f);   // Camera will look at front wheel distance.

	internal float targetFieldOfViewValue = 60f;	// Camera will adapt its field of view to this target field of view. All field of views below this line will feed this value.

	[FormerlySerializedAs("TPSMinimumFOV")] public float TPSMinimumFOVValue = 50f;			// Minimum field of view related with vehicle speed.
	[FormerlySerializedAs("TPSMaximumFOV")] public float TPSMaximumFOVValue = 70f;			// Maximum field of view related with vehicle speed.
	[FormerlySerializedAs("hoodCameraFOV")] public float hoodCameraFOVValue = 60f;			// Hood field of view.
	[FormerlySerializedAs("wheelCameraFOV")] public float wheelCameraFOVValue = 60f;			// Wheel field of view.
	[FormerlySerializedAs("minimumOrtSize")] public float minimumOrtSizeValue = 10f;			// Minimum ortho size related with vehicle speed.
	[FormerlySerializedAs("maximumOrtSize")] public float maximumOrtSizeValue = 20f;			// Maximum ortho size related with vehicle speed.

	internal int cameraSwitchCountValue = 0;					// Used in switch case for running corresponding camera mode method.
	private RCC_HoodCameraBehaviour hoodCamera;					// Hood camera. It's a null script. Just used for finding hood camera parented to our player vehicle.
	private RCC_WheelCameraController wheelCamera;				// Wheel camera. It's a null script. Just used for finding wheel camera parented to our player vehicle.
	private RCC_FixedCamera fixedCamera;					// Fixed Camera System.
	private RCC_CinematicCamera cinematicCamera;		// Cinematic Camera System.

	private Vector3 collisionVectorValue = Vector3.zero;				// Collision vector.
	private Vector3 collisionPosValue = Vector3.zero;					// Collision position.
	private Quaternion collisionRotValue = Quaternion.identity;	// Collision rotation.

	private Quaternion orbitRotationQuaternion = Quaternion.identity;		// Orbit rotation.

	// Orbit X and Y inputs.
	internal float orbitXValue = 0f;
	internal float orbitYValue = 0f;

	// Minimum and maximum Orbit X, Y degrees.
	[FormerlySerializedAs("minOrbitY")] public float minOrbitYValue = -20f;
	[FormerlySerializedAs("maxOrbitY")] public float maxOrbitYValue = 80f;

	//	Orbit X and Y speeds.
	[FormerlySerializedAs("orbitXSpeed")] public float orbitXSpeedValue = 50f;
	[FormerlySerializedAs("orbitYSpeed")] public float orbitYSpeedValue = 50f;
	[FormerlySerializedAs("orbitSmooth")] public float orbitSmoothValue = 10f;

	//	Resetting orbits.
	[FormerlySerializedAs("orbitReset")] public bool orbitResetFlag = false;
	private float orbitResetTimerValue = 0f;
	private float oldOrbitXValue = 0f;
	[FormerlySerializedAs("oldOrbitY")] public float oldOrbitYValue = 0f;

	// Calculate the current rotation angles for TPS mode.
	private Quaternion currentRotationQuaternion = Quaternion.identity;
	private Quaternion wantedRotationQuaternion = Quaternion.identity;
	private float currentHeightValue = 0f;
	private float wantedHeightValue = 0f;

	public delegate void onBCGCameraSpawnedDelegate(GameObject BCGCamera);
	public static event onBCGCameraSpawnedDelegate OnBCGCameraSpawnedEvent;

	private void Awake(){
		
		// Getting Camera.
		thisCamera = GetComponentInChildren<Camera>();

		GameObject dir = new GameObject ("Camera Direction");
		dir.transform.SetParent (thisCamera.transform, false);
		dir.transform.localPosition = new Vector3 (0f, 0f, 10f);
		dir.transform.localRotation = Quaternion.identity;

	}

	private void OnEnable(){

		// Calling this event when BCG Camera spawned.
		if(OnBCGCameraSpawnedEvent != null)
			OnBCGCameraSpawnedEvent (gameObject);

		// Listening player vehicle collisions for crashing effects.
		RCC_CarMainControllerV3.OnRCCPlayerCollision += RCC_CarControllerV3_OnRCCPlayerCarCollision;

	}

	private void RCC_CarControllerV3_OnRCCPlayerCarCollision (RCC_CarMainControllerV3 RCC, Collision collision){

		CollisionCar (collision);
		
	}

	private void GetTargetObject(){

		// Return if we don't have the player vehicle.
		if(!playerCarController)
			return;

		if(TPSAutoFocusFlag)
			StartCoroutine(AutoFocusCoroutine ());

		// Getting rigid of the player vehicle.
		playerRigidbody = playerCarController.GetComponent<Rigidbody>();

		// Getting camera modes from the player vehicle.
		hoodCamera = playerCarController.GetComponentInChildren<RCC_HoodCameraBehaviour>();
		wheelCamera = playerCarController.GetComponentInChildren<RCC_WheelCameraController>();
		fixedCamera = GameObject.FindObjectOfType<RCC_FixedCamera>();
		cinematicCamera = GameObject.FindObjectOfType<RCC_CinematicCamera>();

		ResetCameraData ();

		// Setting transform and position to player vehicle when switched camera target.
//		transform.position = playerCar.transform.position;
//		transform.rotation = playerCar.transform.rotation * Quaternion.AngleAxis(10f, Vector3.right);

	}

	public void SetTargetObject(GameObject player){

		playerCarController = player.GetComponent<RCC_CarMainControllerV3>();
		GetTargetObject ();

	}

	public void RemoveTargetObject(){

		transform.SetParent (null);
		playerCarController = null;
		playerRigidbody = null;

	}

	private void Update(){

		// If it's active, enable the camera. If it's not, disable the camera.
		if (!isRenderingFlag) {

			if(thisCamera.gameObject.activeInHierarchy)
				thisCamera.gameObject.SetActive (false);

			return;

		} else {

			if(!thisCamera.gameObject.activeInHierarchy)
				thisCamera.gameObject.SetActive (true);

		}

		// Early out if we don't have the player vehicle.
		if (!playerCarController || !playerRigidbody){

			GetTargetObject();
			return;

		}

		GetInputs ();

		// Speed of the vehicle (smoothed).
		playerSpeedValue = Mathf.Lerp(playerSpeedValue, playerCarController.speed, Time.deltaTime * 5f);

		// Velocity of the vehicle.
		playerVelocityVector = playerCarController.transform.InverseTransformDirection(playerRigidbody.velocity);

		// Lerping current field of view to target field of view.
		thisCamera.fieldOfView = Mathf.Lerp (thisCamera.fieldOfView, targetFieldOfViewValue, Time.deltaTime * 5f);

	}

	private void LateUpdate (){

		// Early out if we don't have the player vehicle.
		if (!playerCarController || !playerRigidbody)
			return;

		// Even if we have the player vehicle and it's disabled, return.
		if (!playerCarController.gameObject.activeSelf)
			return;

		// Run the corresponding method with choosen camera mode.
		switch(cameraModeValue){

		case CameraMode.TPS:
			TPSMode();
			if (useOrbitInTPSCameraModeFlag)
				ORBITMode ();
			break;

		case CameraMode.FPS:
			FPSMode ();
			if (useOrbitInHoodCameraModeFlag)
				ORBITMode ();
			break;

		case CameraMode.WHEEL:
			WHEELMode();
			break;

		case CameraMode.FIXED:
			FIXEDMode();
			break;

		case CameraMode.CINEMATIC:
			CINEMATICMode();
			break;

		case CameraMode.TOP:
			TOPMode();
			break;

		}

		if (lastCameraModeValue != cameraModeValue)
			ResetCameraData ();

		lastCameraModeValue = cameraModeValue;
		autoChangeCameraTimerValue += Time.deltaTime;

		if (useAutoChangeCameraFlag && autoChangeCameraTimerValue > 10) {

			autoChangeCameraTimerValue = 0f;
			ChangeCameraObject ();

		}

	}

	private void GetInputs(){
		
		switch(RCCSettingsInstance.selectedControllerTypeR){

		case RCC_SettingsData.ControllerType.Keyboard:

			if (Input.GetKeyDown (RCCSettingsInstance.changeCameraKBCode))
				ChangeCameraObject ();

			orbitXValue += Input.GetAxis (RCCSettingsInstance.mouseXInputValue) * orbitXSpeedValue * .02f;
			orbitYValue -= Input.GetAxis (RCCSettingsInstance.mouseYInputValue) * orbitYSpeedValue * .02f;

			break;

		case RCC_SettingsData.ControllerType.XBox360One:

			if (Input.GetButtonDown (RCCSettingsInstance.Xbox_changeCameraKBValue))
				ChangeCameraObject ();

			orbitXValue += Input.GetAxis (RCCSettingsInstance.Xbox_mouseXInputValue) * orbitXSpeedValue * .01f;
			orbitYValue -= Input.GetAxis (RCCSettingsInstance.Xbox_mouseYInputValue) * orbitYSpeedValue * .01f;

			break;

		case RCC_SettingsData.ControllerType.PS4:

			if (Input.GetButtonDown (RCCSettingsInstance.PS4_changeCameraKBValue))
				ChangeCameraObject ();

			orbitXValue += Input.GetAxis (RCCSettingsInstance.PS4_mouseXInputValue) * orbitXSpeedValue * .01f;
			orbitYValue -= Input.GetAxis (RCCSettingsInstance.PS4_mouseYInputValue) * orbitYSpeedValue * .01f;

			break;

		case RCC_SettingsData.ControllerType.LogitechSteeringWheel:
			
			#if BCG_LOGITECH
			if (RCC_LogitechSteeringWheel.GetKeyTriggered (0, RCCSettings.LogiSteeringWheel_changeCameraKB))
				ChangeCamera ();
			#endif

			break;

		}

	}

	// Change camera by increasing camera switch counter.
	public void ChangeCameraObject(){

		// Increasing camera switch counter at each camera changing.
		cameraSwitchCountValue ++;

		// We have 6 camera modes at total. If camera switch counter is greater than maximum, set it to 0.
		if (cameraSwitchCountValue >= 6)
			cameraSwitchCountValue = 0;

		switch(cameraSwitchCountValue){

		case 0:
			cameraModeValue = CameraMode.TPS;
			break;

		case 1:
			if(useHoodCameraModeFlag && hoodCamera){
				cameraModeValue = CameraMode.FPS;
			}else{
				ChangeCameraObject();
			}
			break;

		case 2:
			if(useWheelCameraModeFlag && wheelCamera){
				cameraModeValue = CameraMode.WHEEL;
			}else{
				ChangeCameraObject();
			}
			break;

		case 3:
			if(useFixedCameraModeFlag && fixedCamera){
				cameraModeValue = CameraMode.FIXED;
			}else{
				ChangeCameraObject();
			}
			break;

		case 4:
			if(useCinematicCameraModeFlag && cinematicCamera){
				cameraModeValue = CameraMode.CINEMATIC;
			}else{
				ChangeCameraObject();
			}
			break;

		case 5:
			if(useTopCameraModeFlag){
				cameraModeValue = CameraMode.TOP;
			}else{
				ChangeCameraObject();
			}
			break;

		}

	}

	// Change camera by directly setting it to specific mode.
	public void ChangeCameraObject(CameraMode mode){

		cameraModeValue = mode;

	}

	private void FPSMode(){

		// Assigning orbit rotation, and transform rotation.
		transform.rotation = Quaternion.Lerp(transform.rotation, playerCarController.transform.rotation * orbitRotationQuaternion, Time.deltaTime * 50f);

	}

	private void WHEELMode(){

		if (useOcclusionFlag && OccludingMode (playerCarController.transform.position))
			ChangeCameraObject (CameraMode.TPS);

	}

	private void TPSMode(){

		if (lastDirectionValue != playerCarController.direction) {

			directionValue = playerCarController.direction;
			orbitXValue = 0f;
			orbitYValue = 0f;

		}

		lastDirectionValue = playerCarController.direction;

		// Calculate the current rotation angles for TPS mode.
		if(TPSAutoReverseFlag)
			wantedRotationQuaternion = playerCarController.transform.rotation * Quaternion.AngleAxis ((directionValue == 1 ? 0 : 180), Vector3.up);
		else
			wantedRotationQuaternion = playerCarController.transform.rotation;

		switch (RCCSettingsInstance.selectedControllerTypeR){

		case RCC_SettingsData.ControllerType.Keyboard:

			if(Input.GetKey(RCCSettingsInstance.lookBackKBCode))
				wantedRotationQuaternion = wantedRotationQuaternion * Quaternion.AngleAxis ((180), Vector3.up);

			break;

		case RCC_SettingsData.ControllerType.XBox360One:

			if(Input.GetButton(RCCSettingsInstance.Xbox_lookBackKBValue))
				wantedRotationQuaternion = wantedRotationQuaternion * Quaternion.AngleAxis ((180), Vector3.up);

			break;

		case RCC_SettingsData.ControllerType.LogitechSteeringWheel:

			#if BCG_LOGITECH
			if(RCC_LogitechSteeringWheel.GetKeyPressed(0, RCCSettings.LogiSteeringWheel_lookBackKB))
				wantedRotation = wantedRotation * Quaternion.AngleAxis ((180), Vector3.up);
			#endif

			break;

		}

		// Wanted height.
		wantedHeightValue = playerCarController.transform.position.y + TPSHeightValue + TPSOffsetYValue;

		// Damp the height.
		currentHeightValue = Mathf.Lerp (currentHeightValue, wantedHeightValue, TPSHeightDampingValue * Time.fixedDeltaTime);

		// Damp the rotation around the y-axis.
		if(Time.time > 1)
			currentRotationQuaternion = Quaternion.Lerp(currentRotationQuaternion, wantedRotationQuaternion, 1f - Mathf.Exp(-TPSRotationDampingValue * Time.deltaTime));
//			currentRotation = Quaternion.Lerp(currentRotation, wantedRotation, TPSRotationDamping * Time.deltaTime);
		else
			currentRotationQuaternion = wantedRotationQuaternion;

		// Rotates camera by Z axis for tilt effect.
		TPSTiltAngleValue = Mathf.Lerp(0f, TPSTiltMaximumValue * Mathf.Clamp(-playerVelocityVector.x, -1f, 1f), Mathf.Abs(playerVelocityVector.x) / 50f);
		TPSTiltAngleValue *= TPSTiltMultiplierValue;

		// Set the position of the camera on the x-z plane to distance meters behind the target.
		targetPositionVector = playerCarController.transform.position;
		targetPositionVector -= (currentRotationQuaternion * orbitRotationQuaternion) * Vector3.forward * TPSDistanceValue;
		targetPositionVector += Vector3.up * (TPSHeightValue * Mathf.Lerp(1f, .75f, (playerRigidbody.velocity.magnitude * 3.6f) / 100f));

		transform.position = targetPositionVector;

//		thisCam.transform.localPosition = Vector3.Lerp(thisCam.transform.localPosition, new Vector3 (TPSTiltAngle / 10f, 0f, 0f), Time.deltaTime * 3f);

		// Always look at the target.
		Vector3 lookAtPosition = playerCarController.transform.position;

		if (TPSOffsetVector != Vector3.zero)
			lookAtPosition += playerCarController.transform.rotation * TPSOffsetVector;

		transform.LookAt (lookAtPosition);

		transform.position = transform.position + (transform.right * TPSOffsetXValue) + (transform.up * TPSOffsetYValue);
		transform.rotation *= Quaternion.Euler (TPSPitchAngleValue * Mathf.Lerp(1f, .75f, (playerSpeedValue) / 100f), 0, Mathf.Clamp(-TPSTiltAngleValue, -TPSTiltMaximumValue, TPSTiltMaximumValue) + TPSYawAngleValue);

		// Collision positions and rotations that affects pivot of the camera.
		collisionPosValue = Vector3.Lerp(new Vector3(collisionPosValue.x, collisionPosValue.y, collisionPosValue.z), Vector3.zero, Time.deltaTime * 5f);

		if(Time.deltaTime != 0)
			collisionRotValue = Quaternion.Lerp(collisionRotValue, Quaternion.identity, Time.deltaTime * 5f);

		// Lerping position and rotation of the pivot to collision.
		pivotObject.transform.localPosition = Vector3.Lerp(pivotObject.transform.localPosition, collisionPosValue, Time.deltaTime * 10f);
		pivotObject.transform.localRotation = Quaternion.Lerp(pivotObject.transform.localRotation, collisionRotValue, Time.deltaTime * 10f);

		// Lerping targetFieldOfView from TPSMinimumFOV to TPSMaximumFOV related with vehicle speed.
		targetFieldOfViewValue = Mathf.Lerp(TPSMinimumFOVValue, TPSMaximumFOVValue, Mathf.Abs(playerSpeedValue) / 150f);

		if(useOcclusionFlag)
			OccludeRaycast (playerCarController.transform.position);

	}

	void FIXEDMode(){

		if(fixedCamera.transform.parent != null)
			fixedCamera.transform.SetParent(null);

		if (useOcclusionFlag && OccludingMode (playerCarController.transform.position))
			fixedCamera.ChangePosition ();

	}

	private void TOPMode(){

		// Early out if we don't have the player vehicle.
		if (!playerCarController || !playerRigidbody)
			return;

		// Setting ortho or perspective?
		thisCamera.orthographic = useOrthoForTopCameraFlag;

		distanceOffsetValue = Mathf.Lerp (0f, maximumZDistanceOffsetValue, Mathf.Abs(playerSpeedValue) / 100f);
		targetFieldOfViewValue = Mathf.Lerp (minimumOrtSizeValue, maximumOrtSizeValue, Mathf.Abs(playerSpeedValue) / 100f);
		thisCamera.orthographicSize = targetFieldOfViewValue;

		// Setting proper targetPosition for top camera mode.
		targetPositionVector = playerCarController.transform.position;
		targetPositionVector += playerCarController.transform.rotation * Vector3.forward * distanceOffsetValue;

		// Assigning position and rotation.
		transform.position = targetPositionVector;
		transform.rotation = Quaternion.Euler (topCameraAngleVector);

		// Pivot position.
		pivotObject.transform.localPosition = new Vector3 (0f, 0f, -topCameraDistanceValue);

	}

	private void ORBITMode(){

		// Clamping Y.
		orbitYValue = Mathf.Clamp(orbitYValue, minOrbitYValue, maxOrbitYValue);

		if (orbitXValue < -360f)
			orbitXValue += 360f;
		if (orbitXValue > 360f)
			orbitXValue -= 360f;

		orbitRotationQuaternion = Quaternion.Lerp(orbitRotationQuaternion, Quaternion.Euler (orbitYValue, orbitXValue, 0f), orbitSmoothValue * Time.deltaTime);

		if (oldOrbitXValue != orbitXValue) {

			oldOrbitXValue = orbitXValue;
			orbitResetTimerValue = 2f;

		}

		if (oldOrbitYValue != orbitYValue) {

			oldOrbitYValue = orbitYValue;
			orbitResetTimerValue = 2f;

		}

		if(orbitResetTimerValue > 0)
			orbitResetTimerValue -= Time.deltaTime;

		Mathf.Clamp (orbitResetTimerValue, 0f, 2f);

		if (orbitResetFlag && playerSpeedValue >= 25f && orbitResetTimerValue <= 0f) {

			orbitXValue = 0f;
			orbitYValue = 0f;

		}

	}

	public void OnDrag(PointerEventData pointerData){

		// Receiving drag input from UI.
		orbitXValue += pointerData.delta.x * orbitXSpeedValue / 1000f;
		orbitYValue -= pointerData.delta.y * orbitYSpeedValue / 1000f;

		orbitResetTimerValue = 0f;

	}

	public void OnDrag(float x, float y){

		// Receiving drag input from UI.
		orbitXValue += x * orbitXSpeedValue / 10f;
		orbitYValue -= y * orbitYSpeedValue / 10f;

		orbitResetTimerValue = 0f;

	}

	private void CINEMATICMode(){

		if(cinematicCamera.transform.parent != null)
			cinematicCamera.transform.SetParent(null);

		targetFieldOfViewValue = cinematicCamera.targetFOV;

		if (useOcclusionFlag && OccludingMode (playerCarController.transform.position))
			ChangeCameraObject (CameraMode.TPS);

	}

	public void CollisionCar(Collision collision){

		// If it's not enable or camera mode is TPS, return.
		if(!enabled || !isRenderingFlag || cameraModeValue != CameraMode.TPS || !TPSCollisionFlag)
			return;

		// Local relative velocity.
		Vector3 colRelVel = collision.relativeVelocity;
		colRelVel *= 1f - Mathf.Abs(Vector3.Dot(transform.up, collision.contacts[0].normal));

		float cos = Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, colRelVel.normalized));

		if (colRelVel.magnitude * cos >= 5f){

			collisionVectorValue = transform.InverseTransformDirection(colRelVel) / (30f);

			collisionPosValue -= collisionVectorValue * 5f;
			collisionRotValue = Quaternion.Euler(new Vector3(-collisionVectorValue.z * 10f, -collisionVectorValue.y * 10f, -collisionVectorValue.x * 10f));
			targetFieldOfViewValue = thisCamera.fieldOfView - Mathf.Clamp(collision.relativeVelocity.magnitude, 0f, 15f);

		}

	}

	private void ResetCameraData(){
		
		if(fixedCamera)
			fixedCamera.canTrackNow = false;

		TPSTiltAngleValue = 0f;

		collisionPosValue = Vector3.zero;
		collisionRotValue = Quaternion.identity;

		thisCamera.transform.localPosition = Vector3.zero;
		thisCamera.transform.localRotation = Quaternion.identity;

		pivotObject.transform.localPosition = collisionPosValue;
		pivotObject.transform.localRotation = collisionRotValue;

		orbitXValue = TPSStartRotationVector.y;
		orbitYValue = TPSStartRotationVector.x;

		if (TPSStartRotationVector != Vector3.zero)
			TPSStartRotationVector = Vector3.zero;

		thisCamera.orthographic = false;

		switch (cameraModeValue) {

		case CameraMode.TPS:
			transform.SetParent(null);
			targetFieldOfViewValue = TPSMinimumFOVValue;
			break;

		case CameraMode.FPS:
			transform.SetParent (hoodCamera.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			targetFieldOfViewValue = hoodCameraFOVValue;
			hoodCamera.LaunchFixShake ();
			break;

		case CameraMode.WHEEL:
			transform.SetParent(wheelCamera.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			targetFieldOfViewValue = wheelCameraFOVValue;
			break;

		case CameraMode.FIXED:
			transform.SetParent(fixedCamera.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			targetFieldOfViewValue = 60;
			fixedCamera.canTrackNow = true;
			break;

		case CameraMode.CINEMATIC:
			transform.SetParent(cinematicCamera.pivot.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			targetFieldOfViewValue = 30f;
			break;

		case CameraMode.TOP:
			transform.SetParent(null);
			targetFieldOfViewValue = minimumOrtSizeValue;
			pivotObject.transform.localPosition = Vector3.zero;
			pivotObject.transform.localRotation = Quaternion.identity;
			targetPositionVector = playerCarController.transform.position;
			targetPositionVector += playerCarController.transform.rotation * Vector3.forward * distanceOffsetValue;
			transform.position = playerCarController.transform.position;
			break;

		}

	}

	public void ToggleCameraState(bool state){

		// Enabling / disabling activity.
		isRenderingFlag = state;

	}

	private void OccludeRaycast(Vector3 targetFollow){

		//declare a new raycast hit.
		RaycastHit wallHit = new RaycastHit();

		if (Physics.Linecast (targetFollow, transform.position, out wallHit, occlusionLayerMaskValue)) {

			if (!wallHit.collider.isTrigger && !wallHit.transform.IsChildOf (playerCarController.transform)) {

				//the x and z coordinates are pushed away from the wall by hit.normal.
				//the y coordinate stays the same.
				Vector3 occludedPosition = new Vector3 (wallHit.point.x + wallHit.normal.x * .2f, wallHit.point.y + wallHit.normal.y * .2f, wallHit.point.z + wallHit.normal.z * .2f);

				transform.position = occludedPosition;

			}

		}

	}

	private bool OccludingMode(Vector3 targetFollow){

		//declare a new raycast hit.
		RaycastHit wallHit = new RaycastHit();

		//linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
		if (Physics.Linecast (targetFollow, transform.position, out wallHit, ~(1 << LayerMask.NameToLayer(RCCSettingsInstance.RCCLayerValue)))) {

			if (!wallHit.collider.isTrigger && !wallHit.transform.IsChildOf (playerCarController.transform))
				return true;

		}

		return false;

	}

	public IEnumerator AutoFocusCoroutine(){

		float timer = 3f;

		while (timer > 0f) {
			
			timer -= Time.deltaTime;
			TPSDistanceValue = Mathf.Lerp(TPSDistanceValue, RCC_GetBoundsSize.MaxBoundsExtentValue (playerCarController.transform) * 2.55f, Time.deltaTime);
			TPSHeightValue = Mathf.Lerp (TPSHeightValue, RCC_GetBoundsSize.MaxBoundsExtentValue (playerCarController.transform) * .75f, Time.deltaTime);
			yield return null;

		}

	}

	public IEnumerator AutoFocusCoroutine(Transform bounds){

		float timer = 3f;

		while (timer > 0f) {

			timer -= Time.deltaTime;
			TPSDistanceValue = Mathf.Lerp(TPSDistanceValue, RCC_GetBoundsSize.MaxBoundsExtentValue (bounds) * 2.55f, Time.deltaTime);
			TPSHeightValue = Mathf.Lerp(TPSHeightValue, RCC_GetBoundsSize.MaxBoundsExtentValue (bounds) * .75f, Time.deltaTime);
			yield return null;

		}

	}

	public IEnumerator AutoFocusCoroutine(Transform bounds1, Transform bounds2){

		float timer = 3f;

		while (timer > 0f) {

			timer -= Time.deltaTime;
			TPSDistanceValue = Mathf.Lerp(TPSDistanceValue, ((RCC_GetBoundsSize.MaxBoundsExtentValue (bounds1) * 2.55f) + (RCC_GetBoundsSize.MaxBoundsExtentValue (bounds2) * 2.75f)), Time.deltaTime);
			TPSHeightValue = Mathf.Lerp(TPSHeightValue, ((RCC_GetBoundsSize.MaxBoundsExtentValue (bounds1) * .75f) + (RCC_GetBoundsSize.MaxBoundsExtentValue (bounds2) * .6f)), Time.deltaTime);
			yield return null;

		}

	}

	public IEnumerator AutoFocusCoroutine(Transform bounds1, Transform bounds2, Transform bounds3){

		float timer = 5f;

		while (timer > 0f) {

			timer -= Time.deltaTime;
			TPSDistanceValue = Mathf.Lerp(TPSDistanceValue, ((RCC_GetBoundsSize.MaxBoundsExtentValue (bounds1) * 2.75f) + (RCC_GetBoundsSize.MaxBoundsExtentValue (bounds2) * 2.75f) + (RCC_GetBoundsSize.MaxBoundsExtentValue (bounds3) * 2.75f)), Time.deltaTime);
			TPSHeightValue = Mathf.Lerp(TPSHeightValue, ((RCC_GetBoundsSize.MaxBoundsExtentValue (bounds1) * .6f) + (RCC_GetBoundsSize.MaxBoundsExtentValue (bounds2) * .6f) + (RCC_GetBoundsSize.MaxBoundsExtentValue (bounds3) * .6f)), Time.deltaTime);
			yield return null;

		}

	}

	private void OnDisable(){

		RCC_CarMainControllerV3.OnRCCPlayerCollision -= RCC_CarControllerV3_OnRCCPlayerCarCollision;

	}

}