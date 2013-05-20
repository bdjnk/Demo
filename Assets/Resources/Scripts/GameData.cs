using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour
{
	public NetworkPlayer netPlayer;
	
	private int redCount;
	private int blueCount;
	private bool cubesUpdated = false;
	
	public Texture GetTeam()
	{
		if (redCount < blueCount)
		{
			networkView.RPC("joinRed", RPCMode.AllBuffered);
			return Resources.Load("Textures/Red") as Texture;
		}
		else // blueCount <= redCount
		{
			networkView.RPC("joinBlue", RPCMode.AllBuffered);
			return Resources.Load("Textures/Blue") as Texture;
		}
	}
	
	void Update(){
		if((redCount > 0 || blueCount > 0) && !cubesUpdated){
			totalCubes = GetComponent<PG_Map>().cubeCount;
			cubesUpdated = true;
		}
		if(cubesUpdated && totalCubes > 0){
			redTeamPercent = (int) (100.0f * redScore/totalCubes);
			blueTeamPercent = (int) (100.0f * blueScore/totalCubes);
		}
	}
	
	public void ResetData(){
		redScore = 0;
	  	blueScore = 0;
	  	totalCubes = 0;
	  	redTeamPercent = 0;
	  	blueTeamPercent = 0;
		cubesUpdated = false;
	}
	
	[RPC] private void joinRed() { redCount++; }
	[RPC] private void joinBlue() { blueCount++; }
	
	public int RedCount { get { return redCount; } }
	public int BlueCount { get { return blueCount; } }
	
	public int redScore = 0;
	public int blueScore = 0;
	public int totalCubes = 0;
	public int redTeamPercent = 0;
	public int blueTeamPercent = 0;
}
