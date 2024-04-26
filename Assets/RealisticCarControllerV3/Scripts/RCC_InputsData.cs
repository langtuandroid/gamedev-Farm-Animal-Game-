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

[System.Serializable]
public class RCC_InputsData{

	[FormerlySerializedAs("throttleInput")] public float throttleInputValue = 0f;
	[FormerlySerializedAs("brakeInput")] public float brakeInputValue = 0f;
	[FormerlySerializedAs("steerInput")] public float steerInputValue = 0f;
	[FormerlySerializedAs("clutchInput")] public float clutchInputValue = 0f;
	[FormerlySerializedAs("handbrakeInput")] public float handbrakeInputValue = 0f;
	[FormerlySerializedAs("boostInput")] public float boostInputValue = 0f;
	[FormerlySerializedAs("gearInput")] public int gearInputValue = 0;

	public void SetInputData(float _throttleInput, float _brakeInput, float _steerInput, float _clutchInput, float _handbrakeInput, float _boostInput){

		throttleInputValue = _throttleInput;
		brakeInputValue = _brakeInput;
		steerInputValue = _steerInput;
		clutchInputValue = _clutchInput;
		handbrakeInputValue = _handbrakeInput;
		boostInputValue = _boostInput;

	}

	public void SetInputData(float _throttleInput, float _brakeInput, float _steerInput, float _clutchInput, float _handbrakeInput){

		throttleInputValue = _throttleInput;
		brakeInputValue = _brakeInput;
		steerInputValue = _steerInput;
		clutchInputValue = _clutchInput;
		handbrakeInputValue = _handbrakeInput;

	}

//	public void SetInput(float _throttleInput, float _brakeInput, float _steerInput, float _handbrakeInput, float _boostInput){
//
//		throttleInput = _throttleInput;
//		brakeInput = _brakeInput;
//		steerInput = _steerInput;
//		handbrakeInput = _handbrakeInput;
//		boostInput = _boostInput;
//
//	}

	public void SetInputData(float _throttleInput, float _brakeInput, float _steerInput, float _handbrakeInput){

		throttleInputValue = _throttleInput;
		brakeInputValue = _brakeInput;
		steerInputValue = _steerInput;
		handbrakeInputValue = _handbrakeInput;

	}

	public void SetInputData(float _throttleInput, float _brakeInput, float _steerInput){

		throttleInputValue = _throttleInput;
		brakeInputValue = _brakeInput;
		steerInputValue = _steerInput;

	}
    
}
