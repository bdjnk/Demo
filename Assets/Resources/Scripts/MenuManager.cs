using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Vector2 scrollPosition = Vector2.zero;
	
	public GUISkin skin;
	
	public int serverCount = 40;
	public int serverHeight = 25;
	private float edging = 0.01f;
	private float edge;
	
	private string playerName;
	
	private int innerWidth;
	private int scrollBarWidth = 17;
	private bool creating = false;
	
	private GUIStyle style;
	
	private bool notPlaying = true;
	//server and host list variables
	private bool refreshing = false;
	private HostData[] hostData = null;
	private string gameName = "123Paint the Town123"; 
	public string defaultServerInstanceName = "Paint The Town";
	public string serverInstanceName = "";
	
	//player data
	private string playerColor = "red";
	
	Hashtable hash = new Hashtable();
	
	void Awake()
	{
		if (!PlayerPrefs.HasKey("name"))
		{
			PlayerPrefs.SetString("name", "Default");
		}
		playerName = PlayerPrefs.GetString("name");
		creating = false;
		
		style = new GUIStyle();
		style.normal.textColor = Color.white;
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		edge = edging * Screen.width;
		
		if (creating)
		{
			CreateServer();
		}
		else if (notPlaying)
		{
			ListServers();
		}
	}
	
	public int[] citySize = {3, 3, 3};
    public int[] minBuildingSize = {1, 1, 1};
    public int[] maxBuildingSize = {3, 3, 3};
	public int maxPlayers = 10;
	public bool bots = true;
	public bool upgrades = true;
	public bool list = true;
	
	public int sliderHeight = 22;
	
	int IntSlider(Rect rect, int val, int min, int max, string label, int step, bool dynamic)
	{
		Vector2 size = skin.GetStyle("Label").CalcSize(new GUIContent(max.ToString()));
		style.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(rect.x, rect.y, size.x, rect.height), val.ToString(), style);
		
		float width = dynamic ? (max-min+step)*10/step : rect.width;
		GUI.Label(new Rect(rect.x+(width)+(size.x+5)+5, rect.y, rect.width-90-(size.x+5)-5, rect.height), label);
		
		float f = GUI.HorizontalSlider(new Rect(rect.x+(size.x+5), rect.y+(rect.height-12)/2, width, 12), val, min, max);
		val = Mathf.RoundToInt(f / (float)step) * step;
		return val;
	}
	
	void CreateServer() // CREATE A NEW SERVER
	{
		GUI.BeginGroup(new Rect(Screen.width/2-200, edge, 400, sliderHeight*11+edge*3), skin.box);
		
		GUI.Label(new Rect(edge, sliderHeight*0+edge, 200, sliderHeight), "City Dimensions");
		citySize[0] = IntSlider(new Rect(edge, sliderHeight*1+edge, 200, sliderHeight), citySize[0], 1, 9, "Width", 1, true);
		citySize[1] = IntSlider(new Rect(edge, sliderHeight*2+edge, 200, sliderHeight), citySize[1], 1, 9, "Height", 1, true);
		citySize[2] = IntSlider(new Rect(edge, sliderHeight*3+edge, 200, sliderHeight), citySize[2], 1, 9, "Depth", 1, true);
		
		maxPlayers = IntSlider(new Rect(edge+200, sliderHeight*0+edge, 200, sliderHeight), maxPlayers, 2, 14, "Max Players", 2, true);
		
		bots = GUI.Toggle(new Rect(edge+200, sliderHeight*1+edge, 200, sliderHeight), bots, " Bots");
		upgrades = GUI.Toggle(new Rect(edge+200, sliderHeight*2+edge, 200, sliderHeight), upgrades, " Upgrades");
		list = GUI.Toggle(new Rect(edge+200, sliderHeight*3+edge, 200, sliderHeight), list, " Make Public");
		
		GUI.Label(new Rect(edge, sliderHeight*5+edge, 300, sliderHeight), "Minimum Building Dimensions");
		minBuildingSize[0] = IntSlider(new Rect(edge, sliderHeight*6+edge, 200, sliderHeight), minBuildingSize[0], 1, 9, "Width", 1, true);
		minBuildingSize[1] = IntSlider(new Rect(edge, sliderHeight*7+edge, 200, sliderHeight), minBuildingSize[1], 1, 9, "Height", 1, true);
		minBuildingSize[2] = IntSlider(new Rect(edge, sliderHeight*8+edge, 200, sliderHeight), minBuildingSize[2], 1, 9, "Depth", 1, true);
		
		GUI.Label(new Rect(edge+200, sliderHeight*5+edge, 200, sliderHeight), "Maximum Building Dimensions");
		maxBuildingSize[0] = Mathf.Max(minBuildingSize[0],
			IntSlider(new Rect(edge+200, sliderHeight*6+edge, 200, sliderHeight), maxBuildingSize[0], 1, 9, "Width", 1, true));
		maxBuildingSize[1] = Mathf.Max(minBuildingSize[1],
			IntSlider(new Rect(edge+200, sliderHeight*7+edge, 200, sliderHeight), maxBuildingSize[1], 1, 9, "Height", 1, true));
		maxBuildingSize[2] = Mathf.Max(minBuildingSize[2],
			IntSlider(new Rect(edge+200, sliderHeight*8+edge, 200, sliderHeight), maxBuildingSize[2], 1, 9, "Depth", 1, true));
		
		if(GUI.Button(new Rect(100, sliderHeight*10+edge, 100, sliderHeight), "Create")){
			StartServer();
			creating = false;
			refreshing = true;
		}
		if(GUI.Button(new Rect(200, sliderHeight*10+edge, 100, sliderHeight), "Cancel")){
			creating = false;
		}
		
		GUI.EndGroup();
	}
	
	void ListServers() // LIST OF SERVERS, THIS IS THE INITIAL MENU PAGE
	{
		innerWidth = Screen.width-scrollBarWidth;
		
		GUI.BeginGroup(new Rect(edge, 5, 400, 25));
		
		if(GUI.Button(new Rect(0, 0, 65, 25), "Refresh")){
			MasterServer.RequestHostList(gameName);
			refreshing = true;
		}
		GUI.Label(new Rect(70, 0, 200, 25), "the server list to join a game, or");
		creating = GUI.Button(new Rect(260, 0, 80, 25), "create one.");
		
		GUI.EndGroup();
		
		GUI.Label(new Rect((Screen.width-edge)-190, 5, 40, 25), "Name:");
		playerName = GUI.TextField(new Rect((Screen.width-edge)-150, 5, 150, 25), playerName, 16);
		PlayerPrefs.SetString("name", playerName); // may be inefficient...
		
		GUI.BeginGroup(new Rect(edge, 35, innerWidth, 25));
			
		GUI.Label(new Rect(edge, 0, innerWidth-300, 25), "Name");
		GUI.Label(new Rect(innerWidth-300, 0, 100, 25), "Size");
		GUI.Label(new Rect(innerWidth-200, 0, 100, 25), "Players");
		GUI.Label(new Rect(innerWidth-100, 0, 100, 25), "Ping");
		
		GUI.EndGroup();
		if (hostData != null && hostData.Length!=null){
			serverCount = hostData.Length;
		} else {
			serverCount = 0;
		}
		
		if (serverCount == 0)
		{
			GUI.Label(new Rect(edge, 60, 250, serverHeight), "No servers available, try refreshing.");
		}
		else
		{
			// this scrollbox's outer height is limited by the screen height in increments of serverHeight
			scrollPosition = GUI.BeginScrollView(
				new Rect(edge, 60, (Screen.width-edge*2),
					Mathf.Min(serverHeight*serverCount, Mathf.Round((Screen.height-60)/serverHeight)*serverHeight)),
				scrollPosition, new Rect(0, 0, innerWidth*0.98f, serverHeight*serverCount), false, true);
			
			for (int i = 0; i < serverCount; i++) // for each server we know about
			{
				GUI.BeginGroup(new Rect(0, serverHeight*i, innerWidth*0.98f, serverHeight), new GUIContent());
				
				if(GUI.Button(new Rect(0, 0, innerWidth-edge*2, serverHeight), "")){
					Network.Connect (hostData[i]);
				}
				
				GUI.Label(new Rect(edge, 0, innerWidth-300, serverHeight), hostData[i].gameName);
				
				GUI.Label(new Rect(innerWidth-300, 0, 100, serverHeight), "size");
				GUI.Label(new Rect(innerWidth-200, 0, 100, serverHeight), hostData[i].connectedPlayers.ToString());
				GUI.Label(new Rect(innerWidth-100, 0, 100, serverHeight), "Ping");
				
				GUI.EndGroup();
			}
			
	        GUI.EndScrollView();
		}
	}
	
	void Update () {
	
		if (refreshing)
		{
			if (MasterServer.PollHostList().Length > 0)
			{
				refreshing = false;
				//Debug.Log (MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
			}
		}
	}
	
	void StartServer()
	{
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
		if (serverInstanceName != "")
		{
			defaultServerInstanceName = serverInstanceName;
		}
		else
		{
			string randomGameNum = " " + Random.Range (1,1000).ToString();
			defaultServerInstanceName += randomGameNum;
		}
		MasterServer.RegisterHost(gameName, defaultServerInstanceName, "CSS385 Game");
		
		//Lists the IP address for MasterServer
		//Debug.Log("Master Server Info:" + MasterServer.ipAddress +":"+ MasterServer.port);
	}
	
	void OnServerInitialized()
	{
		Debug.Log ("Server Initialized");
		GameManagerScript mainGameScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		//GameManagerScript mainGameScript = GetComponent<GameManagerScript>();
		mainGameScript.resetAllData();
		mainGameScript.InitializeWorld(citySize,minBuildingSize,maxBuildingSize);
		while (mainGameScript.requestingUpdatePermission())
		{}
		mainGameScript.createPlayer(playerName,playerColor,Network.player.guid);
		notPlaying = false;
	}
	
	void OnDisconnectedFromServer()
	{
		//TODO: reset local menu data
	}
	
	void OnConnectedToServer()
	{
		Debug.Log ("Connected to Server");
		GameManagerScript mainGame = GameObject.Find ("GameManager").GetComponent<GameManagerScript>();
		//GameManagerScript mainGameScript = GetComponent<GameManagerScript>();
		//if(Network.isClient){
		//mainGame.InitializeWorld(citySize,minBuildingSize,maxBuildingSize);
		notPlaying = false;
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		//Debug.Log("Setup for player " + player + " " + player.guid + " " + playerNumber);
		
	}
	
}
