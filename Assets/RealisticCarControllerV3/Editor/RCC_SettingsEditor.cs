//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RCC_SettingsData))]
public class RCC_SettingsEditor : Editor {

	RCC_SettingsData RCCSettingsAsset;

	Color originalGUIColor;
	Vector2 scrollPos;
	PhysicMaterial[] physicMaterials;

	bool foldGeneralSettings = false;
	bool foldBehaviorSettings = false;
	bool foldControllerSettings = false;
	bool foldUISettings = false;
	bool foldWheelPhysics = false;
	bool foldSFX = false;
	bool foldOptimization = false;
	bool foldTagsAndLayers = false;

	void OnEnable(){

		foldGeneralSettings = RCC_SettingsData.InstanceR.foldGeneralSettingsFlag;
		foldBehaviorSettings = RCC_SettingsData.InstanceR.foldBehaviorSettingsFlag;
		foldControllerSettings = RCC_SettingsData.InstanceR.foldControllerSettingsFlag;
		foldUISettings = RCC_SettingsData.InstanceR.foldUISettingsFlag;
		foldWheelPhysics = RCC_SettingsData.InstanceR.foldWheelPhysicsFlag;
		foldSFX = RCC_SettingsData.InstanceR.foldSFXFlag;
		foldOptimization = RCC_SettingsData.InstanceR.foldOptimizationFlag;
		foldTagsAndLayers = RCC_SettingsData.InstanceR.foldTagsAndLayersFlag;

	}

	void OnDestroy(){

		RCC_SettingsData.InstanceR.foldBehaviorSettingsFlag = foldBehaviorSettings;
		RCC_SettingsData.InstanceR.foldControllerSettingsFlag = foldControllerSettings;
		RCC_SettingsData.InstanceR.foldUISettingsFlag = foldUISettings;
		RCC_SettingsData.InstanceR.foldWheelPhysicsFlag = foldWheelPhysics;
		RCC_SettingsData.InstanceR.foldSFXFlag = foldSFX;
		RCC_SettingsData.InstanceR.foldOptimizationFlag = foldOptimization;
		RCC_SettingsData.InstanceR.foldTagsAndLayersFlag = foldTagsAndLayers;

	}

	public override void OnInspectorGUI (){

		serializedObject.Update();
		RCCSettingsAsset = (RCC_SettingsData)target;

		originalGUIColor = GUI.color;
		EditorGUIUtility.labelWidth = 250;
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("RCC Asset Settings Editor Window", EditorStyles.boldLabel);
		GUI.color = new Color(.75f, 1f, .75f);
		EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project for RCC. Don't change directory of the ''Resources/RCC Assets''.", EditorStyles.helpBox);
		GUI.color = originalGUIColor;
		EditorGUILayout.Space();

		EditorGUI.indentLevel++;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false );

		EditorGUILayout.Space();

		foldGeneralSettings = EditorGUILayout.Foldout(foldGeneralSettings, "General Settings");

