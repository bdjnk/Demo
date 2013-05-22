using UnityEngine;
using System.Collections;

public class WinnerDisplay : MonoBehaviour
{
	private GameData gameData;
	
	private double persist = 8;
	private double timeToStart;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
	}
	void Start()
	{
		timeToStart = Network.time;
	}
	
	private void Update()
	{/*
		if(Network.time > timeToStart + persist){
			if(Network.isServer){
				Network.Destroy (this.gameObject);
			}
		}*/
	}
	
	private void OnGUI()
	{

		float buttonW = Screen.width*0.24f;
		float buttonH = Screen.width*0.40f;
		
		if (gameData.blueScore > gameData.redScore) // display the lists
		{
			GUI.Box(new Rect(Screen.width/2-buttonW/2, Screen.height/2 - buttonH/2, buttonW, buttonH),
				"Red Team Wins!!!\n"+gameData.RedCount+" players\n"+gameData.redScore+" cubes\n"+gameData.redPercent+"%");
		}
		else 
		{
			GUI.Box(new Rect(Screen.width/2-buttonW/2, Screen.height/2 - buttonH/2, buttonW, buttonH),
				"Blue Team Wins!!!\n"+gameData.BlueCount+" players\n"+gameData.blueScore+" cubes\n"+gameData.bluePercent+"%");
		}

	}
}