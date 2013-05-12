using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (NetworkView))]
public class PlayerDataScript : MonoBehaviour
{
	private GameObject mainGame = null;
	private GameManagerScript mainGameScript = null;
	//private GameObject myPlayer = null;
	
	private int blueTeamTotalPlayers;
	private int redTeamTotalPlayers;
	private int totalCubes;
	
	private string playerName;
	private string playerNetworkID;
	private string playerTeamColor;
	private int myTotalClaims;
	private int myTotalOwned;
	private int myPercentOfTeamTotal;
	
	private int playerNumber;
	private bool showGUI = false;
	private bool showRed = false;
	private bool showBlue = false;
	
	//sizes for buttons
	private float buttonX = Screen.width*0.02f;
	private float buttonY = Screen.width*0.02f;
	private float buttonW = Screen.width*0.12f;
	private float buttonH = Screen.width*0.2f;
	
	
	private string myTeam = "red";
	private bool mouseLooking = true;
	private bool updateGUI = false;
	//private float timeCheck;
	private int guiSecondsToWait = 2;//will display half this time
	
	//try to remove these if possible
	
	private string redTeamPlayersString;
	private string blueTeamPlayersString;
	private int redTeamTotalScore;
	private int blueTeamTotalScore;
	
	private Color playerColor = Color.red;
	
	public enum playerState
	{
		Normal,
		GuiOn,
		InitServer
	}
	
	public playerState mCurrentState;
	
	// Use this for initialization
	void Start()
	{
		//timeCheck = Time.realtimeSinceStartup;
		mainGame = GameObject.Find ("GameManager");
		mainGameScript = mainGame.GetComponent<GameManagerScript>();
		
		//myPlayer = this.parent;
		playerName = PlayerPrefs.GetString("name");//needs to be set
		playerNetworkID = Network.player.guid;
		playerTeamColor = PlayerPrefs.GetString("teamColor");
		myTotalClaims = 0;
	  	myTotalOwned = 0;
	 	myPercentOfTeamTotal = 0;
	
	}
	
	[RPC]
	void SetMyName(string name)
	{
		this.name = name;	
	}
	
	[RPC]
	void updateTeamCounts(int red,int blue)
	{
		redTeamTotalPlayers += red;
		blueTeamTotalPlayers += blue;
	}
	
	[RPC]
	void setNewPlayerData(string teamColor, string myName, string myID)
	{
		playerName = myName;
		playerNetworkID = myID;
		//default color is red
		GameObject playerShotColor = Resources.Load("Prefabs/RedShot") as GameObject;
		Material playerMaterialColor = Resources.Load("Materials/Red") as Material;
		if (teamColor == "blue")
		{
			playerShotColor = Resources.Load("Prefabs/BlueShot") as GameObject;
			playerMaterialColor = Resources.Load ("Materials/Blue") as Material;
			myTeam = "blue";
			this.name = "Blue Team Player (" + myName + ")";
		}
		else
		{
			this.name = "Red Team Player (" + myName + ")";
		}
		this.GetComponentInChildren<Camera>().GetComponent<PG_Gun>().shot = playerShotColor;
		this.GetComponentInChildren<MeshRenderer>().renderer.material = playerMaterialColor;
	}
	// Update is called once per frame
	void Update()
	{
		//disable if not local player
		if (playerNetworkID != Network.player.guid)
		{
			enabled = false;
		}
		
		//temporary pause updates while p is down
		if (Input.GetKeyDown("p"))
		{
			updateMouseLook(!mouseLooking);
			mouseLooking = !mouseLooking;
		}
		
		//set when to update GUI lists
		if ((int)Time.realtimeSinceStartup % guiSecondsToWait == 0)
		{
			updateGUI = true;
		}
		else
		{
			updateGUI = false;
		}
			
		if (Input.GetKeyDown("t") && Network.player.guid == playerNetworkID)
		{
			showGUI = !showGUI;
			if (myTeam == "blue")
			{
				showRed = false;
				showBlue = true;
			}
			else
			{
				showRed = true;
				showBlue = false;
			}
		}
	}
	
	private void updateMyScores()
	{
		if (Network.player.guid == playerNetworkID)
		{
			myTotalClaims = mainGameScript.getMyTotalClaims(playerNetworkID);
			myTotalOwned = mainGameScript.getMyTotalOwned(playerNetworkID);
			myPercentOfTeamTotal = mainGameScript.getMyPercentage(playerNetworkID);
		}
	}
	
