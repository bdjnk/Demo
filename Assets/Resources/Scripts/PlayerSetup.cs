using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSetup : MonoBehaviour
{
	private GameData gameData;
	
	PlayerManager playerManager;
	
	private bool done = false;
	private float wait;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
		playerManager = GetComponent<PlayerManager>();
	}
	
	private void Update()
	{
		if (!done && playerManager.ready) // do once when the player is ready.
		{
			gameData.enabled = true;
			
			if (Network.isServer && !networkView.isMine) // RPC them the data.
			{
				PG_Cube cubeScript;
				
				foreach (GameObject cube in gameData.GetComponent<UpgradeManager>().cubes)
				{
					cubeScript = cube.GetComponent<PG_Cube>(); // RPC them amountRed and amountBlue, the rest can be calculated
					networkView.RPC("SetCube", networkView.owner, cube.networkView.viewID, cubeScript.amountRed, cubeScript.amountBlue);
				}
				//send current score to client
				//networkView.RPC ("GetCurrentScores",networkView.owner,gameData.blueScore,gameData.blueOwned,gameData.redScore,gameData.redOwned);
				
			}
			else if (networkView.isMine)
			{
				playerManager.Enable(true);
				playerManager.JoinTeam(false); // false means we're not switching colors
			}
			done = true;
		}
	}
	//update client scoring when starting
	/*
	[RPC] private void GetCurrentScores(int blueScore,int blueOwned,int redScore,int redOwned){
		gameData.blueScore = blueScore;
		gameData.blueOwned = blueOwned;
		gameData.redScore = redScore;
		gameData.redOwned = redOwned;
	}	*/
	
	[RPC] private void SetCube(NetworkViewID id, float amountRed, float amountBlue)
	{
		GameObject cube = NetworkView.Find(id).gameObject;
		PG_Cube cubeScript = cube.GetComponent<PG_Cube>();
		cubeScript.amountRed = amountRed;
		cubeScript.amountBlue = amountBlue;
		cubeScript.SetColor(null);
	}
	
	void OnTriggerEnter(Collider other)
	{
	}
}