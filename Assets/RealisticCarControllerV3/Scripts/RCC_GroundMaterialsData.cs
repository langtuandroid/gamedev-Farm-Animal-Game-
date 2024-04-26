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
using UnityEngine.Serialization;

[System.Serializable]
public class RCC_GroundMaterialsData : ScriptableObject {
	
	#region singleton
	private static RCC_GroundMaterialsData instanceR;
	public static RCC_GroundMaterialsData InstanceR{	get{if(instanceR == null) instanceR = Resources.Load("RCC Assets/RCC_GroundMaterials") as RCC_GroundMaterialsData; return instanceR;}}
	#endregion

	[System.Serializable]
	public class GroundMaterialFrictionsData{
		
		[FormerlySerializedAs("groundMaterial")] public PhysicMaterial groundMaterialValue;
		[FormerlySerializedAs("forwardStiffness")] public float forwardStiffnessValue = 1f;
		[FormerlySerializedAs("sidewaysStiffness")] public float sidewaysStiffnessValue = 1f;
		[FormerlySerializedAs("slip")] public float slipValue = .25f;
		[FormerlySerializedAs("damp")] public float dampValue = 1f;
		[FormerlySerializedAs("volume")] [Range(0f, 1f)]public float volumeValue = 1f;
		[FormerlySerializedAs("groundParticles")] public GameObject groundParticlesObject;
		[FormerlySerializedAs("groundSound")] public AudioClip groundAudioSound;
		[FormerlySerializedAs("skidmark")] public RCC_SkidmarksController skidmarkController;

	}
		
	[FormerlySerializedAs("frictions")] public GroundMaterialFrictionsData[] frictionsMass;

	[System.Serializable]
	public class TerrainFrictionsData{

		[FormerlySerializedAs("groundMaterial")] public PhysicMaterial groundPhysicMaterial;

		[System.Serializable]
		public class SplatmapIndexesData{

			[FormerlySerializedAs("index")] public int indexValue = 0;
			[FormerlySerializedAs("groundMaterial")] public PhysicMaterial groundPhysicMaterial;

		}

		[FormerlySerializedAs("splatmapIndexes")] public SplatmapIndexesData[] splatmapIndexesMass;

	}

	[FormerlySerializedAs("terrainFrictions")] public TerrainFrictionsData[] terrainFrictionsMass;

}


