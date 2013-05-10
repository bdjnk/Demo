using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Vector2 scrollPosition = Vector2.zero;
	
	public GUISkin skin;
	
	public int serverCount = 40;
	public int serverHeight = 25;
	private float edge = 0.01f;
	
	private int innerWidth;
	private int scrollBarWidth = 17;
	
	void Awake()
	{
		edge *= Screen.width;
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		
		innerWidth = Screen.width-scrollBarWidth;
		
		GUI.BeginGroup(new Rect(edge, 5, 400, 25));
		
		GUI.Button(new Rect(0, 0, 65, 25), "Refresh");
		GUI.Label(new Rect(70, 0, 200, 25), "the server list to join a game, or");
		GUI.Button(new Rect(260, 0, 80, 25), "create one.");
		
		GUI.EndGroup();
		
		GUI.Button(new Rect((Screen.width-edge)-70, 5, 70, 25), "Settings");
		
		GUI.BeginGroup(new Rect(edge, 35, innerWidth, 25));
			
		GUI.Label(new Rect(0, 0, innerWidth-300, 25), "Name");
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
				//TODO this group should be clickable because it has a GUIContent, so figure that out
				GUI.BeginGroup(new Rect(0, serverHeight*i, innerWidth*0.98f, serverHeight), new GUIContent());
				
				GUI.Label(new Rect(    0, 0, innerWidth-300, serverHeight), "Test Server Name #"+i);
				GUI.Label(new Rect(innerWidth-300, 0, 100, serverHeight), "Size");
				GUI.Label(new Rect(innerWidth-200, 0, 100, serverHeight), "Players");
				GUI.Label(new Rect(innerWidth-100, 0, 100, serverHeight), "Ping");
				
				GUI.EndGroup();
			}
			
	        GUI.EndScrollView();
		}
	}
}
