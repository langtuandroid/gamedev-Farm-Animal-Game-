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

/// <summary>
/// Scene manager that contains current player vehicle, current player camera, current player UI, current player character, recording/playing mechanim, and other vehicles as well.
/// 
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Scene Manager")]
public class RCC_SceneManager : MonoBehaviour {

	#region singleton
	private static RCC_SceneManager instance;
	public static RCC_SceneManager Instance{
		
		get{
			
			if (instance == null) {

				instance = FindObjectOfType<RCC_SceneManager> ();

				if (instance == null) {
					
					GameObject sceneManager = new GameObject ("_RCCSceneManager");
					instance = sceneManager.AddComponent<RCC_SceneManager> ();

				}

			}
			
			return instance;

		}

	}

	#endregion

	public RCC_CarMainControllerV3 activePlayerVehicle;
	private RCC_CarMainControllerV3 lastActivePlayerVehicle;
	public RCC_CameraController activePlayerCamera;
	public RCC_UIDashboardDisplayController activePlayerCanvas;
	public Camera activeMainCamera;

	public bool registerFirstVehicleAsPlayer = true;
	public bool disableUIWhenNoPlayerVehicle = false;
	public bool loadCustomizationAtFirst = true;

	public List <RCC_RecorderController> allRecorders = new List<RCC_RecorderController> ();
	public enum RecordMode{Neutral, Play, Record}
	public RecordMode recordMode;

	// Default time scale of the game.
	private float orgTimeScale = 1f;

	public List <RCC_CarMainControllerV3> allVehicles = new List<RCC_CarMainControllerV3> ();

	#if BCG_ENTEREXIT
	public BCG_EnterExitPlayer activePlayerCharacter;
	#endif

	// Firing an event when main controller changed.
	public delegate void onControllerChanged();
	public static event onControllerChanged OnControllerChanged;

	// Firing an event when main behavior changed.
	public delegate void onBehaviorChanged();
	public static event onBehaviorChanged OnBehaviorChanged;

	// Firing an event when player vehicle changed.
	public delegate void onVehicleChanged();
	public static event onVehicleChanged OnVehicleChanged;

	void Awake(){

		if(RCC_SettingsData.InstanceR.overrideFPSFlag)
			Application.targetFrameRate = RCC_SettingsData.InstanceR.maxFPSValue;

		if (RCC_SettingsData.InstanceR.useTelemetryObject)
			GameObject.Instantiate (RCC_SettingsData.InstanceR.RCCTelemetryObject, Vector3.zero, Quaternion.identity);

		RCC_CameraController.OnBCGCameraSpawnedEvent += RCC_Camera_OnBCGCameraSpawned;

		RCC_CarMainControllerV3.OnRCCPlayerSpawned += RCC_CarControllerV3_OnRCCSpawned;
		RCC_AICarMovementController.OnRCCAISpawnedEvent += RCC_AICarController_OnRCCAISpawned;
		RCC_CarMainControllerV3.OnRCCPlayerDestroyed += RCC_CarControllerV3_OnRCCPlayerDestroyed;
		RCC_AICarMovementController.OnRCCAIDestroyedEvent += RCC_AICarController_OnRCCAIDestroyed;
		activePlayerCanvas = GameObject.FindObjectOfType<RCC_UIDashboardDisplayController> ();

		#if BCG_ENTEREXIT
		BCG_EnterExitPlayer.OnBCGPlayerSpawned += BCG_EnterExitPlayer_OnBCGPlayerSpawned;
		BCG_EnterExitPlayer.OnBCGPlayerDestroyed += BCG_EnterExitPlayer_OnBCGPlayerDestroyed;
		#endif

		// Getting default time scale of the game.
		orgTimeScale = Time.timeScale;

		if(RCC_SettingsData.InstanceR.lockAndUnlockCursorFlag)
			Cursor.lockState = CursorLockMode.Locked;

		#if ENABLE_VR
		UnityEngine.XR.XRSettings.enabled = RCC_SettingsData.InstanceR.useVRFlag;
		#endif
		
	}

	#region ONSPAWNED

