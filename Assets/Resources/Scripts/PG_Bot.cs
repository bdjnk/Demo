using UnityEngine;
using System.Collections;

public class PG_Bot : MonoBehaviour
{
	public float fov = 60.0f;
	private RaycastHit hit;
	
	public PG_Gun gun;
	private Color myColor;
	
	private GameData gameData;
	
	private bool ready = false;
	
	private void Start()
	{
	}
	
	public void Setup(string color)
	{
		GetComponentInChildren<PG_Gun>().shotPrefab = Resources.Load("Prefabs/"+color+"Shot") as GameObject;
		
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
		
		ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
		myColor = color == "Blue" ? gameData.blue : gameData.red;
		ps.startColor = ps.light.color = color == "Blue" ? Color.blue : Color.red;
		
		ready = true;
	}
	
	private void Update()
	{
		if (!ready) { return; }
		
		Transform target = null;
		float distance = 999;
		
		foreach (GameObject player in gameData.players)
		{
			if (player.GetComponent<PlayerManager>().myColor != myColor)
			{
				float tempd = Vector3.Distance(player.transform.position, transform.position);
				
				if (tempd < distance)
				{
					distance = tempd;
					target = player.transform;
				}
			}
		}
		
		if (target != null)
		{
			if (Mathf.Approximately(Vector3.Angle(target.position - transform.position, transform.forward), 0))
			{
				gun.Shoot(); // circumnavigates a strange hit comparison delay bug.
			}
			else if (LineOfSight(target))
			{
				transform.forward = target.position - transform.position; // causes sudden jumps
				gun.Shoot();
			}
		}
	}
	
	private bool LineOfSight(Transform target)
	{
		if (Vector3.Angle(target.position - transform.position, transform.forward) <= fov
			&& Physics.Raycast(new Ray(transform.position, (target.position - transform.position).normalized), out hit, 25)
			&& hit.transform == target)
		{
			return true;
		}
		return false;
	}
}
