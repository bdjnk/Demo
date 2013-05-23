using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
	public NetworkPlayer networkPlayer;
	public GameObject player; // this client's "First Person Controller(Clone)"
	
	private bool ready = false;
	
	public Color red;
	public Color blue;
	public Color gray;
	
	private int redCount;
	private int blueCount;
	
	public int redScore = 0;
	public int blueScore = 0;
	
	public int redPercent = 0;
	public int bluePercent = 0;
	
	public int totalCubes = 0;
	[RPC] public void SetCubeCount(int cubeCount) { totalCubes = cubeCount; }
	
	public float gameLength;
	[RPC] public void SetTimer(float timer) { gameLength = timer; }
	
	public Vector3 mapCenter;
	[RPC] public void SetMapCenter(Vector3 center) { mapCenter = center; }
	
	public List<GameObject> players;
	
	private void Awake()
	{
		red = new Color(1, 0.4f, 0.4f);
		blue = new Color(0.4f, 0.6f, 1);
		gray = new Color(0.8f, 0.8f, 0.8f);
		
		players = new List<GameObject>(20);
	}
	
	public Vector3 GetTeam(GameObject player)
	{
		this.player = player;
		ready = true;
		
		if (redCount < blueCount)
		{
			networkView.RPC("joinRed", RPCMode.AllBuffered);
			return new Vector3(red.r, red.g, red.b);
		}
		else // blueCount <= redCount
		{
			networkView.RPC("joinBlue", RPCMode.AllBuffered);
			return new Vector3(blue.r, blue.g, blue.b);
		}
	}
	
	[RPC] private void joinRed() { redCount++; }
	[RPC] private void joinBlue() { blueCount++; }
	
	public void LeaveGame() // should only ever happen on the server
	{
		players.Remove(player);
		Network.RemoveRPCs(player.networkView.viewID);
		
		Color color = player.GetComponentInChildren<MeshRenderer>().material.color;
		if (color == red)
		{
			networkView.RPC("leaveRed", RPCMode.AllBuffered);
		}
		else // color == blue
		{
			networkView.RPC("leaveBlue", RPCMode.AllBuffered);
		}
	}
	
	[RPC] private void leaveRed() { redCount--; }
	[RPC] private void leaveBlue() { blueCount--; }
	
	public int RedCount { get { return redCount; } }
	public int BlueCount { get { return blueCount; } }
	
	[RPC] public void ClearData(bool exiting)
	{
		if (exiting)
		{
			redCount = 0;
			blueCount = 0;
	  		totalCubes = 0;
			ready = false;
		}
		redScore = 0;
		blueScore = 0;
	  	redPercent = 0;
	  	bluePercent = 0;
	}
	
	private void Update()
	{
		if (!ready) { return; }
		
		redPercent = (int)(100.0f * redScore/totalCubes);
		bluePercent = (int)(100.0f * blueScore/totalCubes);
	}
}