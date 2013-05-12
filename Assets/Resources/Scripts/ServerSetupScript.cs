using UnityEngine;
using System.Collections;


//[RequireComponent(typeof (NetworkView))]
public class ServerSetupScript : MonoBehaviour {
	
	private GameObject mainGame = null;
	private GameManagerScript mainGameScript = null;
	//private GameObject myGame = null;
	
	//public GameObject playerPrefab = null;  
	private GameObject mainPlayer = null;  
	private PlayerDataScript mainPlayerScript = null; 
	public float mSpeed = 5.0f;
	public int playerNumber = 0;
	
	private float buttonX = Screen.width*0.05f;
	private float buttonY = Screen.width*0.05f;
	private float buttonW = Screen.width*0.2f;
	private float buttonH = Screen.width*0.1f;
	
	//move to game manager
	public float spawnX = 8f;
	public float spawnY = 1f;
	public float spawnZ = -20f;
	
	private bool GUIgettingServerName = false;
	private bool GUIgettingPlayerData = false;
	private bool GUIgettingPlayerName = false;
	//private bool networkMode = false;//using network mode
	
	private bool refreshing = false;
	private HostData[] hostData = null;
	
	private string gameName = "123Paint the Town123"; 
	public string defaultServerInstanceName = "Paint The Town";
	public string serverInstanceName = "";
	
	private string myTeamInputColor = "red";
	private string myInputName = "";
	
	private bool invalidInput = false;
	
	// Use this for initialization
	void Start () {
		mainGame = GameObject.Find ("GameManager");
		mainGameScript = mainGame.GetComponent<GameManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(refreshing){
			if(MasterServer.PollHostList().Length > 0){
				refreshing = false;
				//Debug.Log (MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
			}
		}
	}
	
	void refreshHostList(){
		MasterServer.RequestHostList(gameName);
		refreshing = true;
	}
	
	void OnGUI() {
		//no server connection at start
		if(!Network.isClient && !Network.isServer && !GUIgettingServerName){
			if(mainPlayerScript!=null){
				mainPlayerScript.updateMouseLook(false);
			}
						
			if(GUI.Button(new Rect(buttonX,buttonY,buttonW,buttonH),"Start Server")){
				//Debug.Log ("Starting Server");
				GUIgettingServerName = true;
			}
			
			if(GUI.Button(new Rect(buttonX,buttonY + 1.2f * buttonW,buttonW,buttonH),"Refresh Hosts")){
				//Debug.Log ("Refreshing");
				refreshHostList();
			}
			if(hostData!=null){
				for (int i=0;i<hostData.Length;i++)
				{
					if(GUI.Button(new Rect(buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH*i),buttonW*3f,buttonH*0.5f),hostData[i].gameName)){
						Network.Connect(hostData[i]);
						//this wont really work to track largest value of # of players
						//(could get duplicates)
						//playerNumber = hostData[i].connectedPlayers + 1;
						
					}
				}
			}
		}
		
