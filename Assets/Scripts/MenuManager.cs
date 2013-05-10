using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Vector2 scrollPosition = Vector2.zero;
	
	public GUISkin skin;
	
	public int serverCount = 40;
	public int serverHeight = 25;
	private int w, h;
	private int scrollBarWidth = 17;
	
	void OnGUI()
	{
		GUI.skin = skin;
		
		w = Screen.width-scrollBarWidth;
		
		GUI.BeginGroup(new Rect(w*0.01f, 5, w, 25));
		
		GUI.Button(new Rect(0, 0, 65, 25), "Refresh");
		GUI.Label(new Rect(70, 0, 200, 25), "the server list to join a game, or");
		GUI.Button(new Rect(260, 0, 80, 25), "create one.");
		
		GUI.EndGroup();
		
		GUI.Button(new Rect(Screen.width*0.99f-70, 5, 70, 25), "Settings");
		
		GUI.BeginGroup(new Rect(w*0.01f, 35, w, 25));
			
		GUI.Label(new Rect(0, 0, w-300, 25), "Name");
		GUI.Label(new Rect(w-300, 0, 100, 25), "Size");
		GUI.Label(new Rect(w-200, 0, 100, 25), "Players");
		GUI.Label(new Rect(w-100, 0, 100, 25), "Ping");
		
		GUI.EndGroup();
		
		scrollPosition = GUI.BeginScrollView(
			new Rect(w*0.01f, 60, Screen.width*0.98f, Mathf.Min(serverHeight*serverCount, Mathf.Round((Screen.height-60)/30)*30)),
			scrollPosition, new Rect(0, 0, w*0.98f, serverHeight*serverCount), false, true);
		
		for (int i = 0; i < serverCount; i++)
		{
			//GUI.BeginGroup(new Rect(0, serverHeight*i, w*0.98f, serverHeight*(i+1)), new GUIContent());
			GUI.BeginGroup(new Rect(0, serverHeight*i, w*0.98f, serverHeight), new GUIContent());
			
			GUI.Label(new Rect(    0, 0, w-300, serverHeight), "Test Server Name #"+i);
			GUI.Label(new Rect(w-300, 0, 100, serverHeight), "Size");
			GUI.Label(new Rect(w-200, 0, 100, serverHeight), "Players");
			GUI.Label(new Rect(w-100, 0, 100, serverHeight), "Ping");
			
			GUI.EndGroup();
		}
		
        GUI.EndScrollView();
	}
}
