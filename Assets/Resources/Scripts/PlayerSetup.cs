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
		gameData.players.Add(gameObject);
		
		playerManager = GetComponent<PlayerManager>();
	}
	
	private void Update()
	{
		if (!done && playerManager.ready)
		{
			if (Network.isServer && !networkView.isMine) // RPC them the data.
				{
				PG_Cube cubeScript;
				
				foreach (GameObject cube in gameData.GetComponent<UpgradeManager>().cubes) // doesn't work here because the cubes aren't created yet.
				{
					cubeScript = cube.GetComponent<PG_Cube>(); // RPC them amountRed and amountBlue, the rest can be calculated
					networkView.RPC("SetCubes", networkView.owner, cube.networkView.viewID, cubeScript.amountRed, cubeScript.amountBlue);
				}
			}
			else if (networkView.isMine)
			{
				playerManager.Enable(true);
				playerManager.JoinTeam();
			}
			done = true;
		}
	}
	
	[RPC] private void SetCubes(NetworkViewID id, float amountRed, float amountBlue)
	{
		GameObject cube = NetworkView.Find(id).gameObject;
		PG_Cube cubeScript = cube.GetComponent<PG_Cube>();
		cubeScript.amountRed = amountRed;
		cubeScript.amountBlue = amountBlue;
		cubeScript.SetColor();
	}
}
