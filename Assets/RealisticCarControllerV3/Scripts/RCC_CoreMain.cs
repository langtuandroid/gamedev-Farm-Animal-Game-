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
using UnityEngine.Audio;
using System;

public class RCC_CoreMain : MonoBehaviour{

    #region Create AudioSource

    /// <summary>
    /// Creates new audiosource with specified settings.
    /// </summary>
	public static AudioSource CreateNewAudioSource(AudioMixerGroup audioMixer, GameObject go, string audioName, float minDistance, float maxDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished){

        GameObject audioSourceObject = new GameObject(audioName);

        if (go.transform.Find("All Audio Sources")){
            audioSourceObject.transform.SetParent(go.transform.Find("All Audio Sources"));
        }else{
            GameObject allAudioSources = new GameObject("All Audio Sources");
            allAudioSources.transform.SetParent(go.transform, false);
            audioSourceObject.transform.SetParent(allAudioSources.transform, false);
        }

        audioSourceObject.transform.position = go.transform.position;
        audioSourceObject.transform.rotation = go.transform.rotation;

        audioSourceObject.AddComponent<AudioSource>();
        AudioSource source = audioSourceObject.GetComponent<AudioSource>();

		if (audioMixer)
			source.outputAudioMixerGroup = audioMixer;

        //audioSource.GetComponent<AudioSource>().priority =1;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.volume = volume;
        source.clip = audioClip;
        source.loop = loop;
        source.dopplerLevel = .5f;

        if (minDistance == 0 && maxDistance == 0)
            source.spatialBlend = 0f;
        else
            source.spatialBlend = 1f;

        if (playNow){

            source.playOnAwake = true;
            source.Play();

        }else{

            source.playOnAwake = false;

        }

        if (destroyAfterFinished){

            if (audioClip)
                Destroy(audioSourceObject, audioClip.length);
            else
                Destroy(audioSourceObject);

        }

        return source;

    }

	/// <summary>
	/// Creates new audiosource with specified settings.
	/// </summary>
	public static AudioSource CreateNewAudioSource(GameObject go, string audioName, float minDistance, float maxDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished){

		GameObject audioSourceObject = new GameObject(audioName);

		if (go.transform.Find("All Audio Sources")){
			audioSourceObject.transform.SetParent(go.transform.Find("All Audio Sources"));
		}else{
			GameObject allAudioSources = new GameObject("All Audio Sources");
			allAudioSources.transform.SetParent(go.transform, false);
			audioSourceObject.transform.SetParent(allAudioSources.transform, false);
		}

		audioSourceObject.transform.position = go.transform.position;
		audioSourceObject.transform.rotation = go.transform.rotation;

		audioSourceObject.AddComponent<AudioSource>();
		AudioSource source = audioSourceObject.GetComponent<AudioSource>();

		//audioSource.GetComponent<AudioSource>().priority =1;
		source.minDistance = minDistance;
		source.maxDistance = maxDistance;
		source.volume = volume;
		source.clip = audioClip;
		source.loop = loop;
		source.dopplerLevel = .5f;

		if (minDistance == 0 && maxDistance == 0)
			source.spatialBlend = 0f;
		else
			source.spatialBlend = 1f;

		if (playNow){

			source.playOnAwake = true;
			source.Play();

		}else{

			source.playOnAwake = false;

		}

		if (destroyAfterFinished){

			if (audioClip)
				Destroy(audioSourceObject, audioClip.length);
			else
				Destroy(audioSourceObject);

		}

		return source;

	}

