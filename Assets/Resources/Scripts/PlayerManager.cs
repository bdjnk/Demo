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
	}
	
	public void MouseEnable(bool state)
	{
		Screen.lockCursor = state;
		
		foreach (MouseLook mouseLook in GetComponentsInChildren<MouseLook>())
		{
			mouseLook.enabled = state;
		}
	}
	
	public void JoinTeam()
	{
		Texture color = gameData.GetTeam(gameObject);
		
		Debug.LogWarning(networkView.viewID);
		Debug.LogWarning(networkView.isMine);
		networkView.RPC("SetColor", RPCMode.AllBuffered, color.name);
	}
	
	[RPC] private void SetColor(string color)
	{
		GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", Resources.Load("Textures/"+color) as Texture);
		GetComponentInChildren<PG_Gun>().shotPrefab = Resources.Load("Prefabs/"+color+"Shot") as GameObject;
		tag = color;
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
			// do game over code (see Ben's code)
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
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Red Team:\n"+gameData.redScore+"\n "+gameData.redPercent+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Blue Team:\n"+gameData.blueScore+"\n "+gameData.bluePercent+"%");
		}
		else if (tag == "Blue")
		{
			//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Blue Team:\n"+gmScript.blueTeamString);
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Blue Team:\n"+gameData.blueScore+"\n "+gameData.bluePercent+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Red Team:\n"+gameData.redScore+"\n "+gameData.redPercent+"%");
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