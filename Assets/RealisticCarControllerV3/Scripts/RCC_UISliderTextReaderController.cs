//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// Receives float from UI Slider, and displays the value as a text.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Slider Text Reader")]
public class RCC_UISliderTextReaderController : MonoBehaviour {

	[FormerlySerializedAs("slider")] public Slider sliderObject;
	[FormerlySerializedAs("text")] public Text textObject;

	private void Awake () {

		if(!sliderObject)
			sliderObject = GetComponentInParent<Slider> ();
		
		if(!textObject)
			textObject = GetComponentInChildren<Text> (); 
	
	}

	private void Update () {

		if (!sliderObject || !textObject)
			return;
		
		textObject.text = sliderObject.value.ToString ("F1");

	}

}
