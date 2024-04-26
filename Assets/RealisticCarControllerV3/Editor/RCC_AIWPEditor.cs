//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RCC_AIWaypointsDrawingContainer))]
public class RCC_AIWPEditor : Editor {
	
	RCC_AIWaypointsDrawingContainer wpScript;
	
	public override void  OnInspectorGUI () {
		
		serializedObject.Update();

		wpScript = (RCC_AIWaypointsDrawingContainer)target;

		EditorGUILayout.HelpBox("Create Waypoints By Shift + Left Mouse Button On Your Road", MessageType.Info);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"), new GUIContent("Waypoints", "Waypoints"), true);

		foreach (Transform item in wpScript.transform) {

			if (item.gameObject.GetComponent<RCC_WaypointR> () == null)
				item.gameObject.AddComponent<RCC_WaypointR> ();
			
		}

		if (GUILayout.Button ("Delete Waypoints")) {
			
			foreach (RCC_WaypointR t in wpScript.waypointsList) {
				DestroyImmediate (t.gameObject);
			}
			wpScript.waypointsList.Clear ();

		}

		serializedObject.ApplyModifiedProperties();
		
	}

	void OnSceneGUI(){

		Event e = Event.current;
		wpScript = (RCC_AIWaypointsDrawingContainer)target;

		if(e != null){

			if(e.isMouse && e.shift && e.type == EventType.MouseDown){

				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(ray, out hit, 5000.0f)) {

					Vector3 newTilePosition = hit.point;

					GameObject wp = new GameObject("Waypoint " + wpScript.waypointsList.Count.ToString());
					wp.AddComponent<RCC_WaypointR> ();
					wp.transform.position = newTilePosition;
					wp.transform.SetParent(wpScript.transform);

					GetWaypoints();

				}

			}

			if(wpScript)
				Selection.activeGameObject = wpScript.gameObject;

		}

		GetWaypoints();

	}
	
	public void GetWaypoints(){
		
		wpScript.waypointsList = new List<RCC_WaypointR>();
		
		RCC_WaypointR[] allTransforms = wpScript.transform.GetComponentsInChildren<RCC_WaypointR>();
		
		foreach(RCC_WaypointR t in allTransforms){
			
			if(t != wpScript.transform)
				wpScript.waypointsList.Add(t);
			
		}
		
	}
	
}
