//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// Stored all general shared RCC settings here.
/// </summary>
[System.Serializable]
public class RCC_SettingsData : ScriptableObject {

	public const string RCCVersionName = "V3.45";
	
	#region singleton
	private static RCC_SettingsData instanceR;
	public static RCC_SettingsData InstanceR{	get{if(instanceR == null) instanceR = Resources.Load("RCC Assets/RCC_Settings") as RCC_SettingsData; return instanceR;}}
	#endregion

	[FormerlySerializedAs("controllerSelectedIndex")] public int controllerSelectedIndexValue;
	[FormerlySerializedAs("behaviorSelectedIndex")] public int behaviorSelectedIndexValue;

	public ControllerType selectedControllerTypeR{
		get{
			return controllerTypeR;
		}set{
			controllerTypeR = value;
		}
	}
	public BehaviorTypeData selectedBehaviorTypeR{
		get{
			if (overrideBehaviorFlag)
				return behaviorTypesMass [behaviorSelectedIndexValue];
			else
				return null;
		}
	}

	[FormerlySerializedAs("overrideBehavior")] public bool overrideBehaviorFlag = true;
	[FormerlySerializedAs("overrideFPS")] public bool overrideFPSFlag = true;
	[FormerlySerializedAs("overrideFixedTimeStep")] public bool overrideFixedTimeStepFlag = true;
	[FormerlySerializedAs("fixedTimeStep")] [Range(.005f, .06f)]public float fixedTimeStepValue = .02f;
	[FormerlySerializedAs("maxAngularVelocity")] [Range(.5f, 20f)]public float maxAngularVelocityValue = 6;
	[FormerlySerializedAs("maxFPS")] public int maxFPSValue = 60;

	// Behavior Types
	[System.Serializable]
	public class BehaviorTypeData{

		[FormerlySerializedAs("behaviorName")] public string behaviorNameValue = "New Behavior";

		[FormerlySerializedAs("steeringHelper")] [Header("Steering Helpers")]
		public bool steeringHelperFlag = true;
		[FormerlySerializedAs("tractionHelper")] public bool tractionHelperFlag = true;
		[FormerlySerializedAs("angularDragHelper")] public bool angularDragHelperFlag = false;
		[FormerlySerializedAs("counterSteering")] public bool counterSteeringFlag = true;
		[FormerlySerializedAs("ABS")] public bool ABSFlag = false;
		[FormerlySerializedAs("ESP")] public bool ESPFlag = false;
		[FormerlySerializedAs("TCS")] public bool TCSFlag = false;
		[FormerlySerializedAs("applyExternalWheelFrictions")] public bool applyExternalWheelFrictionsFlag = false;
		[FormerlySerializedAs("applyRelativeTorque")] public bool applyRelativeTorqueFlag = false;

		[FormerlySerializedAs("highSpeedSteerAngleMinimum")] [Space()]
		public float highSpeedSteerAngleMinimumValue = 20f;
		[FormerlySerializedAs("highSpeedSteerAngleMaximum")] public float highSpeedSteerAngleMaximumVlaue = 40f;

		[FormerlySerializedAs("highSpeedSteerAngleAtspeedMinimum")] public float highSpeedSteerAngleAtspeedMinimumValue = 100f;
		[FormerlySerializedAs("highSpeedSteerAngleAtspeedMaximum")] public float highSpeedSteerAngleAtspeedMaximumValue = 200f;

		[FormerlySerializedAs("counterSteeringMinimum")] [Space()]
		public float counterSteeringMinimumValue = .1f;
		[FormerlySerializedAs("counterSteeringMaximum")] public float counterSteeringMaximumValue = 1f;

		[FormerlySerializedAs("steerHelperAngularVelStrengthMinimum")]
		[Space()]
		[Range(0f, 1f)]public float steerHelperAngularVelStrengthMinimumValue = .1f;
		[FormerlySerializedAs("steerHelperAngularVelStrengthMaximum")] [Range(0f, 1f)]public float steerHelperAngularVelStrengthMaximumValue = 1;

		[FormerlySerializedAs("steerHelperLinearVelStrengthMinimum")] [Range(0f, 1f)]public float steerHelperLinearVelStrengthMinimumValue = .1f;
		[FormerlySerializedAs("steerHelperLinearVelStrengthMaximum")] [Range(0f, 1f)]public float steerHelperLinearVelStrengthMaximumValue = 1f;

