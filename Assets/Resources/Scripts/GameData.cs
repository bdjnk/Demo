using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour
{
	public NetworkPlayer netPlayer;
	
	private int redCount;
	private int blueCount;
	
	public Texture GetTeam()
	{
		if (redCount < blueCount)
		{
			networkView.RPC("joinRed", RPCMode.AllBuffered);
			return Resources.Load("Textures/Red") as Texture;
		}
		else // blueCount <= redCount
		{
			networkView.RPC("joinBlue", RPCMode.AllBuffered);
			return Resources.Load("Textures/Blue") as Texture;
		}
	}
	
	[RPC] private void joinRed() { redCount++; }
	[RPC] private void joinBlue() { blueCount++; }
	
	public int RedCount { get { return redCount; } }
	public int BlueCount { get { return blueCount; } }
	
	public int redScore = 0;
	public int blueScore = 0;
}
