using UnityEngine;
using System.Collections;

/* Buildings should keep track of:
 * 
 * 
 */
public class PG_Building : MonoBehaviour
{
	// this should hold number of cubes, percentage captured, etc.
	public int red;
	public int blue;
	public bool owned = false;
	
	public int totalCubes = 0;
	[RPC] public void SetCubeCount(int cubeCount) { totalCubes = cubeCount; }
	
	private GameData gameData;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
	}
	
	private void Update()
	{
		if (!owned && Network.isServer)//revised only call from server
		{
			if (red == totalCubes)
			{	gameData.AddRedOwned(totalCubes);
				//gameData.networkView.RPC("AddRedOwned", RPCMode.Server, totalCubes);
				owned = true;
			}
			else if (blue == totalCubes)
			{
				gameData.AddBlueOwned(totalCubes);
				//gameData.networkView.RPC("AddBlueOwned", RPCMode.Server, totalCubes);
				owned = true;
			}
		}
	}
}
