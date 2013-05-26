using UnityEngine;
using System.Collections;

public class EffectBehavior : MonoBehaviour {
	
	public float persist = 6f;
	private float timeAtStart = 0f;
	// Use this for initialization
	void Start () {
		timeAtStart = (float) Network.time;
		if(networkView==null){
			Destroy (this.gameObject,persist);
		} 
	}
	
	// Update is called once per frame
	void Update () {
		if(networkView!=null && (float) Network.time > timeAtStart + persist){
			Network.Destroy(NetworkView.Find (networkView.viewID).gameObject);
		}
		
	}
}
