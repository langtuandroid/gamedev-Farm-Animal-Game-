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
/// Exhaust based on Particle System. Based on vehicle controller's throttle situation.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Exhaust")]
public class RCC_Exhaust : RCC_CoreMain {

	// Getting an Instance of Main Shared RCC Settings.
	#region RCC Settings Instance

	private RCC_SettingsData RCCSettingsInstance;
	private RCC_SettingsData RCCSettings {
		get {
			if (RCCSettingsInstance == null) {
				RCCSettingsInstance = RCC_SettingsData.InstanceR;
				return RCCSettingsInstance;
			}
			return RCCSettingsInstance;
		}
	}

	#endregion

	private RCC_CarMainControllerV3 carController;
	private ParticleSystem particle;
	private ParticleSystem.EmissionModule emission;
	public ParticleSystem flame;
	private ParticleSystem.EmissionModule subEmission;

	private Light flameLight;
	private LensFlare lensFlare;

	public float flareBrightness = 1f;
	private float finalFlareBrightness;

	public float flameTime = 0f;
	private AudioSource flameSource;

	public Color flameColor = Color.red;
	public Color boostFlameColor = Color.blue;

	public bool previewFlames = false;

	public float minEmission = 5f;
	public float maxEmission = 50f;

	public float minSize = 2.5f;
	public float maxSize = 5f;

	public float minSpeed = .5f;
	public float maxSpeed = 5f;

	void Start () {

		if (RCCSettings.dontUseAnyParticleEffectsFlag) {
			Destroy (gameObject);
			return;
		}

		carController = GetComponentInParent<RCC_CarMainControllerV3>();
		particle = GetComponent<ParticleSystem>();
		emission = particle.emission;

		if(flame){

			subEmission = flame.emission;
			flameLight = flame.GetComponentInChildren<Light>();
			flameSource = CreateNewAudioSource(RCCSettings.audioMixerValue, gameObject, "Exhaust Flame AudioSource", 10f, 25f, .5f, RCCSettings.exhaustFlameClipsMass[0], false, false, false);
			flameLight.renderMode = RCCSettings.useLightsAsVertexLightsValue ? LightRenderMode.ForceVertex : LightRenderMode.ForcePixel;

		}

		lensFlare = GetComponentInChildren<LensFlare> ();

		if (flameLight) {

			if (flameLight.flare != null)
				flameLight.flare = null;

		}

	}

	void Update () {

		if(!carController || !particle)
			return;

		Smoke ();
		Flame ();

		if (lensFlare)
			LensFlare ();

	}

	void Smoke(){

		if (carController.engineRunning) {

			var main = particle.main;

			if (carController.speed < 20) {

				if (!emission.enabled)
					emission.enabled = true;

				if (carController.throttleInput > .35f) {

					emission.rateOverTime = maxEmission;
					main.startSpeed = maxSpeed;
					main.startSize = maxSize;

				} else {
					
					emission.rateOverTime = minEmission;
					main.startSpeed = minSpeed;
					main.startSize = minSize;

				}

			} else {

				if (emission.enabled)
					emission.enabled = false;

			}

		} else {

			if (emission.enabled)
				emission.enabled = false;

		}

	}

	void Flame(){

		if(carController.engineRunning){

			var main = flame.main;

			if(carController.throttleInput >= .25f)
				flameTime = 0f;

			if(((carController.useExhaustFlame && carController.engineRPM >= 5000 && carController.engineRPM <= 5500 && carController.throttleInput <= .25f && flameTime <= .5f) || carController.boostInput >= .75f) || previewFlames){

				flameTime += Time.deltaTime;
				subEmission.enabled = true;

				if(flameLight)
					flameLight.intensity = flameSource.pitch * 3f * Random.Range(.25f, 1f) ;

				if(carController.boostInput >= .75f && flame){
					main.startColor = boostFlameColor;
					flameLight.color = main.startColor.color;
				}else{
					main.startColor = flameColor;
					flameLight.color = main.startColor.color;
				}

				if(!flameSource.isPlaying){
					flameSource.clip = RCCSettings.exhaustFlameClipsMass[Random.Range(0, RCCSettings.exhaustFlameClipsMass.Length)];
					flameSource.Play();
				}

			}else{

				subEmission.enabled = false;

				if(flameLight)
					flameLight.intensity = 0f;
				if(flameSource.isPlaying)
					flameSource.Stop();

			}

		}else{

			if(emission.enabled)
				emission.enabled = false;

			subEmission.enabled = false;

			if(flameLight)
				flameLight.intensity = 0f;
			if(flameSource.isPlaying)
				flameSource.Stop();

		}

	}

	private void LensFlare(){

		if (!RCC_SceneManager.Instance.activePlayerCamera)
			return;

		float distanceTocam = Vector3.Distance(transform.position, RCC_SceneManager.Instance.activePlayerCamera.thisCamera.transform.position);
		float angle = Vector3.Angle(transform.forward,  RCC_SceneManager.Instance.activePlayerCamera.thisCamera.transform.position - transform.position);

		if(angle != 0)
			finalFlareBrightness = flareBrightness * (4 / distanceTocam) * ((100f - (1.11f * angle)) / 100f) / 2f;

		lensFlare.brightness = finalFlareBrightness * flameLight.intensity;
		lensFlare.color = flameLight.color;

	}

}
