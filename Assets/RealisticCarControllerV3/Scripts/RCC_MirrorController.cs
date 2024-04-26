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

/// <summary>
/// It must be attached to external camera. This external camera will be used as mirror.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Mirror")]
public class RCC_MirrorController : MonoBehaviour {

	private Camera currCam;
	private RCC_CarMainControllerV3 carMainController;

	private void Awake(){

		InvertCurrCamera ();

	}

	private void OnEnable(){

		StartCoroutine (FixDepthCoroutine());

	}

	private IEnumerator FixDepthCoroutine(){

		yield return new WaitForEndOfFrame ();

		currCam.depth = 1f;

	}
	
	private void InvertCurrCamera () {

		currCam = GetComponent<Camera>();

		currCam.ResetWorldToCameraMatrix ();
		currCam.ResetProjectionMatrix ();
		currCam.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
		carMainController = GetComponentInParent<RCC_CarMainControllerV3>();

	}
	
	private void OnPreRender () {
		
		GL.invertCulling = true;

	}
	
	private void OnPostRender () {
		
		GL.invertCulling = false;

	}

	private void Update(){

		if(!currCam)
			return;

		currCam.enabled = carMainController.canControl;

	}

}
