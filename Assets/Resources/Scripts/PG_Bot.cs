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
	}
	
	public void Setup(string myColor)
	{
		GetComponentInChildren<PG_Gun>().shotPrefab = Resources.Load("Prefabs/"+myColor+"Shot") as GameObject;
		
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
		Transform target = enemies[index].transform;
		
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
