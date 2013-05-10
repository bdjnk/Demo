using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Vector2 scrollPosition = Vector2.zero;
	
	public int serverCount = 40;
	public int serverHeight = 30;
	private int w, h;
	private int scrollBarWidth = 16;
	
	void OnGUI()
	{
		w = Screen.width-scrollBarWidth;
		
		GUI.Label(new Rect(w*0.01f, 0, w-300, 40), "Name");
		GUI.Label(new Rect(w-300+w*0.01f, 0, 100, 40), "Size");
		GUI.Label(new Rect(w-200+w*0.01f, 0, 100, 40), "Players");
		GUI.Label(new Rect(w-100+w*0.01f, 0, 100, 40), "Ping");
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
