using UnityEngine;
using System.Collections;

public class PG_Gun : MonoBehaviour {
	
	public GameObject shot;
	public float speed = 15f; // speed of shot
	public float rate = 0.2f; // rate of fire, portion of a second before firing again
	public float power = 3f;
	private float delay = 0;
	
	public Texture2D crosshairImage;
	public Texture bs, eb, fs, qm, rf;
	
	void OnGUI() // replace with GUITextures (much faster)
	{
		if (tag != "Bot") // human player
		{
			float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
			float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
			
			if (bs != null)
				GUI.DrawTexture(new Rect(0, 0, 40, 40), bs);
			if (rf != null)
				GUI.DrawTexture(new Rect(40, 0, 40, 40), rf);
			if (fs != null)
				GUI.DrawTexture(new Rect(80, 0, 40, 40), fs);
			if (qm != null)
				GUI.DrawTexture(new Rect(120, 0, 40, 40), qm);
			if (eb != null)
				GUI.DrawTexture(new Rect(160, 0, 40, 40), eb);
		}
	}
	
	void Start ()
	{
		Screen.showCursor = false;
	}
	
	void Update ()
	{
		if (tag != "Bot") // human player
		{
			if (Input.GetButton("Fire1"))
			{
				Screen.showCursor = false;
				Shoot();
			}
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Network.Disconnect();
				Application.LoadLevel("PrototypeMenu");
			}
		}
	}
	
	public void Shoot()
	{
		if (Time.time > delay)
		{
			delay = Time.time + rate;
			Vector3 pos = transform.position + transform.forward * transform.localScale.z * 1f;
			//GameObject clone = Instantiate(shot, pos, transform.rotation) as GameObject;
			//should change to separate group?
			GameObject clone;
			
			
			if (Network.connections.Length > 0)
			//if (Network.isClient || Network.isServer)
			{	//using group 10 for shots
				
				clone = Network.Instantiate(shot, pos, transform.rotation,10) as GameObject;
			}
			else
			{
				clone = Instantiate(shot, pos, transform.rotation) as GameObject;
			}
			clone.rigidbody.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
			clone.GetComponent<PG_Shot>().gun = this;
		}
	}
}