	void OnGUI()
	{
		if(showGUI)
		{
			if(updateGUI)
			{
				//get update to current player lists
				//mainGameScript.g
				redTeamPlayersString = mainGameScript.getRedTeamString();
				blueTeamPlayersString = mainGameScript.getBlueTeamString();
				redTeamTotalScore = mainGameScript.getRedTeamScore();
				blueTeamTotalScore = mainGameScript.getBlueTeamScore();
				totalCubes = mainGameScript.getTotalCubes();
				updateMyScores();
			}
			//display the lists
			if (showRed)
			{
				//Debug.Log ("red team display by" + playerName);
				GUI.Box(new Rect(buttonX,buttonY,buttonW,buttonH),"Red Team: \n" + redTeamPlayersString);
				GUI.Box(new Rect(Screen.width - buttonW - buttonX,buttonY,buttonW,buttonH/2),"Red Team: \n" + redTeamTotalScore + "\n " + ((int) (100.0f* redTeamTotalScore/totalCubes))+ "%");
				GUI.Box(new Rect(Screen.width - buttonW - buttonX,buttonY+buttonH*0.6f,buttonW,buttonH/2),"Blue Team: \n" + blueTeamTotalScore + "\n " + ((int)(100.0f* blueTeamTotalScore/totalCubes))+ "%");
				GUI.Box(new Rect(Screen.width - buttonW - buttonX,buttonY+buttonH*1.2f,buttonW,buttonH),"My Cubes: \n" + myTotalOwned + "\n" + myPercentOfTeamTotal + "%\nClaims: \n" + myTotalClaims);
			}
			if (showBlue)
			{
				//Debug.Log ("blue team display by " + playerName);
				GUI.Box(new Rect(buttonX,buttonY,buttonW,buttonH),"Blue Team: \n" + blueTeamPlayersString);
				GUI.Box(new Rect(Screen.width - buttonW - buttonX,buttonY,buttonW,buttonH/2), "Blue Team: \n" + blueTeamTotalScore + "\n " + ((int)(100.0f* blueTeamTotalScore/totalCubes)) + "%");
				GUI.Box(new Rect(Screen.width - buttonW - buttonX,buttonY+buttonH*0.6f,buttonW,buttonH/2),"Red Team: \n" + redTeamTotalScore + "\n " + ((int)(100.0f* redTeamTotalScore/totalCubes)) + "%");
				GUI.Box(new Rect(Screen.width - buttonW - buttonX,buttonY+buttonH*1.2f,buttonW,buttonH),"My Cubes: \n" + myTotalOwned + "\n" + myPercentOfTeamTotal + "%\nClaims: \n" + myTotalClaims);
			}
		}
	}
	
	public void updateMouseLook(bool mouseLookingSet)
	{
		//GetComponentInChildren<Camera>().enabled = mouseLookingSet;
		GetComponentInChildren<Camera>().GetComponent<MouseLook>().enabled = mouseLookingSet;
		GetComponent<FPSInputController>().enabled = mouseLookingSet;
		GetComponent<CharacterMotor>().enabled = mouseLookingSet;
		GetComponent<MouseLook>().enabled = mouseLookingSet;
		
		Screen.lockCursor = mouseLookingSet;
		Screen.showCursor = mouseLookingSet;
	}
	
	public void updateInitialSettings(bool boolSet)
	{
		enabled = true;//enable for this player only
		GetComponentInChildren<Camera>().enabled = boolSet;
		GetComponentInChildren<Camera>().GetComponent<PG_Gun>().enabled = boolSet;
	}
	
	public string nameOfPlayer
	{
	    get { return playerName; }
	    set { playerName = value; }
	}
	public string color
	{
	    get { return playerTeamColor; }
	    set { playerTeamColor = value; }
	}
	public string networkID
	{
	    get { return playerNetworkID; }
	    set { playerNetworkID = value; }
	}
	public int totalClaims
	{
	    get { return myTotalClaims; }
	    set { myTotalClaims = value; }
	}
	public int totalOwned
	{
	    get { return myTotalOwned; }
	    set { myTotalOwned = value; }
	}
	public int myPercent
	{
	    get { return myPercentOfTeamTotal; }
	    set { myPercentOfTeamTotal = value; }
	}
}
