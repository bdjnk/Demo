using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour
{
	private int redCount;
	private int blueCount;
	
	public Texture next;
	
	public Texture GetTeam()
	{
		if (redCount < blueCount)
		{
			networkView.RPC("redPlus", RPCMode.AllBuffered);
			return Resources.Load("Textures/Red") as Texture;
		}
		else // blueCount <= redCount
		{
			networkView.RPC("bluePlus", RPCMode.AllBuffered);
			return Resources.Load("Textures/Blue") as Texture;
		}
	}
	
	[RPC] private void redPlus() { redCount++; }
	[RPC] private void bluePlus() { blueCount++; }
}
