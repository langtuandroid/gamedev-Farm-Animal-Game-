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
/// Record / Replay system. Saves player's input on record, and replays it when on playback.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Recorder")]
public class RCC_RecorderController : MonoBehaviour {

	[System.Serializable]
	public class RecordedData{

		[FormerlySerializedAs("recordName")] public string recordNameValue = "New Record";

		[FormerlySerializedAs("inputs")] [HideInInspector]public PlayerInputData[] inputsMass;
		[FormerlySerializedAs("transforms")] [HideInInspector]public PlayerTransformData[] transformsMass;
		[FormerlySerializedAs("rigids")] [HideInInspector]public PlayerRigidBodyData[] rigidsMass;

		public RecordedData (PlayerInputData[] _inputs, PlayerTransformData[] _transforms, PlayerRigidBodyData[] _rigids, string _recordName){

			inputsMass = _inputs;
			transformsMass = _transforms;
			rigidsMass = _rigids;
			recordNameValue = _recordName;

		}

	}

	[FormerlySerializedAs("recorded")] public RecordedData recordedData;

	[FormerlySerializedAs("carController")] public RCC_CarMainControllerV3 carControllerMain;

	[FormerlySerializedAs("Inputs")] public List <PlayerInputData> InputsList;
	[FormerlySerializedAs("Transforms")] public List <PlayerTransformData> TransformsList;
	[FormerlySerializedAs("Rigidbodies")] public List <PlayerRigidBodyData> RigidbodiesList;

	[System.Serializable]
	public class PlayerInputData{

		[FormerlySerializedAs("throttleInput")] public float throttleInputValue = 0f;
		[FormerlySerializedAs("brakeInput")] public float brakeInputValue = 0f;
		[FormerlySerializedAs("steerInput")] public float steerInputValue = 0f;
		[FormerlySerializedAs("handbrakeInput")] public float handbrakeInputValue = 0f;
		[FormerlySerializedAs("clutchInput")] public float clutchInputValue = 0f;
		[FormerlySerializedAs("boostInput")] public float boostInputValue = 0f;
		[FormerlySerializedAs("fuelInput")] public float fuelInputValue = 0f;
		[FormerlySerializedAs("direction")] public int directionValue = 1;
		[FormerlySerializedAs("canGoReverse")] public bool canGoReverseFlag = false;
		[FormerlySerializedAs("currentGear")] public int currentGearValue = 0;
		[FormerlySerializedAs("changingGear")] public bool changingGearFlag = false;

		[FormerlySerializedAs("indicatorsOn")] public RCC_CarMainControllerV3.IndicatorsOn indicatorsOnR = RCC_CarMainControllerV3.IndicatorsOn.Off;
		[FormerlySerializedAs("lowBeamHeadLightsOn")] public bool lowBeamHeadLightsOnR = false;
		[FormerlySerializedAs("highBeamHeadLightsOn")] public bool highBeamHeadLightsOnR = false;

		public PlayerInputData(float _gasInput, float _brakeInput, float _steerInput, float _handbrakeInput, float _clutchInput, float _boostInput, float _fuelInput, int _direction, bool _canGoReverse, int _currentGear, bool _changingGear, RCC_CarMainControllerV3.IndicatorsOn _indicatorsOn, bool _lowBeamHeadLightsOn, bool _highBeamHeadLightsOn){

			throttleInputValue = _gasInput;
			brakeInputValue = _brakeInput;
			steerInputValue = _steerInput;
			handbrakeInputValue = _handbrakeInput;
			clutchInputValue = _clutchInput;
			boostInputValue = _boostInput;
			fuelInputValue = _fuelInput;
			directionValue = _direction;
			canGoReverseFlag = _canGoReverse;
			currentGearValue = _currentGear;
			changingGearFlag = _changingGear;

			indicatorsOnR = _indicatorsOn;
			lowBeamHeadLightsOnR = _lowBeamHeadLightsOn;
			highBeamHeadLightsOnR = _highBeamHeadLightsOn;

		}

	}

	[System.Serializable]
	public class PlayerTransformData{

		[FormerlySerializedAs("position")] public Vector3 positionVector;
		[FormerlySerializedAs("rotation")] public Quaternion rotationQuaternion;

		public PlayerTransformData(Vector3 _pos, Quaternion _rot){

			positionVector = _pos;
			rotationQuaternion = _rot;

		}

	}

	[System.Serializable]
	public class PlayerRigidBodyData{

		[FormerlySerializedAs("velocity")] public Vector3 velocityVector;
		[FormerlySerializedAs("angularVelocity")] public Vector3 angularVelocityVector;

		public PlayerRigidBodyData(Vector3 _vel, Vector3 _angVel){

			velocityVector = _vel;
			angularVelocityVector = _angVel;

		}

	}

	public enum Mode{Neutral, Play, Record}
	[FormerlySerializedAs("mode")] public Mode modeR;

    private void Awake() {

		InputsList = new List<PlayerInputData>();
		TransformsList = new List<PlayerTransformData>();
		RigidbodiesList = new List<PlayerRigidBodyData>();

	}

    public void RecordSwitch(){

		if (modeR != Mode.Record) {
			modeR = Mode.Record;
		} else {
			modeR = Mode.Neutral;
			SaveRecordR ();
		}

		if(modeR == Mode.Record){

			InputsList.Clear();
			TransformsList.Clear ();
			RigidbodiesList.Clear ();

		}

	}

    private void SaveRecordR(){

		print ("Record saved!");
		recordedData = new RecordedData(InputsList.ToArray(), TransformsList.ToArray(), RigidbodiesList.ToArray(), RCC_RecordsData.InstanceR.recordsList.Count.ToString() + "_" + carControllerMain.transform.name);
		RCC_RecordsData.InstanceR.recordsList.Add (recordedData);

	}

