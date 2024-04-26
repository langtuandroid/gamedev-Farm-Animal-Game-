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

/// <summary>
/// Handles RCC Canvas dashboard elements.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Info Displayer")]
[RequireComponent (typeof(Text))]
public class RCC_InfoLabelController : MonoBehaviour {

	#region singleton
	private static RCC_InfoLabelController instanceR;
	public static RCC_InfoLabelController InstanceR{

		get{

			if (instanceR == null) {

				if (GameObject.FindObjectOfType<RCC_InfoLabelController> ())
					instanceR = GameObject.FindObjectOfType<RCC_InfoLabelController> ();

			}

			return instanceR;

		}

	}
	#endregion

	private Text textUI;
	private float timerValue = 1f;

	private void Start () {

		textUI = GetComponent<Text> ();
		textUI.enabled = false;
		
	}

	private void Update(){

		if (timerValue < 1f) {
			
			if (!textUI.enabled)
				textUI.enabled = true;
			
		} else {
			
			if (textUI.enabled)
				textUI.enabled = false;
			
		}

		timerValue += Time.deltaTime;

	}

	public void ShowInformation (string info) {

		if (!textUI)
			return;

		textUI.text = info;
		timerValue = 0f;

//		StartCoroutine (ShowInfoCo(info, time));
		
	}

	private IEnumerator ShowInfoCoroutine(string info, float time){

		textUI.enabled = true;
		textUI.text = info;
		yield return new WaitForSeconds (time);
		textUI.enabled = false;

	}

}
