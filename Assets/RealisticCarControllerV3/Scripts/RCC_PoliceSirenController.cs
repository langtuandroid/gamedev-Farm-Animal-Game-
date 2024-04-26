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

public class RCC_PoliceSirenController : MonoBehaviour {

	private RCC_AICarMovementController AICar;

	[FormerlySerializedAs("sirenMode")] public SirenMode sirenModeR;
	public enum SirenMode{Off, On}

	[FormerlySerializedAs("redLights")] public Light[] redLightsMass;
	[FormerlySerializedAs("blueLights")] public Light[] blueLightsMass;

	private void Start () {

		AICar = GetComponentInParent<RCC_AICarMovementController> ();
		
	}

	private void Update () {

		switch (sirenModeR) {

		case SirenMode.Off:

			for (int i = 0; i < redLightsMass.Length; i++)
				redLightsMass[i].intensity = Mathf.Lerp (redLightsMass[i].intensity, 0f, Time.deltaTime * 50f);

			for (int i = 0; i < blueLightsMass.Length; i++)
				blueLightsMass[i].intensity = Mathf.Lerp (blueLightsMass[i].intensity, 0f, Time.deltaTime * 50f);

			break;

		case SirenMode.On:

			if(Mathf.Approximately((int)(Time.time)%2, 0) && Mathf.Approximately((int)(Time.time * 20)%3, 0)){

				for (int i = 0; i < redLightsMass.Length; i++)
					redLightsMass[i].intensity = Mathf.Lerp (redLightsMass[i].intensity, 1f, Time.deltaTime * 50f);
				
			}else{

				for (int i = 0; i < redLightsMass.Length; i++)
					redLightsMass[i].intensity = Mathf.Lerp (redLightsMass[i].intensity, 0f, Time.deltaTime * 10f);

				if(Mathf.Approximately((int)(Time.time * 20)%3, 0)){
					
					for (int i = 0; i < blueLightsMass.Length; i++)
						blueLightsMass[i].intensity = Mathf.Lerp (blueLightsMass[i].intensity, 1f, Time.deltaTime * 50f);
					
				}else{
					
					for (int i = 0; i < blueLightsMass.Length; i++)
						blueLightsMass[i].intensity = Mathf.Lerp (blueLightsMass[i].intensity, 0f, Time.deltaTime * 10f);
					
				}

			}

			break;

		}

		if (AICar) {

			if (AICar.targetChaseTransform != null)
				sirenModeR = SirenMode.On;
			else
				sirenModeR = SirenMode.Off;

		}
		
	}

	public void SetSirenState(bool state){

		if (state)
			sirenModeR = SirenMode.On;
		else
			sirenModeR = SirenMode.Off;

	}

}
