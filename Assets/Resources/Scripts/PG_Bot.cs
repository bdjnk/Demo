using UnityEngine;
using System.Collections;

public class PG_Bot : MonoBehaviour
{
	public float fov = 60.0f;
	private RaycastHit hit;
	private int speed = 3;
	
	public PG_Gun gun;
	public Color myColor;
	
	private GameData gameData;
	
	private bool ready = false;
	
	private void Start()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
	}
	
	public void SetColor(string color)
	{
		GetComponentInChildren<PG_Gun>().shotPrefab = Resources.Load("Prefabs/"+color+"Shot") as GameObject;
		
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
				float closest = Vector3.Distance(player.transform.position, transform.position);
				
				if (closest < distance)
				{
					distance = closest;
					target = player.transform;
				}
			}
		}
		
		if (target != null && target.GetComponentInChildren<PG_Gun>().eb == null)
		{
			float angle = Vector3.Angle(target.position - transform.position, transform.forward);
			
			if (angle > -2 && angle < 2)
			{
				gun.Shoot();
			}
			if (LineOfSight(target))
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.position - transform.position), speed);
			}
			else
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(gameData.mapCenter - transform.position), speed);
			}
		}
		else
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(gameData.mapCenter - transform.position), speed);
		}
		
		if (transform.position.x <= -5 && transform.position.z <= gameData.extent.z+5) //(0,f,0->f)
		{
			transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * speed, Space.World);
		}
		if (transform.position.x <= gameData.extent.x+5 && transform.position.z >= gameData.extent.z+5) //(0->f,f,f)
		{
			transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * speed, Space.World);
		}
		if (transform.position.x >= gameData.extent.x+5 && transform.position.z >= -5) //(f,f,f->0)
		{
			transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * speed, Space.World);
		}
		if (transform.position.x >= -5 && transform.position.z <= -5) //(f->0,f,0)
		{
			transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * speed, Space.World);
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
