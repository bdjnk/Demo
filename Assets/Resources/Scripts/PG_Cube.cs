using UnityEngine;
using System.Collections;

/* Cubes should keep track of their color
 *    and which building they belong to.
 * They don't even need an update function.
 */
public class PG_Cube : MonoBehaviour
{
	public Material gray;
	public Texture red;
	public Texture blue;
	
	public GUITexture test;
	
	public int resistence = 4;
	public int maxColor = 5;
	private float amountBlue;
	private float amountRed;
	private string cubeOwnerID;
	
	private Up up = Up.none;
	private enum Up
	{
		none, blast, fast, rapid, move, evade
	};
	
	private PG_Building building;
	
	void Start()
	{
		building = transform.parent.GetComponent<PG_Building>();
		cubeOwnerID="";
	}
	
	public void Struck(PG_Shot shot)
	{		
		if(shot!=null){
			foreach (Transform child in transform.parent) // splash effect
			{
				float distance = Vector3.Distance(transform.position, child.position);
				//testing for minimum reaction on adjacent
				if (distance < 2.9f) // only consider adjacent cubes
				{
					PG_Cube cubeScript = child.GetComponent<PG_Cube>();
					if (cubeScript != null) // this is a cube
					{	if(shot!=null){
							cubeScript.Effects(shot, distance);
						}
					}
				}
			}
			
			Texture upgrade = renderer.material.GetTexture("_DecalTex");
			
			if (upgrade != null)
			{
				// this is silly, fix with an enum in production
				if (upgrade.name == "BlastShots")
				{
					if (shot.gun.bs == null) {
						shot.gun.bs = upgrade;
						shot.gun.power += 2;
					}
				}
				else if (upgrade.name == "EvadeBots")
				{
					if (shot.gun.eb == null) {
						shot.gun.eb = upgrade;
						// do nothing else, for now
					}
				}
				else if (upgrade.name == "FastShots")
				{
					if (shot.gun.fs == null) {
						shot.gun.fs = upgrade;
						shot.gun.speed *= 2;
					}
				}
				else if (upgrade.name == "MoveQuick")
				{
					if (shot.gun.qm == null) {
						shot.gun.qm = upgrade;
						CharacterMotor cm = shot.gun.transform.parent.GetComponent<CharacterMotor>();
						cm.jumping.baseHeight = 4;
						cm.movement.maxForwardSpeed *= 2;
						cm.movement.maxSidewaysSpeed *= 2;
						cm.movement.maxBackwardsSpeed *= 2;
						cm.movement.maxGroundAcceleration *= 3;
					}
				}
				else if (upgrade.name == "RapidFire")
				{
					if (shot.gun.rf == null) {
						shot.gun.rf = upgrade;
						shot.gun.rate /= 2;
					}
				}
				renderer.material.SetTexture("_DecalTex", null);
			}
		}
	}
	
	public void Effects(PG_Shot shot, float distance)
	{//Debug.Log ("start effects: " + shot.gun.ToString());
		if(shot!=null && shot.gun != null && distance!=null){
			//Debug.Log ("next effects: ");
			float effect = shot.gun.power - distance;
			
			Texture texture = shot.renderer.sharedMaterial.mainTexture;
			Debug.Log ("current texture: " + texture.ToString());
			if (texture == blue)
			{
				amountRed = Mathf.Max(0, amountRed - effect);
				amountBlue = Mathf.Min(maxColor, amountBlue + effect);
				
				if (amountBlue > resistence)
				{
					if (Network.isClient || Network.isServer)
					{
						this.networkView.RPC("UpdateCubeMaterial", RPCMode.AllBuffered, "blue",shot.getShotOwnerID());
					}
					else // remove when all shall be networked (in the final game)
					{
						//renderer.material = blue;
						renderer.material.SetTexture("_MainTex", blue);
					}
				}
			}
			else if (texture == red)
			{
				amountBlue = Mathf.Max(0, amountBlue - effect);
				amountRed = Mathf.Min(maxColor, amountRed + effect);
				
				if (amountRed > resistence)
				{
					if (Network.isClient || Network.isServer)
					{
						this.networkView.RPC("UpdateCubeMaterial", RPCMode.AllBuffered, "red",shot.getShotOwnerID());
					}
					else // remove when all shall be networked (in the final game)
					{
						//renderer.material = red;
						renderer.material.SetTexture("_MainTex", red);
					}
				}
			}
		}
	}
	
	[RPC]
	public void UpdateCubeMaterial(string newMaterial,string shotOwnerID){
	
		GameObject mainGame = GameObject.Find ("GameManager");
		Debug.Log ("updating cube");
		if (newMaterial == "blue")
		{	//update score based on server only
			if(Network.isServer){
				if(renderer.material.GetTexture("_MainTex")==red){//change from red
					mainGame.networkView.RPC ("blueScore",RPCMode.AllBuffered,1);
					mainGame.networkView.RPC ("redScore",RPCMode.AllBuffered,-1);
					//update player total claims and reduce previous owner claim
					mainGame.networkView.RPC("updatePlayersScore", RPCMode.AllBuffered,shotOwnerID,cubeOwnerID);
					//update cube owner
					cubeOwnerID = shotOwnerID;
				} else if(renderer.material.GetTexture("_MainTex")==blue){//no change
				} else {//initial claim
					mainGame.networkView.RPC ("blueScore",RPCMode.AllBuffered,1);
					//update player total claims
					mainGame.networkView.RPC("updatePlayersScore", RPCMode.AllBuffered,shotOwnerID,cubeOwnerID);
					cubeOwnerID = shotOwnerID;
					Debug.Log ("initial change: " );
				}
			}
			//renderer.material = blue;
			renderer.material.SetTexture("_MainTex", blue);
		}
		else if (newMaterial == "red")
		{	//update score based on server only
			if(Network.isServer){
				Debug.Log ("start change: " );
				if(renderer.material.GetTexture("_MainTex")==blue){//change from blue
					mainGame.networkView.RPC ("redScore",RPCMode.AllBuffered,1);
					mainGame.networkView.RPC ("blueScore",RPCMode.AllBuffered,-1);
					//update player total claims and reduce previous owner claim
					mainGame.networkView.RPC("updatePlayersScore", RPCMode.AllBuffered,shotOwnerID,cubeOwnerID);
					//update cube owner
					cubeOwnerID = shotOwnerID;
				} else if(renderer.material.GetTexture("_MainTex")==red){//no change
				} else {//initial claim
					mainGame.networkView.RPC ("redScore",RPCMode.AllBuffered,1);
					//update player total claims
					mainGame.networkView.RPC("updatePlayersScore", RPCMode.AllBuffered,shotOwnerID,cubeOwnerID);
					cubeOwnerID = shotOwnerID;
					Debug.Log ("initial change: " );
				}
			}
			//renderer.material = red;
			renderer.material.SetTexture("_MainTex", red);
		}
			
	}
}
