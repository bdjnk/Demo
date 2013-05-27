using UnityEngine;
using System.Collections;

public class PG_Shot : MonoBehaviour
{
	public PG_Gun gun;
	
	public float persist = 6f;
	private float timeAtStart;
	private GameObject stunSound;
	
	private void Start()
	{
		if (gun == null)
		{
			Destroy(gameObject);
		}
		timeAtStart = (float)Network.time;
		
		stunSound = Resources.Load("Prefabs/Stun") as GameObject;
	}
	
	private void Update()
	{
		// persist for network functionality && this shot belongs to me (on the network)
		if (Network.time  > timeAtStart + persist && gun != null && gun.networkView.isMine)
		{
			Network.Destroy(gameObject); // destroy for the server and all clients
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (Network.isServer) // <== AUTHORITATIVE SERVER
		{
			PG_Cube cubeScript =  other.GetComponent<PG_Cube>();
			
			if (cubeScript != null) // this is a cube we're dealing with here
			{
				cubeScript.Struck(this);
			}
			else if (gun.tag != "Bot" && (gun.transform.parent.tag == "Red" || gun.transform.parent.tag == "Blue")) // shot was fired by a player
			{
				if (other.tag == "Red" || other.tag == "Blue") // shot hit a player
				{
					gun.freezeTimeout = (float)Network.time + 2f;
					Instantiate(stunSound, this.transform.position, Quaternion.identity); //play sound
				}
			}
		}
		Destroy(gameObject);
	}
}
