using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
	public GameObject player; // this client's "First Person Controller(Clone)"
	
	private bool ready = false;
	
	public Color red;
	public Color blue;
	public Color gray;
	
	public int redCount;
	public int blueCount;
	
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
	public List<NetworkPlayer> netPlayers;
	
	private GameObject winSound;
	private GameObject ground;
	
	public State state;
	public enum State
    {
		betweenGames,
		inGame
    };
	
	public LevelType levelType;
	public enum LevelType
	{
		space,
		sky
	};
	
	public float gameLength = 0;
	[RPC] public void SetGameLength(float length) { gameLength = length; }
	
	public float gameEndTime = 0;
	[RPC] private void SetEndTime(float endTime, int newState, int levelState)
	{
		gameEndTime = endTime;

		state = (State)newState;
		levelType = (LevelType)levelState;
		
		Instantiate(winSound, this.transform.position, Quaternion.identity);
		
		if (state == State.inGame) // starting a new round
		{
			ClearData(false);
			
			if (levelType == LevelType.space)
			{
				LoadSpace();
			} 
			if (levelType == LevelType.sky)
			{
				LoadSky();
			}
		}
	}
	
	private void LoadSpace()
	{
		Material mSky = Resources.Load("Skyboxes/DeepSpaceBlueWithPlanet/DSBWP") as Material;
		RenderSettings.skybox = mSky;
		
		if (ground != null) // disable it
		{
			ground.SetActive(false);
		}
	}
	
	private void LoadSky()
	{
		Material mSky = Resources.Load("Skyboxes/Sunny2/Sunny2 Skybox") as Material;
		RenderSettings.skybox = mSky;
		
		if (ground != null)
		{
			ground.SetActive(true);
		}
		if (player != null && player.transform.position.y < -0.5f) // keep player above ground
		{
			player.transform.position= new Vector3(player.transform.position.x, 3f, player.transform.position.z);
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
					networkView.RPC("SetEndTime", RPCMode.AllBuffered, (float)Network.time + 8, (int)state, (int)levelType);
				}
			}
			else if (state == State.betweenGames && Network.time > gameEndTime)
			{
				state = State.inGame;
				
				//change level type before new game
				if (levelType == LevelType.sky)
				{
					levelType = LevelType.space;
				}
				else
				{
					levelType = LevelType.sky;
				}
				networkView.RPC("SetEndTime", RPCMode.AllBuffered, (float)Network.time + gameLength, (int)state, (int)levelType);
				
				foreach (GameObject cube in GetComponent<UpgradeManager>().cubes)
				{
					if (cube != null && (cube.renderer.material.color != gray || cube.renderer.material.GetTexture("_DecalTex") != null))
					{
						cube.networkView.RPC ("SetGray", RPCMode.AllBuffered);
					}
				}
			}
		}
	}
	
	private void OnEnable()
	{
		ground = GameObject.Find("Ground(Clone)");
		
		if (Network.isServer)
		{
			networkView.RPC("SetEndTime", RPCMode.AllBuffered, (float)Network.time + gameLength, (int)State.inGame, (int)LevelType.sky);
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
	
	public Vector3 GetTeam(GameObject newPlayer)
	{
		player = newPlayer;
		players.Add(player);
		netPlayers.Add(player.networkView.owner);
		
		ready = true;
		
		if (redCount < blueCount)
		{
			networkView.RPC("JoinRed", RPCMode.AllBuffered);
			return new Vector3(red.r, red.g, red.b);
		}
		else // blueCount <= redCount
		{
			networkView.RPC("JoinBlue", RPCMode.AllBuffered);
			return new Vector3(blue.r, blue.g, blue.b);
		}
	}
	
	[RPC] private void JoinRed() { redCount++; }
	[RPC] private void JoinBlue() { blueCount++; }
	
	[RPC] public void LeaveRed(NetworkPlayer netPlayer)
	{
		RemovePlayer(netPlayer);
		redCount--;
	}
	[RPC] public void LeaveBlue(NetworkPlayer netPlayer)
	{
		RemovePlayer(netPlayer);
		blueCount--;
	}
	private void RemovePlayer(NetworkPlayer netPlayer)
	{
		foreach (GameObject player in players)
		{
			if (player.networkView.owner == netPlayer)
			{
				players.Remove(player);
				netPlayers.Remove(netPlayer);
			}
		}
	}
	
	[RPC] public void ClearData(bool all)
	{
		redScore = 0;
		blueScore = 0;
	  	redPercent = 0;
	  	bluePercent = 0;
		
		if (all)
		{
			redCount = 0;
			blueCount = 0;
	  		totalCubes = 0;
			ready = false;
			gameLength = 0;
			gameEndTime = 0;
			mapCenter = Vector3.zero;
			players = new List<GameObject>(20);
			netPlayers = new List<NetworkPlayer>(20);
			enabled = false;
		}
	}
}