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

public class RCC_XRToggleController : MonoBehaviour {

	[FormerlySerializedAs("XREnabled")] public bool XREnabledFlag = false;

	private void Update () {

		if (Input.GetKeyDown (KeyCode.V))
			ToggleXREnable ();

	}

	private void ToggleXREnable(){

		UnityEngine.XR.XRSettings.enabled = !UnityEngine.XR.XRSettings.enabled;
		XREnabledFlag = UnityEngine.XR.XRSettings.enabled;

	}

}
