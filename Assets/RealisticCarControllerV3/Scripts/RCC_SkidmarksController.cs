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
/// Skidmarks Manager for RCC.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/Skidmarks")]
public class RCC_SkidmarksController : MonoBehaviour {

	private MeshFilter meshFilterValue;
	private Mesh meshValue;

	[FormerlySerializedAs("maxMarks")] public int maxMarksValue = 1024;			// Maximum number of marks total handled by one instance of the script.
	[FormerlySerializedAs("markWidth")] public float markWidthValue = 0.275f;		// The width of the skidmarks. Should match the width of the wheel that it is used for. In meters.
	[FormerlySerializedAs("groundOffset")] public float groundOffsetValue = 0.02f;	// The distance the skidmarks is places above the surface it is placed upon. In meters.
	[FormerlySerializedAs("minDistance")] public float minDistanceValue = 0.1f;		// The minimum distance between two marks places next to each other. 

	private int indexShiftValue;
	private int numMarksValue = 0;

	// Variables for each mark created. Needed to generate the correct mesh.
	private class markSectionData{
		public Vector3 posVector = Vector3.zero;
		public Vector3 normalVector = Vector3.zero;
		public Vector4 tangentVector = Vector4.zero;
		public Vector3 poslVector = Vector3.zero;
		public Vector3 posrVector = Vector3.zero;
		public float intensityValue = 0.0f;
		public int lastIndexValue = 0;
	};

	[FormerlySerializedAs("skidmarks")] [SerializeField] private markSectionData[] skidmarksMass;

	[FormerlySerializedAs("updated")] [SerializeField] private bool  updatedFlag = false;

	// Initiallizes the array holding the skidmark sections.
	private void Awake (){

		skidmarksMass = new markSectionData[maxMarksValue];

		for(int i= 0; i < maxMarksValue; i++)
			skidmarksMass[i]=new markSectionData();

		meshFilterValue = GetComponent<MeshFilter> ();
		meshValue = meshFilterValue.mesh;

	}

	private void Start (){

		if (transform.position != Vector3.zero)
			transform.position = Vector3.zero;

	}

	// Function called by the wheels that is skidding. Gathers all the information needed to
	// create the mesh later. Sets the intensity of the skidmark section b setting the alpha
	// of the vertex color.
	public int AddWheelSkidMark ( Vector3 pos ,   Vector3 normal ,   float intensity ,   int lastIndex){

		if(intensity > 1f)
			intensity = 1f;
		if(intensity < 0f)
			return -1;

		if (lastIndex > 0) {

			float sqrDistance = (pos - skidmarksMass [lastIndex % maxMarksValue].posVector).sqrMagnitude;

			if (sqrDistance < minDistanceValue)
				return lastIndex;

		}

		markSectionData curr = skidmarksMass[numMarksValue % maxMarksValue];
		curr.posVector = pos + normal * groundOffsetValue;
		curr.normalVector = normal;
		curr.intensityValue = intensity;
		curr.lastIndexValue = lastIndex;

		if(lastIndex != -1)
		{
			markSectionData last = skidmarksMass[lastIndex % maxMarksValue];
			Vector3 dir = (curr.posVector - last.posVector);
			Vector3 xDir = Vector3.Cross(dir,normal).normalized;

			curr.poslVector = curr.posVector + xDir * markWidthValue * 0.5f;
			curr.posrVector = curr.posVector - xDir * markWidthValue * 0.5f;
			curr.tangentVector = new Vector4(xDir.x, xDir.y, xDir.z, 1);

			if(last.lastIndexValue == -1)
			{
				last.tangentVector = curr.tangentVector;
				last.poslVector = curr.posVector + xDir * markWidthValue * 0.5f;
				last.posrVector = curr.posVector - xDir * markWidthValue * 0.5f;
			}
		}
		numMarksValue++;
		updatedFlag = true;
		return numMarksValue -1;

	}

	// If the mesh needs to be updated, i.e. a new section has been added,
	// the current mesh is removed, and a new mesh for the skidmarks is generated.
	private void LateUpdate (){

		if (!updatedFlag)
			return;

		updatedFlag = false;

		meshValue.Clear ();

		int segmentCount = 0;

		for (int j = 0; j < numMarksValue && j < maxMarksValue; j++) {

			if (skidmarksMass [j].lastIndexValue != -1 && skidmarksMass [j].lastIndexValue > numMarksValue - maxMarksValue)
				segmentCount++;

		}

		Vector3[] vertices = new Vector3[segmentCount * 4];
		Vector3[] normals = new Vector3[segmentCount * 4];
		Vector4[] tangents = new Vector4[segmentCount * 4];
		Color[] colors = new Color[segmentCount * 4];
		Vector2[] uvs = new Vector2[segmentCount * 4];

		int[] triangles = new int[segmentCount * 6];
		segmentCount = 0;

		for (int i = 0; i < numMarksValue && i < maxMarksValue; i++){

			if (skidmarksMass [i].lastIndexValue != -1 && skidmarksMass [i].lastIndexValue > numMarksValue - maxMarksValue) {

				markSectionData curr = skidmarksMass [i];
				markSectionData last = skidmarksMass [curr.lastIndexValue % maxMarksValue];

				vertices [segmentCount * 4 + 0] = last.poslVector;
				vertices [segmentCount * 4 + 1] = last.posrVector;
				vertices [segmentCount * 4 + 2] = curr.poslVector;
				vertices [segmentCount * 4 + 3] = curr.posrVector;

				normals [segmentCount * 4 + 0] = last.normalVector;
				normals [segmentCount * 4 + 1] = last.normalVector;
				normals [segmentCount * 4 + 2] = curr.normalVector;
				normals [segmentCount * 4 + 3] = curr.normalVector;

				tangents [segmentCount * 4 + 0] = last.tangentVector;
				tangents [segmentCount * 4 + 1] = last.tangentVector;
				tangents [segmentCount * 4 + 2] = curr.tangentVector;
				tangents [segmentCount * 4 + 3] = curr.tangentVector;

				colors [segmentCount * 4 + 0] = new Color (0, 0, 0, last.intensityValue);
				colors [segmentCount * 4 + 1] = new Color (0, 0, 0, last.intensityValue);
				colors [segmentCount * 4 + 2] = new Color (0, 0, 0, curr.intensityValue);
				colors [segmentCount * 4 + 3] = new Color (0, 0, 0, curr.intensityValue);

				uvs [segmentCount * 4 + 0] = new Vector2 (0, 0);
				uvs [segmentCount * 4 + 1] = new Vector2 (1, 0);
				uvs [segmentCount * 4 + 2] = new Vector2 (0, 1);
				uvs [segmentCount * 4 + 3] = new Vector2 (1, 1);

				triangles [segmentCount * 6 + 0] = segmentCount * 4 + 0;
				triangles [segmentCount * 6 + 2] = segmentCount * 4 + 1;
				triangles [segmentCount * 6 + 1] = segmentCount * 4 + 2;

				triangles [segmentCount * 6 + 3] = segmentCount * 4 + 2;
				triangles [segmentCount * 6 + 5] = segmentCount * 4 + 1;
				triangles [segmentCount * 6 + 4] = segmentCount * 4 + 3;
				segmentCount++;

			}

		}

		meshValue.vertices=vertices;
		meshValue.normals=normals;
		meshValue.tangents=tangents;
		meshValue.triangles=triangles;
		meshValue.colors=colors;
		meshValue.uv=uvs;

	}

}