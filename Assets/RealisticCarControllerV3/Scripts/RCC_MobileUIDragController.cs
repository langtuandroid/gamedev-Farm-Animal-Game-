//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

#pragma warning disable 0414

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Mobile UI Drag used for orbiting RCC Camera.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Drag")]
public class RCC_MobileUIDragController : MonoBehaviour, IDragHandler, IEndDragHandler{

	private bool isPressingFlag = false;

	public void OnDrag(PointerEventData data){

		if (RCC_SettingsData.InstanceR.selectedControllerTypeR != RCC_SettingsData.ControllerType.Mobile)
			return;

		isPressingFlag = true;

		RCC_SceneManager.Instance.activePlayerCamera.OnDrag (data);

	}

	public void OnEndDrag(PointerEventData data){

		if (RCC_SettingsData.InstanceR.selectedControllerTypeR != RCC_SettingsData.ControllerType.Mobile)
			return;

		isPressingFlag = false;

	}

}
