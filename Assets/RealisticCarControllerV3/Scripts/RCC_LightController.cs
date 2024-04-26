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
/// General lighting system for vehicles. It has all kind of lights such as Headlight, Brake Light, Indicator Light, Reverse Light.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Light/RCC Light")]
public class RCC_LightController : RCC_CoreMain {

	// Getting an Instance of Main Shared RCC Settings.
	#region RCC Settings Instance

	private RCC_SettingsData RCCSettingsInstanceR;
	private RCC_SettingsData RCCSettingsR {
		get {
			if (RCCSettingsInstanceR == null) {
				RCCSettingsInstanceR = RCC_SettingsData.InstanceR;
				return RCCSettingsInstanceR;
			}
			return RCCSettingsInstanceR;
		}
	}

	#endregion

	private RCC_CarMainControllerV3 carMainController;
	private Light _lightObject;
	private Projector projectorObject;
	private LensFlare lensFlareObject;
	private TrailRenderer trailObject;
	private Camera mainCameraObject;

	[FormerlySerializedAs("defaultIntensity")] public float defaultIntensityValue = 0f;
	[FormerlySerializedAs("flareBrightness")] public float flareBrightnessValue = 1.5f;
	private float finalFlareBrightnessValue;

	[FormerlySerializedAs("lightType")] public LightType lightTypeValue;
	public enum LightType{HeadLight, BrakeLight, ReverseLight, Indicator, ParkLight, HighBeamHeadLight, External};
	[FormerlySerializedAs("inertia")] public float inertiaValue = 1f;
	[FormerlySerializedAs("flare")] public Flare flareValue;

	[FormerlySerializedAs("refreshRate")] public int refreshRateValue = 30;
	private float refreshTimerValue = 0f;

	private bool parkLightFoundFlag = false;
	private bool highBeamLightFoundFlag = false;

	// For Indicators.
	private RCC_CarMainControllerV3.IndicatorsOn indicatorsOnValue;
	private AudioSource indicatorSoundObject;
	public AudioClip indicatorAudioClip{get{return RCCSettingsR.indicatorClipValue;}}

	private void Start () {
		
		carMainController = GetComponentInParent<RCC_CarMainControllerV3>();
		_lightObject = GetComponent<Light>();
		_lightObject.enabled = true;
		lensFlareObject = GetComponent<LensFlare> ();
		trailObject = GetComponentInChildren<TrailRenderer> ();

		if (lensFlareObject) {
			
			if (_lightObject.flare != null)
				_lightObject.flare = null;
			
		}

		if(RCCSettingsR.useLightProjectorForLightingEffectValue){
			
			projectorObject = GetComponent<Projector>();
			if(projectorObject == null){
				projectorObject = ((GameObject)Instantiate(RCCSettingsR.projectorObject, transform.position, transform.rotation)).GetComponent<Projector>();
				projectorObject.transform.SetParent(transform, true);
			}
			projectorObject.ignoreLayers = RCCSettingsR.projectorIgnoreLayerMask;
			if(lightTypeValue != LightType.HeadLight)
				projectorObject.transform.localRotation = Quaternion.Euler(20f, transform.localPosition.z > 0f ? 0f : 180f, 0f);
			Material newMaterial = new Material(projectorObject.material);
			projectorObject.material = newMaterial ;

		}

		if(RCCSettingsR.useLightsAsVertexLightsValue){
			_lightObject.renderMode = LightRenderMode.ForceVertex;
			_lightObject.cullingMask = 0;
		}else{
			_lightObject.renderMode = LightRenderMode.ForcePixel;
		}

		if(lightTypeValue == LightType.Indicator){
			
			if(!carMainController.transform.Find("All Audio Sources/Indicator Sound AudioSource"))
				indicatorSoundObject = CreateNewAudioSource(RCCSettingsR.audioMixerValue, carMainController.gameObject, "Indicator Sound AudioSource", 1f, 3f, 1, indicatorAudioClip, false, false, false);
			else
				indicatorSoundObject = carMainController.transform.Find("All Audio Sources/Indicator Sound AudioSource").GetComponent<AudioSource>();
			
		}

		RCC_LightController[] allLights = carMainController.GetComponentsInChildren<RCC_LightController> ();

		for (int i = 0; i < allLights.Length; i++) {

			if (allLights [i].lightTypeValue == LightType.ParkLight)
				parkLightFoundFlag = true;

			if (allLights [i].lightTypeValue == LightType.HighBeamHeadLight)
				highBeamLightFoundFlag = true;

		}

		CheckTransformRotation ();
		CheckCurrLensFlare ();

	}

