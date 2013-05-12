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
		else
		{
			ListServers();
		}
	}

	public int[] citySize = {3, 3, 3};
    public int[] minBuildingSize = {1, 1, 1};
    public int[] maxBuildingSize = {3, 3, 3};
	int maxPlayers = 6;
	public int sliderHeight = 25;
	
	int IntSlider(Rect rect, int val, int min, int max, string label, int step)
	{
		Vector2 size = skin.GetStyle("Label").CalcSize(new GUIContent(max.ToString()));
		style.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(rect.x, rect.y, size.x, rect.height), val.ToString(), style);
		float f = GUI.HorizontalSlider(new Rect(rect.x+(size.x+5), rect.y+(rect.height-12)/2, (max-min+1)*10, 12), val, min, max);
		val = Mathf.RoundToInt(f / (float)step) * step;
		GUI.Label(new Rect(rect.x+((max-min+1)*10)+(size.x+5)+5, rect.y, rect.width-90-(size.x+5)-5, rect.height), label);
		return val;
	}
	
	void CreateServer() // CREATE A NEW SERVER
	{
		GUI.Label(new Rect(0, sliderHeight*0, 300, sliderHeight), "City Dimensions");
		citySize[0] = IntSlider(new Rect(0, sliderHeight*1, 300, sliderHeight), citySize[0], 1, 9, "Width", 1);
		citySize[1] = IntSlider(new Rect(0, sliderHeight*2, 300, sliderHeight), citySize[1], 1, 9, "Height", 1);
		citySize[2] = IntSlider(new Rect(0, sliderHeight*3, 300, sliderHeight), citySize[2], 1, 9, "Depth", 1);
		
		GUI.Label(new Rect(0, sliderHeight*5, 300, sliderHeight), "Minimum Building Dimensions");
		minBuildingSize[0] = IntSlider(new Rect(0, sliderHeight*6, 300, sliderHeight), minBuildingSize[0], 1, 9, "Width", 1);
		minBuildingSize[1] = IntSlider(new Rect(0, sliderHeight*7, 300, sliderHeight), minBuildingSize[1], 1, 9, "Height", 1);
		minBuildingSize[2] = IntSlider(new Rect(0, sliderHeight*8, 300, sliderHeight), minBuildingSize[2], 1, 9, "Depth", 1);
		
		GUI.Label(new Rect(0, sliderHeight*10, 300, sliderHeight), "Maximum Building Dimensions");
		maxBuildingSize[0] = IntSlider(new Rect(0, sliderHeight*11, 300, sliderHeight), maxBuildingSize[0], 1, 9, "Width", 1);
		maxBuildingSize[1] = IntSlider(new Rect(0, sliderHeight*12, 300, sliderHeight), maxBuildingSize[1], 1, 9, "Height", 1);
		maxBuildingSize[2] = IntSlider(new Rect(0, sliderHeight*13, 300, sliderHeight), maxBuildingSize[2], 1, 9, "Depth", 1);
		
		maxPlayers = IntSlider(new Rect(0, sliderHeight*15, 300, sliderHeight), maxPlayers, 2, 12, "Players", 2);
	}
	
	void ListServers() // LIST OF SERVERS, THIS IS THE INITIAL MENU PAGE
	{
		innerWidth = Screen.width-scrollBarWidth;
		
		GUI.BeginGroup(new Rect(edge, 5, 400, 25));
		
		GUI.Button(new Rect(0, 0, 65, 25), "Refresh");
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
				
				GUI.Button(new Rect(0, 0, innerWidth-edge*2, serverHeight), "");
				
				GUI.Label(new Rect(edge, 0, innerWidth-300, serverHeight), "Test Server Name #"+i);
				GUI.Label(new Rect(innerWidth-300, 0, 100, serverHeight), "Size");
				GUI.Label(new Rect(innerWidth-200, 0, 100, serverHeight), "Players");
				GUI.Label(new Rect(innerWidth-100, 0, 100, serverHeight), "Ping");
				
				GUI.EndGroup();
			}
			
	        GUI.EndScrollView();
		}
	}
}
