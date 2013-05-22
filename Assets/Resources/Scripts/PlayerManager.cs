using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	private GameData gameData;
	
	private int percentToWin = 75;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
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
		Debug.LogWarning(networkView.viewID);
		Debug.LogWarning(networkView.isMine);
		
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
		
		if (gameData.redPercent >= percentToWin || gameData.bluePercent >= percentToWin)
		{
			//TODO add game over code (see Ben's code)
			gameData.ClearData(false);
			foreach (GameObject cube in gameData.GetComponent<UpgradeManager>().cubes)
			{
				cube.networkView.RPC("SetGray", RPCMode.AllBuffered);
			}
		}
	}
	
	private int totalCubes;
	
	private void Start()
	{
		totalCubes = gameData.GetComponent<PG_Map>().cubeCount;
	}
	
	private bool showHUD = true;
	
	private void OnGUI()
	{
		if (!showHUD) { return; } // else show HUD
		
		float buttonX = Screen.width*0.02f;
		float buttonY = Screen.width*0.02f;
		float buttonW = Screen.width*0.12f;
		float buttonH = Screen.width*0.20f;
		
		if (tag == "Red") // display the lists
		{
			//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Red Team:\n"+gmScript.redTeamString);
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Red Team:\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Blue Team:\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%");
		}
		else if (tag == "Blue")
		{
			//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Blue Team:\n"+gmScript.blueTeamString);
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Blue Team:\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Red Team:\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%");
		}
		//GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH), "My Cubes:\n"+myTotalOwned+"\n"+myPercentOfTeamTotal+"%\nClaims:\n"+myTotalClaims);
		
		if (gameData.redPercent > percentToWin-5 || gameData.bluePercent > percentToWin-5)
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