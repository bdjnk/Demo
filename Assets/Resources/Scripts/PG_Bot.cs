using UnityEngine;
using System.Collections;

public class PG_Bot : MonoBehaviour
{
	public float fov = 60.0f;
	private RaycastHit hit;
	
	public PG_Gun gun;
	
	private Transform enemy;
	
	private void Start()
	{
		enemy = GameObject.Find("First Person Controller").transform;
		// make an array of transforms, and use GameObject.FindGameObjectsWithTag("blue");
	}
	
	private void Update()
	{
		if (Vector3.Distance(enemy.position, transform.position) < 30 && LineOfSight(enemy))
		{
			transform.forward = enemy.position - transform.position; // causes sudden jumps
			gun.Shoot();
		}
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
