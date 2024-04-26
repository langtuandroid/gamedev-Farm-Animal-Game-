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

/// <summary>
/// Locks rotation of the shadow projector to avoid stretching.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Shadow")]
public class RCC_ShadowRotConstController : MonoBehaviour {

	private Transform rootTransform;

	private void Start () {

		rootTransform = GetComponentInParent<RCC_CarMainControllerV3>().transform;
	
	}

	private void Update () {

		transform.rotation = Quaternion.Euler(90f, rootTransform.eulerAngles.y, 0f);
	
	}

}
