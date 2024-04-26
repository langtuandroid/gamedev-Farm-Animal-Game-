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
/// Feeding material's emission channel for self illumin effect.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Light/RCC Light Emission")]
public class RCC_LightEmissionController : MonoBehaviour {

	private Light currSharedLight;
	[FormerlySerializedAs("lightRenderer")] public Renderer currLightRenderer;
	[FormerlySerializedAs("materialIndex")] public int materialIndexValue = 0;
	[FormerlySerializedAs("noTexture")] public bool noTextureFlag = false;
	[FormerlySerializedAs("applyAlpha")] public bool applyAlphaFlag = false;
	[FormerlySerializedAs("multiplier")] public float multiplierValue = 1f;

	private int emissionColorIDValue;
	private int colorIDValue;

	private Material currMaterial;
	private Color currTargetColor;

	private void Start () {

		currSharedLight = GetComponent<Light>();
		currMaterial = currLightRenderer.materials [materialIndexValue];
		currMaterial.EnableKeyword("_EMISSION");
		emissionColorIDValue = Shader.PropertyToID("_EmissionColor");
		colorIDValue = Shader.PropertyToID("_Color");

		if (!currMaterial.HasProperty (emissionColorIDValue))
			enabled = false;

	}

	private void Update () {

		if(!currSharedLight.enabled)
			currTargetColor = Color.white * 0f;

		if (!noTextureFlag)
			currTargetColor = Color.white * currSharedLight.intensity * multiplierValue;
		else
			currTargetColor = currSharedLight.color * currSharedLight.intensity * multiplierValue;

		if (applyAlphaFlag)
			currMaterial.SetColor (colorIDValue, new Color(1f, 1f, 1f, currSharedLight.intensity * multiplierValue));

		if (currMaterial.GetColor (emissionColorIDValue) != (currTargetColor))
			currMaterial.SetColor (emissionColorIDValue, currTargetColor);
		
	}

}