	private void OnEnable(){

		if(!_lightObject)
			_lightObject = GetComponent<Light>();

		if(defaultIntensityValue == 0)
			defaultIntensityValue = _lightObject.intensity;
		
		_lightObject.intensity = 0f;

	}

	private void Update () {
		
		if(RCCSettingsR.useLightProjectorForLightingEffectValue)
			SetProjectors();

		if (lensFlareObject)
			SetLensFlare ();

		if (trailObject)
			SetTrail ();

		switch(lightTypeValue){

		case LightType.HeadLight:
			if (highBeamLightFoundFlag) {

				SetLighting (carMainController.lowBeamHeadLightsOn ? defaultIntensityValue : 0f, 50f, 90f);

			}else{

				SetLighting (carMainController.lowBeamHeadLightsOn ? defaultIntensityValue : 0f, 50f, 90f);

				if (!carMainController.lowBeamHeadLightsOn && !carMainController.highBeamHeadLightsOn)
					SetLighting (0f);
				if (carMainController.lowBeamHeadLightsOn && !carMainController.highBeamHeadLightsOn) {
					SetLighting (defaultIntensityValue, 50f, 90f);
					transform.localEulerAngles = new Vector3 (10f, 0f, 0f);
				} else if (carMainController.highBeamHeadLightsOn) {
					SetLighting (defaultIntensityValue, 100f, 45f);
					transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
				}

			}
			break;

		case LightType.BrakeLight:
			
			if(parkLightFoundFlag)
				SetLighting(carMainController.brakeInput >= .1f ? defaultIntensityValue : 0f);
			else
				SetLighting(carMainController.brakeInput >= .1f ? defaultIntensityValue : !carMainController.lowBeamHeadLightsOn ? 0f : .25f);
			break;

		case LightType.ReverseLight:
			SetLighting(carMainController.direction == -1 ? defaultIntensityValue : 0f);
			break;

		case LightType.ParkLight:
			SetLighting((!carMainController.lowBeamHeadLightsOn ? 0f : defaultIntensityValue));
			break;

		case LightType.Indicator:
			indicatorsOnValue = carMainController.indicatorsOn;
			SetIndicators();
			break;

		case LightType.HighBeamHeadLight:
			SetLighting(carMainController.highBeamHeadLightsOn ? defaultIntensityValue : 0f, 200f, 45f);
			break;

		}
		
	}

	private void SetLighting(float input){

		_lightObject.intensity = Mathf.Lerp(_lightObject.intensity, input, Time.deltaTime * inertiaValue * 20f);

	}

	private void SetLighting(float input, float range, float spotAngle){

		_lightObject.intensity = Mathf.Lerp(_lightObject.intensity, input, Time.deltaTime * inertiaValue * 20f);
		_lightObject.range = range;
		_lightObject.spotAngle = spotAngle;

	}

