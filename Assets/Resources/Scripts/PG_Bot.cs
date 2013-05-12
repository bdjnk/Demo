using UnityEngine;
using System.Collections;

public class PG_Bot : MonoBehaviour
{
	public float fov = 60.0f;
	private RaycastHit hit;
	
	/*
	public GameObject shot;
	public float speed = 20f; // speed of shot
	public float rate = 0.5f; // rate of fire, portion of a second before firing again
	public float power = 3f;
	private float delay = 0;
	*/
	public PG_Gun gun;
	
	private Transform enemy;
	
	void Start ()
	{
		enemy = GameObject.Find("First Person Controller").transform;
		// make an array of transforms, and use GameObject.FindGameObjectsWithTag("blue");
	}
	
	void Update ()
	{
		transform.forward = enemy.position - transform.position;
		if (Vector3.Distance(enemy.position, transform.position) < 30 && LineOfSight(enemy))
		{
			/*
			delay = Time.time + rate;
			Vector3 pos = transform.position + transform.forward * transform.localScale.z;// * 1f;
			GameObject clone = Instantiate(shot, pos, transform.rotation) as GameObject;
			clone.rigidbody.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
			//clone.GetComponent<PG_Shot>().gun = this;
			*/
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
