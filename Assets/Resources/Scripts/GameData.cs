using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour
{
	public NetworkPlayer netPlayer;
	public GameObject player; // this client's "First Person Controller(Clone)"
	
	private int redCount;
	private int blueCount;
	
	private bool ready = false;
	
	public Texture GetTeam(GameObject player)
	{
		this.player = player;
		totalCubes = GetComponent<PG_Map>().cubeCount; // stupid place to do it, find better one
		ready = true;
		
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
	
	[RPC] private void joinRed() { redCount++; }
	[RPC] private void joinBlue() { blueCount++; }
	
	public void LeaveTeam()
	{
		Network.RemoveRPCs(player.networkView.viewID);
		
		string color = player.GetComponentInChildren<MeshRenderer>().material.mainTexture.name;
		if (color == "Red")
		{
			networkView.RPC("leaveRed", RPCMode.AllBuffered);
		}
		else // color == "Blue"
		{
			networkView.RPC("leaveBlue", RPCMode.AllBuffered);
		}
	}
	
	[RPC] private void leaveRed() { redCount--; }
	[RPC] private void leaveBlue() { blueCount--; }
	
	public int RedCount { get { return redCount; } }
	public int BlueCount { get { return blueCount; } }
	
	public int redScore = 0;
	public int blueScore = 0;
	
	public int totalCubes = 0;
	public int redPercent = 0;
	public int bluePercent = 0;
	
	public void ClearData()
	{
		redCount = 0;
		blueCount = 0;
		redScore = 0;
		blueScore = 0;
	  	totalCubes = 0;
	  	redPercent = 0;
	  	bluePercent = 0;
		ready = false;
	}
	
	private void Update()
	{
		if (!ready) { return; }
		
		redPercent = (int)(100.0f * redScore/totalCubes);
		bluePercent = (int)(100.0f * blueScore/totalCubes);
	}
}