		[FormerlySerializedAs("tractionHelperStrengthMinimum")] [Range(0f, 1f)]public float tractionHelperStrengthMinimumValue = .1f;
		[FormerlySerializedAs("tractionHelperStrengthMaximum")] [Range(0f, 1f)]public float tractionHelperStrengthMaximumValue = 1f;

		[FormerlySerializedAs("antiRollFrontHorizontalMinimum")] [Space()]
		public float antiRollFrontHorizontalMinimumValue = 1000f;
		[FormerlySerializedAs("antiRollRearHorizontalMinimum")] public float antiRollRearHorizontalMinimumValue = 1000f;

		[FormerlySerializedAs("gearShiftingDelayMaximum")]
		[Space()]
		[Range(0f, 1f)]public float gearShiftingDelayMaximumValue = .15f;

		[FormerlySerializedAs("angularDrag")] [Range(0f, 10f)]public float angularDragValue = .1f;
		[FormerlySerializedAs("angularDragHelperMinimum")] [Range(0f, 1f)]public float angularDragHelperMinimumValue = .1f;
		[FormerlySerializedAs("angularDragHelperMaximum")] [Range(0f, 1f)]public float angularDragHelperMaximumValue = 1f;

		[FormerlySerializedAs("forwardExtremumSlip")] [Header("Wheel Frictions Forward")]
		public float forwardExtremumSlipValue = .4f;
		[FormerlySerializedAs("forwardExtremumValue")] public float forwardExtremumValueR = 1f;
		[FormerlySerializedAs("forwardAsymptoteSlip")] public float forwardAsymptoteSlipValue = .8f;
		[FormerlySerializedAs("forwardAsymptoteValue")] public float forwardAsymptoteValueR = .5f;

		[FormerlySerializedAs("sidewaysExtremumSlip")] [Header("Wheel Frictions Sideways")]
		public float sidewaysExtremumSlipValue = .2f;
		[FormerlySerializedAs("sidewaysExtremumValue")] public float sidewaysExtremumValueR = 1f;
		[FormerlySerializedAs("sidewaysAsymptoteSlip")] public float sidewaysAsymptoteSlipValue = .5f;
		[FormerlySerializedAs("sidewaysAsymptoteValue")] public float sidewaysAsymptoteValueR = .75f;
	
	}

	[FormerlySerializedAs("useFixedWheelColliders")] public bool useFixedWheelCollidersFlag = true;
	[FormerlySerializedAs("lockAndUnlockCursor")] public bool lockAndUnlockCursorFlag = true;

	// Controller Type
	[FormerlySerializedAs("controllerType")] public ControllerType controllerTypeR;
	public enum ControllerType{Keyboard, Mobile, XBox360One, PS4, LogitechSteeringWheel, Custom}

	// Behavior Types
	[FormerlySerializedAs("behaviorTypes")] public BehaviorTypeData[] behaviorTypesMass;

	// Keyboard Inputs
	[FormerlySerializedAs("verticalInput")] public string verticalInputValue = "Vertical";
	[FormerlySerializedAs("horizontalInput")] public string horizontalInputValue = "Horizontal";
	[FormerlySerializedAs("mouseXInput")] public string mouseXInputValue = "Mouse X";
	[FormerlySerializedAs("mouseYInput")] public string mouseYInputValue = "Mouse Y";
	[FormerlySerializedAs("handbrakeKB")] public KeyCode handbrakeKBCode = KeyCode.Space;
	[FormerlySerializedAs("startEngineKB")] public KeyCode startEngineKBCode = KeyCode.I;
	[FormerlySerializedAs("lowBeamHeadlightsKB")] public KeyCode lowBeamHeadlightsKBCode = KeyCode.L;
	[FormerlySerializedAs("highBeamHeadlightsKB")] public KeyCode highBeamHeadlightsKBCode = KeyCode.K;
	[FormerlySerializedAs("rightIndicatorKB")] public KeyCode rightIndicatorKBCode = KeyCode.E;
	[FormerlySerializedAs("leftIndicatorKB")] public KeyCode leftIndicatorKBCode = KeyCode.Q;
	[FormerlySerializedAs("hazardIndicatorKB")] public KeyCode hazardIndicatorKBCode = KeyCode.Z;
	[FormerlySerializedAs("shiftGearUp")] public KeyCode shiftGearUpCode = KeyCode.LeftShift;
	[FormerlySerializedAs("shiftGearDown")] public KeyCode shiftGearDownCode = KeyCode.LeftControl;
	[FormerlySerializedAs("NGear")] public KeyCode NGearCode = KeyCode.N;
	[FormerlySerializedAs("boostKB")] public KeyCode boostKBCode = KeyCode.F;
	[FormerlySerializedAs("slowMotionKB")] public KeyCode slowMotionKBCode = KeyCode.G;
	[FormerlySerializedAs("changeCameraKB")] public KeyCode changeCameraKBCode = KeyCode.C;
	[FormerlySerializedAs("recordKB")] public KeyCode recordKBCode = KeyCode.R;
	[FormerlySerializedAs("playbackKB")] public KeyCode playbackKBCode = KeyCode.P;
	[FormerlySerializedAs("lookBackKB")] public KeyCode lookBackKBCode = KeyCode.B;
	[FormerlySerializedAs("trailerAttachDetach")] public KeyCode trailerAttachDetachCode = KeyCode.T;