	private void SetIndicators(){

		Vector3 relativePos = carMainController.transform.InverseTransformPoint (transform.position);

		switch(indicatorsOnValue){

		case RCC_CarMainControllerV3.IndicatorsOn.Left:

			if(relativePos.x > 0f){
				SetLighting (0);
				break;
			}

			if(carMainController.indicatorTimer >= .5f){
				SetLighting (0);
				if(indicatorSoundObject.isPlaying)
					indicatorSoundObject.Stop();
			}else{
				SetLighting (defaultIntensityValue);
				if(!indicatorSoundObject.isPlaying && carMainController.indicatorTimer <= .05f)
					indicatorSoundObject.Play();
			}
			if(carMainController.indicatorTimer >= 1f)
				carMainController.indicatorTimer = 0f;
			break;

		case RCC_CarMainControllerV3.IndicatorsOn.Right:

			if(relativePos.x < 0f){
				SetLighting (0);
				break;
			}

			if(carMainController.indicatorTimer >= .5f){
				SetLighting (0);
			if(indicatorSoundObject.isPlaying)
				indicatorSoundObject.Stop();
			}else{
				SetLighting (defaultIntensityValue);
				if(!indicatorSoundObject.isPlaying && carMainController.indicatorTimer <= .05f)
					indicatorSoundObject.Play();
			}
			if(carMainController.indicatorTimer >= 1f)
				carMainController.indicatorTimer = 0f;
			break;

		case RCC_CarMainControllerV3.IndicatorsOn.All:
			
			if(carMainController.indicatorTimer >= .5f){
				SetLighting (0);
				if(indicatorSoundObject.isPlaying)
					indicatorSoundObject.Stop();
			}else{
				SetLighting (defaultIntensityValue);
				if(!indicatorSoundObject.isPlaying && carMainController.indicatorTimer <= .05f)
					indicatorSoundObject.Play();
			}
			if(carMainController.indicatorTimer >= 1f)
				carMainController.indicatorTimer = 0f;
			break;

		case RCC_CarMainControllerV3.IndicatorsOn.Off:
			
			SetLighting (0);
			carMainController.indicatorTimer = 0f;
			break;
			
		}

	}

	private void SetProjectors(){

		if(!_lightObject.enabled){
			projectorObject.enabled = false;
			return;
		}else{
			projectorObject.enabled = true;
		}

		projectorObject.material.color = _lightObject.color * (_lightObject.intensity / 5f);

		projectorObject.farClipPlane = Mathf.Lerp(10f, 40f, (_lightObject.range - 50) / 150f);
		projectorObject.fieldOfView = Mathf.Lerp(40f, 30f, (_lightObject.range - 50) / 150f);

	}

	private void SetLensFlare(){

		if (refreshTimerValue > (1f / refreshRateValue)) {
			
			refreshTimerValue = 0f;

			if(!mainCameraObject)
				mainCameraObject = RCC_SceneManager.Instance.activeMainCamera;

			if (!mainCameraObject)
				return;

			float distanceTocam = Vector3.Distance(transform.position, mainCameraObject.transform.position);
			float angle = 1f;

			if(lightTypeValue != LightType.External)
				angle = Vector3.Angle(transform.forward,  mainCameraObject.transform.position - transform.position);

			if(angle != 0)
				finalFlareBrightnessValue = flareBrightnessValue * (4f / distanceTocam) * ((300f - (3f * angle)) / 300f) / 3f;

			lensFlareObject.brightness = finalFlareBrightnessValue * _lightObject.intensity;
			lensFlareObject.color = _lightObject.color;

		}

		refreshTimerValue += Time.deltaTime;

	}

	private void SetTrail(){

		trailObject.emitting = _lightObject.intensity > .1f ? true : false;
		trailObject.startColor = _lightObject.color;

	}

	private void CheckTransformRotation(){

		Vector3 relativePos = transform.GetComponentInParent<RCC_CarMainControllerV3>().transform.InverseTransformPoint (transform.position);

		if (relativePos.z > 0f) {

			if (Mathf.Abs (transform.localRotation.y) > .5f)
				transform.localRotation = Quaternion.identity;

		} else {

			if (Mathf.Abs (transform.localRotation.y) < .5f)
				transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

		}

	}

	private void CheckCurrLensFlare(){

		if (transform.GetComponent<LensFlare> () == null) {

			gameObject.AddComponent<LensFlare> ();
			LensFlare lf = gameObject.GetComponent<LensFlare> ();
			lf.brightness = 0f;
			lf.color = Color.white;
			lf.fadeSpeed = 20f;

		}

		if(gameObject.GetComponent<LensFlare>().flare == null)
			gameObject.GetComponent<LensFlare>().flare = flareValue;
			
		gameObject.GetComponent<Light> ().flare = null;

	}

	private void Reset(){

		CheckTransformRotation ();
		CheckCurrLensFlare ();

	}
		
}
