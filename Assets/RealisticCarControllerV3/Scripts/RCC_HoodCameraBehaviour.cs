﻿//----------------------------------------------
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
/// RCC Camera will be parented to this gameobject when current camera mode is Hood Camera.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Camera/RCC Hood Camera")]
public class RCC_HoodCameraBehaviour : MonoBehaviour {

	public void LaunchFixShake(){

		StartCoroutine (FixShakeDelayedCoroutine());
		
	}

	private IEnumerator FixShakeDelayedCoroutine(){

		if (!GetComponent<Rigidbody> ())
			yield break;

		yield return new WaitForFixedUpdate ();
		GetComponent<Rigidbody> ().interpolation = RigidbodyInterpolation.None;
		yield return new WaitForFixedUpdate ();
		GetComponent<Rigidbody> ().interpolation = RigidbodyInterpolation.Interpolate;

	}

}

