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

public class RCC_RecordsData : ScriptableObject {

	#region singleton
	private static RCC_RecordsData instanceR;
	public static RCC_RecordsData InstanceR{	get{if(instanceR == null) instanceR = Resources.Load("RCC Assets/RCC_Records") as RCC_RecordsData; return instanceR;}}
	#endregion

	[FormerlySerializedAs("records")] public List<RCC_RecorderController.RecordedData> recordsList = new List<RCC_RecorderController.RecordedData>();

}
