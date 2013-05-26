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
	
	public int percentToWin = 75;
	
	public int totalCubes = 0;
	[RPC] public void SetCubeCount(int cubeCount) { totalCubes = cubeCount; }
	
	public Vector3 mapCenter;
	[RPC] public void SetMapCenter(Vector3 center) { mapCenter = center; }
	
	public List<GameObject> players;
	
	private GameObject winSound;
	
	public State state;
	public enum State
    {
		betweenGames,
		inGame
    };
	
	public float gameLength = 0;
	[RPC] public void SetGameLength(float length) { gameLength = length; }
	
	public float gameEndTime = 0;
	[RPC] private void SetEndTime(float endTime, int newState)
	{
		gameEndTime = endTime;

		state = (State)newState;
		Instantiate(winSound, this.transform.position, Quaternion.identity);
		
		if (state == State.inGame) // starting a new round
		{
			ClearData(false);
		}
	}
	
	private void Update()
	{
		if (!ready) { return; }
		
		redPercent = (int)(100.0f * redScore/totalCubes);
		bluePercent = (int)(100.0f * blueScore/totalCubes);
		
		if (Network.isServer)
		{
			//if (gameLength == 0) { gameEndTime = (float)Network.time + 8f; } // don't end because of the timer
			
			if (state == State.inGame)
			{
				if ((redPercent >= percentToWin || bluePercent >= percentToWin) || (gameLength!=0 && gameEndTime > 0 && Network.time > gameEndTime))
				{
					state = State.betweenGames;
					//gameEndTime = (float)Network.time + 8;
					networkView.RPC("SetEndTime", RPCMode.AllBuffered, (float)Network.time + 8, (int)state);
				}
			}
			else if (state == State.betweenGames && Network.time > gameEndTime)
			{
				state = State.inGame;
				networkView.RPC("SetEndTime", RPCMode.AllBuffered, (float)Network.time + gameLength, (int)state);
				
				foreach (GameObject cube in GetComponent<UpgradeManager>().cubes)
				{
					if (cube != null && cube.renderer.material.color != gray)
					{
						cube.networkView.RPC ("SetGray", RPCMode.AllBuffered);
					}
				}
			}
		}
	}
	
	private void Start()
	{
		if (Network.isServer)
		{
			networkView.RPC("SetEndTime", RPCMode.AllBuffered, (float)Network.time + gameLength, (int)State.inGame);
		}
	}
	
	private void Awake()
	{
		red = new Color(1, 0.4f, 0.4f);
		blue = new Color(0.4f, 0.6f, 1);
		gray = new Color(0.8f, 0.8f, 0.8f);
		
		ClearData(true);
		
		winSound = Resources.Load ("Prefabs/Winner") as GameObject;
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
	
	[RPC] public void leaveRed() { redCount--; }
	[RPC] public void leaveBlue() { blueCount--; }
	
	public int RedCount { get { return redCount; } }
	public int BlueCount { get { return blueCount; } }
	
	[RPC] public void ClearData(bool all)
	{
		if (all)
		{
			redCount = 0;
			blueCount = 0;
	  		totalCubes = 0;
			ready = false;
			gameLength = 0;
			mapCenter = Vector3.zero;
			players = new List<GameObject>(20);
		}
		redScore = 0;
		blueScore = 0;
	  	redPercent = 0;
	  	bluePercent = 0;
	}
}