using UnityEngine;
using System.Collections;

public class PG_Shot : MonoBehaviour
{
	public PG_Gun gun;
	
	public float persist = 6f;
	private float timeAtStart;
	
	private void Start()
	{
		if (gun == null)
		{
			Destroy(gameObject);
		}
		timeAtStart = (float)Network.time;
	}
	
	private void Update()
	{
		// persist for network functionality && this shot belongs to me (on the network)
		if (Network.time  > timeAtStart + persist && gun != null && gun.networkView.isMine)
		{
			Network.RemoveRPCs(networkView.viewID);
			Network.Destroy(gameObject); // destroy for the server and all clients
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		PG_Cube cubeScript =  other.GetComponent<PG_Cube>();
		
		if (cubeScript != null) // this is a cube we're dealing with here
		{
			cubeScript.Struck(this);
		}
		if (gun != null && gun.networkView.isMine) // this shot belongs to me (on the network)
		{
			Network.RemoveRPCs(networkView.viewID);
			Network.Destroy(gameObject); // destroy for the server and all clients
		}
	}
}
