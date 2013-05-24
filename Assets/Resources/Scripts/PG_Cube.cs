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
	private PlayerManager captor;
	
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
		latest = shot;
	
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
		
		Texture upgrade = renderer.material.GetTexture("_DecalTex");
		
		if (upgrade != null)
		{
			shot.gun.Upgrade(upgrade);
			SetDecal("");
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
			SetBlue();
			InformCaptor(shot);
		}
		else if (amountRed > resistence && renderer.material.color != gameData.red)
		{
			SetRed();
			InformCaptor(shot);
		}
	}
	
	private void InformCaptor(PG_Shot shot)
	{
		if (captor != null)
			captor.myScore--; // do we have a former captor?
		if (shot != null)
			captor = shot.gun.transform.parent.GetComponent<PlayerManager>();
		if (captor != null) 
			captor.myScore++; // give the player their due
	}
	
	//TODO URGENT ensure game score data is persisting properly
	// scoring is only broken when the quit bug occurs, otherwise it's fine
	
	[RPC] private void SetRed()
	{	
		if (renderer.material.color == gameData.blue)
		{
			gameData.blueScore--;
		}
		renderer.material.color = gameData.red;
		gameData.redScore++;
	}
	
	
	[RPC] private void SetBlue()
	{	
		if (renderer.material.color == gameData.red)
		{
			gameData.redScore--;
		}
		renderer.material.color = gameData.blue;
		gameData.blueScore++;
	}
	
	[RPC] public void SetGray()
	{
		if (captor != null)
		{
			captor.myScore--;
			captor = null;
		}
		amountRed = amountBlue = 0;
		renderer.material.color = gameData.gray;
		SetDecal("");
	}
}