	//	public static void createNewRecipe(RecipeType type)
	//	{
	//		AssetDatabase.CreateAsset (type, "Assets/Resources/RecipeObject/"+type.name.Replace(" ", "")+".asset");
	//		AssetDatabase.SaveAssets ();
	//		EditorUtility.FocusProjectWindow ();
	//		Selection.activeObject = type;
	//	}

	public void PlayR(){

		if (recordedData == null)
			return;

		if (modeR != Mode.Play)
			modeR = Mode.Play;
		else
			modeR = Mode.Neutral;

		if (modeR == Mode.Play)
			carControllerMain.externalController = true;
		else
			carControllerMain.externalController = false;

		if(modeR == Mode.Play){

			StartCoroutine(ReplayCoroutine());

			if (recordedData != null && recordedData.transformsMass.Length > 0) {

				carControllerMain.transform.position = recordedData.transformsMass [0].positionVector;
				carControllerMain.transform.rotation = recordedData.transformsMass [0].rotationQuaternion;

			}

			StartCoroutine(RevelCoroutine());

		}

	}

	public void PlayR(RecordedData _recorded){

		recordedData = _recorded;

		print ("Replaying record " + recordedData.recordNameValue);

		if (recordedData == null)
			return;

		if (modeR != Mode.Play)
			modeR = Mode.Play;
		else
			modeR = Mode.Neutral;

		if (modeR == Mode.Play)
			carControllerMain.externalController = true;
		else
			carControllerMain.externalController = false;

		if(modeR == Mode.Play){

			StartCoroutine(ReplayCoroutine());

			if (recordedData != null && recordedData.transformsMass.Length > 0) {

				carControllerMain.transform.position = recordedData.transformsMass [0].positionVector;
				carControllerMain.transform.rotation = recordedData.transformsMass [0].rotationQuaternion;

			}

			StartCoroutine(RevelCoroutine());

		}

	}

	public void StopR(){

		modeR = Mode.Neutral;
		carControllerMain.externalController = false;

	}

	private IEnumerator ReplayCoroutine(){

		for(int i = 0; i<recordedData.inputsMass.Length && modeR == Mode.Play; i++){

			carControllerMain.externalController = true;
			carControllerMain.throttleInput = recordedData.inputsMass[i].throttleInputValue;
			carControllerMain.brakeInput = recordedData.inputsMass[i].brakeInputValue;
			carControllerMain.steerInput = recordedData.inputsMass[i].steerInputValue;
			carControllerMain.handbrakeInput = recordedData.inputsMass[i].handbrakeInputValue;
			carControllerMain.clutchInput = recordedData.inputsMass[i].clutchInputValue;
			carControllerMain.boostInput = recordedData.inputsMass[i].boostInputValue;
			carControllerMain.fuelInput = recordedData.inputsMass[i].fuelInputValue;
			carControllerMain.direction = recordedData.inputsMass[i].directionValue;
			carControllerMain.canGoReverseNow = recordedData.inputsMass[i].canGoReverseFlag;
			carControllerMain.currentGear = recordedData.inputsMass[i].currentGearValue;
			carControllerMain.changingGear = recordedData.inputsMass[i].changingGearFlag;

			carControllerMain.indicatorsOn = recordedData.inputsMass[i].indicatorsOnR;
			carControllerMain.lowBeamHeadLightsOn = recordedData.inputsMass[i].lowBeamHeadLightsOnR;
			carControllerMain.highBeamHeadLightsOn = recordedData.inputsMass[i].highBeamHeadLightsOnR;

			yield return new WaitForFixedUpdate();

		}

		modeR = Mode.Neutral;

		carControllerMain.externalController = false;

	}

	private IEnumerator ReposCoroutine(){

		for(int i = 0; i<recordedData.transformsMass.Length && modeR == Mode.Play; i++){

			carControllerMain.transform.position = recordedData.transformsMass [i].positionVector;
			carControllerMain.transform.rotation = recordedData.transformsMass [i].rotationQuaternion;

			yield return new WaitForEndOfFrame();

		}

		modeR = Mode.Neutral;

		carControllerMain.externalController = false;

	}

	private IEnumerator RevelCoroutine(){

		for(int i = 0; i<recordedData.rigidsMass.Length && modeR == Mode.Play; i++){

			carControllerMain.rigid.velocity = recordedData.rigidsMass [i].velocityVector;
			carControllerMain.rigid.angularVelocity = recordedData.rigidsMass [i].angularVelocityVector;

			yield return new WaitForFixedUpdate();

		}

		modeR = Mode.Neutral;

		carControllerMain.externalController = false;

	}

	private void FixedUpdate () {

		if (!carControllerMain)
			return;

		switch (modeR) {

		case Mode.Neutral:

			break;

		case Mode.Play:

			carControllerMain.externalController = true;

			break;

		case Mode.Record:

			InputsList.Add(new PlayerInputData(carControllerMain.throttleInput, carControllerMain.brakeInput, carControllerMain.steerInput, carControllerMain.handbrakeInput, carControllerMain.clutchInput, carControllerMain.boostInput, carControllerMain.fuelInput, carControllerMain.direction, carControllerMain.canGoReverseNow, carControllerMain.currentGear, carControllerMain.changingGear, carControllerMain.indicatorsOn, carControllerMain.lowBeamHeadLightsOn, carControllerMain.highBeamHeadLightsOn));
			TransformsList.Add (new PlayerTransformData(carControllerMain.transform.position, carControllerMain.transform.rotation));
			RigidbodiesList.Add(new PlayerRigidBodyData(carControllerMain.rigid.velocity, carControllerMain.rigid.angularVelocity));

			break;

		}

	}

}
