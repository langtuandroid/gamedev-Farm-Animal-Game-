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
using UnityEngine.Serialization;

public class RCC_SkidmarksGroundManager : MonoBehaviour {

	[FormerlySerializedAs("skidmarks")] [SerializeField] private RCC_SkidmarksController[] skidmarksMass;
	[FormerlySerializedAs("skidmarksIndexes")] [SerializeField] private int[] skidmarksIndexesMass;
	private int _lastGroundIndexValue = 0;

	private void Start () {
		
		skidmarksMass = new RCC_SkidmarksController[RCC_GroundMaterialsData.InstanceR.frictionsMass.Length];
		skidmarksIndexesMass = new int[skidmarksMass.Length];

		for (int i = 0; i < skidmarksMass.Length; i++) {
			
			skidmarksMass [i] = Instantiate (RCC_GroundMaterialsData.InstanceR.frictionsMass [i].skidmarkController, Vector3.zero, Quaternion.identity);
			skidmarksMass [i].transform.name = skidmarksMass[i].transform.name + "_" + RCC_GroundMaterialsData.InstanceR.frictionsMass[i].groundMaterialValue.name;
			skidmarksMass [i].transform.SetParent (transform, true);

		}
		
	}
	
	// Function called by the wheels that is skidding. Gathers all the information needed to
	// create the mesh later. Sets the intensity of the skidmark section b setting the alpha
	// of the vertex color.
	public int AddGroundSkidMark ( Vector3 pos ,   Vector3 normal ,   float intensity ,   int lastIndex, int groundIndex  ){

		if (_lastGroundIndexValue != groundIndex){

			_lastGroundIndexValue = groundIndex;
			return -1;

		}

		skidmarksIndexesMass[groundIndex] = skidmarksMass [groundIndex].AddWheelSkidMark (pos, normal, intensity, lastIndex);
		
		return skidmarksIndexesMass[groundIndex];

	}

	// Function called by the wheels that is skidding. Gathers all the information needed to
	// create the mesh later. Sets the intensity of the skidmark section b setting the alpha
	// of the vertex color.
	public int AddGroundSkidMark ( Vector3 pos ,   Vector3 normal ,   float intensity ,   int lastIndex, int groundIndex, float width){

		if (_lastGroundIndexValue != groundIndex){

			_lastGroundIndexValue = groundIndex;
			return -1;

		}

		skidmarksMass [groundIndex].markWidthValue = width;
		skidmarksIndexesMass[groundIndex] = skidmarksMass [groundIndex].AddWheelSkidMark (pos, normal, intensity, lastIndex);

		return skidmarksIndexesMass[groundIndex];

	}

}
