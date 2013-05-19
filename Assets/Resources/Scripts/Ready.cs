using UnityEngine;
using System.Collections;

public class Ready : MonoBehaviour
{
	public GameObject playerPrefab;
	private GameObject player;
	
	void Start() // at this point we are ready to go
	{
		SpawnPlayer();
		
		foreach (GameObject bot in GameObject.FindGameObjectsWithTag("Bot"))
		{
			bot.GetComponent<PG_Bot>().Setup("Red"); //TODO get color properly
		}
	}
	
	private void SpawnPlayer()
	{
		player = Network.Instantiate(playerPrefab, new Vector3(-10, 2, -10), Quaternion.identity, 0) as GameObject;
		
		PlayerManager playerManager = player.GetComponent<PlayerManager>();
		
		playerManager.Enable(true);
		playerManager.JoinTeam();
	}
}