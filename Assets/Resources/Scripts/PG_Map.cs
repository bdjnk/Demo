using UnityEngine;
//using UnityEditor;
using System.Collections;

public class PG_Map 
{
	public bool floor = true;
	public float spacing = 1.5f;
	
    public int[] citySize = {6, 1, 6}; // {width, height, depth}
	
    public int[] minBuildingSize = {1, 1, 1};
    public int[] maxBuildingSize = {3, 3, 3};
	
	public Object cubePrefab;
	public Object groundPrefab;
	
	public int cubeCount = 0;
	
	
	// Builds a Map	
	
	public void createMap(){
		Vector3 offset = Vector3.zero;
		Vector3 temp = Vector3.zero; //TODO give a meaningful name
		
		int width = citySize[0];
		int height = citySize[1];
		int depth = citySize[2];
		//Debug.Log ("creating; " + width + ", " + height + ", " + depth);
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				for (int d = 0; d < depth; d++)
				{
					temp.Set(offset.x * w, offset.y * h, offset.z * d);
					offset = Vector3.Max(offset, MakeBuilding(temp));
				}
			}
		}
		if (floor)
		{	groundPrefab = Resources.Load ("Prefabs/Ground");
			//Debug.Log ("cube: " + groundPrefab.ToString());
			GameObject ground = (GameObject) Network.Instantiate(groundPrefab,new Vector3(0f,-0.5f,0f),Quaternion.identity,1);
			//GameObject ground = Instantiate(groundPrefab) as GameObject;
			ground.transform.position = new Vector3(0, -0.5f, 0);
		}
	}
	
	
	Vector3 MakeBuilding(Vector3 offset)
	{
		//GameObject building = new GameObject("Building");
		//GameObject build = new GameObject("Building");
		GameObject buildingPrefab = Resources.Load ("Prefabs/Building") as GameObject;
		GameObject building = (GameObject) Network.Instantiate (buildingPrefab,
												Vector3.zero,Quaternion.identity,2);
		//building.AddComponent("PG_Building"); // add the building script
		building.AddComponent("NetworkView"); // add the network view
		
		Vector3 center = Vector3.zero;
		
		int width = Random.Range(minBuildingSize[0], maxBuildingSize[0]+1);
		int height = Random.Range(minBuildingSize[1], maxBuildingSize[1]+1);
		int depth = Random.Range(minBuildingSize[2], maxBuildingSize[2]+1);
		//Debug.Log ("creating bldg: " + width + ", " + height + ", " + depth);
		
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				for (int d = 0; d < depth; d++)
				{
					cubePrefab = Resources.Load ("Prefabs/Cube");
					//Debug.Log ("cube: " + cubePrefab.ToString());
					GameObject cube = (GameObject) Network.Instantiate(cubePrefab,new Vector3(1.5f*w,1.5f*h,1.5f*d),Quaternion.identity,1);
					//GameObject cube = Instantiate(cubePrefab) as GameObject;
			    	cubeCount++;
					cube.transform.parent = building.transform;
					//cube.transform.localPosition = new Vector3(1.5f * w, 1.5f * h, 1.5f * d);
					cube.AddComponent("PG_Cube");
					
					center += cube.transform.position;
				}
			}
		}
		building.transform.position += offset;
		
		int count = width * height * depth;
		center /= count;
		
		//GameObject light = new GameObject("Light");
		GameObject lightingPrefab = Resources.Load ("Prefabs/Lighting") as GameObject;
		GameObject light = (GameObject) Network.Instantiate (lightingPrefab,
										Vector3.zero,Quaternion.identity,2);
		light.AddComponent(typeof(Light));
		light.transform.parent = building.transform;
		light.transform.localPosition = center;
		light.light.intensity = count / 20f;
		
		return (new Vector3(width * 1.5f, height * 1.5f, depth * 1.5f) * spacing);
	}
}
