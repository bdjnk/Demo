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
	private float amountBlue;
	private float amountRed;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
	}
	
	[RPC] private void SetDecal(string upgrade)
	{
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/"+upgrade) as Texture);
	}
	
	public void Struck(PG_Shot shot)
	{		
		if (shot != null && shot.gun.networkView.isMine)
		{
			foreach (Transform child in transform.parent) // splash effect
			{
				float distance = Vector3.Distance(transform.position, child.position);
				
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
				
				if (amountBlue > resistence && renderer.material.color != gameData.blue)
				{
					// do necessary scoring here
					networkView.RPC("SetBlue", RPCMode.AllBuffered);
				}
			}
			else if (texture == red)
			{
				amountBlue = Mathf.Max(0, amountBlue - effect);
				amountRed = Mathf.Min(maxColor, amountRed + effect);
				
				if (amountRed > resistence && renderer.material.color != gameData.red)
				{
					// do necessary scoring here
					networkView.RPC("SetRed", RPCMode.AllBuffered);
				}
			}
		}
	}
	
	[RPC] private void SetRed()
	{
		if (renderer.material.color == gameData.blue)
		{
			gameData.blueScore--;
		}
		//renderer.material.SetTexture("_MainTex", red);
		renderer.material.color = new Color(1, 0.4f, 0.4f);
		gameData.redScore++;
	}
	
	[RPC] private void SetBlue()
	{
		if (renderer.material.color == gameData.red)
		{
			gameData.redScore--;
		}
		//renderer.material.SetTexture("_MainTex", blue);
		renderer.material.color = new Color(0.4f, 0.6f, 1);
		gameData.blueScore++;
	}
}
