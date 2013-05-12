using UnityEngine;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
	public Texture[] textures;
	private GameObject[] cubes;

	// Use this for initialization
	void Start () {
		cubes = GameObject.FindGameObjectsWithTag("Cube");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.E))
		{
			GameObject cube = cubes[Random.Range(0, cubes.Length)]; // grab a random cube from the map
			PG_Cube cubeScript = cube.GetComponent<PG_Cube>();
			Texture upgrade = textures[Random.Range(0, textures.Length)];
			cube.renderer.material.SetTexture("_DecalTex", upgrade);
			//cube.renderer.material.SetTexture("_MainTex", null);
		}
	}
}
