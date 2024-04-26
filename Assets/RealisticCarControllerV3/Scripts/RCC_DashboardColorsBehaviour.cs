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
using UnityEngine.UI;

/// <summary>
/// Changes HUD image colors by UI Sliders.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Dashboard Colors")]
public class RCC_DashboardColorsBehaviour : MonoBehaviour {

	[FormerlySerializedAs("huds")] public Image[] hudsMass;
	[FormerlySerializedAs("hudColor")] public Color hudColorValue = Color.white;

	[FormerlySerializedAs("hudColor_R")] public Slider hudColor_RSlider;
	[FormerlySerializedAs("hudColor_G")] public Slider hudColor_GSlider;
	[FormerlySerializedAs("hudColor_B")] public Slider hudColor_BSlider;

	private void Start () {

		if(hudsMass == null || hudsMass.Length < 1)
			enabled = false;

		if(hudColor_RSlider && hudColor_GSlider && hudColor_BSlider){
			
			hudColor_RSlider.value = hudColorValue.r;
			hudColor_GSlider.value = hudColorValue.g;
			hudColor_BSlider.value = hudColorValue.b;

		}
	
	}

	private void Update () {

		if(hudColor_RSlider && hudColor_GSlider && hudColor_BSlider)
			hudColorValue = new Color(hudColor_RSlider.value, hudColor_GSlider.value, hudColor_BSlider.value);

		for (int i = 0; i < hudsMass.Length; i++) {

			hudsMass[i].color = new Color(hudColorValue.r, hudColorValue.g, hudColorValue.b, hudsMass[i].color.a);

		}
	
	}

}