	// XBox Inputs
	[FormerlySerializedAs("Xbox_verticalInput")] public string Xbox_verticalInputValue = "Xbox_Vertical";
	[FormerlySerializedAs("Xbox_horizontalInput")] public string Xbox_horizontalInputValue = "Xbox_Horizontal";
	[FormerlySerializedAs("Xbox_triggerLeftInput")] public string Xbox_triggerLeftInputValue = "Xbox_TriggerLeft";
	[FormerlySerializedAs("Xbox_triggerRightInput")] public string Xbox_triggerRightInputValue = "Xbox_TriggerRight";
	[FormerlySerializedAs("Xbox_mouseXInput")] public string Xbox_mouseXInputValue = "Xbox_MouseX";
	[FormerlySerializedAs("Xbox_mouseYInput")] public string Xbox_mouseYInputValue = "Xbox_MouseY";
	[FormerlySerializedAs("Xbox_handbrakeKB")] public string Xbox_handbrakeKBValue = "Xbox_B";
	[FormerlySerializedAs("Xbox_startEngineKB")] public string Xbox_startEngineKBValue = "Xbox_Y";
	[FormerlySerializedAs("Xbox_lowBeamHeadlightsKB")] public string Xbox_lowBeamHeadlightsKBValue = "Xbox_LB";
	[FormerlySerializedAs("Xbox_highBeamHeadlightsKB")] public string Xbox_highBeamHeadlightsKBValue = "Xbox_RB";
	[FormerlySerializedAs("Xbox_indicatorKB")] public string Xbox_indicatorKBValue = "Xbox_DPadHorizontal";
	[FormerlySerializedAs("Xbox_hazardIndicatorKB")] public string Xbox_hazardIndicatorKBValue = "Xbox_DPadVertical";
	[FormerlySerializedAs("Xbox_shiftGearUp")] public string Xbox_shiftGearUpValue = "Xbox_RB";
	[FormerlySerializedAs("Xbox_shiftGearDown")] public string Xbox_shiftGearDownValue = "Xbox_LB";
	[FormerlySerializedAs("Xbox_boostKB")] public string Xbox_boostKBValue = "Xbox_A";
	[FormerlySerializedAs("Xbox_changeCameraKB")] public string Xbox_changeCameraKBValue = "Xbox_Back";
	[FormerlySerializedAs("Xbox_lookBackKB")] public string Xbox_lookBackKBValue = "Xbox_ClickRight";
	[FormerlySerializedAs("Xbox_trailerAttachDetach")] public string Xbox_trailerAttachDetachValue = "Xbox_ClickLeft";