    /// <summary>
    /// Creates new audiosource with specified settings.
    /// </summary>
    public static AudioSource CreateNewAudioSource(AudioMixerGroup audioMixer, GameObject go, Vector3 localPosition, string audioName, float minDistance, float maxDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished) {

        GameObject audioSourceObject = new GameObject(audioName);

        if (go.transform.Find("All Audio Sources")) {
            audioSourceObject.transform.SetParent(go.transform.Find("All Audio Sources"));
        } else {
            GameObject allAudioSources = new GameObject("All Audio Sources");
            allAudioSources.transform.SetParent(go.transform, false);
            audioSourceObject.transform.SetParent(allAudioSources.transform, false);
        }

        audioSourceObject.transform.position = go.transform.position;
        audioSourceObject.transform.rotation = go.transform.rotation;
        audioSourceObject.transform.localPosition = localPosition;

        audioSourceObject.AddComponent<AudioSource>();
        AudioSource source = audioSourceObject.GetComponent<AudioSource>();

        if (audioMixer)
            source.outputAudioMixerGroup = audioMixer;

        //audioSource.GetComponent<AudioSource>().priority =1;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.volume = volume;
        source.clip = audioClip;
        source.loop = loop;
        source.dopplerLevel = .5f;

        if (minDistance == 0 && maxDistance == 0)
            source.spatialBlend = 0f;
        else
            source.spatialBlend = 1f;

        if (playNow) {

            source.playOnAwake = true;
            source.Play();

        } else {

            source.playOnAwake = false;

        }

        if (destroyAfterFinished) {

            if (audioClip)
                Destroy(audioSourceObject, audioClip.length);
            else
                Destroy(audioSourceObject);

        }

        return source;

    }

    /// <summary>
    /// Creates new audiosource with specified settings.
    /// </summary>
    public static AudioSource CreateNewAudioSource(GameObject go, Vector3 localPosition, string audioName, float minDistance, float maxDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished) {

        GameObject audioSourceObject = new GameObject(audioName);

        if (go.transform.Find("All Audio Sources")) {
            audioSourceObject.transform.SetParent(go.transform.Find("All Audio Sources"));
        } else {
            GameObject allAudioSources = new GameObject("All Audio Sources");
            allAudioSources.transform.SetParent(go.transform, false);
            audioSourceObject.transform.SetParent(allAudioSources.transform, false);
        }

        audioSourceObject.transform.position = go.transform.position;
        audioSourceObject.transform.rotation = go.transform.rotation;
        audioSourceObject.transform.localPosition = localPosition;

        audioSourceObject.AddComponent<AudioSource>();
        AudioSource source = audioSourceObject.GetComponent<AudioSource>();

        //audioSource.GetComponent<AudioSource>().priority =1;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.volume = volume;
        source.clip = audioClip;
        source.loop = loop;
        source.dopplerLevel = .5f;

        if (minDistance == 0 && maxDistance == 0)
            source.spatialBlend = 0f;
        else
            source.spatialBlend = 1f;

        if (playNow) {

            source.playOnAwake = true;
            source.Play();

        } else {

            source.playOnAwake = false;

        }

        if (destroyAfterFinished) {

            if (audioClip)
                Destroy(audioSourceObject, audioClip.length);
            else
                Destroy(audioSourceObject);

        }

        return source;

    }

    /// <summary>
    /// Adds High Pass Filter to audiosource. Used for turbo.
    /// </summary>
    public static void AddNewHighPassFilter(AudioSource source, float freq, int level){

        if (source == null)
            return;

        AudioHighPassFilter highFilter = source.gameObject.AddComponent<AudioHighPassFilter>();
        highFilter.cutoffFrequency = freq;
        highFilter.highpassResonanceQ = level;

    }

    /// <summary>
    /// Adds Low Pass Filter to audiosource. Used for engine off sounds.
    /// </summary>
    public static void AddNewLowPassFilter(AudioSource source, float freq){

        if (source == null)
            return;

        AudioLowPassFilter lowFilter = source.gameObject.AddComponent<AudioLowPassFilter>();
        lowFilter.cutoffFrequency = freq;
        //      lowFilter.highpassResonanceQ = level;

    }

    #endregion

    #region Create WheelColliders

