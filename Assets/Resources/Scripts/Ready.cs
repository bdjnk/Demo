using UnityEngine;
using System.Collections;

public class Ready : MonoBehaviour
{
	public GameObject playerPrefab;
	private GameObject player;
	
	void Start() // at this point we are ready to go
	{
		SpawnPlayer();
		
		// bot stuff is on hold for now, but in the future bot color should be recalculated with each join
		foreach (GameObject bot in GameObject.FindGameObjectsWithTag("Bot"))
		{
			bot.GetComponent<PG_Bot>().SetColor("Red"); //TODO get color properly
		}
	}
	
	private void SpawnPlayer()
	{
		player = Network.Instantiate(playerPrefab, new Vector3(-10, 2, -10), Quaternion.identity, 4) as GameObject;
	}
}