using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Vector2 scrollPosition = Vector2.zero;
	
	public GUISkin skin;
	
	public int serverCount = 40;
	public int serverHeight = 25;
	private int w, h;
	private int scrollBarWidth = 16;
	
	void OnGUI()
	{
		GUI.skin = skin;
		
		w = Screen.width-scrollBarWidth;
		
		GUI.Button(new Rect(w*0.01f, 0, 100, 25), "Refresh");
		GUI.Label(new Rect(w*0.01f+100, 0, 300, 25), "the server list to join a game, or");
		GUI.Button(new Rect(w*0.01f+400, 0, 100, 25), "create one");
		
		GUI.BeginGroup(new Rect(w*0.01f, 25, w, 50));
			
		GUI.Label(new Rect(0, 0, w-300, 25), "Name");
		GUI.Label(new Rect(w-300, 0, 100, 25), "Size");
		GUI.Label(new Rect(w-200, 0, 100, 25), "Players");
		GUI.Label(new Rect(w-100, 0, 100, 25), "Ping");
		
		GUI.EndGroup();
		
		scrollPosition = GUI.BeginScrollView(
			new Rect(w*0.01f, 50, w*0.98f+scrollBarWidth, Mathf.Min(serverHeight*serverCount, Mathf.Round((Screen.height-50)/30)*30)),
			scrollPosition, new Rect(0, 0, w*0.98f, serverHeight*serverCount), false, true);
		
		for (int i = 0; i < serverCount; i++)
		{
			GUI.BeginGroup(new Rect(0, serverHeight*i, w*0.98f, serverHeight*(i+1)), new GUIContent());
			
			GUI.Label(new Rect(    0, 0, w-300, serverHeight), "Test Server Name #"+i);
			GUI.Label(new Rect(w-300, 0, 100, serverHeight), "Size");
			GUI.Label(new Rect(w-200, 0, 100, serverHeight), "Players");
			GUI.Label(new Rect(w-100, 0, 100, serverHeight), "Ping");
			
			GUI.EndGroup();
		}
		
        GUI.EndScrollView();
	}
}
