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

public class RCC_DemoVehiclesData : ScriptableObject {

	[FormerlySerializedAs("vehicles")] public RCC_CarMainControllerV3[] vehiclesMass;

	#region singleton
	private static RCC_DemoVehiclesData instanceR;
	public static RCC_DemoVehiclesData InstanceR{	get{if(instanceR == null) instanceR = Resources.Load("RCC Assets/RCC_DemoVehicles") as RCC_DemoVehiclesData; return instanceR;}}
	#endregion

}