	void RCC_CarControllerV3_OnRCCSpawned (RCC_CarMainControllerV3 RCC){
		
		if (!allVehicles.Contains (RCC)) {
			
			allVehicles.Add (RCC);

			allRecorders = new List<RCC_RecorderController> ();
			allRecorders.AddRange (gameObject.GetComponentsInChildren<RCC_RecorderController> ());

			RCC_RecorderController recorder = null;

			if (allRecorders != null && allRecorders.Count > 0) {

				for (int i = 0; i < allRecorders.Count; i++) {
				
					if (allRecorders[i] != null && allRecorders [i].carControllerMain == RCC) {
						recorder = allRecorders [i];
						break;
					}

				}

			}

			if (recorder == null) {
				
				recorder = gameObject.AddComponent<RCC_RecorderController> ();
				recorder.carControllerMain = RCC;

			}

		}

		StartCoroutine(CheckMissingRecorders ());

		if (registerFirstVehicleAsPlayer)
			RegisterPlayer (RCC);

		#if BCG_ENTEREXIT
		if (RCC.gameObject.GetComponent<BCG_EnterExitVehicle> ())
			RCC.gameObject.GetComponent<BCG_EnterExitVehicle> ().correspondingCamera = activePlayerCamera.gameObject;
		#endif

	}

	void RCC_AICarController_OnRCCAISpawned (RCC_AICarMovementController RCCAI){
		
		if (!allVehicles.Contains (RCCAI.carMainController)) {
			
			allVehicles.Add (RCCAI.carMainController);

			allRecorders = new List<RCC_RecorderController> ();
			allRecorders.AddRange (gameObject.GetComponentsInChildren<RCC_RecorderController> ());

			RCC_RecorderController recorder = null;

			if (allRecorders != null && allRecorders.Count > 0) {

				for (int i = 0; i < allRecorders.Count; i++) {

					if (allRecorders [i] != null && allRecorders [i].carControllerMain == RCCAI.carMainController) {
						recorder = allRecorders [i];
						break;
					}

				}

			}

			if (recorder == null) {

				recorder = gameObject.AddComponent<RCC_RecorderController> ();
				recorder.carControllerMain = RCCAI.carMainController;

			}

		}

		StartCoroutine(CheckMissingRecorders ());

	}

	void RCC_Camera_OnBCGCameraSpawned (GameObject BCGCamera){

		activePlayerCamera = BCGCamera.GetComponent<RCC_CameraController>();

	}

	#if BCG_ENTEREXIT
	void BCG_EnterExitPlayer_OnBCGPlayerSpawned (BCG_EnterExitPlayer player){

		activePlayerCharacter = player;

	}
	#endif

	#endregion

	#region ONDESTROYED

	void RCC_CarControllerV3_OnRCCPlayerDestroyed (RCC_CarMainControllerV3 RCC){
		
		if (allVehicles.Contains (RCC))
			allVehicles.Remove (RCC);

		StartCoroutine(CheckMissingRecorders ());

	}

	void RCC_AICarController_OnRCCAIDestroyed (RCC_AICarMovementController RCCAI){
		
		if (allVehicles.Contains (RCCAI.carMainController))
			allVehicles.Remove (RCCAI.carMainController);

		StartCoroutine(CheckMissingRecorders ());

	}

	#if BCG_ENTEREXIT
	void BCG_EnterExitPlayer_OnBCGPlayerDestroyed (BCG_EnterExitPlayer player){

	}
	#endif

	#endregion

	void Update(){

		if (activePlayerVehicle) {

			if (activePlayerVehicle != lastActivePlayerVehicle) {
				
				if (OnVehicleChanged != null)
					OnVehicleChanged ();

			}

			lastActivePlayerVehicle = activePlayerVehicle;

		}

		if(disableUIWhenNoPlayerVehicle && activePlayerCanvas)
			CheckCanvas ();

		if (Input.GetKeyDown (RCC_SettingsData.InstanceR.recordKBCode))
			RCC_Manager.StartStopRecordPlayer ();

		if (Input.GetKeyDown (RCC_SettingsData.InstanceR.playbackKBCode))
			RCC_Manager.StartStopReplayPlayer ();

		if (Input.GetKey (RCC_SettingsData.InstanceR.slowMotionKBCode))
			Time.timeScale = .2f;

		if (Input.GetKeyUp (RCC_SettingsData.InstanceR.slowMotionKBCode))
			Time.timeScale = orgTimeScale;

		if(Input.GetButtonDown("Cancel"))
			Cursor.lockState = CursorLockMode.None;

		activeMainCamera = Camera.main;

		if (allRecorders != null && allRecorders.Count > 0) {

			switch (allRecorders [0].modeR) {

			case RCC_RecorderController.Mode.Neutral:

				recordMode = RecordMode.Neutral;

				break;

			case RCC_RecorderController.Mode.Play:

				recordMode = RecordMode.Play;

				break;

			case RCC_RecorderController.Mode.Record:

				recordMode = RecordMode.Record;

				break;

			}

		}

	}

