using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
	private GameData gameData;
	public bool isServer;
	
	private int percentToWin = 75;
	public int myScore = 0;
	private int myPercent = 0;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
	}
	
	public float gameEndTime;
	
	//TODO the first end game is not occuring for newly joined players
	// perhaps make those player wait until the current game finishes...
	[RPC] private void SetTimer(float endTime) { gameEndTime = endTime; }
	
	public bool ready = false;
	[RPC] public void Ready() { ready = true; }
	
	private void OnNetworkInstantiate (NetworkMessageInfo info)
	{
		networkView.RPC("Ready", RPCMode.All);
	}
	
	public void Enable(bool state)
	{
		enabled = state;
		
		GetComponent<CharacterMotor>().enabled = state;
		GetComponent<FPSInputController>().enabled = state;
		
		GetComponentInChildren<Camera>().enabled = state;
		GetComponentInChildren<PG_Gun>().enabled = state;
		
		MouseEnable(state);
		
		gameData.GetComponent<UpgradeManager>().enabled = true;
		
		if (Network.isServer) // only gets called once...
		{
			networkView.RPC("SetAsServer", RPCMode.AllBuffered);
			networkView.RPC("SetTimer", RPCMode.AllBuffered, (float)Network.time + gameData.gameLength);
		}
	}
	[RPC] private void SetAsServer() { isServer = true; }
	
	public void MouseEnable(bool state)
	{
		Screen.lockCursor = state;
		
		foreach (MouseLook mouseLook in GetComponentsInChildren<MouseLook>())
		{
			mouseLook.enabled = state;
		}
	}
	
	public void JoinTeam() // called once from PlayerSetup.Update()
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
		myPercent = (int)(100.0f * myScore/gameData.totalCubes);
		
		if (Input.GetKeyUp(KeyCode.Q))
		{
			showHUD = !showHUD;
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			MouseEnable(false);
		}
		
		if (gameData.gameLength == 0) { gameEndTime = (float)Network.time + 1; } // don't end because of the timer
		
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
			
			//gameData.ClearData(false);
			
			foreach (GameObject cube in gameData.GetComponent<UpgradeManager>().cubes)
			{
				if (cube != null && cube.renderer.material.color != gameData.gray)
				{
					cube.GetComponent<PG_Cube>().SetGray();
				}
			}
			
			GetComponentInChildren<PG_Gun>().enabled = true;
			
			if (Network.isServer)
			{
				foreach (GameObject player in gameData.players)
				{
					player.networkView.RPC("SetTimer", RPCMode.AllBuffered, (float)Network.time + gameData.gameLength);
				}
			}
			gameEndTime = (float)Network.time + gameData.gameLength;
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
		float edge = Screen.width*0.02f;
		float buttonW = 100f;
		float buttonH = 70f;
		
		if (!won && gameData.gameLength > 0)
		{
			GUI.Box(new Rect(Screen.width-buttonW-edge, Screen.height-buttonH*0.6f-edge, buttonW, buttonH*0.6f), "Countdown:\n"+Mathf.CeilToInt(gameEndTime-(float)Network.time));
		}
		
		if (!showHUD) { return; } // else show HUD
		
		if (!won)
		{
			if (tag == "Red") // display the lists
			{
				GUI.Box(new Rect(Screen.width-buttonW-edge, edge, buttonW, buttonH), "Red Team:\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%");
				GUI.Box(new Rect(Screen.width-buttonW-edge, edge*2+buttonH, buttonW, buttonH), "Blue Team:\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%");
			}
			else if (tag == "Blue")
			{
				GUI.Box(new Rect(Screen.width-buttonW-edge, edge, buttonW, buttonH), "Blue Team:\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%");
				GUI.Box(new Rect(Screen.width-buttonW-edge, edge*2+buttonH, buttonW, buttonH), "Red Team:\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%");
			}
			GUI.Box(new Rect(Screen.width-buttonW-edge, edge*3+buttonH*2, buttonW, buttonH*0.8f), "Personal:\n"+myScore+" cubes\n"+myPercent+"%");
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
				GUI.Box(new Rect(Screen.width-buttonW-edge, edge*4+buttonH*2+buttonH*0.8f, buttonW, buttonH*0.6f), "Blue Team\n"+(percentToWin - gameData.bluePercent)+"% to Win");
			}
			else
			{
				GUI.Box(new Rect(Screen.width-buttonW-edge, edge*4+buttonH*2+buttonH*0.8f, buttonW, buttonH*0.6f), "Red Team\n"+(percentToWin - gameData.redPercent)+"% to Win");
			}
		}
	}
}