using UnityEngine;
using System.Collections;

public class WinnerGUI : MonoBehaviour
{
	private GameData gameData;
	
	private double persist = 8;
	private double timeToStart;
	private double countTime;
	
	public int redCount = 0;
	public int blueCount = 0;
	
	public int redScore = 0;
	public int blueScore = 0;
	
	public int totalCubes = 0;
	public int redPercent = 0;
	public int bluePercent = 0;
	
	private int counter = 8;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
		redCount = gameData.RedCount;
		blueCount = gameData.BlueCount;
		
		redScore = gameData.redScore;
		blueScore = gameData.blueScore;
		
		totalCubes = gameData.totalCubes;
		redPercent = gameData.redPercent;
		bluePercent = gameData.bluePercent;
		
	}
	void Start()
	{
		timeToStart = Network.time;
		countTime = timeToStart;		
	}
	
	private void Update()
	{
		if(Network.time > timeToStart + persist){
			if(Network.isServer){
				Network.RemoveRPCs(this.networkView.viewID);
				Network.Destroy (this.networkView.viewID);
				//Network.Destroy (this);
			}
		}
		if(Network.time > countTime + 1){
			countTime = Network.time;
			counter--;
		}
	}
	
	private void OnGUI()
	{

		float buttonW = Screen.width*0.24f;
		float buttonH = Screen.width*0.40f;
		
		if (gameData.redScore > gameData.blueScore) // display the appropriate list
		{
			GUI.Box(new Rect(Screen.width/2-buttonW/2, Screen.height/2 - buttonH/2, buttonW, buttonH),
				"Red Team Wins!!!\n"+redCount+" players\n"+redScore+" cubes\n"+redPercent+"%"
				+ "\n\n\nBlue Team \n"+blueCount+" players\n"+blueScore+" cubes\n"+bluePercent+"%"
				+ "\n\n\nRestart in: \n"+counter);
		}
		else 
		{
			GUI.Box(new Rect(Screen.width/2-buttonW/2, Screen.height/2 - buttonH/2, buttonW, buttonH),
				"Blue Team Wins!!!\n"+blueCount+" players\n"+blueScore+" cubes\n"+bluePercent+"%"
				+"\n\n\nRed Team \n"+redCount+" players\n"+redScore+" cubes\n"+redPercent+"%"
				+ "\n\n\nRestart in: \n"+counter);
		}

	}
}