using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
	private GameData gameData;
	
	private int percentToWin = 75;
	private int totalCubes;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
	}
	
	private void Start()
	{
		totalCubes = gameData.GetComponent<PG_Map>().cubeCount;
	}
	
	public float gameLength;
	private float gameEndTime;
	
	//TODO CRITICAL SetTimer is only being called on the local PlayerManager of the
	// PlayerManager that invoked it remotely. In fact, if you only add a PlayerManager
	// to the active local player, it throws an error about being unable to call SetTimer!
	[RPC] private void SetTimer(float endTime) { gameEndTime = endTime; }
	
	public void Enable(bool state)
	{
		enabled = state;
		
		GetComponent<CharacterMotor>().enabled = state;
		GetComponent<FPSInputController>().enabled = state;
		
		GetComponentInChildren<Camera>().enabled = state;
		GetComponentInChildren<PG_Gun>().enabled = state;
		
		MouseEnable(state);
		
		gameData.GetComponent<UpgradeManager>().enabled = true;
		
		gameLength = 0.2f*60;
		
		if (Network.isServer)
		{
			foreach (GameObject player in gameData.players)
			{
				player.networkView.RPC("SetTimer", RPCMode.AllBuffered, (float)Network.time + gameLength);
			}
		}
		
		//gameData.networkView.RPC("AddPlayer", RPCMode.Server, networkView.viewID, gameData.networkPlayer);
	}
	
	public void MouseEnable(bool state)
	{
		Screen.lockCursor = state;
		
		foreach (MouseLook mouseLook in GetComponentsInChildren<MouseLook>())
		{
			mouseLook.enabled = state;
		}
	}
	
	public void JoinTeam() // called from Ready.SpawnPlayer()
	{		
		Vector3 color = gameData.GetTeam(gameObject);
		networkView.RPC("SetColor", RPCMode.AllBuffered, color, networkView.viewID);
		
		//GameObject colorize = Network.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, 5) as GameObject;
		//networkView.RPC("InitializeColorize", RPCMode.All, colorize.networkView.viewID, color);
	}
	
	[RPC] private void SetColor(Vector3 color, NetworkViewID playerID)
	{
		NetworkView playerNetView = NetworkView.Find(playerID);
		
		if (playerNetView != null)
		{
			GameObject player = playerNetView.gameObject;
			
			if (player != null)
			{
				player.GetComponentInChildren<MeshRenderer>().material.color = new Color(color.x, color.y, color.z);
				
				string colorName = Mathf.Approximately(color.x, 1) ? "Red" : "Blue"; // if red ~= 255
				Debug.LogWarning("Color set to "+colorName);
				
				player.GetComponentInChildren<PG_Gun>().shotPrefab = Resources.Load("Prefabs/"+colorName+"Shot") as GameObject;
				player.tag = colorName;
			}
		}
	}
	
	private bool won = false;
	private float winWait = 8;
	
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Q))
		{
			showHUD = !showHUD;
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			MouseEnable(false);
		}
		
		if (!won)
		{
			if ((gameData.redPercent >= percentToWin || gameData.bluePercent >= percentToWin) || (gameEndTime > 0 && Network.time > gameEndTime))
			{
				won = true;
				winWait += Time.time;
				
				GetComponentInChildren<PG_Gun>().enabled = false;
			}
		}
		
		if (won && Time.time > winWait)
		{
			won = false;
			winWait = 8;
			
			gameData.ClearData(false);
			
			foreach (GameObject cube in gameData.GetComponent<UpgradeManager>().cubes)
			{
				if (cube != null)
					cube.GetComponent<PG_Cube>().SetGray();
			}
			
			GetComponentInChildren<PG_Gun>().enabled = true;
			
			if (Network.isServer)
			{
				foreach (GameObject player in gameData.players)
				{
					player.networkView.RPC("SetTimer", RPCMode.AllBuffered, (float)Network.time + gameLength);
				}
			}
			gameEndTime = (float)Network.time + gameLength;
		}
	}
	
	[RPC] private void EndGame()
	{
		won = true;
		winWait += Time.time;
		
		GetComponentInChildren<PG_Gun>().enabled = false;
	}
	
	private bool showHUD = true;
	
	private void OnGUI()
	{
		if (!showHUD) { return; } // else show HUD
			
		float buttonX = Screen.width*0.02f;
		float buttonY = Screen.width*0.02f;
		float buttonW = Screen.width*0.12f;
		float buttonH = Screen.width*0.20f;
		
		if (!won)
		{
			if (tag == "Red") // display the lists
			{
				//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Red Team:\n"+gmScript.redTeamString);
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Red Team:\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%\nCountdown\n"+Mathf.CeilToInt(gameEndTime-(float)Network.time));
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Blue Team:\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%");
			}
			else if (tag == "Blue")
			{
				//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Blue Team:\n"+gmScript.blueTeamString);
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Blue Team:\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%\nCountdown\n"+Mathf.CeilToInt(gameEndTime-(float)Network.time));
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Red Team:\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%");
			}
			//GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH), "My Cubes:\n"+myTotalOwned+"\n"+myPercentOfTeamTotal+"%\nClaims:\n"+myTotalClaims);
		}
		
		if (won)
		{
			if (gameData.redScore > gameData.blueScore) // display the appropriate list
			{
				GUI.Box(new Rect(-9, -9, Screen.width+9, Screen.height+9), "\nRed Team Wins!\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%"
					+"\n\n\nBlue Team \n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%"+"\n\n\nRestart in: \n"+Mathf.CeilToInt(winWait-Time.time));
			}
			else 
			{
				GUI.Box(new Rect(-9, -9, Screen.width+9, Screen.height+9), "\nBlue Team Wins!\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%"
					+"\n\n\nRed Team \n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%"+"\n\n\nRestart in: \n"+Mathf.CeilToInt(winWait-Time.time));
			}
		}
		else if (gameData.redPercent > percentToWin-5 || gameData.bluePercent > percentToWin-5)
		{
			if (gameData.blueScore > gameData.redScore)
			{
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH/2), "Blue Team\n"+(percentToWin - gameData.bluePercent)+"%\nfrom Win");
			}
			else
			{
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH/2), "Red Team\n"+(percentToWin - gameData.redPercent)+"%\nfrom Win");
			}
		}
	}
}