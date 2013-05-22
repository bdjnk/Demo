using UnityEngine;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
	public Texture[] upgrades;
	public GameObject[] cubes;

	private void Start()
	{
		cubes = GameObject.FindGameObjectsWithTag("Cube");
	}
	
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.E))
		{
			Texture upgrade = upgrades[Random.Range(0, upgrades.Length)]; // random upgrade texture
			GameObject cube = cubes[Random.Range(0, cubes.Length)]; // grab a random cube from the map
			
			cube.GetComponent<PG_Cube>().networkView.RPC("SetDecal", RPCMode.AllBuffered, upgrade.name);
		}
	}
}