		if(foldGeneralSettings){

			EditorGUILayout.BeginVertical (GUI.skin.box);
			GUILayout.Label("General Settings", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideFixedTimeStep"), new GUIContent("Override FixedTimeStep"));

			if(RCCSettingsAsset.overrideFixedTimeStepFlag)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("fixedTimeStep"), new GUIContent("Fixed Timestep"));
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAngularVelocity"), new GUIContent("Maximum Angular Velocity"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideFPS"), new GUIContent("Override FPS"));

			if(RCCSettingsAsset.overrideFPSFlag)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFPS"), new GUIContent("Maximum FPS"));

			EditorGUILayout.HelpBox("You can find all references to any mode. Open up ''RCC_Settings.cs'' and right click to any mode. Hit ''Find references'' to find all modifications.", MessageType.Info);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("useFixedWheelColliders"), new GUIContent("Use Fixed WheelColliders", "Improves stability by increasing mass of the WheelColliders."));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("lockAndUnlockCursor"), new GUIContent("Locks Cursor", "Locks Cursor."));
			EditorGUILayout.EndVertical ();

		}

		EditorGUILayout.Space();

		foldBehaviorSettings = EditorGUILayout.Foldout(foldBehaviorSettings, "Behavior Settings");

		if (foldBehaviorSettings) {

			EditorGUILayout.BeginVertical(GUI.skin.box);

			GUILayout.Label("Behavior Settings", EditorStyles.boldLabel);

			GUI.color = new Color(.75f, 1f, .75f);
			EditorGUILayout.HelpBox("Using behavior preset will override wheelcollider settings, chassis joint, antirolls, and other stuff. Using ''Custom'' mode will not override anything.", MessageType.Info);
			GUI.color = originalGUIColor;

			RCCSettingsAsset.overrideBehaviorFlag = EditorGUILayout.BeginToggleGroup ("Override Behavior", RCCSettingsAsset.overrideBehaviorFlag);

			EditorGUI.indentLevel++;
			EditorGUIUtility.labelWidth = 305;
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("behaviorTypes"), new GUIContent ("Behavior Types"), true);
			EditorGUIUtility.labelWidth = 250;

			List<string> behaviorTypeStrings = new List<string> ();

			GUI.color = new Color (.5f, 1f, 1f, 1f);

			for (int i = 0; i < RCCSettingsAsset.behaviorTypesMass.Length; i++) {

				behaviorTypeStrings.Add (RCCSettingsAsset.behaviorTypesMass [i].behaviorNameValue);

			}

			RCCSettingsAsset.behaviorSelectedIndexValue = GUILayout.Toolbar (RCCSettingsAsset.behaviorSelectedIndexValue, behaviorTypeStrings.ToArray ());

			EditorGUI.indentLevel--;
			GUI.color = originalGUIColor;

			EditorGUILayout.EndToggleGroup ();

			EditorGUILayout.EndVertical();

		}

		EditorGUILayout.Space();

		foldControllerSettings = EditorGUILayout.Foldout(foldControllerSettings, "Controller Settings");

		if(foldControllerSettings){
			
			List<string> controllerTypeStrings =  new List<string>();
			controllerTypeStrings.Add("Keyboard");	controllerTypeStrings.Add("Mobile");	controllerTypeStrings.Add("XBox");	 controllerTypeStrings.Add("PS4");	 controllerTypeStrings.Add("Logitech Steering Wheel");	 controllerTypeStrings.Add("Custom");
			EditorGUILayout.BeginVertical (GUI.skin.box);

			GUI.color = new Color(.5f, 1f, 1f, 1f);
			GUILayout.Label("Main Controller Type", EditorStyles.boldLabel);
			RCCSettingsAsset.controllerSelectedIndexValue = GUILayout.Toolbar(RCCSettingsAsset.controllerSelectedIndexValue, controllerTypeStrings.ToArray());
			GUI.color = originalGUIColor;
			EditorGUILayout.Space();

			if(RCCSettingsAsset.controllerSelectedIndexValue == 0){

				RCCSettingsAsset.controllerTypeR = RCC_SettingsData.ControllerType.Keyboard;

				EditorGUILayout.BeginVertical (GUI.skin.box);

				GUILayout.Label("Keyboard Settings", EditorStyles.boldLabel);

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("In this mode, inputs will be received from Keyboard.", MessageType.Info);
				GUI.color = originalGUIColor;

				EditorGUILayout.PropertyField(serializedObject.FindProperty("verticalInput"), new GUIContent("Gas/Reverse Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("horizontalInput"), new GUIContent("Steering Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseXInput"), new GUIContent("Mouse X Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("mouseYInput"), new GUIContent("Mouse Y Input Axis"));
				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("You can edit your vertical and horizontal input axis in Edit --> Project Settings --> Input.", MessageType.Info);
				GUI.color = originalGUIColor;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeKB"), new GUIContent("Handbrake"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("startEngineKB"), new GUIContent("Start / Stop Engine Key"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("lowBeamHeadlightsKB"), new GUIContent("Low Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("highBeamHeadlightsKB"), new GUIContent("High Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("changeCameraKB"), new GUIContent("Change Camera"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("rightIndicatorKB"), new GUIContent("Indicator Right"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("leftIndicatorKB"), new GUIContent("Indicator Left"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("hazardIndicatorKB"), new GUIContent("Indicator Hazard"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftGearUp"), new GUIContent("Gear Shift Up"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("shiftGearDown"), new GUIContent("Gear Shift Down"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("NGear"), new GUIContent("N Gear"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("boostKB"), new GUIContent("Boost / NOS"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("slowMotionKB"), new GUIContent("Slow Motion"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("recordKB"), new GUIContent("Record"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("playbackKB"), new GUIContent("Playback"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("lookBackKB"), new GUIContent("Look Back"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("trailerAttachDetach"), new GUIContent("Trailer Attach/Detach"));
				EditorGUILayout.Space();

				EditorGUILayout.EndVertical ();

		}
	
		if(RCCSettingsAsset.controllerSelectedIndexValue == 1){

			EditorGUILayout.BeginVertical (GUI.skin.box);

			RCCSettingsAsset.controllerTypeR = RCC_SettingsData.ControllerType.Mobile;

			GUILayout.Label("Mobile Settings", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("mobileController"), new GUIContent("Mobile Controller"));

			GUI.color = new Color(.75f, 1f, .75f);
			EditorGUILayout.HelpBox("In this mode, all inputs will be received from UI buttons.", MessageType.Info);
			GUI.color = originalGUIColor;

			EditorGUILayout.PropertyField(serializedObject.FindProperty("UIButtonSensitivity"), new GUIContent("UI Button Sensitivity"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UIButtonGravity"), new GUIContent("UI Button Gravity"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gyroSensitivity"), new GUIContent("Gyro Sensitivity"));

			EditorGUILayout.Space();
			
			GUI.color = new Color(.75f, 1f, .75f);
			EditorGUILayout.HelpBox("You can enable/disable Accelerometer in your game by just calling ''RCCSettings.Instance.useAccelerometerForSteering = true/false;''.", MessageType.Info);
			EditorGUILayout.HelpBox("You can enable/disable Steering Wheel Controlling in your game by just calling ''RCCSettings.Instance.useSteeringWheelForSteering = true/false;''.", MessageType.Info);
			GUI.color = originalGUIColor;
			EditorGUILayout.Space();

			EditorGUILayout.EndVertical ();

		}

		if(RCCSettingsAsset.controllerSelectedIndexValue == 2){

				RCCSettingsAsset.controllerTypeR = RCC_SettingsData.ControllerType.XBox360One;

				EditorGUILayout.BeginVertical (GUI.skin.box);

				GUILayout.Label("XBox 360 & One Settings", EditorStyles.boldLabel);

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("In this mode, inputs will be received from XBox 360 & One controllers.", MessageType.Info);
				GUI.color = originalGUIColor;

				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_triggerRightInput"), new GUIContent("Gas Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_triggerLeftInput"), new GUIContent("Brake/Reverse Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_horizontalInput"), new GUIContent("Steering Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_mouseXInput"), new GUIContent("Mouse X Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_mouseYInput"), new GUIContent("Mouse Y Input Axis"));

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("You can edit your vertical and horizontal input axis in Edit --> Project Settings --> Input.", MessageType.Info);
				GUI.color = originalGUIColor;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_handbrakeKB"), new GUIContent("Handbrake"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_startEngineKB"), new GUIContent("Start / Stop Engine Key"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_lowBeamHeadlightsKB"), new GUIContent("Low Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_highBeamHeadlightsKB"), new GUIContent("High Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_changeCameraKB"), new GUIContent("Change Camera"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_indicatorKB"), new GUIContent("Indicator Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_hazardIndicatorKB"), new GUIContent("Indicator Hazard"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_shiftGearUp"), new GUIContent("Gear Shift Up"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_shiftGearDown"), new GUIContent("Gear Shift Down"));
//				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_NGear"), new GUIContent("N Gear"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_boostKB"), new GUIContent("Boost / NOS"));
//				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_slowMotionKB"), new GUIContent("Slow Motion"));
//				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_recordKB"), new GUIContent("Record"));
//				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_playbackKB"), new GUIContent("Playback"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_lookBackKB"), new GUIContent("Look Back"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Xbox_trailerAttachDetach"), new GUIContent("Trailer Attach/Detach"));
				EditorGUILayout.Space();

				EditorGUILayout.EndVertical ();
			
		}

			if (RCCSettingsAsset.controllerSelectedIndexValue == 3){

				RCCSettingsAsset.controllerTypeR = RCC_SettingsData.ControllerType.PS4;

				EditorGUILayout.BeginVertical(GUI.skin.box);

				GUILayout.Label("PS4 Settings", EditorStyles.boldLabel);

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("In this mode, inputs will be received from PS4 controllers.", MessageType.Info);
				GUI.color = originalGUIColor;

				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_triggerRightInput"), new GUIContent("Gas Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_triggerLeftInput"), new GUIContent("Brake/Reverse Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_horizontalInput"), new GUIContent("Steering Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_mouseXInput"), new GUIContent("Mouse X Input Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_mouseYInput"), new GUIContent("Mouse Y Input Axis"));

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("You can edit your vertical and horizontal input axis in Edit --> Project Settings --> Input.", MessageType.Info);
				GUI.color = originalGUIColor;
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_handbrakeKB"), new GUIContent("Handbrake"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_startEngineKB"), new GUIContent("Start / Stop Engine Key"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_lowBeamHeadlightsKB"), new GUIContent("Low Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_highBeamHeadlightsKB"), new GUIContent("High Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_changeCameraKB"), new GUIContent("Change Camera"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_indicatorKB"), new GUIContent("Indicator Axis"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_hazardIndicatorKB"), new GUIContent("Indicator Hazard"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_shiftGearUp"), new GUIContent("Gear Shift Up"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_shiftGearDown"), new GUIContent("Gear Shift Down"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_boostKB"), new GUIContent("Boost / NOS"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_lookBackKB"), new GUIContent("Look Back"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PS4_trailerAttachDetach"), new GUIContent("Trailer Attach/Detach"));
				EditorGUILayout.Space();

				EditorGUILayout.EndVertical();

			}

			if (RCCSettingsAsset.controllerSelectedIndexValue == 4){

				EditorGUILayout.BeginVertical (GUI.skin.box);

				RCCSettingsAsset.controllerTypeR = RCC_SettingsData.ControllerType.LogitechSteeringWheel;

				GUILayout.Label("Logitech Steering Wheel Settings", EditorStyles.boldLabel);

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("In this mode, inputs will be received from Logitech Steering Wheel.", MessageType.Info);
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Be sure your project has latest Logitech Gaming SDK.", MessageType.Warning);
				EditorGUILayout.Space();
				GUI.color = originalGUIColor;

				if (GUILayout.Button ("Download & Import Logitech Gaming SDK")) {

					string url = RCC_AssetPathsR.logitechConst;
					Application.OpenURL (url);

				}

				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_handbrakeKB"), new GUIContent("Handbrake"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_startEngineKB"), new GUIContent("Start / Stop Engine"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_lowBeamHeadlightsKB"), new GUIContent("Low Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_highBeamHeadlightsKB"), new GUIContent("Hıgh Beam Headlights"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_hazardIndicatorKB"), new GUIContent("Hazard Indicator"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_shiftGearUp"), new GUIContent("Gear Shift Up"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_shiftGearDown"), new GUIContent("Gear Shift Down"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_boostKB"), new GUIContent("Boost / NOS"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_changeCameraKB"), new GUIContent("Change Camera"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LogiSteeringWheel_lookBackKB"), new GUIContent("Look Back"));

				EditorGUILayout.EndVertical ();

			}

			if(RCCSettingsAsset.controllerSelectedIndexValue == 5){

				EditorGUILayout.BeginVertical (GUI.skin.box);

				RCCSettingsAsset.controllerTypeR = RCC_SettingsData.ControllerType.Custom;

				GUILayout.Label("Custom Input Settings", EditorStyles.boldLabel);

				GUI.color = new Color(.75f, 1f, .75f);
				EditorGUILayout.HelpBox("In this mode, RCC won't receive these inputs from keyboard or UI buttons. You need to feed these inputs in your own script.", MessageType.Info);
				EditorGUILayout.HelpBox("All inputs are processed by RCC_InputManager script. Open the script and you can see all controller types here. Scroll down to ''Custom'' case and use your own values here.", MessageType.Info);
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("RCC uses these inputs; \n  \n    throttleInput = Clamped 0f - 1f.  \n    brakeInput = Clamped 0f - 1f.  \n    steerInput = Clamped -1f - 1f. \n    clutchInput = Clamped 0f - 1f. \n    handbrakeInput = Clamped 0f - 1f. \n    boostInput = Clamped 0f - 1f.", MessageType.Info);
				EditorGUILayout.Space();
				GUI.color = originalGUIColor;

				EditorGUILayout.EndVertical ();

			}

//			EnableReWired = EditorGUILayout.ToggleLeft(new GUIContent("Enable ReWired", "It will enable ReWired support for RCC. Be sure you have imported latest ReWired to your project before enabling this."), EnableReWired);

			EditorGUILayout.Space();

//			if (!EnableReWired) {
//
//				GUI.color = new Color(.75f, .75f, 0f);
//				EditorGUILayout.HelpBox ("It will enable ReWired support for RCC. Be sure you have imported latest ReWired to your project before enabling this.", MessageType.Warning);
//				GUI.color = originalGUIColor;
//
//			} else {
//
////				EditorGUILayout.BeginVertical (GUI.skin.box);
////
////				GUILayout.Label("ReWired Settings", EditorStyles.boldLabel);
////
////				#if RTC_REWIRED
////				GUI.color = new Color(.75f, 1f, .75f);
////				EditorGUILayout.HelpBox("These input strings must be exactly same with your ReWired Inputs. You can edit them from ''ReWired Input Manager'' on your scene.", MessageType.Info);
////				GUI.color = originalGUIColor;
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_gasInput"), new GUIContent("Gas / Reverse Input Axis"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_steerInput"), new GUIContent("Steering Input Axis"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_mainGunXInput"), new GUIContent("Main Gun X Input Axis"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_mainGunYInput"), new GUIContent("Main Gun Y Input Axis"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_startEngineKB"), new GUIContent("Start / Stop Engine Key"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_handbrakeKB"), new GUIContent("Handbrake Key"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_headlightsKB"), new GUIContent("Toggle Headlights"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_changeCameraKB"), new GUIContent("Change Camera"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_enterExitVehicleKB"), new GUIContent("Get In & Get Out Of The Vehicle"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_fireKB"), new GUIContent("Fire"));
////				EditorGUILayout.PropertyField(serializedObject.FindProperty("RW_changeAmmunation"), new GUIContent("Change Ammunation"));
////				#endif
////
////				EditorGUILayout.Space();
////
////				EditorGUILayout.EndVertical ();
//
//			}

			EditorGUILayout.BeginVertical(GUI.skin.box);

			GUILayout.Label("Main Controller Settings", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("units"), new GUIContent("Units"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useVR"), new GUIContent("Use VR / XR"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useAutomaticGear"), new GUIContent("Use Automatic Gear"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useAutomaticClutch"), new GUIContent("Use Automatic Clutch"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("runEngineAtAwake"), new GUIContent("Engines Are Running At Awake"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoReverse"), new GUIContent("Auto Reverse"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoReset"), new GUIContent("Auto Reset"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("contactParticles"), new GUIContent("Contact Particles On Collision"));

			EditorGUILayout.EndVertical ();

		EditorGUILayout.EndVertical ();




		}

		EditorGUILayout.Space();

		foldUISettings = EditorGUILayout.Foldout(foldUISettings, "UI Settings");

		if(foldUISettings){
			
			EditorGUILayout.BeginVertical (GUI.skin.box);
			GUILayout.Label("UI Dashboard Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useTelemetry"), new GUIContent("Use Telemetry"));
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical ();

		}

		EditorGUILayout.Space();

		foldWheelPhysics = EditorGUILayout.Foldout(foldWheelPhysics, "Wheel Physics Settings");

		if(foldWheelPhysics){

			if(RCC_GroundMaterialsData.InstanceR.frictionsMass != null && RCC_GroundMaterialsData.InstanceR.frictionsMass.Length > 0){

					EditorGUILayout.BeginVertical (GUI.skin.box);
					GUILayout.Label("Ground Physic Materials", EditorStyles.boldLabel);

					physicMaterials = new PhysicMaterial[RCC_GroundMaterialsData.InstanceR.frictionsMass.Length];
					
					for (int i = 0; i < physicMaterials.Length; i++) {
						physicMaterials[i] = RCC_GroundMaterialsData.InstanceR.frictionsMass[i].groundMaterialValue;
						EditorGUILayout.BeginVertical(GUI.skin.box);
						EditorGUILayout.ObjectField("Ground Physic Materials " + i, physicMaterials[i], typeof(PhysicMaterial), false);
						EditorGUILayout.EndVertical();
					}

					EditorGUILayout.Space();

			}

			GUI.color = new Color(.5f, 1f, 1f, 1f);
			
			if(GUILayout.Button("Configure Ground Physic Materials")){
				Selection.activeObject = Resources.Load("RCC Assets/RCC_GroundMaterials") as RCC_GroundMaterialsData;
			}

			GUI.color = originalGUIColor;

			EditorGUILayout.EndVertical ();

		}

		EditorGUILayout.Space();

		foldSFX = EditorGUILayout.Foldout(foldSFX, "SFX Settings");

		if(foldSFX){

			EditorGUILayout.BeginVertical(GUI.skin.box);

			GUILayout.Label("Sound FX", EditorStyles.boldLabel);

			EditorGUILayout.Space();
			GUI.color = new Color(.5f, 1f, 1f, 1f);

			if(GUILayout.Button("Configure Wheel Slip Sounds"))
				Selection.activeObject = Resources.Load("RCC Assets/RCC_GroundMaterials") as RCC_GroundMaterialsData;
			
			GUI.color = originalGUIColor;
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("audioMixer"), new GUIContent("Main Audio Mixer"), false);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("crashClips"), new GUIContent("Crashing Sounds"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftingClips"), new GUIContent("Gear Shifting Sounds"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorClip"), new GUIContent("Indicator Clip"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("bumpClip"), new GUIContent("Bump Clip"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("exhaustFlameClips"), new GUIContent("Exhaust Flame Clips"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("NOSClip"), new GUIContent("NOS Clip"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("turboClip"), new GUIContent("Turbo Clip"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("blowoutClip"), new GUIContent("Blowout Clip"), true);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("reversingClip"), new GUIContent("Reverse Transmission Sound"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("windClip"), new GUIContent("Wind Sound"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeClip"), new GUIContent("Brake Sound"), true);
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxGearShiftingSoundVolume"), new GUIContent("Max Gear Shifting Sound Volume"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxCrashSoundVolume"), new GUIContent("Max Crash Sound Volume"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxWindSoundVolume"), new GUIContent("Max Wind Sound Volume"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxBrakeSoundVolume"), new GUIContent("Max Brake Sound Volume"), true);

			EditorGUILayout.EndVertical();

		}

		EditorGUILayout.Space();

		foldOptimization = EditorGUILayout.Foldout(foldOptimization, "Optimization");

		if(foldOptimization){

			EditorGUILayout.BeginVertical(GUI.skin.box);

			GUILayout.Label("Optimization", EditorStyles.boldLabel);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useLightsAsVertexLights"), new GUIContent("Use Lights As Vertex Lights On Vehicles"));
			GUI.color = new Color(.75f, 1f, .75f);
			EditorGUILayout.HelpBox("Always use vertex lights for mobile platform. Even only one pixel light will drop your performance dramaticaly!", MessageType.Info);
			GUI.color = originalGUIColor;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useLightProjectorForLightingEffect"), new GUIContent("Use Light Projector For Lighting Effect"));
			GUI.color = new Color(.75f, .75f, 0f);
			EditorGUILayout.HelpBox("Unity's Projector will be used for lighting effect. Be sure it effects to your road only. Select ignored layers below this section. Don't let projectors hits the vehicle itself. It may increase your drawcalls if it hits unnecessary high numbered materials. It should just hit the road, nothing else.", MessageType.Warning);
			GUI.color = originalGUIColor;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("projectorIgnoreLayer"), new GUIContent("Light Projector Ignore Layer"));
			GUI.color = new Color(.75f, 1f, .75f);
			EditorGUILayout.HelpBox("For ex, 4 Audio Sources will be created for each wheelslip SFX. This option merges them to only 1 Audio Source.", MessageType.Info);
			GUI.color = originalGUIColor;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dontUseAnyParticleEffects"), new GUIContent("Do Not Use Any Particle Effects"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dontUseSkidmarks"), new GUIContent("Do Not Use Skidmarks"));

			GUI.color = originalGUIColor;

			EditorGUILayout.Space();

			EditorGUILayout.EndVertical();

		}

		EditorGUILayout.Space();

		foldTagsAndLayers = EditorGUILayout.Foldout(foldTagsAndLayers, "Tags & Layers");

		if (foldTagsAndLayers) {

			EditorGUILayout.BeginVertical (GUI.skin.box);

			GUILayout.Label ("Tags & Layers", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("setTagsAndLayers"), new GUIContent("Set Tags And Layers Auto"), false);

			if (RCCSettingsAsset.setTagsAndLayersFlag) {

				EditorGUILayout.PropertyField (serializedObject.FindProperty ("RCCLayer"), new GUIContent ("Vehicle Layer"), false);
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("RCCTag"), new GUIContent ("Vehicle Tag"), false);
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("tagAllChildrenGameobjects"), new GUIContent ("Tag All Children Gameobjects"), false);
				GUI.color = new Color (.75f, 1f, .75f);
				EditorGUILayout.HelpBox ("Be sure you have that tag and layer in your Tags & Layers", MessageType.Warning);
				EditorGUILayout.HelpBox ("All vehicles powered by Realistic Car Controller are using this layer. What does this layer do? It was used for masking wheel rays, light masks, and projector masks. Just create a new layer for vehicles from Edit --> Project Settings --> Tags & Layers, and select the layer here.", MessageType.Info);
				GUI.color = originalGUIColor;

			}

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

		}

		EditorGUILayout.BeginVertical (GUI.skin.box);

		GUILayout.Label ("Resources", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("headLights"), new GUIContent("Head Lights"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeLights"), new GUIContent("Brake Lights"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("reverseLights"), new GUIContent("Reverse Lights"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("indicatorLights"), new GUIContent("Indicator Lights"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("lightTrailers"), new GUIContent("Light Trailers"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("mirrors"), new GUIContent("Mirrors"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("skidmarksManager"), new GUIContent("Skidmarks Manager"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("projector"), new GUIContent("Light Projector"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("exhaustGas"), new GUIContent("Exhaust Gas"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("chassisJoint"), new GUIContent("Chassis Joint"), false);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCMainCamera"), new GUIContent("RCC Main Camera"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("hoodCamera"), new GUIContent("Hood Camera"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("cinematicCamera"), new GUIContent("Cinematic Camera"), false);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCCanvas"), new GUIContent("RCC UI Canvas"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("RCCTelemetry"), new GUIContent("RCC Telemetry Canvas"), false);

		EditorGUILayout.Space();

		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndScrollView();
		
		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical (GUI.skin.button);

		GUI.color = new Color(.75f, 1f, .75f);

		GUI.color = new Color(.5f, 1f, 1f, 1f);
		
		if(GUILayout.Button("Reset To Defaults")){
			ResetToDefaults();
			Debug.Log("Resetted To Defaults!");
		}
		
		if(GUILayout.Button("Open PDF Documentation")){
			string url = "http://www.bonecrackergames.com/realistic-car-controller";
			Application.OpenURL(url);
		}

		GUI.color = originalGUIColor;
		
		EditorGUILayout.LabelField("Realistic Car Controller " + RCC_SettingsData.RCCVersionName + " \nBoneCracker Games", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

		EditorGUILayout.LabelField("Created by Buğra Özdoğanlar", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
		
		if(GUI.changed)
			EditorUtility.SetDirty(RCCSettingsAsset);

	}

	void ResetToDefaults(){

		RCCSettingsAsset.overrideFixedTimeStepFlag = true;
		RCCSettingsAsset.fixedTimeStepValue = .02f;
		RCCSettingsAsset.maxAngularVelocityValue = 6f;
//		RCCSettingsAsset.behaviorType = RCC_Settings.BehaviorType.Custom;

		RCCSettingsAsset.verticalInputValue = "Vertical";
		RCCSettingsAsset.horizontalInputValue = "Horizontal";
		RCCSettingsAsset.handbrakeKBCode = KeyCode.Space;
		RCCSettingsAsset.startEngineKBCode = KeyCode.I;
		RCCSettingsAsset.lowBeamHeadlightsKBCode = KeyCode.L;
		RCCSettingsAsset.highBeamHeadlightsKBCode = KeyCode.K;
		RCCSettingsAsset.rightIndicatorKBCode = KeyCode.E;
		RCCSettingsAsset.leftIndicatorKBCode = KeyCode.Q;
		RCCSettingsAsset.hazardIndicatorKBCode = KeyCode.Z;
		RCCSettingsAsset.shiftGearUpCode = KeyCode.LeftShift;
		RCCSettingsAsset.shiftGearDownCode = KeyCode.LeftControl;
		RCCSettingsAsset.NGearCode = KeyCode.N;
		RCCSettingsAsset.boostKBCode = KeyCode.F;
		RCCSettingsAsset.slowMotionKBCode = KeyCode.G;
		RCCSettingsAsset.changeCameraKBCode = KeyCode.C;
		RCCSettingsAsset.recordKBCode = KeyCode.R;
		RCCSettingsAsset.playbackKBCode = KeyCode.P;

		RCCSettingsAsset.useAutomaticGearFlag = true;
		RCCSettingsAsset.runEngineAtAwakeFlag = true;
		RCCSettingsAsset.autoReverseFlag = true;
		RCCSettingsAsset.autoResetFlag = true;
		RCCSettingsAsset.unitsR = RCC_SettingsData.Units.KMH;
		RCCSettingsAsset.useTelemetryObject = false;
		RCCSettingsAsset.mobileControllerObject = RCC_SettingsData.MobileController.TouchScreen;
		RCCSettingsAsset.UIButtonSensitivityValue = 3f;
		RCCSettingsAsset.UIButtonGravityValue = 5f;
		RCCSettingsAsset.gyroSensitivityValue = 2f;
		RCCSettingsAsset.useLightsAsVertexLightsValue = true;
		RCCSettingsAsset.useLightProjectorForLightingEffectValue = false;
		RCCSettingsAsset.setTagsAndLayersFlag = true;
		RCCSettingsAsset.RCCLayerValue = "RCC";
		RCCSettingsAsset.RCCTagValue = "Player";
		RCCSettingsAsset.tagAllChildrenGameobjectsFlag = false;
		RCCSettingsAsset.dontUseAnyParticleEffectsFlag = false;
		RCCSettingsAsset.dontUseSkidmarksFlag = false;
		RCCSettingsAsset.maxGearShiftingSoundVolumeValue = .25f;
		RCCSettingsAsset.maxCrashSoundVolumeValue = 1f;
		RCCSettingsAsset.maxWindSoundVolumeValue = .1f;
		RCCSettingsAsset.maxBrakeSoundVolumeValue = .1f;
		RCCSettingsAsset.foldGeneralSettingsFlag = false;
		RCCSettingsAsset.foldControllerSettingsFlag = false;
		RCCSettingsAsset.foldUISettingsFlag = false;
		RCCSettingsAsset.foldWheelPhysicsFlag = false;
		RCCSettingsAsset.foldSFXFlag = false;
		RCCSettingsAsset.foldOptimizationFlag = false;
		RCCSettingsAsset.foldTagsAndLayersFlag = false;

	}

}
