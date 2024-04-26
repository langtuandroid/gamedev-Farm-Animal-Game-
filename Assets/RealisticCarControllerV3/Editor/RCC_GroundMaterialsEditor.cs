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
using System.Linq;

[CustomEditor(typeof(RCC_GroundMaterialsData))]
public class RCC_GroundMaterialsEditor : Editor {

	RCC_GroundMaterialsData prop;

	Vector2 scrollPos;
	List<RCC_GroundMaterialsData.GroundMaterialFrictionsData> groundMaterials = new List<RCC_GroundMaterialsData.GroundMaterialFrictionsData>();

	Color orgColor;

	public override void OnInspectorGUI (){

		serializedObject.Update();
		prop = (RCC_GroundMaterialsData)target;
		orgColor = GUI.color;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Wheels Editor", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("This editor will keep update necessary .asset files in your project. Don't change directory of the ''Resources/RCC Assets''.", EditorStyles.helpBox);
		EditorGUILayout.Space();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false );

		EditorGUIUtility.labelWidth = 110f;
//		EditorGUIUtility.fieldWidth = 10f;

		GUILayout.Label("Ground Materials", EditorStyles.boldLabel);

		for (int i = 0; i < prop.frictionsMass.Length; i++) {

			EditorGUILayout.BeginVertical(GUI.skin.box);
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal ();

			if(prop.frictionsMass[i].groundMaterialValue)
				EditorGUILayout.LabelField(prop.frictionsMass[i].groundMaterialValue.name + (i == 0 ? " (Default)" : ""), EditorStyles.boldLabel);

			GUI.color = Color.red;		if(GUILayout.Button("X", GUILayout.Width(25f))){RemoveGroundMaterial(i);}	GUI.color = orgColor;

			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			prop.frictionsMass[i].groundMaterialValue = (PhysicMaterial)EditorGUILayout.ObjectField("Physic Material", prop.frictionsMass[i].groundMaterialValue, typeof(PhysicMaterial), false, GUILayout.Width(250f));
			prop.frictionsMass[i].forwardStiffnessValue = EditorGUILayout.FloatField("Forward Stiffness", prop.frictionsMass[i].forwardStiffnessValue, GUILayout.Width(150f));

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			prop.frictionsMass[i].groundAudioSound = (AudioClip)EditorGUILayout.ObjectField("Wheel Sound", prop.frictionsMass[i].groundAudioSound, typeof(AudioClip), false, GUILayout.Width(250f));
			prop.frictionsMass[i].sidewaysStiffnessValue = EditorGUILayout.FloatField("Sideways Stiffness", prop.frictionsMass[i].sidewaysStiffnessValue, GUILayout.Width(150f));

			EditorGUILayout.EndHorizontal();

			prop.frictionsMass[i].volumeValue = EditorGUILayout.Slider("Volume", prop.frictionsMass[i].volumeValue, 0f, 1f, GUILayout.Width(250f));

			EditorGUILayout.BeginHorizontal();
			prop.frictionsMass[i].groundParticlesObject = (GameObject)EditorGUILayout.ObjectField("Wheel Particles", prop.frictionsMass[i].groundParticlesObject, typeof(GameObject), false, GUILayout.Width(200f));
			prop.frictionsMass[i].skidmarkController = (RCC_SkidmarksController)EditorGUILayout.ObjectField("Wheel Skidmarks", prop.frictionsMass[i].skidmarkController, typeof(RCC_SkidmarksController), false, GUILayout.Width(200f));

			EditorGUILayout.Space();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			prop.frictionsMass[i].slipValue = EditorGUILayout.FloatField("Slip", prop.frictionsMass[i].slipValue, GUILayout.Width(150f));
			prop.frictionsMass[i].dampValue = EditorGUILayout.FloatField("Damp", prop.frictionsMass[i].dampValue, GUILayout.Width(150f));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();

		}

		EditorGUILayout.BeginVertical(GUI.skin.box);
		GUILayout.Label("Terrain Ground Materials", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField (serializedObject.FindProperty ("terrainFrictions"), new GUIContent ("Terrain Physic Material"), true);

		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();

		GUI.color = Color.cyan;

		if(GUILayout.Button("Create New Ground Material")){

			AddNewWheel();

		}

		if(GUILayout.Button("--< Return To Asset Settings")){

			OpenGeneralSettings();

		}

		GUI.color = orgColor;

		EditorGUILayout.EndScrollView();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Created by Buğra Özdoğanlar\nBoneCrackerGames", EditorStyles.centeredGreyMiniLabel, GUILayout.MaxHeight(50f));

		serializedObject.ApplyModifiedProperties();

		if(GUI.changed)
			EditorUtility.SetDirty(prop);

	}

	void AddNewWheel(){

		groundMaterials.Clear();
		groundMaterials.AddRange(prop.frictionsMass);
		RCC_GroundMaterialsData.GroundMaterialFrictionsData newGroundMaterial = new RCC_GroundMaterialsData.GroundMaterialFrictionsData();
		groundMaterials.Add(newGroundMaterial);
		prop.frictionsMass = groundMaterials.ToArray();

	}

	void RemoveGroundMaterial(int index){

		groundMaterials.Clear();
		groundMaterials.AddRange(prop.frictionsMass);
		groundMaterials.RemoveAt(index);
		prop.frictionsMass = groundMaterials.ToArray();

	}

	void OpenGeneralSettings(){

		Selection.activeObject =RCC_SettingsData.InstanceR;

	}

}
