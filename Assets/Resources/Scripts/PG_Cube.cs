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
	
	public int resistence = 4;
	public int maxColor = 5;
	private float amountBlue;
	private float amountRed;
	private string cubeOwnerID;
	
	//private PG_Building building;
	
	private void Start()
	{
		//building = transform.parent.GetComponent<PG_Building>();
		cubeOwnerID="";
	}
	
	public void SetUpgrade(Texture upgrade) // sets an upgrade on this cube
	{
		networkView.RPC("SetDecal", RPCMode.AllBuffered, upgrade.name);
	}
	
	[RPC]
	private void SetDecal(string upgrade)
	{
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/"+upgrade) as Texture);
	}
	
	public void Struck(PG_Shot shot)
	{		
		if(shot != null)
		{
			foreach (Transform child in transform.parent) // splash effect
			{
				float distance = Vector3.Distance(transform.position, child.position);
				// testing for minimum reaction on adjacent
				if (distance < 2.9f) // only consider adjacent cubes
				{
					PG_Cube cubeScript = child.GetComponent<PG_Cube>();
					
					if (cubeScript != null && shot != null) // this is a cube
					{
						cubeScript.Effects(shot, distance);
					}
				}
			}
			
			Texture upgrade = renderer.material.GetTexture("_DecalTex");
			
			if (upgrade != null)
			{
				shot.gun.Upgrade(upgrade);
				networkView.RPC("SetDecal", RPCMode.AllBuffered, "");
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
				
				if (amountBlue > resistence)
				{
					// do necessary scoring here
					
					networkView.RPC("SetBlue", RPCMode.AllBuffered);
				}
			}
			else if (texture == red)
			{
				amountBlue = Mathf.Max(0, amountBlue - effect);
				amountRed = Mathf.Min(maxColor, amountRed + effect);
				
				if (amountRed > resistence)
				{
					// do necessary scoring here
					
					networkView.RPC("SetRed", RPCMode.AllBuffered);
				}
			}
		}
	}
	
	[RPC]
	private void SetRed()
	{
		renderer.material.SetTexture("_MainTex", red);
	}
	
	[RPC]
	private void SetBlue()
	{
		renderer.material.SetTexture("_MainTex", blue);
	}
}
