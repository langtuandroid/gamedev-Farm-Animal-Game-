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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// Receiving inputs from UI Joystick.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Joystick")]
public class RCC_UIJoystickInput : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	[FormerlySerializedAs("backgroundSprite")] public RectTransform backgroundSpriteTransform;
	[FormerlySerializedAs("handleSprite")] public RectTransform handleSpriteTransform;

	internal Vector2 inputVectorValue = Vector2.zero;
	public float inputHorizontalValue { get { return inputVectorValue.x; } }
	public float inputVerticalValue { get { return inputVectorValue.y; } }

	private Vector2 joystickPositionValue = Vector2.zero;
	private Camera _refCamera = new Camera();

	private void Start(){

		joystickPositionValue = RectTransformUtility.WorldToScreenPoint(_refCamera, backgroundSpriteTransform.position);

	}

	public void OnDrag(PointerEventData eventData){

		Vector2 direction = eventData.position - joystickPositionValue;
		inputVectorValue = (direction.magnitude > backgroundSpriteTransform.sizeDelta.x / 2f) ? direction.normalized : direction / (backgroundSpriteTransform.sizeDelta.x / 2f);
		handleSpriteTransform.anchoredPosition = (inputVectorValue * backgroundSpriteTransform.sizeDelta.x / 2f) * 1f;

	}

	public void OnPointerUp(PointerEventData eventData){

		inputVectorValue = Vector2.zero;
		handleSpriteTransform.anchoredPosition = Vector2.zero;

	}

	public virtual void OnPointerDown(PointerEventData eventData){



	}

}
