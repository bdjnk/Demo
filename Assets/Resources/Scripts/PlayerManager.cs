using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	private GameObject master;
	private GameData gameData;

	//public int redScorePercent;
	//public int blueScorePercent;
	private int percentToWin = 50;
	
	private void Awake()
	{
		master = GameObject.FindGameObjectWithTag("Master");
		gameData = master.GetComponent<GameData>();
	}
	
	public void Enable(bool state)
	{
		Screen.showCursor = !state;
		
		foreach (MouseLook mouseLook in GetComponentsInChildren<MouseLook>())
		{
			mouseLook.enabled = state;
		}
		GetComponent<CharacterMotor>().enabled = state;
		GetComponent<FPSInputController>().enabled = state;
		
		GetComponentInChildren<Camera>().enabled = state;
		GetComponentInChildren<PG_Gun>().enabled = state;
		
		enabled = state;
		
		master.GetComponent<UpgradeManager>().enabled = true;
	}
	
	public void JoinTeam()
	{
		Texture color = master.GetComponent<GameData>().GetTeam();
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
		if(checkForWin()){
			Enable(false);
			master.GetComponent<MenuManager>().EndGame();
		}
		
	}
	
	//private int totalCubes;
	
	private void Start()
	{
		//totalCubes = master.GetComponent<PG_Map>().cubeCount;
	}
	
	private bool showHUD = true;
	
	private bool checkForWin(){
		
		//redScorePercent = (int)(100.0f*gameData.redScore/totalCubes);
		//blueScorePercent = (int)(100.0f*gameData.blueScore/gameData.totalCubes);
		
		if(gameData.redTeamPercent >= percentToWin || gameData.blueTeamPercent >= percentToWin){
			return true;
		}
		return false;
	}
	
	private void OnGUI()
	{
		if (!showHUD) { return; } // else show HUD
		
		float buttonX = Screen.width*0.02f;
		float buttonY = Screen.width*0.02f;
		float buttonW = Screen.width*0.12f;
		float buttonH = Screen.width*0.20f;
		
		
		if (tag == "Red") // display the lists
		{
			//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Red Team: \n"+gmScript.redTeamString);
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Red Team: \n"+gameData.redScore+"\n "+gameData.redTeamPercent+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Blue Team: \n"+gameData.blueScore+"\n "+gameData.blueTeamPercent+"%");
		}
		else if (tag == "Blue")
		{
			//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Blue Team: \n"+gmScript.blueTeamString);
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Blue Team: \n"+gameData.blueScore+"\n "+gameData.blueTeamPercent+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Red Team: \n"+gameData.redScore+"\n "+gameData.redTeamPercent+"%");
		}
		//display only within 5 percent of win
		if(gameData.redTeamPercent > percentToWin - 5 || gameData.blueTeamPercent > percentToWin - 5){
			if(gameData.blueScore > gameData.redScore){
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH/2), "Blue Team \n" + (percentToWin - gameData.blueTeamPercent) + "%\n from Win");
			} else {
				GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH/2), "Red Team \n" + (percentToWin - gameData.redTeamPercent) + "%\n from Win");
			//GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH), "My Cubes: \n"+myTotalOwned+"\n"+myPercentOfTeamTotal+"%\nClaims: \n"+myTotalClaims);
			}
		}
	}
}