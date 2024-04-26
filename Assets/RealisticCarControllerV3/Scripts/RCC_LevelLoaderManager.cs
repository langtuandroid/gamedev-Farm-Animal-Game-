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
using UnityEngine.SceneManagement;

public class RCC_LevelLoaderManager : MonoBehaviour {

	public void LoadLevelByName (string levelName) {

		SceneManager.LoadScene (levelName);
		
	}

}
