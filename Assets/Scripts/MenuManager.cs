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
	
	void Awake()
	{
		if (!PlayerPrefs.HasKey("name"))
		{
			PlayerPrefs.SetString("name", "Default");
		}
		playerName = PlayerPrefs.GetString("name");
		creating = false;
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
	public int sliderHeight = 20;
	
	void CreateServer()
	{
		citySize[0] = (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*0+4, 100, 12), citySize[0], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*0, 50, sliderHeight), citySize[0].ToString());
		citySize[1] = (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*1+4, 100, 12), citySize[1], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*1, 50, sliderHeight), citySize[1].ToString());
		citySize[2] = (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*2+4, 100, 12), citySize[2], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*2, 50, sliderHeight), citySize[2].ToString());
		
		minBuildingSize[0] = (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*4+4, 100, 12), minBuildingSize[0], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*4, 50, sliderHeight), minBuildingSize[0].ToString());
		minBuildingSize[1] = (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*5+4, 100, 12), minBuildingSize[1], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*5, 50, sliderHeight), minBuildingSize[1].ToString());
		minBuildingSize[2] = (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*6+4, 100, 12), minBuildingSize[2], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*6, 50, sliderHeight), minBuildingSize[2].ToString());
		
		maxBuildingSize[0]= (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*8+4, 100, 12), maxBuildingSize[0], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*8, 50, sliderHeight), maxBuildingSize[0].ToString());
		maxBuildingSize[1]= (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*9+4, 100, 12), maxBuildingSize[1], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*9, 50, sliderHeight), maxBuildingSize[1].ToString());
		maxBuildingSize[2]= (int)GUI.HorizontalSlider(new Rect(0, sliderHeight*10+4, 100, 12), maxBuildingSize[2], 1, 9);
		GUI.Label(new Rect(110, sliderHeight*10, 50, sliderHeight), maxBuildingSize[2].ToString());
	}
	
	void ListServers()
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
