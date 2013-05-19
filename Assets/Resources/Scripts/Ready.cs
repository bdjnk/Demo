using UnityEngine;
using System.Collections;

public class Ready : MonoBehaviour
{
	public GameObject playerPrefab;
	private GameObject player;
	
	void Start() // shows we are ready to go
	{
		SpawnPlayer();
	}
	
	private void SpawnPlayer()
	{
		player = Network.Instantiate(playerPrefab, new Vector3(-10, 2, -10), Quaternion.identity, 0) as GameObject;
		
		// wait for server to finish sending me state data... wait for "Ground(Clone)"
		PlayerManager playerManager = player.GetComponent<PlayerManager>();
		
		playerManager.Enable(true);
		playerManager.JoinTeam();
	}
}