	public void Record(){

		if (allRecorders != null && allRecorders.Count > 0) {

			for (int i = 0; i < allRecorders.Count; i++)
				allRecorders [i].RecordSwitch ();

		}

	}

	public void Play(){

		if (allRecorders != null && allRecorders.Count > 0) {

			for (int i = 0; i < allRecorders.Count; i++)
				allRecorders [i].PlayR ();

		}

	}

	public void Stop(){

		if (allRecorders != null && allRecorders.Count > 0) {

			for (int i = 0; i < allRecorders.Count; i++)
				allRecorders [i].StopR ();

		}

	}

	private IEnumerator CheckMissingRecorders(){
		
		yield return new WaitForFixedUpdate ();

		allRecorders = new List<RCC_RecorderController> ();
		allRecorders.AddRange (gameObject.GetComponentsInChildren<RCC_RecorderController> ());

		if (allRecorders != null && allRecorders.Count > 0) {

			for (int i = 0; i < allRecorders.Count; i++) {

				if (allRecorders [i].carControllerMain == null)
					Destroy (allRecorders [i]);

			}

		}
		yield return new WaitForFixedUpdate ();

		allRecorders = new List<RCC_RecorderController> ();
		allRecorders.AddRange (gameObject.GetComponentsInChildren<RCC_RecorderController> ());

	}

	public void RegisterPlayer(RCC_CarMainControllerV3 playerVehicle){

		activePlayerVehicle = playerVehicle;

		if(activePlayerCamera)
			activePlayerCamera.SetTargetObject (activePlayerVehicle.gameObject);

		if (loadCustomizationAtFirst)
			RCC_Customization.LoadStats (RCC_SceneManager.Instance.activePlayerVehicle);

		if (GameObject.FindObjectOfType<RCC_CustomizerExample> ()) 
			GameObject.FindObjectOfType<RCC_CustomizerExample> ().CheckUIs ();

	}

	public void RegisterPlayer(RCC_CarMainControllerV3 playerVehicle, bool isControllable){

		activePlayerVehicle = playerVehicle;
		activePlayerVehicle.SetCanControl(isControllable);

		if(activePlayerCamera)
			activePlayerCamera.SetTargetObject (activePlayerVehicle.gameObject);

		if (GameObject.FindObjectOfType<RCC_CustomizerExample> ()) 
			GameObject.FindObjectOfType<RCC_CustomizerExample> ().CheckUIs ();

	}

	public void RegisterPlayer(RCC_CarMainControllerV3 playerVehicle, bool isControllable, bool engineState){

		activePlayerVehicle = playerVehicle;
		activePlayerVehicle.SetCanControl(isControllable);
		activePlayerVehicle.SetEngine (engineState);

		if(activePlayerCamera)
			activePlayerCamera.SetTargetObject (activePlayerVehicle.gameObject);

		if (GameObject.FindObjectOfType<RCC_CustomizerExample> ()) 
			GameObject.FindObjectOfType<RCC_CustomizerExample> ().CheckUIs ();

	}

	public void DeRegisterPlayer(){

		if (activePlayerVehicle)
			activePlayerVehicle.SetCanControl (false);
		
		activePlayerVehicle = null;

		if (activePlayerCamera)
			activePlayerCamera.RemoveTargetObject ();

	}

	public void CheckCanvas(){

		if (!activePlayerVehicle || !activePlayerVehicle.canControl || !activePlayerVehicle.gameObject.activeInHierarchy || !activePlayerVehicle.enabled) {

//			if (activePlayerCanvas.displayType == RCC_UIDashboardDisplay.DisplayType.Full)
//				activePlayerCanvas.SetDisplayType(RCC_UIDashboardDisplay.DisplayType.Off);

			activePlayerCanvas.SetDisplayTypeValue(RCC_UIDashboardDisplayController.DisplayType.Off);

			return;

		}

//		if(!activePlayerCanvas.gameObject.activeInHierarchy)
//			activePlayerCanvas.displayType = RCC_UIDashboardDisplay.DisplayType.Full;

		if(activePlayerCanvas.displayTypeData != RCC_UIDashboardDisplayController.DisplayType.Customization)
			activePlayerCanvas.displayTypeData = RCC_UIDashboardDisplayController.DisplayType.Full;

	}

