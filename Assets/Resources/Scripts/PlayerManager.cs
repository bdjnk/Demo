using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	private GameData gameData;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
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
		
		gameData.GetComponent<UpgradeManager>().enabled = true;
	}
	
	public void JoinTeam()
	{
		Texture color = gameData.GetTeam();
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
	}
	
	private int totalCubes;
	
	private void Start()
	{
		totalCubes = gameData.GetComponent<PG_Map>().cubeCount;
	}
	
	private bool showHUD = false;
	
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
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Red Team: \n"+gameData.redScore+"\n "+(int)(100.0f*gameData.redScore/totalCubes)+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Blue Team: \n"+gameData.blueScore+"\n "+(int)(100.0f*gameData.blueScore/totalCubes)+"%");
		}
		else if (tag == "Blue")
		{
			//GUI.Box(new Rect(buttonX, buttonY, buttonW, buttonH),"Blue Team: \n"+gmScript.blueTeamString);
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY, buttonW, buttonH/2), "Blue Team: \n"+gameData.blueScore+"\n "+(int)(100.0f*gameData.blueScore/totalCubes)+"%");
			GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*0.6f, buttonW, buttonH/2), "Red Team: \n"+gameData.redScore+"\n "+(int)(100.0f*gameData.redScore/totalCubes)+"%");
		}
		//GUI.Box(new Rect(Screen.width-buttonW-buttonX, buttonY+buttonH*1.2f, buttonW, buttonH), "My Cubes: \n"+myTotalOwned+"\n"+myPercentOfTeamTotal+"%\nClaims: \n"+myTotalClaims);
	}
}