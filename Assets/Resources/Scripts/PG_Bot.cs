using UnityEngine;
using System.Collections;

public class PG_Bot : MonoBehaviour
{
	public float fov = 60.0f;
	private RaycastHit hit;
	
	public PG_Gun gun;
	
	private GameObject[] enemies;
	//private Transform enemy;
	
	private bool ready = false;
	
	private void Start()
	{
		//enemy = GameObject.Find("First Person Controller").transform;
		// make an array of transforms, and use GameObject.FindGameObjectsWithTag("Color");
	}
	
	public void Setup(string myColor)
	{
		GetComponentInChildren<PG_Gun>().shot = Resources.Load("Prefabs/"+myColor+"Shot") as GameObject;
		
		ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
		Color color = myColor == "Blue" ? Color.blue : Color.red;
		ps.startColor = color;
		ps.light.color = color;
		
		enemies = GameObject.FindGameObjectsWithTag(myColor == "Blue" ? "Red" : "Blue");
		
		ready = true;
	}
	
	private void Update()
	{
		if (!ready) { return; }
		
		int index = 0;
		float distance = 999;
		
		for (int i = 0; i < enemies.Length; i++)
		{
			float tempd = Vector3.Distance(enemies[i].transform.position, transform.position);
			
			if (tempd < distance)
			{
				distance = tempd;
				index = i;
			}
		}
		if (distance < 30 && LineOfSight(enemies[index].transform))
		{
			transform.forward = enemies[index].transform.position - transform.position; // causes sudden jumps
			gun.Shoot();
		}
		/*
		if (Vector3.Distance(enemy.position, transform.position) < 30 && LineOfSight(enemy))
		{
			transform.forward = enemy.position - transform.position; // causes sudden jumps
			gun.Shoot();
		}
		*/
	}
	
	private bool LineOfSight(Transform target)
	{
		if (Vector3.Angle(target.position - transform.position, transform.forward) <= fov
			&& Physics.Linecast(transform.position, target.position, out hit)
			&& hit.collider.transform == target)
		{
			return true;
		}
		return false;
	}
}