	///<summary>
	/// Sets new behavior.
	///</summary>
	public static void SetBehavior(int behaviorIndex){

		RCC_SettingsData.InstanceR.overrideBehaviorFlag = true;
		RCC_SettingsData.InstanceR.behaviorSelectedIndexValue = behaviorIndex;

		if (OnBehaviorChanged != null)
			OnBehaviorChanged ();

	}

	///<summary>
	/// Sets the main controller type.
	///</summary>
	public static void SetController(int controllerIndex){

		RCC_SettingsData.InstanceR.controllerSelectedIndexValue = controllerIndex;

		switch (controllerIndex) {

		case 0:
			RCC_SettingsData.InstanceR.selectedControllerTypeR = RCC_SettingsData.ControllerType.Keyboard;
			break;

		case 1:
			RCC_SettingsData.InstanceR.selectedControllerTypeR = RCC_SettingsData.ControllerType.Mobile;
			break;

		case 2:
			RCC_SettingsData.InstanceR.selectedControllerTypeR = RCC_SettingsData.ControllerType.XBox360One;
			break;

		case 3:
			RCC_SettingsData.InstanceR.selectedControllerTypeR = RCC_SettingsData.ControllerType.PS4;
			break;

			case 4:
			RCC_SettingsData.InstanceR.selectedControllerTypeR = RCC_SettingsData.ControllerType.LogitechSteeringWheel;
			break;

		case 5:
			RCC_SettingsData.InstanceR.selectedControllerTypeR = RCC_SettingsData.ControllerType.Custom;
			break;

		}

		if(OnControllerChanged != null)
			OnControllerChanged ();

	}

	// Changes current camera mode.
	public void ChangeCamera () {

		if(RCC_SceneManager.Instance.activePlayerCamera)
			RCC_SceneManager.Instance.activePlayerCamera.ChangeCameraObject();

	}

	/// <summary>
	/// Transport player vehicle the specified position and rotation.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	public void Transport(Vector3 position, Quaternion rotation){

		if (activePlayerVehicle) {

			activePlayerVehicle.rigid.velocity = Vector3.zero;
			activePlayerVehicle.rigid.angularVelocity = Vector3.zero;

			activePlayerVehicle.transform.position = position;
			activePlayerVehicle.transform.rotation = rotation;

		}

	}

	/// <summary>
	/// Transport target vehicle the specified position and rotation.
	/// </summary>
	/// <param name="vehicle"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	public void Transport(RCC_CarMainControllerV3 vehicle, Vector3 position, Quaternion rotation) {

		if (vehicle) {

			vehicle.rigid.velocity = Vector3.zero;
			vehicle.rigid.angularVelocity = Vector3.zero;

			vehicle.transform.position = position;
			vehicle.transform.rotation = rotation;

		}

	}

	void OnDisable(){

		RCC_CameraController.OnBCGCameraSpawnedEvent -= RCC_Camera_OnBCGCameraSpawned;

		RCC_CarMainControllerV3.OnRCCPlayerSpawned -= RCC_CarControllerV3_OnRCCSpawned;
		RCC_AICarMovementController.OnRCCAISpawnedEvent -= RCC_AICarController_OnRCCAISpawned;
		RCC_CarMainControllerV3.OnRCCPlayerDestroyed -= RCC_CarControllerV3_OnRCCPlayerDestroyed;
		RCC_AICarMovementController.OnRCCAIDestroyedEvent -= RCC_AICarController_OnRCCAIDestroyed;

		#if BCG_ENTEREXIT
		BCG_EnterExitPlayer.OnBCGPlayerSpawned -= BCG_EnterExitPlayer_OnBCGPlayerSpawned;
		BCG_EnterExitPlayer.OnBCGPlayerDestroyed -= BCG_EnterExitPlayer_OnBCGPlayerDestroyed;
		#endif

	}

}