	// PS4 Inputs
	[FormerlySerializedAs("PS4_verticalInput")] public string PS4_verticalInputValue = "PS4_Vertical";
	[FormerlySerializedAs("PS4_horizontalInput")] public string PS4_horizontalInputValue = "PS4_Horizontal";
	[FormerlySerializedAs("PS4_triggerLeftInput")] public string PS4_triggerLeftInputValue = "PS4_TriggerLeft";
	[FormerlySerializedAs("PS4_triggerRightInput")] public string PS4_triggerRightInputValue = "PS4_TriggerRight";
	[FormerlySerializedAs("PS4_mouseXInput")] public string PS4_mouseXInputValue = "PS4_MouseX";
	[FormerlySerializedAs("PS4_mouseYInput")] public string PS4_mouseYInputValue = "PS4_MouseY";
	[FormerlySerializedAs("PS4_handbrakeKB")] public string PS4_handbrakeKBValue = "PS4_B";
	[FormerlySerializedAs("PS4_startEngineKB")] public string PS4_startEngineKBValue = "PS4_Y";
	[FormerlySerializedAs("PS4_lowBeamHeadlightsKB")] public string PS4_lowBeamHeadlightsKBValue = "PS4_LB";
	[FormerlySerializedAs("PS4_highBeamHeadlightsKB")] public string PS4_highBeamHeadlightsKBValue = "PS4_RB";
	[FormerlySerializedAs("PS4_indicatorKB")] public string PS4_indicatorKBValue = "PS4_DPadHorizontal";
	[FormerlySerializedAs("PS4_hazardIndicatorKB")] public string PS4_hazardIndicatorKBValue = "PS4_DPadVertical";
	[FormerlySerializedAs("PS4_shiftGearUp")] public string PS4_shiftGearUpValue = "PS4_RB";
	[FormerlySerializedAs("PS4_shiftGearDown")] public string PS4_shiftGearDownValue = "PS4_LB";
	[FormerlySerializedAs("PS4_boostKB")] public string PS4_boostKBValue = "PS4_A";
	[FormerlySerializedAs("PS4_changeCameraKB")] public string PS4_changeCameraKBValue = "PS4_Back";
	[FormerlySerializedAs("PS4_lookBackKB")] public string PS4_lookBackKBValue = "PS4_ClickRight";
	[FormerlySerializedAs("PS4_trailerAttachDetach")] public string PS4_trailerAttachDetachValue = "PS4_ClickLeft";