    /// <summary>
    /// Creates the wheel colliders.
    /// </summary>
    public void CreateCarWheelColliders(RCC_CarMainControllerV3 carController){

        // Creating a list for all wheel models.
        List<Transform> allWheelModels = new List<Transform>();
        allWheelModels.Add(carController.FrontLeftWheelTransform); allWheelModels.Add(carController.FrontRightWheelTransform); allWheelModels.Add(carController.RearLeftWheelTransform); allWheelModels.Add(carController.RearRightWheelTransform);

        // If we have additional rear wheels, add them too.
        if (carController.ExtraRearWheelsTransform.Length > 0 && carController.ExtraRearWheelsTransform[0])
        {

            foreach (Transform t in carController.ExtraRearWheelsTransform)
                allWheelModels.Add(t);

        }

        // If we don't have any wheelmodels, throw an error.
        if (allWheelModels != null && allWheelModels[0] == null)
        {

            Debug.LogError("You haven't choosen your Wheel Models. Please select all of your Wheel Models before creating Wheel Colliders. Script needs to know their sizes and positions, aye?");
            return;

        }

        // Holding default rotation.
        Quaternion currentRotation = transform.rotation;

        // Resetting rotation.
        transform.rotation = Quaternion.identity;

        // Creating a new gameobject called Wheel Colliders for all Wheel Colliders, and parenting it to this gameobject.
        GameObject WheelColliders = new GameObject("Wheel Colliders");
        WheelColliders.transform.SetParent(transform, false);
        WheelColliders.transform.localRotation = Quaternion.identity;
        WheelColliders.transform.localPosition = Vector3.zero;
        WheelColliders.transform.localScale = Vector3.one;

        // Creating WheelColliders.
        foreach (Transform wheel in allWheelModels)
        {

            GameObject wheelcollider = new GameObject(wheel.transform.name);

            wheelcollider.transform.position = RCC_GetBoundsSize.GetBoundsCenterVector(wheel.transform);
            wheelcollider.transform.rotation = transform.rotation;
            wheelcollider.transform.name = wheel.transform.name;
            wheelcollider.transform.SetParent(WheelColliders.transform);
            wheelcollider.transform.localScale = Vector3.one;
            wheelcollider.AddComponent<WheelCollider>();

            Bounds biggestBound = new Bounds();
            Renderer[] renderers = wheel.GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renderers)
            {
                if (render != GetComponent<Renderer>())
                {
                    if (render.bounds.size.z > biggestBound.size.z)
                        biggestBound = render.bounds;
                }
            }

            wheelcollider.GetComponent<WheelCollider>().radius = (biggestBound.extents.y) / transform.localScale.y;
            wheelcollider.AddComponent<RCC_WheelColliderController>();
            JointSpring spring = wheelcollider.GetComponent<WheelCollider>().suspensionSpring;

            spring.spring = 40000f;
            spring.damper = 1500f;
            spring.targetPosition = .5f;

            wheelcollider.GetComponent<WheelCollider>().suspensionSpring = spring;
            wheelcollider.GetComponent<WheelCollider>().suspensionDistance = .2f;
            wheelcollider.GetComponent<WheelCollider>().forceAppPointDistance = 0f;
            wheelcollider.GetComponent<WheelCollider>().mass = 40f;
            wheelcollider.GetComponent<WheelCollider>().wheelDampingRate = 1f;

            WheelFrictionCurve sidewaysFriction;
            WheelFrictionCurve forwardFriction;

            sidewaysFriction = wheelcollider.GetComponent<WheelCollider>().sidewaysFriction;
            forwardFriction = wheelcollider.GetComponent<WheelCollider>().forwardFriction;

            forwardFriction.extremumSlip = .4f;
            forwardFriction.extremumValue = 1;
            forwardFriction.asymptoteSlip = .8f;
            forwardFriction.asymptoteValue = .6f;
            forwardFriction.stiffness = 1f;

            sidewaysFriction.extremumSlip = .25f;
            sidewaysFriction.extremumValue = 1;
            sidewaysFriction.asymptoteSlip = .5f;
            sidewaysFriction.asymptoteValue = .8f;
            sidewaysFriction.stiffness = 1f;

            wheelcollider.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
            wheelcollider.GetComponent<WheelCollider>().forwardFriction = forwardFriction;

        }

        RCC_WheelColliderController[] allWheelColliders = new RCC_WheelColliderController[allWheelModels.Count];
        allWheelColliders = GetComponentsInChildren<RCC_WheelColliderController>();

