using UnityEngine;
using System.Collections;

/* Cubes should keep track of their color
 *    and which building they belong to.
 * They don't even need an update function.
 */
public class PG_Cube : MonoBehaviour
{
	private GameData gameData;
	
	public Material gray;
	public Texture red;
	public Texture blue;
	
	public int resistence = 4;
	public int maxColor = 5;
	public float amountBlue;
	public float amountRed;
	private float adjacentCubeDistance = 2.9f;
	
	private PG_Shot latest;
	private NetworkViewID captorID;
	
	public GameObject upgradeClaim;
	public AudioClip standardClaim;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
		captorID = NetworkViewID.unassigned;
	}
	
	private PG_Building building;
	
	private void Start()
	{
		building = transform.parent.GetComponent<PG_Building>();
	}
	//BEN ADD TO TRY AND FIX THE RPC ISSUE WITH SCORING
	public void Update(){
		if(building==null){
			building = transform.parent.GetComponent<PG_Building>();
		}
	}
	
	[RPC] private void SetDecal(string upgrade)
	{
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/"+upgrade) as Texture);
	}
	
	public void Struck(PG_Shot shot)
	{
		latest = shot;
		
		Texture upgrade = renderer.material.GetTexture("_DecalTex");
		
		if (upgrade != null)
		{
			shot.gun.networkView.RPC("Upgrade", RPCMode.AllBuffered, upgrade.name);
			networkView.RPC("SetDecal", RPCMode.AllBuffered, "");
			Network.Instantiate(upgradeClaim, transform.position, Quaternion.identity, 210);
		}
		
		if (building.owned) { return; } // nothing to be done <-- COMMENT TO UNDO BUILDING LOCKING
	
		foreach (Transform child in transform.parent) // foreach cube in building, do splash effect
		{
			float distance = Vector3.Distance(transform.position, child.position);
			
			if (distance < adjacentCubeDistance) // only consider adjacent cubes
			{
				PG_Cube cubeScript = child.GetComponent<PG_Cube>();
				
				if (cubeScript != null) // this is a cube
				{
					cubeScript.Effects(shot, distance);
				}
			}
		}
	}
	
	private void Effects(PG_Shot shot, float distance)
	{
		if (shot != null && shot.gun != null)
		{
			float effect = shot.gun.power - distance;
			
			Texture texture = shot.renderer.sharedMaterial.mainTexture;
			if (texture == blue)
			{
				amountRed = Mathf.Max(0, amountRed - effect);
				amountBlue = Mathf.Min(maxColor, amountBlue + effect);
			}
			else if (texture == red)
			{
				amountBlue = Mathf.Max(0, amountBlue - effect);
				amountRed = Mathf.Min(maxColor, amountRed + effect);
			}
			SetColor(shot);
		}
	}
	
	public void SetColor(PG_Shot shot)
	{
		if (amountBlue > resistence && renderer.material.color != gameData.blue)
		{
			//networkView.RPC("SetBlue", RPCMode.All);
			SetBlue ();
			
			if (shot != null && shot.gun != null && shot.gun.tag != "Bot")
			{
				networkView.RPC("InformCaptor", RPCMode.AllBuffered, shot.gun.transform.parent.networkView.viewID);
			}
		}
		else if (amountRed > resistence && renderer.material.color != gameData.red)
		{
			//networkView.RPC("SetRed", RPCMode.All);
			SetRed ();
			
			if (shot != null && shot.gun != null && shot.gun.tag != "Bot")
			{
				networkView.RPC("InformCaptor", RPCMode.AllBuffered, shot.gun.transform.parent.networkView.viewID);
			}
		}
	}
	
	[RPC] private void InformCaptor(NetworkViewID id) //TODO only send to the actual captor(s).
	{
		NetworkView netView = NetworkView.Find(id);
		if (netView == null) { return; }
		
		GameObject captor = netView.gameObject;
		
		if (this.captorID != NetworkViewID.unassigned && captorID.isMine)
		{
			captor.GetComponent<PlayerManager>().myScore--; // do we have a previous captor?
		}
		captorID = id;
		
		if (captorID.isMine)
		{
			captor.GetComponent<PlayerManager>().myScore++; // give the player their due
		}
	}
	
	[RPC] private void SetRed()
	{
		// putting the sound here (and not right before the call) makes it play for everyone,
		// but maybe it should just play for the person who shot...
		GetComponent<AudioSource>().PlayOneShot(standardClaim);
		
		//BEN ADD: force to only allow update in real-time (if a client joins in middle does not update)
		//if((float)gameData.time - timeShot < 1.0f){
			if (renderer.material.color == gameData.blue)
			{
				gameData.blueScore--;
				building.blue--;
			}
			//renderer.material.color = gameData.red;
			gameData.redScore++;
			building.red++;
		//}
		renderer.material.color = gameData.red;
		networkView.RPC ("SetTextureRed",RPCMode.OthersBuffered);
		networkView.RPC ("clientHitVisualsRed",RPCMode.Others);
		StartCoroutine("HitVisuals", "Red");
	}
	[RPC] private void clientHitVisualsRed(){
		GetComponent<AudioSource>().PlayOneShot(standardClaim);
		StartCoroutine("HitVisuals", "Red");
	}
	
	[RPC] private void SetTextureRed(){
		renderer.material.color = gameData.red;
	}
	
	[RPC] private void SetBlue()
	{
		GetComponent<AudioSource>().PlayOneShot(standardClaim);
		//if((float) gameData.time - timeShot < 1.0f){
			if (renderer.material.color == gameData.red)
				{
					gameData.redScore--;
					building.red--;
				}
			//renderer.material.color = gameData.blue;
			gameData.blueScore++;
			building.blue++;
		//}
		renderer.material.color = gameData.blue;
		networkView.RPC ("SetTextureBlue",RPCMode.OthersBuffered);	
		networkView.RPC ("clientHitVisualsBlue",RPCMode.Others);	
		StartCoroutine("HitVisuals", "Blue");
	}
	[RPC] private void SetTextureBlue(){
		renderer.material.color = gameData.blue;
	}
	[RPC] private void clientHitVisualsBlue(){
		GetComponent<AudioSource>().PlayOneShot(standardClaim);
		StartCoroutine("HitVisuals", "Blue");
	}
	
	
	IEnumerator HitVisuals(string hitColor)
	{
		//Texture prior = renderer.material.mainTexture;//GetTexture("_DecalTex");
		Texture prior = Resources.Load ("Textures/Cube") as Texture;
		for (int i = 1; i <= 5; i++)
		{
			renderer.material.mainTexture = Resources.Load("Textures/"+hitColor+"Hit"+i) as Texture;
			yield return new WaitForSeconds(0.1f);
		}
		renderer.material.mainTexture = prior;
	}
	
	[RPC] public void SetGray() // never used [06/03/2013]
	{
		if (captorID != NetworkViewID.unassigned && captorID.isMine)
		{
			NetworkView.Find(captorID).GetComponent<PlayerManager>().myScore--;
			captorID = NetworkViewID.unassigned;
		}
		amountRed = amountBlue = 0;
		renderer.material.color = gameData.gray;
		SetDecal("");
	}
}