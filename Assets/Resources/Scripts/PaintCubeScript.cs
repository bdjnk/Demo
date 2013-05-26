using UnityEngine;
using System.Collections;

public class PaintCubeScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	bool updatedTexture = false;
	
	
	void OnTriggerEnter(Collider other)
	{
		StartCoroutine("setNextTexture",false);
		other.transform.rigidbody.velocity= new Vector3(0f,0f,0f);
		other.transform.localPosition = new Vector3(0f,0f,0f);
		other.transform.localScale = new Vector3(0f,0f,0f);

	}
	
	IEnumerator setNextTexture(bool reduce){
		//int iLocal = iStart;
		
		if(!reduce){
			for (int i=1;i<=7;i++){
				renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/PaintTheTown"+i) as Texture);
				yield return new WaitForSeconds(0.25f);
			}
		} 
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/CreatedBy") as Texture);
		renderer.material.SetTextureScale("_DecalTex", new Vector2(1.0f,0.2f));
		renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,-0.05f));
		StartCoroutine("setTextureOffset");
	}
	
	IEnumerator setTextureOffset(){
		for (int i=0;i<88;i++){
			renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,0.01f*(i-5)));
			yield return new WaitForSeconds(0.2f);
		}	
		SpotLight mLight = GameObject.Find("Camera").GetComponent<SpotLight>();
		mLight.startDimLight();
		yield return new WaitForSeconds(3.0f);
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/PaintTheTown1") as Texture);
		renderer.material.SetTextureScale("_DecalTex", new Vector2(1.0f,1.0f));
		renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,0f));
		mLight.Reset();
	}

}
