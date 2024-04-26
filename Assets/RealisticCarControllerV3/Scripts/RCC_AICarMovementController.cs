//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// AI Controller of RCC. It's not professional, but it does the job. Follows all waypoints, or follows/chases the target gameobject.
/// </summary>
[RequireComponent(typeof(RCC_CarMainControllerV3))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/AI/RCC AI Car Controller")]
public class RCC_AICarMovementController : MonoBehaviour {

	internal RCC_CarMainControllerV3 carMainController;		// Main RCC of this vehicle.

	[FormerlySerializedAs("waypointsContainer")] public RCC_AIWaypointsDrawingContainer waypointsAIContainer;					// Waypoints Container.
	[FormerlySerializedAs("currentWaypointIndex")] public int currentWaypointIndexValue = 0;											// Current index in Waypoint Container.
	[FormerlySerializedAs("targetChase")] public Transform targetChaseTransform;											// Target Gameobject for chasing.
	[FormerlySerializedAs("targetTag")] public string targetObjectTag = "Player";									// Search and chase Gameobjects with tags.

	// AI Type
	[FormerlySerializedAs("navigationMode")] public NavigationMode navigationAIMode;
	public enum NavigationMode {FollowWaypoints, ChaseTarget, FollowTarget}

	// Raycast distances used for detecting obstacles at front of the AI vehicle.
	[FormerlySerializedAs("raycastLength")] [Range(5f, 30f)]public float raycastLengthValue = 3f;
	[FormerlySerializedAs("raycastAngle")] [Range(10f, 90f)] public float raycastAngleValue = 30f;
	[FormerlySerializedAs("obstacleLayers")] public LayerMask obstacleLayersMask = -1;
    [FormerlySerializedAs("obstacle")] public GameObject obstacleObject;

	[FormerlySerializedAs("useRaycasts")] public bool useRaycastsFlag = true;		//	Using forward and sideways raycasts to avoid obstacles.
	private float rayInputValue = 0f;				// Total ray input affected by raycast distances.
	private bool raycastingFlag = false;		// Raycasts hits an obstacle now?
	private float resetTimeValue = 0f;           // This timer was used for deciding go back or not, after crashing.
	private bool reversingNowFlag = false;

	// Steer, Motor, And Brake inputs. Will feed RCC_CarController with these inputs.
	[FormerlySerializedAs("steerInput")] public float steerInputValue = 0f;
	[FormerlySerializedAs("throttleInput")] public float throttleInputValue = 0f;
	[FormerlySerializedAs("brakeInput")] public float brakeInputValue = 0f;
	[FormerlySerializedAs("handbrakeInput")] public float handbrakeInputValue = 0f;

	// Limit speed.
	[FormerlySerializedAs("limitSpeed")] public bool limitSpeedFlag = false;
	[FormerlySerializedAs("maximumSpeed")] public float maximumSpeedValue = 100f;

	// Smoothed steering.
	[FormerlySerializedAs("smoothedSteer")] public bool smoothedSteerFlag = true;
	
	// Counts laps and how many waypoints were passed.
	[FormerlySerializedAs("lap")] public int lapValue = 0;
	[FormerlySerializedAs("totalWaypointPassed")] public int totalWaypointPassedValue = 0;
	[FormerlySerializedAs("nextWaypointPassDistance")] public int nextWaypointPassDistanceValue = 20;
	[FormerlySerializedAs("chaseDistance")] public int chaseDistanceValue = 200;
	[FormerlySerializedAs("startFollowDistance")] public int startFollowDistanceValue = 300;
	[FormerlySerializedAs("stopFollowDistance")] public int stopFollowDistanceValue = 30;
	[FormerlySerializedAs("ignoreWaypointNow")] public bool ignoreWaypointNowFlag = false;
	
	// Unity's Navigator.
	private NavMeshAgent navigatorAgent;

	// Detector with Sphere Collider. Used for finding target Gameobjects in chasing mode.
	private SphereCollider detectorCollider;
	[FormerlySerializedAs("targetsInZone")] public List<Transform> targetsInZoneList = new List<Transform> ();

	// Firing an event when each RCC AI vehicle spawned / enabled.
	public delegate void onRCCAISpawnedDelegate(RCC_AICarMovementController RCCAI);
	public static event onRCCAISpawnedDelegate OnRCCAISpawnedEvent;

	// Firing an event when each RCC AI vehicle disabled / destroyed.
	public delegate void onRCCAIDestroyedDelegate(RCC_AICarMovementController RCCAI);
	public static event onRCCAIDestroyedDelegate OnRCCAIDestroyedEvent;

	private void Awake() {

		// Getting main controller and enabling external controller.
		carMainController = GetComponent<RCC_CarMainControllerV3>();
		carMainController.externalController = true;

		// If Waypoints Container is not selected in Inspector Panel, find it on scene.
		if(!waypointsAIContainer)
			waypointsAIContainer = FindObjectOfType(typeof(RCC_AIWaypointsDrawingContainer)) as RCC_AIWaypointsDrawingContainer;

		// Creating our Navigator and setting properties.
		GameObject navigatorObject = new GameObject("Navigator");
		navigatorObject.transform.SetParent (transform, false);
		navigatorAgent = navigatorObject.AddComponent<NavMeshAgent>();
		navigatorAgent.radius = 1;
		navigatorAgent.speed = 1;
		navigatorAgent.angularSpeed = 100000f;
		navigatorAgent.acceleration = 100000f;
		navigatorAgent.height = 1;
		navigatorAgent.avoidancePriority = 0;

		// Creating our Detector and setting properties. Used for getting nearest target gameobjects.
		GameObject detectorGO = new GameObject ("Detector");
		detectorGO.transform.SetParent (transform, false);
		detectorGO.layer = LayerMask.NameToLayer("Ignore Raycast");
		detectorCollider = detectorGO.gameObject.AddComponent<SphereCollider> ();
		detectorCollider.isTrigger = true;
		detectorCollider.radius = 10f;

	}

	private void OnEnable(){

		// Calling this event when AI vehicle spawned.
		if (OnRCCAISpawnedEvent != null)
			OnRCCAISpawnedEvent (this);

	}
	
	private void Update(){

		// If not controllable, no need to go further.
		if(!carMainController.canControl)
			return;

		// Assigning navigator's position to front wheels of the vehicle.
		//navigator.transform.localPosition = new Vector3(0f, carController.FrontLeftWheelCollider.transform.localPosition.y, carController.FrontLeftWheelCollider.transform.localPosition.z);
		navigatorAgent.transform.localPosition = Vector3.zero;
		navigatorAgent.transform.localPosition += Vector3.forward * carMainController.FrontLeftWheelCollider.transform.localPosition.z;

		CheckCarTargets();

	}
	
	private void FixedUpdate (){

		// If not controllable, no need to go further.
		if(!carMainController.canControl)
			return;

		if (useRaycastsFlag)
			FixedRaycastsInput();            // Recalculates steerInput if one of raycasts detects an object front of AI vehicle.

		NavigationInput();             // Calculates steerInput based on navigator.
		CheckCrashReset();           // Was used for deciding go back or not after crashing.
		FeedRCCInput();              // Feeds inputs of the RCC.

	}
	
	private void NavigationInput (){

		// Navigator Input is multiplied by 1.5f for fast reactions.
		float navigatorInput = Mathf.Clamp(transform.InverseTransformDirection(navigatorAgent.desiredVelocity).x * 1.5f, -1f, 1f);

		switch (navigationAIMode) {

			case NavigationMode.FollowWaypoints:

				// If our scene doesn't have a Waypoint Container, return with error.
				if (!waypointsAIContainer) {

					Debug.LogError ("Waypoints Container Couldn't Found!");
					StopCar ();
					return;

				}

				// If our scene has Waypoints Container and it doesn't have any waypoints, return with error.
				if (waypointsAIContainer && waypointsAIContainer.waypointsList.Count < 1) {

					Debug.LogError ("Waypoints Container Doesn't Have Any Waypoints!");
					StopCar ();
					return;

				}

				// Next waypoint and its position.
				RCC_WaypointR currentWaypoint = waypointsAIContainer.waypointsList [currentWaypointIndexValue];

				// Checks for the distance to next waypoint. If it is less than written value, then pass to next waypoint.
				float distanceToNextWaypoint = GetPathLengthValue(navigatorAgent.path);

				// Setting destination of the Navigator. 
				if (!navigatorAgent.hasPath)
					navigatorAgent.SetDestination(waypointsAIContainer.waypointsList[currentWaypointIndexValue].transform.position);

				if (distanceToNextWaypoint != 0 && distanceToNextWaypoint < nextWaypointPassDistanceValue) {
					
					currentWaypointIndexValue++;
					totalWaypointPassedValue++;

                    // If all waypoints were passed, sets the current waypoint to first waypoint and increase lap.
                    if (currentWaypointIndexValue >= waypointsAIContainer.waypointsList.Count) {

                        currentWaypointIndexValue = 0;
                        lapValue++;

                    }

					// Setting destination of the Navigator. 
					if (navigatorAgent.isOnNavMesh)
						navigatorAgent.SetDestination(waypointsAIContainer.waypointsList[currentWaypointIndexValue].transform.position);

				}

				if (!reversingNowFlag) {

					throttleInputValue = (distanceToNextWaypoint < (nextWaypointPassDistanceValue * (carMainController.speed / 30f))) ? (Mathf.Clamp01(currentWaypoint.targetSpeedR - carMainController.speed)) : 1f;
					throttleInputValue *= Mathf.Clamp01(Mathf.Lerp(10f, 0f, (carMainController.speed) / maximumSpeedValue));
					brakeInputValue = (distanceToNextWaypoint < (nextWaypointPassDistanceValue * (carMainController.speed / 30f))) ? (Mathf.Clamp01(carMainController.speed - currentWaypoint.targetSpeedR)) : 0f;
					handbrakeInputValue = 0f;

					if (carMainController.speed > 30f) {

						throttleInputValue -= Mathf.Abs(navigatorInput) / 3f;
						brakeInputValue += Mathf.Abs(navigatorInput) / 3f;

					}

				}

				break;

		case NavigationMode.ChaseTarget:

				detectorCollider.radius = chaseDistanceValue;

				// If our scene doesn't have a Waypoints Container, return with error.
				if (!targetChaseTransform){
				
				StopCar();
				return;
	
			}

			// Setting destination of the Navigator. 
			if(navigatorAgent.isOnNavMesh)
				navigatorAgent.SetDestination (targetChaseTransform.position);

			if (!reversingNowFlag) {

				throttleInputValue = 1f;
				throttleInputValue *= Mathf.Clamp01(Mathf.Lerp(10f, 0f, (carMainController.speed) / maximumSpeedValue));
				brakeInputValue = 0f;
				handbrakeInputValue = 0f;

				if (carMainController.speed > 30f) {

					throttleInputValue -= Mathf.Abs(navigatorInput) / 3f;
					brakeInputValue += Mathf.Abs(navigatorInput) / 3f;

				}

			}

			break;

			case NavigationMode.FollowTarget:

				detectorCollider.radius = startFollowDistanceValue;

				// If our scene doesn't have a Waypoints Container, return with error.
				if (!targetChaseTransform) {

					StopCar();
					return;

				}

				// Setting destination of the Navigator. 
				if (navigatorAgent.isOnNavMesh)
					navigatorAgent.SetDestination(targetChaseTransform.position);

				// Checks for the distance to target. 
				float distanceToTarget = GetPathLengthValue(navigatorAgent.path);
				
				if (!reversingNowFlag) {

					throttleInputValue = distanceToTarget < (stopFollowDistanceValue * Mathf.Lerp(1f, 5f, carMainController.speed / 50f)) ? Mathf.Lerp(-5f, 1f, distanceToTarget / (stopFollowDistanceValue / 1f)) : 1f;
					throttleInputValue *= Mathf.Clamp01(Mathf.Lerp(10f, 0f, (carMainController.speed) / maximumSpeedValue));
					brakeInputValue = distanceToTarget < (stopFollowDistanceValue * Mathf.Lerp(1f, 5f, carMainController.speed / 50f)) ? Mathf.Lerp(5f, 0f, distanceToTarget / (stopFollowDistanceValue / 1f)) : 0f;
					handbrakeInputValue = 0f;

					if (carMainController.speed > 30f) {

						throttleInputValue -= Mathf.Abs(navigatorInput) / 3f;
						brakeInputValue += Mathf.Abs(navigatorInput) / 3f;

					}

					if (throttleInputValue < .05f)
						throttleInputValue = 0f;
					if (brakeInputValue < .05f)
						brakeInputValue = 0f;

				}

				break;

		}

        // Steer Input.
        steerInputValue = (ignoreWaypointNowFlag ? rayInputValue : navigatorInput + rayInputValue);
        steerInputValue = Mathf.Clamp(steerInputValue, -1f, 1f) * carMainController.direction;
        throttleInputValue = Mathf.Clamp01(throttleInputValue);
		brakeInputValue = Mathf.Clamp01(brakeInputValue);
		handbrakeInputValue = Mathf.Clamp01(handbrakeInputValue);

		if (reversingNowFlag) {

			throttleInputValue = 0f;
			brakeInputValue = 1f;
			handbrakeInputValue = 0f;

		} else {

			if (carMainController.speed < 5f && brakeInputValue >= .5f) {

				brakeInputValue = 0f;
				handbrakeInputValue = 1f;

			}
		
		}

    }
		
	private void CheckCrashReset (){

		if (navigationAIMode == NavigationMode.FollowTarget && GetPathLengthValue(navigatorAgent.path) < startFollowDistanceValue) {

			reversingNowFlag = false;
			resetTimeValue = 0;
			return;

		}

		// If unable to move forward, puts the gear to R.
		if(carMainController.speed <= 5 && transform.InverseTransformDirection(carMainController.rigid.velocity).z < 1f)
			resetTimeValue += Time.deltaTime;

		if (resetTimeValue >= 2)
			reversingNowFlag = true;

		if (resetTimeValue >= 4 || carMainController.speed >= 25){

			reversingNowFlag = false;
			resetTimeValue = 0;

		}
		
	}

	private void FixedRaycastsInput() {

		int[] anglesOfRaycasts = new int[5];
		anglesOfRaycasts[0] = 0;
		anglesOfRaycasts[1] = Mathf.FloorToInt(raycastAngleValue / 3f);
		anglesOfRaycasts[2] = Mathf.FloorToInt(raycastAngleValue / 1f);
		anglesOfRaycasts[3] = -Mathf.FloorToInt(raycastAngleValue / 1f);
		anglesOfRaycasts[4] = -Mathf.FloorToInt(raycastAngleValue / 3f);

		// Ray pivot position.
		Vector3 pivotPos = transform.position;
		pivotPos += transform.forward * carMainController.FrontLeftWheelCollider.transform.localPosition.z;

		RaycastHit hit;
		rayInputValue = 0f;
		bool casted = false;

		for (int i = 0; i < anglesOfRaycasts.Length; i++) {

			Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLengthValue, Color.white);

			if (Physics.Raycast(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward, out hit, raycastLengthValue, obstacleLayersMask) && !hit.collider.isTrigger && hit.transform.root != transform) {

				switch (navigationAIMode) {

					case NavigationMode.FollowWaypoints:

						Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLengthValue, Color.red);
						casted = true;

						if (i != 0)
							rayInputValue -= Mathf.Lerp(Mathf.Sign(anglesOfRaycasts[i]), 0f, (hit.distance / raycastLengthValue));

						break;

					case NavigationMode.ChaseTarget:

						if (targetChaseTransform && hit.transform != targetChaseTransform && !hit.transform.IsChildOf(targetChaseTransform)) {

							Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLengthValue, Color.red);
							casted = true;

							if (i != 0)
								rayInputValue -= Mathf.Lerp(Mathf.Sign(anglesOfRaycasts[i]), 0f, (hit.distance / raycastLengthValue));

						}

						break;

					case NavigationMode.FollowTarget:

						Debug.DrawRay(pivotPos, Quaternion.AngleAxis(anglesOfRaycasts[i], transform.up) * transform.forward * raycastLengthValue, Color.red);
						casted = true;

						if (i != 0)
							rayInputValue -= Mathf.Lerp(Mathf.Sign(anglesOfRaycasts[i]), 0f, (hit.distance / raycastLengthValue));

						break;

				}

				if (casted)
					obstacleObject = hit.transform.gameObject;
				else
					obstacleObject = null;

			}

		}

		raycastingFlag = casted;
        rayInputValue = Mathf.Clamp(rayInputValue, -1f, 1f);

        if (raycastingFlag && Mathf.Abs(rayInputValue) > .5f)
            ignoreWaypointNowFlag = true;
        else
            ignoreWaypointNowFlag = false;

	}

	private void FeedRCCInput(){

		// Feeding gasInput of the RCC.
		if (!carMainController.changingGear && !carMainController.cutGas)
			carMainController.throttleInput = (carMainController.direction == 1 ? Mathf.Clamp01(throttleInputValue) : Mathf.Clamp01(brakeInputValue));
		else
			carMainController.throttleInput = 0f;

		if (!carMainController.changingGear && !carMainController.cutGas)
			carMainController.brakeInput = (carMainController.direction == 1 ? Mathf.Clamp01(brakeInputValue) : Mathf.Clamp01(throttleInputValue));
		else
			carMainController.brakeInput = 0f;

		// Feeding steerInput of the RCC.
		if (smoothedSteerFlag)
            carMainController.steerInput = Mathf.Lerp(carMainController.steerInput, steerInputValue, Time.deltaTime * 20f);
        else
            carMainController.steerInput = steerInputValue;

		carMainController.handbrakeInput = handbrakeInputValue;

    }

	private void StopCar(){

		throttleInputValue = 0f;
		brakeInputValue = 0f;
		steerInputValue = 0f;
		handbrakeInputValue = 1f;

	}

	private void CheckCarTargets() {

		// Removing unnecessary targets in list.
		for (int i = 0; i < targetsInZoneList.Count; i++) {

			if (targetsInZoneList[i] == null)
				targetsInZoneList.RemoveAt(i);

			if (!targetsInZoneList[i].gameObject.activeInHierarchy)
				targetsInZoneList.RemoveAt(i);

			else {

				if (Vector3.Distance(transform.position, targetsInZoneList[i].transform.position) > (detectorCollider.radius * 1.25f))
					targetsInZoneList.RemoveAt(i);

			}

		}

		// If there is a target, get closest enemy.
		if (targetsInZoneList.Count > 0)
			targetChaseTransform = GetClosestEnemyTransform(targetsInZoneList.ToArray());
		else
			targetChaseTransform = null;

	}
	
	private void OnTriggerEnter (Collider col){

		if(col.transform.root.CompareTag(targetObjectTag)){
			
			if (!targetsInZoneList.Contains (col.transform.root))
				targetsInZoneList.Add (col.transform.root);

		}

	}

	private Transform GetClosestEnemyTransform (Transform[] enemies){

		Transform bestTarget = null;

		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = transform.position;

		foreach(Transform potentialTarget in enemies){

			Vector3 directionToTarget = potentialTarget.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;

			if(dSqrToTarget < closestDistanceSqr){

				closestDistanceSqr = dSqrToTarget;
				bestTarget = potentialTarget;

			}

		}

		return bestTarget;

	}

	public static bool GetPathFlag(NavMeshPath path, Vector3 fromPos, Vector3 toPos, int passableMask) {

		path.ClearCorners();

		if (NavMesh.CalculatePath(fromPos, toPos, passableMask, path) == false)
			return false;

		return true;

	}

	private static float GetPathLengthValue(NavMeshPath path) {

		float lng = 0.0f;

		if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1)) {

			for (int i = 1; i < path.corners.Length; ++i)
				lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);

		}

		return lng;

	}

	private void OnDisable(){

		// Calling this event when AI vehicle is destroyed.
		if (OnRCCAIDestroyedEvent != null)
			OnRCCAIDestroyedEvent (this);

	}
	
}