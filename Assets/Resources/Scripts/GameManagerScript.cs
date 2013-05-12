using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour
{
	//world data	
	private int totalCubesInThisWorld = 54;
	
	//global team data 
	private int redTeamTotalPlayers;
	private int blueTeamTotalPlayers;
	private int redTeamTotalScore;
	private int blueTeamTotalScore;
	
	private string[] blueTeamPlayers;
	private string[] redTeamPlayers;
	
	private bool locked = false;
	
	//default spawn location
	public float spawnX = 8f;
	public float spawnY = 1f;
	public float spawnZ = -20f;
	
	private PG_Map mainMap = null;
	
	
	//private playerInfo[] allPlayers;//need to initialize!!!
	private int numberOfCurrentPlayers = 0;
	private int maxPlayers = 8;
	
	//data stores for player info
	private GameObject[] playerData;
	private GameObject mainGame;
	private Object playerPrefab;
	private PlayerDataScript mainPlayerScript;
	//private GameObject myPlayerData;
	
	void Start()
	{
		//playerData = null;
		mainGame = GameObject.Find("GameManager");
		mainPlayerScript = null;
		playerPrefab = Resources.Load("Prefabs/First Person Controller");
		Debug.Log ("prefab loaded: " + playerPrefab.ToString());
		//myPlayerData = null;
		resetAllData();
		mainMap = new PG_Map();
		//InitializeWorld();
	}
	
	public void resetAllData()
	{
		redTeamTotalPlayers = 0;
		blueTeamTotalPlayers = 0;
		blueTeamPlayers = new string[maxPlayers];
		redTeamPlayers = new string[maxPlayers];
		//allPlayers = new playerInfo[maxPlayers];
		redTeamTotalScore = 0;
		blueTeamTotalScore = 0;
		numberOfCurrentPlayers = 0;
		playerData = new GameObject[maxPlayers];//initialize on all gamemanagers
		mainPlayerScript = null;
	}
	
	public void InitializeWorld(int[] citySize,int[] minBldgSize,int[] maxBldgSize)
	{
		mainMap.citySize = citySize;
		mainMap.minBuildingSize = minBldgSize;
		mainMap.maxBuildingSize = maxBldgSize;
		mainMap.createMap();
	}
	
	void Update(){
		//cleanup shots fired (group 10) every 31 seconds or so
		if ((int) Time.realtimeSinceStartup % 31 ==0)
		{
			Network.RemoveRPCsInGroup(10);
		}
	}
	
	//client should call - while (requestingUpdatePermission())
	//prior to update
	[RPC]
	public bool requestingUpdatePermission()
	{
		if (locked!=true)
		{
			locked=true;
			return true;
		}
		else
		{
			return false;
		}
	}
	
	//create player happens on client only but because it is network.instantiate all will update
	public void createPlayer(string playerName,string teamColor, string playerID){
		//lock any other client changes while creating a player
		if (locked)
		{
			/*if (playerData == null)
			{
				playerData = new GameObject[maxPlayers];
				//myPlayerData = new GameObject();
			}
			//array starts at 0
			if (playerData[numberOfCurrentPlayers] == null)
			{*/
			GameObject mainPlayer = (GameObject) Network.Instantiate(playerPrefab, new Vector3(spawnX,spawnY,spawnZ),Quaternion.identity, 0);
				//playerData[numberOfCurrentPlayers] = (GameObject) Network.Instantiate(playerPrefab, new Vector3(spawnX,spawnY,spawnZ),Quaternion.identity, 0);
				//playerData[numberOfCurrentPlayers] = ((GameObject)Network.Instantiate(playerPrefab, new Vector3(spawnX,spawnY,spawnZ),Quaternion.identity, 0)).GetComponent<PlayerDataScript>();
			//}
			//mainPlayerScript = playerData[numberOfCurrentPlayers].GetComponentInChildren<PlayerDataScript>();
			mainPlayerScript = mainPlayer.GetComponentInChildren<PlayerDataScript>();
			
			mainPlayerScript.updateInitialSettings(true);
			mainPlayerScript.updateMouseLook(true);
			//playerData[numberOfCurrentPlayers].updateMouseLook(true);
			//Debug.Log ("main:" + mainPlayerScript.ToString());
						
			this.networkView.RPC("updateTeamData",RPCMode.AllBuffered,teamColor,playerName,playerID,numberOfCurrentPlayers,mainPlayerScript.name);
			mainPlayerScript.networkView.RPC("setNewPlayerData",RPCMode.AllBuffered,teamColor,playerName,playerID);
			//Debug.Log ("array:" + playerData[numberOfCurrentPlayers].ToString());
			//numberOfCurrentPlayers++;
			locked = false;
		}
	}
	
	public int getTotalCubes()
	{
		return totalCubesInThisWorld;
	}
	
	//player destruct from network
	[RPC]
	public void removePlayer(string playerID)
	{
		//get name from ID
		string myName=null;
		string teamColor=null;
		for (int i = 0; i < playerData.Length; i++)
		{
		
			//Debug.Log ("Looking for: " + playerID);
			//Debug.Log ("position i: " + i + " =" + allPlayers[i].playerGuid);
			if (playerData != null && playerData[i] != null)
			{
				mainPlayerScript = playerData[i].GetComponentInChildren<PlayerDataScript>();
				if (mainPlayerScript.networkID == playerID)
				{
					//Debug.Log ("found at: " + i);
					myName = mainPlayerScript.nameOfPlayer;
					teamColor = mainPlayerScript.color;
				}
			}
		}
		
		Debug.Log ("player to remove: " + myName + ", color: " + teamColor);
		
		if (myName != null)
		{
			//move last item to index of name, remove last item, decrement
			if (teamColor == "blue")
			{
				int index = System.Array.IndexOf(blueTeamPlayers,myName);
				blueTeamPlayers[index] = blueTeamPlayers[blueTeamTotalPlayers-1];
				//Debug.Log ("revising index: " + index + " from " + blueTeamPlayers[index] + " to " + blueTeamPlayers[blueTeamTotalPlayers-1]);
				blueTeamPlayers[blueTeamTotalPlayers-1] = null;
				blueTeamTotalPlayers--;
				//Debug.Log ("revised blue team: " + getBlueTeamString());
			}
			else //red team
			{
				int index = System.Array.IndexOf(redTeamPlayers,myName);
				redTeamPlayers[index] = redTeamPlayers[redTeamTotalPlayers-1];
				//Debug.Log ("revising index: " + index + " from " + redTeamPlayers[index] + " to " + redTeamPlayers[redTeamTotalPlayers-1]);
				redTeamPlayers[redTeamTotalPlayers-1]=null;
				redTeamTotalPlayers--;
				//Debug.Log ("revised red team: " + getRedTeamString());
			}
			int indexLocation = 0;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData != null && playerData[i] != null)
				{
					mainPlayerScript = playerData[i].GetComponentInChildren<PlayerDataScript>();
					if(mainPlayerScript.nameOfPlayer == myName && mainPlayerScript.color == teamColor)
					{
						indexLocation = i;
					}
				}
			}
			playerData[indexLocation] = playerData[numberOfCurrentPlayers-1];
			//allPlayers[numberOfPlayers-1] = new playerInfo();
			playerData[numberOfCurrentPlayers-1] = null;
			//Debug.Log ("revised all players: " + allPlayers.ToString());
			numberOfCurrentPlayers--;
		}
	}
	
	//team data
	[RPC]
	public void updateTeamData(string teamColor, string name, string playerID,int playerNumber,string fullName)
	{
		if (teamColor == "blue")
		{
			blueTeamPlayers[blueTeamTotalPlayers] = name;
			blueTeamTotalPlayers++;
		}
		else //red team
		{
			redTeamPlayers[redTeamTotalPlayers] = name;
			redTeamTotalPlayers++;
		}
		playerData[playerNumber] = GameObject.Find (fullName);
		
		numberOfCurrentPlayers++;
	}
	
	public string getRedTeamString()
	{
		string temp = "";
		for (int i = 0; i < redTeamPlayers.Length; i++)
		{
			temp += redTeamPlayers[i] + "\n";
		}
		return temp;
	}
	
	public string getBlueTeamString()
	{
		string temp = "";
		for (int i = 0; i < blueTeamPlayers.Length; i++){
			temp += blueTeamPlayers[i] + "\n";
		}
		return temp;
	}
	
	public int getRedTeamCount() { return redTeamTotalPlayers; }
	public int getBlueTeamCount() { return blueTeamTotalPlayers; }
	public int getRedTeamScore() { return redTeamTotalScore; }
	public int getBlueTeamScore() { return blueTeamTotalScore; }
		
	//scoring updates
	public int getMyTotalClaims(string myID)
	{
		for (int i = 0; i < maxPlayers; i++)
			
		{
			if (playerData != null && playerData[i] != null)
			{
				mainPlayerScript = playerData[i].GetComponentInChildren<PlayerDataScript>();
				if (mainPlayerScript.networkID == myID)
				{
					return mainPlayerScript.totalClaims;
				}
			}
		}
		return 0;
	}
	
	public int getMyTotalOwned(string myID)
	{
		for (int i=0;i<maxPlayers;i++)
		{
			if (playerData != null && playerData[i] != null)
			{
				mainPlayerScript = playerData[i].GetComponentInChildren<PlayerDataScript>();
				if(mainPlayerScript.networkID == myID)
				{
					return mainPlayerScript.totalOwned;
				}
			}
		}
		return 0;
	}
	
	public int getMyPercentage(string myID)
	{
		for (int i=0;i<maxPlayers;i++)
		{
			if (playerData != null && playerData[i] != null)
			{
				mainPlayerScript = playerData[i].GetComponentInChildren<PlayerDataScript>();
				if (mainPlayerScript.networkID == myID)
				{
					return mainPlayerScript.myPercent;
				}
			}
		}
		return 0;
	}
	
	[RPC]
	public void blueScore(int hits)
	{
		blueTeamTotalScore += hits;
		if (blueTeamTotalScore < 0)
		{
			blueTeamTotalScore = 0;
		}
		//could also check max?
	}
	
	[RPC]
	public void redScore(int hits)
	{
		redTeamTotalScore += hits;
		if (redTeamTotalScore < 0)
		{
			redTeamTotalScore = 0;
		}
		//could also check max?
	}
	
	[RPC]
	public void updatePlayersScore(string newOwnerID,string prevOwnerID)
	{
		if (newOwnerID != "") //make sure we have an owner
		{
			//search list and update scores
			for (int i=0;i<maxPlayers;i++)
			{
				if (playerData != null && playerData[i] != null){
					mainPlayerScript = playerData[i].GetComponentInChildren<PlayerDataScript>();
					
					if (mainPlayerScript.networkID == newOwnerID)
					{
						mainPlayerScript.totalClaims++;
						mainPlayerScript.totalOwned++;
					}
					if (prevOwnerID != "")
					{
						if (mainPlayerScript.networkID == prevOwnerID)
						{
							mainPlayerScript.totalOwned--;
						}
					}
					if (mainPlayerScript.color == "red" && getRedTeamScore()!=0)
					{
						mainPlayerScript.myPercent = (int) 100.0f * mainPlayerScript.totalOwned / getRedTeamScore();
					}
					else if (mainPlayerScript.color =="blue" && getBlueTeamScore()!=0)
					{
						mainPlayerScript.myPercent = (int) 100.0f * mainPlayerScript.totalOwned / getBlueTeamScore();
					}
				}
			}
		}
	}
}
