using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
	// Menu GUI Variables
	public GUISkin skin; // general style for all elements
	private GUIStyle style; // style for small corrections as needed
	
    private Vector2 scrollPosition = Vector2.zero;
	
	public int serverHeight = 25;
	private float edging = 0.01f;
	private float edge;
	
	private int innerWidth;
	private int scrollBarWidth = 17;
	
	// Server Info Variables
	private int[] citySize = {3, 3, 3};
    private int[] minBuildingSize = {1, 1, 1};
    private int[] maxBuildingSize = {3, 3, 3};
	private int maxPlayers = 10;
	private bool bots = true;
	private bool upgrades = true;
	private bool listed = true;
	private bool floor = true;
	private float spacing = 1.5f;
	
	// Server / Host Variables
	private string gameName = "123Paint the Town123";
	private HostData[] hostData = null;
	
	private string serverName;
	private string playerName;
	
	// State Variables
	private int firstRun = 0;
	
	private State state;
	private enum State
    {
		list,
		create,
		play
    };
	
	private void Awake()
	{
		firstRun = PlayerPrefs.GetInt("firstRun");
		if (firstRun == 0)
		{
			PlayerPrefs.SetInt("firstRun", 1);
		}
		else
		{
			// do stuff maybe...
		}
		playerName = PlayerPrefs.GetString("playerName", "Player Name");
		serverName = PlayerPrefs.GetString("serverName", "Server Name");
		
		maxPlayers = PlayerPrefs.GetInt("maxPlayers", 10);
		bots = System.Convert.ToBoolean(PlayerPrefs.GetInt("hasBots", 1));
		upgrades = System.Convert.ToBoolean(PlayerPrefs.GetInt("hasUpgrades", 1));
		listed = System.Convert.ToBoolean(PlayerPrefs.GetInt("isListed", 1));
		floor = System.Convert.ToBoolean(PlayerPrefs.GetInt("hasFloor", 1));
		spacing = PlayerPrefs.GetFloat("buildingSpacing", 1.5f);
		citySize[0] = PlayerPrefs.GetInt("citySizeX", 3);
		citySize[1] = PlayerPrefs.GetInt("citySizeY", 3);
		citySize[2] = PlayerPrefs.GetInt("citySizeZ", 3);
		minBuildingSize[0] = PlayerPrefs.GetInt("minBuildingSizeX", 3);
		minBuildingSize[1] = PlayerPrefs.GetInt("minBuildingSizeY", 3);
		minBuildingSize[2] = PlayerPrefs.GetInt("minBuildingSizeZ", 3);
		maxBuildingSize[0] = PlayerPrefs.GetInt("maxBuildingSizeX", 3);
		maxBuildingSize[1] = PlayerPrefs.GetInt("maxBuildingSizeY", 3);
		maxBuildingSize[2] = PlayerPrefs.GetInt("maxBuildingSizeZ", 3);
		
		state = State.list;
		
		style = new GUIStyle();
		style.normal.textColor = Color.white;
	}
	
	private void Start()
	{
		MasterServer.RequestHostList(gameName);
	}
	
	private void OnGUI()
	{
		GUI.skin = skin;
		edge = edging * Screen.width;
		
		switch(state)
		{
		case State.list:
			ListServers();
			break;
		case State.create:
			CustomizeServer();
			break;
		case State.play:
			break;
		}
	}
	
	public int sliderHeight = 22;
	
	float FloatSlider(Rect rect, float val, float min, float max, string label, float step, bool dynamic)
	{
		Vector2 size = skin.GetStyle("Label").CalcSize(new GUIContent(max.ToString("#0.0")));
		style.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(rect.x, rect.y, size.x, rect.height), val.ToString("#0.0"), style);
		
		float width = dynamic ? (max-min+step)*10/step : rect.width;
		GUI.Label(new Rect(rect.x+(width)+(size.x+5)+5, rect.y, rect.width+90-(size.x+5)-5, rect.height), label);
		
		float f = GUI.HorizontalSlider(new Rect(rect.x+(size.x+5), rect.y+(rect.height-12)/2, width, 12), val, min, max);
		val = Mathf.Round(f / step) * step;
		return val;
	}
	
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
	
	private void CustomizeServer() // CREATE A NEW SERVER
	{
		GUI.BeginGroup(new Rect(Screen.width/2-200, edge, 400, sliderHeight*13+edge*3), skin.box);
		
		GUI.Label(new Rect(edge, sliderHeight*0+edge, 100, sliderHeight), "Server Name: ");
		serverName = GUI.TextField(new Rect(edge+100, sliderHeight*0+edge, 300-edge*2, sliderHeight), serverName);
		
		GUI.Label(new Rect(edge, sliderHeight*2+edge, 200, sliderHeight), "City Dimensions");
		citySize[0] = IntSlider(new Rect(edge, sliderHeight*3+edge, 200, sliderHeight), citySize[0], 1, 9, "Width", 1, true);
		citySize[1] = IntSlider(new Rect(edge, sliderHeight*4+edge, 200, sliderHeight), citySize[1], 1, 9, "Height", 1, true);
		citySize[2] = IntSlider(new Rect(edge, sliderHeight*5+edge, 200, sliderHeight), citySize[2], 1, 9, "Depth", 1, true);
		
		maxPlayers = IntSlider(new Rect(edge+200, sliderHeight*2+edge, 200, sliderHeight), maxPlayers, 2, 14, "Max Players", 2, true);
		
		floor = GUI.Toggle(new Rect(edge+300, sliderHeight*3+edge, 100, sliderHeight), floor, " Floor");
		upgrades = GUI.Toggle(new Rect(edge+200, sliderHeight*3+edge, 100, sliderHeight), upgrades, " Upgrades");
		bots = GUI.Toggle(new Rect(edge+300, sliderHeight*4+edge, 100, sliderHeight), false, " Bots"); // bots are disabled for now
		listed = GUI.Toggle(new Rect(edge+200, sliderHeight*4+edge, 100, sliderHeight), true, " List"); // all games are listed for now
		
		spacing = FloatSlider(new Rect(edge+200, sliderHeight*5+edge, 80, sliderHeight), spacing, 1, 5, "Spacing", 0.1f, false);
		
		GUI.Label(new Rect(edge, sliderHeight*7+edge, 300, sliderHeight), "Minimum Building Dimensions");
		minBuildingSize[0] = IntSlider(new Rect(edge, sliderHeight*8+edge, 200, sliderHeight), minBuildingSize[0], 1, 9, "Width", 1, true);
		minBuildingSize[1] = IntSlider(new Rect(edge, sliderHeight*9+edge, 200, sliderHeight), minBuildingSize[1], 1, 9, "Height", 1, true);
		minBuildingSize[2] = IntSlider(new Rect(edge, sliderHeight*10+edge, 200, sliderHeight), minBuildingSize[2], 1, 9, "Depth", 1, true);
		
		GUI.Label(new Rect(edge+200, sliderHeight*7+edge, 200, sliderHeight), "Maximum Building Dimensions");
		maxBuildingSize[0] = Mathf.Max(minBuildingSize[0],
			IntSlider(new Rect(edge+200, sliderHeight*8+edge, 200, sliderHeight), maxBuildingSize[0], 1, 9, "Width", 1, true));
		maxBuildingSize[1] = Mathf.Max(minBuildingSize[1],
			IntSlider(new Rect(edge+200, sliderHeight*9+edge, 200, sliderHeight), maxBuildingSize[1], 1, 9, "Height", 1, true));
		maxBuildingSize[2] = Mathf.Max(minBuildingSize[2],
			IntSlider(new Rect(edge+200, sliderHeight*10+edge, 200, sliderHeight), maxBuildingSize[2], 1, 9, "Depth", 1, true));
		
		if (GUI.Button(new Rect(100, sliderHeight*12+edge, 100, sliderHeight), "Create"))
		{
			state = State.play;
			
			PlayerPrefs.SetString("serverName", serverName);
			
			PlayerPrefs.SetInt("maxPlayers", maxPlayers);
			
			PlayerPrefs.SetInt("hasBots", bots ? 1 : 0);
			PlayerPrefs.SetInt("hasUpgrades", upgrades ? 1 : 0);
			PlayerPrefs.SetInt("isListed", listed ? 1 : 0);
			PlayerPrefs.SetInt("hasFloor", floor ? 1 : 0);
			
			PlayerPrefs.SetFloat("buildingSpacing", spacing);
			
			PlayerPrefs.SetInt("citySizeX", citySize[0]);
			PlayerPrefs.SetInt("citySizeY", citySize[1]);
			PlayerPrefs.SetInt("citySizeZ", citySize[2]);
			
			PlayerPrefs.SetInt("minBuildingSizeX", minBuildingSize[0]);
			PlayerPrefs.SetInt("minBuildingSizeY", minBuildingSize[1]);
			PlayerPrefs.SetInt("minBuildingSizeZ", minBuildingSize[2]);
			
			PlayerPrefs.SetInt("maxBuildingSizeX", maxBuildingSize[0]);
			PlayerPrefs.SetInt("maxBuildingSizeY", maxBuildingSize[1]);
			PlayerPrefs.SetInt("maxBuildingSizeZ", maxBuildingSize[2]);
			
			CreateServer();
		}
		if (GUI.Button(new Rect(200, sliderHeight*12+edge, 100, sliderHeight), "Cancel"))
		{
			PlayerPrefs.SetString("serverName", serverName);
			state = State.list;
		}
		
		GUI.EndGroup();
	}
	
	private void ListServers() // LIST OF SERVERS, THIS IS THE INITIAL MENU PAGE
	{
		GUI.Box(new Rect(-5, -5, Screen.width+5, Screen.height+5), "");
		
		innerWidth = Screen.width-scrollBarWidth;
		
		GUI.BeginGroup(new Rect(edge, 5, 400, 25));
		
		if (GUI.Button(new Rect(0, 0, 65, 25), "Refresh")) // refresh our list of servers
		{
			MasterServer.RequestHostList(gameName);
		}
		GUI.Label(new Rect(70, 0, 200, 25), "the server list to join a game, or");
		if (GUI.Button(new Rect(260, 0, 80, 25), "create one."))
		{
			state = State.create;
		}
		
		GUI.EndGroup();
		
		GUI.Label(new Rect((Screen.width-edge)-200, 5, 40, 25), "Name:");
		playerName = GUI.TextField(new Rect((Screen.width-edge)-150, 5, 150, 25), playerName, 16);
		PlayerPrefs.SetString("playerName", playerName); // may be inefficient...
		
		GUI.BeginGroup(new Rect(edge, 35, innerWidth, 25));
			
		GUI.Label(new Rect(edge, 0, innerWidth-300, 25), "Name");
		GUI.Label(new Rect(innerWidth-300, 0, 100, 25), "Size");
		GUI.Label(new Rect(innerWidth-200, 0, 100, 25), "Players");
		GUI.Label(new Rect(innerWidth-100, 0, 100, 25), "Ping");
		
		GUI.EndGroup();
		
		hostData = MasterServer.PollHostList();
		if (hostData == null || hostData.Length == 0) // the server list is empty (or null)
		{
			GUI.Label(new Rect(edge, 60, 250, serverHeight), "No servers available, try refreshing.");
		}
		else
		{
			// this scrollbox's outer height is limited by the screen height in increments of serverHeight
			scrollPosition = GUI.BeginScrollView(
				new Rect(edge, 60, (Screen.width-edge*2),
					Mathf.Min(serverHeight*hostData.Length, Mathf.Round((Screen.height-60)/serverHeight)*serverHeight)),
				scrollPosition, new Rect(0, 0, innerWidth*0.98f, serverHeight*hostData.Length), false, true);
			
			for (int i = 0; i < hostData.Length; i++) // for each server we know about
			{
				GUI.BeginGroup(new Rect(0, serverHeight*i, innerWidth*0.98f, serverHeight), new GUIContent());
				
				if (GUI.Button(new Rect(0, 0, innerWidth-edge*2, serverHeight), ""))
				{
					state = State.play;
					Network.Connect(hostData[i]);
				}
				
				GUI.Label(new Rect(edge, 0, innerWidth-300, serverHeight), hostData[i].gameName);
				
				GUI.Label(new Rect(innerWidth-300, 0, 100, serverHeight), "Size");
				GUI.Label(new Rect(innerWidth-200, 0, 100, serverHeight), hostData[i].connectedPlayers.ToString());
				GUI.Label(new Rect(innerWidth-100, 0, 100, serverHeight), "Ping");
				
				GUI.EndGroup();
			}
			
	        GUI.EndScrollView();
		}
	}
	
	public void Update()
	{
		if (state != State.play)
		{
			return; // only proceed in a play state
		}
		
		if (Input.GetKeyUp(KeyCode.X))
		{
			Screen.lockCursor = false;
			
			// This is the wrong place for LeaveGame(). What if the player leaves unexpectedly?!
			GetComponent<GameData>().LeaveGame(); //TODO URGENT move to OnPlayerDisconnected()
			// I attempted the move and it left me very frustrated. For now it's a "known bug".
			
			Network.Disconnect();
			return;
		}
	}
	
	// ========================================= Server Handling (maybe move to seperate script)
	
	private void CreateServer()
	{
		Network.InitializeServer(maxPlayers, Random.Range(40770, 50770), !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameName, serverName);
	}
	
	private void OnServerInitialized()
	{
		GetComponent<PG_Map>().CreateMap();
	}
	
	// Called on the client when you have successfully connected to a server.
	private void OnConnectedToServer()
	{
	}
	
	// Called on the server whenever a new player has successfully connected.
	private void OnPlayerConnected(NetworkPlayer netPlayer)
	{
		GetComponent<GameData>().networkPlayer = netPlayer;
	}
	
	// Called on the server whenever a player is disconnected from the server.
	private void OnPlayerDisconnected(NetworkPlayer netPlayer)
	{
		//TODO these numbers should really be constants in a seprate static class
		Network.RemoveRPCs(netPlayer, 4); // player
        Network.RemoveRPCs(netPlayer, 3); // shots
		//GetComponent<GameData>().LeaveGame(netPlayer);
        Network.DestroyPlayerObjects(netPlayer);
	}
	
	// Called on client during disconnection from server, but also on the server when the connection has disconnected.
	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Screen.showCursor = true;
		GetComponent<GameData>().ClearData(true);
		GetComponent<UpgradeManager>().enabled = false;
			
		foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
		{
			if (go.tag != "Master")
			{
				Destroy(go);
			}
		}
		
		MasterServer.RequestHostList(gameName);
		state = State.list;
	}
	
	// Called on clients or servers when reporting events from the MasterServer.
	private void OnMasterServerEvent(MasterServerEvent mse)
	{
		Debug.Log(mse);
	}
}
