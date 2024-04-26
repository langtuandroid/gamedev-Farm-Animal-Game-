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
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// RCC All In One playable demo scene manager.
/// </summary>
public class RCC_AIOManager : MonoBehaviour {

	// Instance of the script.
	private static RCC_AIOManager instanceM;

	[FormerlySerializedAs("levels")] public GameObject levelsObject;			//	UI levels menu.
	[FormerlySerializedAs("back")] public GameObject backObject;				//	UI back button.

	private AsyncOperation asyncOperation;		//Async.
	[FormerlySerializedAs("slider")] public Slider sliderLoading;						//	Loading slider.

	private void Start () {

		// Getting instance. If same exists, destroy it.
		if (instanceM) {
			Destroy (gameObject);
		} else {
			instanceM = this;
			DontDestroyOnLoad (gameObject);
		}

	}

	private void Update(){

		// If level load is in progress, enable and adjust loading slider. Otherwise, disable it.
		if (asyncOperation != null && !asyncOperation.isDone) {

			if(!sliderLoading.gameObject.activeSelf)
				sliderLoading.gameObject.SetActive (true);
			
			sliderLoading.value = asyncOperation.progress;

		} else {

			if(sliderLoading.gameObject.activeSelf)
				sliderLoading.gameObject.SetActive (false);

		}

	}

	/// <summary>
	/// Loads the target level.
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadLevelAsync (string levelName) {

		asyncOperation = SceneManager.LoadSceneAsync (levelName);

	}

	/// <summary>
	/// Toggles the UI menu.
	/// </summary>
	/// <param name="menu">Menu.</param>
	public void ToggleMenuObject (GameObject menu) {

		levelsObject.SetActive (false);
		backObject.SetActive (false);

		menu.SetActive (true);

	}

	/// <summary>
	/// Closes application.
	/// </summary>
	public void QuitApplication () {

		Application.Quit ();

	}

}
