using UnityEngine;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
	public Texture[] upgrades;
	private GameObject[] cubes;

	private void Start()
	{
		cubes = GameObject.FindGameObjectsWithTag("Cube");
	}
	
	public void ResetData(){
		//cubes = null;
		Destroy (this);
	}
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.E))
		{
			Texture upgrade = upgrades[Random.Range(0, upgrades.Length)];
			GameObject cube = cubes[Random.Range(0, cubes.Length)]; // grab a random cube from the map
			PG_Cube cubeScript = cube.GetComponent<PG_Cube>();
			cubeScript.SetUpgrade(upgrade);
		}
	}
		
}
