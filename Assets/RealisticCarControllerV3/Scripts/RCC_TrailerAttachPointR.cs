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

public class RCC_TrailerAttachPointR : MonoBehaviour {

	void OnTriggerEnter(Collider col){

		RCC_TrailerAttachPointR otherAttacher = col.gameObject.GetComponent<RCC_TrailerAttachPointR> ();

		if (!otherAttacher)
			return;

		RCC_CarMainControllerV3 otherVehicle = otherAttacher.gameObject.GetComponentInParent<RCC_CarMainControllerV3> ();

		if (!otherVehicle)
			return;

		transform.root.SendMessage ("AttachTrailer", otherVehicle, SendMessageOptions.DontRequireReceiver);

	}

}