	// Logitech Steering Wheel Inputs
	[FormerlySerializedAs("LogiSteeringWheel_handbrakeKB")] public int LogiSteeringWheel_handbrakeKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_startEngineKB")] public int LogiSteeringWheel_startEngineKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_lowBeamHeadlightsKB")] public int LogiSteeringWheel_lowBeamHeadlightsKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_highBeamHeadlightsKB")] public int LogiSteeringWheel_highBeamHeadlightsKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_hazardIndicatorKB")] public int LogiSteeringWheel_hazardIndicatorKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_shiftGearUp")] public int LogiSteeringWheel_shiftGearUpValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_shiftGearDown")] public int LogiSteeringWheel_shiftGearDownValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_boostKB")] public int LogiSteeringWheel_boostKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_changeCameraKB")] public int LogiSteeringWheel_changeCameraKBValue = 0;
	[FormerlySerializedAs("LogiSteeringWheel_lookBackKB")] public int LogiSteeringWheel_lookBackKBValue = 0;

	// Main Controller Settings
	[FormerlySerializedAs("useVR")] public bool useVRFlag = false;
	[FormerlySerializedAs("useAutomaticGear")] public bool useAutomaticGearFlag = true;
	[FormerlySerializedAs("useAutomaticClutch")] public bool useAutomaticClutchFlag = true;
	[FormerlySerializedAs("runEngineAtAwake")] public bool runEngineAtAwakeFlag = true;
	[FormerlySerializedAs("autoReverse")] public bool autoReverseFlag = true;
	[FormerlySerializedAs("autoReset")] public bool autoResetFlag = true;
	[FormerlySerializedAs("contactParticles")] public GameObject contactParticlesObject;

	[FormerlySerializedAs("units")] public Units unitsR;
	public enum Units {KMH, MPH}

	// UI Dashboard Type
	public UIType uiType;
	public enum UIType{UI, NGUI, None}

	// Information telemetry about current vehicle
	[FormerlySerializedAs("useTelemetry")] public bool useTelemetryObject = false;

	// For mobile usement
	public enum MobileController{TouchScreen, Gyro, SteeringWheel, Joystick}
	[FormerlySerializedAs("mobileController")] public MobileController mobileControllerObject;

	// Mobile controller buttons and accelerometer sensitivity
	[FormerlySerializedAs("UIButtonSensitivity")] public float UIButtonSensitivityValue = 3f;
	[FormerlySerializedAs("UIButtonGravity")] public float UIButtonGravityValue = 5f;
	[FormerlySerializedAs("gyroSensitivity")] public float gyroSensitivityValue = 2f;

	// Used for using the lights more efficent and realistic
	[FormerlySerializedAs("useLightsAsVertexLights")] public bool useLightsAsVertexLightsValue = true;
	[FormerlySerializedAs("useLightProjectorForLightingEffect")] public bool useLightProjectorForLightingEffectValue = false;

	// Other stuff
	[FormerlySerializedAs("setTagsAndLayers")] public bool setTagsAndLayersFlag = false;
	[FormerlySerializedAs("RCCLayer")] public string RCCLayerValue;
	[FormerlySerializedAs("RCCTag")] public string RCCTagValue;
	[FormerlySerializedAs("tagAllChildrenGameobjects")] public bool tagAllChildrenGameobjectsFlag = false;

	[FormerlySerializedAs("chassisJoint")] public GameObject chassisJointObject;
	[FormerlySerializedAs("exhaustGas")] public GameObject exhaustGasObject;
	[FormerlySerializedAs("skidmarksManager")] public RCC_SkidmarksGroundManager skidmarksManagerR;
	[FormerlySerializedAs("projector")] public GameObject projectorObject;
	[FormerlySerializedAs("projectorIgnoreLayer")] public LayerMask projectorIgnoreLayerMask;

	[FormerlySerializedAs("headLights")] public GameObject headLightsObject;
	[FormerlySerializedAs("brakeLights")] public GameObject brakeLightsObject;
	[FormerlySerializedAs("reverseLights")] public GameObject reverseLightsObject;
	[FormerlySerializedAs("indicatorLights")] public GameObject indicatorLightsObject;
	[FormerlySerializedAs("lightTrailers")] public GameObject lightTrailersObject;
	[FormerlySerializedAs("mirrors")] public GameObject mirrorsObject;

	[FormerlySerializedAs("RCCMainCamera")] public RCC_CameraController RCCMainCameraController;
	[FormerlySerializedAs("hoodCamera")] public GameObject hoodCameraObject;
	[FormerlySerializedAs("cinematicCamera")] public GameObject cinematicCameraObject;
	[FormerlySerializedAs("RCCCanvas")] public GameObject RCCCanvasObject;
	[FormerlySerializedAs("RCCTelemetry")] public GameObject RCCTelemetryObject;

	[FormerlySerializedAs("dontUseAnyParticleEffects")] public bool dontUseAnyParticleEffectsFlag = false;
	[FormerlySerializedAs("dontUseSkidmarks")] public bool dontUseSkidmarksFlag = false;

	// Sound FX
	[FormerlySerializedAs("audioMixer")] public AudioMixerGroup audioMixerValue;
	[FormerlySerializedAs("gearShiftingClips")] public AudioClip[] gearShiftingClipsMass;
	[FormerlySerializedAs("crashClips")] public AudioClip[] crashClipsMass;
	[FormerlySerializedAs("reversingClip")] public AudioClip reversingClipValue;
	[FormerlySerializedAs("windClip")] public AudioClip windClipValue;
	[FormerlySerializedAs("brakeClip")] public AudioClip brakeClipValue;
	[FormerlySerializedAs("indicatorClip")] public AudioClip indicatorClipValue;
	[FormerlySerializedAs("bumpClip")] public AudioClip bumpClipValue;
	[FormerlySerializedAs("NOSClip")] public AudioClip NOSClipValue;
	[FormerlySerializedAs("turboClip")] public AudioClip turboClipValue;
	[FormerlySerializedAs("blowoutClip")] public AudioClip[] blowoutClipMass;
	[FormerlySerializedAs("exhaustFlameClips")] public AudioClip[] exhaustFlameClipsMass;

	[FormerlySerializedAs("maxGearShiftingSoundVolume")] [Range(0f, 1f)]public float maxGearShiftingSoundVolumeValue = .25f;
	[FormerlySerializedAs("maxCrashSoundVolume")] [Range(0f, 1f)]public float maxCrashSoundVolumeValue = 1f;
	[FormerlySerializedAs("maxWindSoundVolume")] [Range(0f, 1f)]public float maxWindSoundVolumeValue = .1f;
	[FormerlySerializedAs("maxBrakeSoundVolume")] [Range(0f, 1f)]public float maxBrakeSoundVolumeValue = .1f;

	// Used for folding sections of RCC Settings
	[FormerlySerializedAs("foldGeneralSettings")] public bool foldGeneralSettingsFlag = false;
	[FormerlySerializedAs("foldBehaviorSettings")] public bool foldBehaviorSettingsFlag = false;
	[FormerlySerializedAs("foldControllerSettings")] public bool foldControllerSettingsFlag = false;
	[FormerlySerializedAs("foldUISettings")] public bool foldUISettingsFlag = false;
	[FormerlySerializedAs("foldWheelPhysics")] public bool foldWheelPhysicsFlag = false;
	[FormerlySerializedAs("foldSFX")] public bool foldSFXFlag = false;
	[FormerlySerializedAs("foldOptimization")] public bool foldOptimizationFlag = false;
	[FormerlySerializedAs("foldTagsAndLayers")] public bool foldTagsAndLayersFlag = false;

}
