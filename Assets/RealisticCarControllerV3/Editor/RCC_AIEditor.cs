﻿//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RCC_AICarMovementController)), CanEditMultipleObjects]
public class RCC_AIEditor : Editor {

	RCC_AICarMovementController aiController;

	[MenuItem("Tools/BoneCracker Games/Realistic Car Controller/AI Controller/Add AI Controller To Vehicle", false)]
	static void CreateAIBehavior(){

		if (!Selection.activeGameObject.GetComponentInParent<RCC_CarMainControllerV3> ()) {
			EditorUtility.DisplayDialog("Your Vehicle Has Not RCC_CarControllerV3", "Your Vehicle Has Not RCC_CarControllerV3.", "Close");
			return;
		}

		if (Selection.activeGameObject.GetComponentInParent<RCC_AICarMovementController> ()) {
			EditorUtility.DisplayDialog("Your Vehicle Already Has AI Car Controller", "Your Vehicle Already Has AI Car Controller", "Close");
			return;
		}

		Selection.activeGameObject.GetComponentInParent<RCC_CarMainControllerV3> ().gameObject.AddComponent<RCC_AICarMovementController>();
		GameObject vehicle = Selection.activeGameObject.GetComponentInParent<RCC_CarMainControllerV3> ().gameObject;
		Selection.activeGameObject = vehicle;

	}

	[MenuItem("Tools/BoneCracker Games/Realistic Car Controller/AI Controller/Add AI Controller To Vehicle", true)]
	static bool CheckAIBehavior() {
		if(Selection.gameObjects.Length > 1 || !Selection.activeTransform)
			return false;
		else
			return true;
	}

	[MenuItem("Tools/BoneCracker Games/Realistic Car Controller/AI Controller/Add Waypoints Container To Scene", false)]
	static void CreateWaypointsContainer(){

		GameObject wp = new GameObject("Waypoints Container");
		wp.transform.position = Vector3.zero;
		wp.transform.rotation = Quaternion.identity;
		wp.AddComponent<RCC_AIWaypointsDrawingContainer>();
		Selection.activeGameObject = wp;

	}

	[MenuItem("Tools/BoneCracker Games/Realistic Car Controller/AI Controller/Add BrakeZones Container To Scene", false)]
	static void CreateBrakeZonesContainer(){

		if(GameObject.FindObjectOfType<RCC_AIBrakeZonesDrawingContainer>() == null){

			GameObject bz = new GameObject("Brake Zones Container");
			bz.transform.position = Vector3.zero;
			bz.transform.rotation = Quaternion.identity;
			bz.AddComponent<RCC_AIBrakeZonesDrawingContainer>();
			Selection.activeGameObject = bz;

		}else{
			EditorUtility.DisplayDialog("Your Scene Already Has Brake Zones Container", "Your Scene Already Has Brake Zones", "Close");
		}

	}

	public override void OnInspectorGUI () {

		serializedObject.Update();

		aiController = (RCC_AICarMovementController)target;

		aiController.gameObject.GetComponent<RCC_CarMainControllerV3>().externalController = true;

		//		if(aiController.gameObject.GetComponent<RCC_CarControllerV3>().canEngineStall)
		//			aiController.gameObject.GetComponent<RCC_CarControllerV3>().canEngineStall = false;
		//
		//		if(!aiController.gameObject.GetComponent<RCC_CarControllerV3>().autoReverse)
		//			aiController.gameObject.GetComponent<RCC_CarControllerV3>().autoReverse = true;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("navigationMode"), new GUIContent("Navigation Mode", "Navigation Mode."), false);

		EditorGUI.indentLevel++;

		if (aiController.navigationAIMode == RCC_AICarMovementController.NavigationMode.FollowWaypoints) {
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("waypointsContainer"), new GUIContent ("Waypoints Container", "Waypoints Container."), false);
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("currentWaypointIndex"), new GUIContent ("Current Waypoint Index", "Current Waypoint Index."), false);
		} else {
			//EditorGUILayout.PropertyField (serializedObject.FindProperty ("targetChase"), new GUIContent ("Target For Chase", "Target For Chase."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTag"), new GUIContent("Target Tag For Chase/Follow", "Target Tag For Chase/Follow."), false);
		}

		switch (aiController.navigationAIMode) {

			case RCC_AICarMovementController.NavigationMode.FollowWaypoints:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("nextWaypointPassDistance"), new GUIContent("Next Waypoint Pass Radius", "If vehicle gets closer then this radius, goes to next waypoint."), false);
				break;

			case RCC_AICarMovementController.NavigationMode.ChaseTarget:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("chaseDistance"), new GUIContent("Chase Distance", "Chasing distance."), false);
				break;

			case RCC_AICarMovementController.NavigationMode.FollowTarget:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("startFollowDistance"), new GUIContent("Start Follow Distance", "Start follow distance."), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("stopFollowDistance"), new GUIContent("Stop Follow Distance", "Stop follow distance."), false);
				break;

		}

		EditorGUI.indentLevel--;
		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("useRaycasts"), new GUIContent("Use Raycasts", "Use Raycasts For Avoid Dynamic Objects."), false);

		if (aiController.useRaycastsFlag) {

			EditorGUILayout.PropertyField(serializedObject.FindProperty("obstacleLayers"), new GUIContent("Obstacle Layers", "Obstacle Layers For Avoid Dynamic Objects."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastLength"), new GUIContent("Ray Distance", "Rays For Avoid Dynamic Objects."), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastAngle"), new GUIContent("Ray Angle", "Ray Angles."), false);

		}
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("limitSpeed"), new GUIContent("Limit Speed", "Limits The Speed."), false);

		if(aiController.limitSpeedFlag)
			EditorGUILayout.Slider(serializedObject.FindProperty("maximumSpeed"), 0f, aiController.GetComponent<RCC_CarMainControllerV3>().maxspeed);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothedSteer"), new GUIContent("Smooth Steering", "Smooth Steering."), false);

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Current Waypoint: ", aiController.currentWaypointIndexValue.ToString());
		EditorGUILayout.LabelField("Laps: ", aiController.lapValue.ToString());
		EditorGUILayout.LabelField("Total Waypoints Passed: ", aiController.totalWaypointPassedValue.ToString());
		EditorGUILayout.LabelField("Obstacle: ", aiController.obstacleObject != null ? aiController.obstacleObject.ToString() : "None");
		EditorGUILayout.LabelField("Ignoring Waypoint Due To Unexpected Obstacle: ", aiController.ignoreWaypointNowFlag.ToString());
		EditorGUILayout.Separator();

		serializedObject.ApplyModifiedProperties();

	}

}
