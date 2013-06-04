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
	
	private void Update()
	{
		if (!owned)
		{
			if (red == totalCubes)
			{
				owned = true;
			}
			else if (blue == totalCubes)
			{
				owned = true;
			}
		}
	}
}
