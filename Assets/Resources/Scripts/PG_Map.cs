using UnityEngine;
using System.Collections;

public class PG_Map : MonoBehaviour
{
	public GameObject donePrefab;
	
	public bool floor = true;
	public float spacing = 1.5f;
	
    private int[] citySize; // {width, height, depth}
	
    private int[] minBuildingSize;
    private int[] maxBuildingSize;
	
	public GameObject cubePrefab;
	public GameObject buildingPrefab;
	public GameObject lightingPrefab;
	public GameObject groundPrefab;
	
	public int cubeCount = 0;
	
	public void CreateMap()
	{
		minBuildingSize = new int [] {
			PlayerPrefs.GetInt("minBuildingSizeX"),
			PlayerPrefs.GetInt("minBuildingSizeY"),
			PlayerPrefs.GetInt("minBuildingSizeZ")
		};
		maxBuildingSize = new int [] {
			PlayerPrefs.GetInt("maxBuildingSizeX"),
			PlayerPrefs.GetInt("maxBuildingSizeY"),
			PlayerPrefs.GetInt("maxBuildingSizeZ")
		};
		
		Vector3 offset = Vector3.zero;
		Vector3 temp = Vector3.zero; //TODO give a meaningful name
		
		int width = PlayerPrefs.GetInt("citySizeX");//citySize[0];
		int height = PlayerPrefs.GetInt("citySizeY");//citySize[1];
		int depth = PlayerPrefs.GetInt("citySizeZ");//citySize[2];
		
		spacing = PlayerPrefs.GetFloat("buildingSpacing", 1.5f);
		floor = System.Convert.ToBoolean(PlayerPrefs.GetInt("hasFloor", 1));
		
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
		{
			GameObject ground = Network.Instantiate(groundPrefab, new Vector3(0f,-0.5f,0f), Quaternion.identity, 0) as GameObject;
			ground.isStatic = true;
		}
		GameObject done = Network.Instantiate(donePrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
		done.isStatic = true;
	}
	
	private Vector3 MakeBuilding(Vector3 offset)
	{
		int width = Random.Range(minBuildingSize[0], maxBuildingSize[0]+1);
		int height = Random.Range(minBuildingSize[1], maxBuildingSize[1]+1);
		int depth = Random.Range(minBuildingSize[2], maxBuildingSize[2]+1);
		
		GameObject building = Network.Instantiate(buildingPrefab, Vector3.zero,Quaternion.identity, 0) as GameObject;
		
		Vector3 center = Vector3.zero;
		
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				for (int d = 0; d < depth; d++)
				{
					GameObject cube = Network.Instantiate(cubePrefab, new Vector3(1.5f*w, 1.5f*h, 1.5f*d), Quaternion.identity, 0) as GameObject;
					networkView.RPC("SetParent", RPCMode.AllBuffered, cube.networkView.viewID, building.networkView.viewID);
					cube.isStatic = true;
					
			    	cubeCount++;
					center += cube.transform.position;
				}
			}
		}
		building.transform.position += offset;
		building.isStatic = true;
		
		int count = width * height * depth;
		center /= count;
		
		GameObject light = Network.Instantiate (lightingPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
		networkView.RPC("SetParent", RPCMode.AllBuffered, light.networkView.viewID, building.networkView.viewID);
		networkView.RPC("AddLight", RPCMode.AllBuffered, light.networkView.viewID, center, count);
		light.isStatic = true;
		
		return (new Vector3(width*1.5f, height*1.5f, depth*1.5f) * spacing);
	}
	
	[RPC]
	private void AddLight(NetworkViewID id, Vector3 center, int count)
	{
		GameObject light = NetworkView.Find(id).gameObject;
		light.AddComponent(typeof(Light));
		light.transform.localPosition = center;
		light.light.intensity = count / 20f;
	}
	
	[RPC] // this ought to be moved to a static class
	private void SetParent(NetworkViewID childID, NetworkViewID parentID)
	{
		NetworkView.Find(childID).transform.parent = NetworkView.Find(parentID).transform;
	}
}
