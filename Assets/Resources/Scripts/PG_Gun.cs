using UnityEngine;
using System.Collections;

public class PG_Gun : MonoBehaviour
{
	public GameObject shot;
	public float speed = 15f; // speed of shot
	public float rate = 0.2f; // rate of fire, portion of a second before firing again
	public float power = 3f;
	private float delay = 0;
	
	public Texture2D crosshairImage;
	public Texture bs, eb, fs, qm, rf;
	public float bsd, ebd, fsd, qmd, rfd;
	
	private void OnGUI() // replace with GUITextures (much faster)
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
	
	private void Start()
	{
		Screen.showCursor = false;
	}
	
	private void Update()
	{
		if (tag != "Bot") // human player
		{
			if (Input.GetButton("Fire1"))
			{
				Screen.showCursor = false;
				Shoot();
			}
		}
		Downgrade();
	}
	
	public void Shoot()
	{
		if (Network.time > delay)
		{
			delay = (float)Network.time + rate;
			Vector3 pos = transform.position + transform.forward * transform.localScale.z * 1f;
			GameObject clone  = Network.Instantiate(shot, pos, transform.rotation, 10) as GameObject;
			networkView.RPC("InitializeShot", RPCMode.All, clone.networkView.viewID); // ???
		}
	}
	
	[RPC]
	private void InitializeShot(NetworkViewID id)
	{
		NetworkView netView = NetworkView.Find(id);
		if (netView != null)
		{
			GameObject clone = netView.gameObject;
			if (clone != null) // clone shouldn't be null, but sometimes it is...
			{
				clone.rigidbody.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
				clone.GetComponent<PG_Shot>().gun = this;
			}
		}
	}
	
	private void Downgrade()
	{
		if (bs != null && bsd - Time.time < 0)
		{
			bs = null;
			power -= 2;
		}
		if (rf != null && rfd - Time.time < 0)
		{
			rf = null;
			rate *= 2;
		}
		if (fs != null && fsd - Time.time < 0)
		{
			fs = null;
			speed /= 2;
		}
		if (qm != null && qmd - Time.time < 0)
		{
			qm = null;
						
			CharacterMotor cm = transform.parent.GetComponent<CharacterMotor>();
			cm.jumping.baseHeight = 1;
			cm.movement.maxForwardSpeed /= 2;
			cm.movement.maxSidewaysSpeed /= 2;
			cm.movement.maxBackwardsSpeed /= 2;
			cm.movement.maxGroundAcceleration /= 3;
		}
		if (eb != null && ebd - Time.time < 0)
		{
			eb = null;
		}
	}
	
	public void Upgrade(Texture upgrade) // all upgrade numbers should be variables
	{
		if (upgrade.name == "BlastShots")
		{
			if (bs == null) {
				bs = upgrade;
				bsd = Time.time + 9.0f;
				
				power += 2;
			}
		}
		else if (upgrade.name == "RapidFire")
		{
			if (rf == null) {
				rf = upgrade;
				rfd = Time.time + 9.0f;
				
				rate /= 2;
			}
		}
		else if (upgrade.name == "FastShots")
		{
			if (fs == null) {
				fs = upgrade;
				fsd = Time.time + 9.0f;
				
				speed *= 2;
			}
		}
		else if (upgrade.name == "MoveQuick")
		{
			if (qm == null) {
				qm = upgrade;
				qmd = Time.time + 9.0f;
				
				CharacterMotor cm = transform.parent.GetComponent<CharacterMotor>();
				cm.jumping.baseHeight = 4;
				cm.movement.maxForwardSpeed *= 2;
				cm.movement.maxSidewaysSpeed *= 2;
				cm.movement.maxBackwardsSpeed *= 2;
				cm.movement.maxGroundAcceleration *= 3;
			}
		}
		else if (upgrade.name == "EvadeBots")
		{
			if (eb == null) {
				eb = upgrade;
				ebd = Time.time + 9.0f;
				
				// do nothing else, for now
			}
		}
	}
}