		//get server name section
		if(GUIgettingServerName){
			Screen.lockCursor = false;
			Screen.showCursor = true;
			if(!invalidInput){
				GUI.Box(new Rect (buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH),buttonW*3f,buttonH*0.5f),"Enter Your Unique Server Name");
			} else {
				GUI.Box(new Rect (buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH),buttonW*3f,buttonH*0.5f),"Server Name is not Unique, try again");
			}
	        serverInstanceName = GUI.TextField(new Rect(buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH*2f),buttonW*3f,buttonH*0.5f), serverInstanceName, 20);
			if(GUI.Button(new Rect(buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH*3f),buttonW*3f,buttonH*0.5f),"Submit")){
				bool clearedAllNames = true;
				if(hostData!=null){
					for (int i=0;i<hostData.Length;i++){//check for uniqueness here
						if(serverInstanceName==hostData[i].ToString()){
							clearedAllNames = false;
						}
					}
				}
				if(serverInstanceName!="" && clearedAllNames){
					GUIgettingServerName = false;
					invalidInput = false;
					StartServer();
				} else {
					invalidInput = true;
				}
			}
		}
			
		//get player team section
		if(GUIgettingPlayerData){
			Screen.lockCursor = false;
			Screen.showCursor = true;
			
			//test to change to just GameManagerScript.......
			//prep to show team data
			int redTeamTotalPlayers = mainGame.GetComponent<GameManagerScript>().getRedTeamCount();
			int blueTeamTotalPlayers = mainGame.GetComponent<GameManagerScript>().getBlueTeamCount();
			string redTeamPlayers = mainGame.GetComponent<GameManagerScript>().getRedTeamString();
			string blueTeamPlayers = mainGame.GetComponent<GameManagerScript>().getBlueTeamString();
			//Debug.Log ("red:" + redTeamTotalPlayers);
			//Debug.Log ("blue:" + blueTeamTotalPlayers);
			
			
			if(GUI.Button(new Rect(buttonX,buttonY,buttonW*1.5f,buttonH*1.5f),"Red Team (" + redTeamTotalPlayers + ")")){
				Debug.Log ("Red Team Selected");
				myTeamInputColor = "red";
				GUIgettingPlayerData = false;
				GUIgettingPlayerName = true;
			}
				
			GUI.Box(new Rect(buttonX+buttonW*2,buttonY,buttonW*1.5f,buttonH*1.5f),"Red Team Members: \n" + redTeamPlayers);
			
			if(GUI.Button(new Rect(buttonX,buttonY + 1.2f * buttonW,buttonW*1.5f,buttonH*1.5f),"Blue Team (" + blueTeamTotalPlayers + ")")){
				Debug.Log ("Blue Team Selected");
				myTeamInputColor = "blue";
				GUIgettingPlayerData = false;
				GUIgettingPlayerName = true;	
			}
				
			GUI.Box(new Rect(buttonX+buttonW*2,buttonY+1.2f * buttonW,buttonW*1.5f,buttonH*1.5f),"Blue Team Members: \n" + blueTeamPlayers);
			
		}
			
		//get player name section
		if (GUIgettingPlayerName){
			//may want to clean this up by moving scope outward
			string redTeamPlayers = mainGameScript.getRedTeamString();
			string blueTeamPlayers = mainGameScript.getBlueTeamString();
			
			Screen.lockCursor = false;
			Screen.showCursor = true;
			if(!invalidInput){
				GUI.Box(new Rect (buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH),buttonW*3f,buttonH*0.5f),"Enter Your Name Below");
			} else { 
				GUI.Box(new Rect (buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH),buttonW*3f,buttonH*0.5f),"Name must be unique, try again:");
			}
	        myInputName = GUI.TextField(new Rect(buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH*2f),buttonW*3f,buttonH*0.5f), myInputName, 15);
			if(GUI.Button(new Rect(buttonX*1.5f + buttonW,buttonY*1.2f+(buttonH*3f),buttonW*3f,buttonH*0.5f),"Submit")){
				if(myInputName!=""){//check for uniqueness here
					if (myTeamInputColor == "blue" && !blueTeamPlayers.Contains(myInputName)){
						invalidInput = false;
						SpawnPlayer();
						GUIgettingPlayerName = false;
					} else {
						invalidInput = true;
					}
					if (myTeamInputColor == "red" && !redTeamPlayers.Contains(myInputName)){
						invalidInput = false;
						SpawnPlayer();
						GUIgettingPlayerName = false;
					} else {
						invalidInput = true;
					}
				}
			}
		}
	}

	void StartServer(){
		//if we want password
		//Network.incomingPassword("test123");
				
		// Use NAT punchthrough if no public IP present 
		//Initialize this server (local)
		int portNum = Random.Range(25001,26100);//some random numbers for testing only
		Network.InitializeServer(8, portNum, !Network.HavePublicAddress());
		//Network.InitializeServer (32,23466, !Network.HavePublicAddress());
		
		//Use this if using our own local masterserver instead of Unity's
		//MasterServer.ipAddress = "127.0.0.1";
		//MasterServer.port = 23466;
		//MasterServer.dedicatedServer = true;
		
		//Register our game with Unitys Master Server
		if(serverInstanceName != ""){
			defaultServerInstanceName = serverInstanceName;
		} else {
			//Random.Range (1f,1000f).ToString();
			string randomGameNum = " " + Random.Range (1,1000).ToString();
			defaultServerInstanceName += randomGameNum;
		}
		MasterServer.RegisterHost(gameName, defaultServerInstanceName, "CSS385 Game");
		
		//Lists the IP address for MasterServer
		//Debug.Log("Master Server Info:" + MasterServer.ipAddress +":"+ MasterServer.port);
	}
	
	void OnServerInitialized(){
		//reset all Data
		//mainGame = GameObject.Find ("GameServer").GetComponent<GameServerScript>();
		//mainGame.enabled = true;
		mainGameScript.resetAllData();
		//Debug.Log ("Server initialized");
		GUIgettingPlayerData = true;		
	}
	
	void OnDisconnectedFromServer(){
		resetAllData();
	}
	
	void OnConnectedToServer(){
		//myGame = GameObject.Find ("GameManager").GetComponent<GameClientScript>();
		//Debug.Log ("Connected to Server");
		//mainGame = GameObject.Find ("GameManagerScript");
		GUIgettingPlayerData = true;
	}
	
	void OnPlayerConnected(NetworkPlayer player){
		Debug.Log("Setup for player " + player + " " + player.guid + " " + playerNumber);
		
	}
	
	IEnumerator OnPlayerDisconnected(NetworkPlayer player) {        
		Debug.Log("Clean up after player " + player + " " + player.guid + " " + playerNumber);        	
		mainGame.networkView.RPC ("removePlayer",RPCMode.AllBuffered,player.guid);
		
		yield return new WaitForSeconds(5.0F);
		//need to include this but with delay for clients to update
		Network.RemoveRPCsInGroup(10);//group 10 is shots - would be nice to make this unique per player
		//Network.RemoveRPCs(player);        
		Network.DestroyPlayerObjects(player);  
		Debug.Log ("removing: " + mainPlayer.ToString());
		//Network.Destroy (mainPlayer);		
	}
	
	
	
	void SpawnPlayer(){
		Debug.Log ("Spawn" + myInputName + "," + myTeamInputColor + "," + Network.player.guid);
		while(mainGameScript.requestingUpdatePermission()){}
		mainGameScript.createPlayer(myInputName,myTeamInputColor,Network.player.guid);
		
		//TODO: make a unique name - check if it already exists before saving it
		/*
		mainPlayer = (GameObject) Network.Instantiate(playerPrefab, new Vector3(spawnX,spawnY,spawnZ),Quaternion.identity, 0);
		
		mainPlayer.GetComponent<PlayerDataScript>().enabled = true;
		mainPlayer.GetComponentInChildren<Camera>().enabled = true;
		mainPlayer.GetComponent<FPSInputController>().enabled = true;
		mainPlayer.GetComponent<CharacterMotor>().enabled = true;
		mainPlayer.GetComponent<MouseLook>().enabled = true;
		mainPlayer.GetComponentInChildren<Camera>().GetComponent<MouseLook>().enabled = true;
		mainPlayer.GetComponentInChildren<Camera>().GetComponent<PG_Gun>().enabled = true;
		
		mainGame.networkView.RPC ("updateTeamData",RPCMode.AllBuffered,myTeamInputColor,myInputName,Network.player.guid);
		mainPlayer.networkView.RPC ("setNewPlayerData",RPCMode.AllBuffered,myTeamInputColor,myInputName,Network.player.guid);
	*/
	}
	
	void resetAllData(){
		mainGame = null;
		//playerPrefab = null;  
		mainPlayer = null;  
		mSpeed = 5.0f;
		playerNumber = 0;
		
		GUIgettingServerName = false;
		GUIgettingPlayerData = false;
		GUIgettingPlayerName = false;
		//networkMode = false;//using network mode
		
		refreshing = false;
		hostData = null;
		
		gameName = "123Paint the Town123"; //could make this public if we want
		defaultServerInstanceName = "Paint The Town";
		serverInstanceName = "";
		
		myTeamInputColor = "red";
		myInputName = "";
		
		invalidInput = false;
		
		Application.LoadLevel("Networking");
		
		
	}
	
	void OnMasterServerEvent(MasterServerEvent mse){
		if(mse == MasterServerEvent.RegistrationSucceeded){
			Debug.Log ("Registered Server");
		}

	
	}
}
