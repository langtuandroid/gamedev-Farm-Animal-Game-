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

/// <summary>
/// Animation attached to "Animation Pivot" of the Cinematic Camera is feeding FOV float value.
/// </summary>
public class RCC_FOVForCinematicCameraBehaviour : MonoBehaviour {

	private RCC_CinematicCamera currCinematicCamera;
	[FormerlySerializedAs("FOV")] public float FOVValue = 30f;

	private void Awake () {

		currCinematicCamera = GetComponentInParent<RCC_CinematicCamera> ();
	
	}

	private void Update () {

		currCinematicCamera.targetFOV = FOVValue;
	
	}

}