        carController.FrontLeftWheelCollider = allWheelColliders[0];
        carController.FrontRightWheelCollider = allWheelColliders[1];
        carController.RearLeftWheelCollider = allWheelColliders[2];
        carController.RearRightWheelCollider = allWheelColliders[3];

        carController.ExtraRearWheelsCollider = new RCC_WheelColliderController[carController.ExtraRearWheelsTransform.Length];

        for (int i = 0; i < carController.ExtraRearWheelsTransform.Length; i++)
        {
            carController.ExtraRearWheelsCollider[i] = allWheelColliders[i + 4];
        }

        transform.rotation = currentRotation;

    }

    #endregion

    #region Set Behavior

    /// <summary>
    /// Overrides the behavior.
    /// </summary>
    public void SetCarBehavior(RCC_CarMainControllerV3 carController){

        if (RCC_SettingsData.InstanceR.selectedBehaviorTypeR == null)
            return;

        RCC_SettingsData.BehaviorTypeData currentBehaviorType = RCC_SettingsData.InstanceR.selectedBehaviorTypeR;

        carController.steeringHelper = currentBehaviorType.steeringHelperFlag;
        carController.tractionHelper = currentBehaviorType.tractionHelperFlag;
		carController.angularDragHelper = currentBehaviorType.angularDragHelperFlag;
		carController.useCounterSteering = currentBehaviorType.counterSteeringFlag;
        carController.ABS = currentBehaviorType.ABSFlag;
        carController.ESP = currentBehaviorType.ESPFlag;
        carController.TCS = currentBehaviorType.TCSFlag;

        carController.highspeedsteerAngle = Mathf.Clamp(carController.highspeedsteerAngle, currentBehaviorType.highSpeedSteerAngleMinimumValue, currentBehaviorType.highSpeedSteerAngleMaximumVlaue);
        carController.highspeedsteerAngleAtspeed = Mathf.Clamp(carController.highspeedsteerAngleAtspeed, currentBehaviorType.highSpeedSteerAngleAtspeedMinimumValue, currentBehaviorType.highSpeedSteerAngleAtspeedMaximumValue);
		carController.counterSteeringFactor = Mathf.Clamp (carController.counterSteeringFactor, currentBehaviorType.counterSteeringMinimumValue, currentBehaviorType.counterSteeringMaximumValue);

        carController.steerHelperAngularVelStrength = Mathf.Clamp(carController.steerHelperAngularVelStrength, currentBehaviorType.steerHelperAngularVelStrengthMinimumValue, currentBehaviorType.steerHelperAngularVelStrengthMaximumValue);
        carController.steerHelperLinearVelStrength = Mathf.Clamp(carController.steerHelperLinearVelStrength, currentBehaviorType.steerHelperLinearVelStrengthMinimumValue, currentBehaviorType.steerHelperLinearVelStrengthMaximumValue);

        carController.tractionHelperStrength = Mathf.Clamp(carController.tractionHelperStrength, currentBehaviorType.tractionHelperStrengthMinimumValue, currentBehaviorType.tractionHelperStrengthMaximumValue);
        carController.antiRollFrontHorizontal = Mathf.Clamp(carController.antiRollFrontHorizontal, currentBehaviorType.antiRollFrontHorizontalMinimumValue, Mathf.Infinity);
        carController.antiRollRearHorizontal = Mathf.Clamp(carController.antiRollRearHorizontal, currentBehaviorType.antiRollRearHorizontalMinimumValue, Mathf.Infinity);

        carController.gearShiftingDelay = Mathf.Clamp(carController.gearShiftingDelay, 0f, currentBehaviorType.gearShiftingDelayMaximumValue);
        carController.rigid.angularDrag = Mathf.Clamp(carController.rigid.angularDrag, currentBehaviorType.angularDragValue, 1f);

		carController.angularDragHelperStrength = Mathf.Clamp(carController.angularDragHelperStrength, currentBehaviorType.angularDragHelperMinimumValue, currentBehaviorType.angularDragHelperMaximumValue);

    }

    #endregion

}

