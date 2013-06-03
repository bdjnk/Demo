using UnityEngine;
using System.Collections;

public class PaintCubeScript : MonoBehaviour {
	
	SpotLight mLight; 
	
	// Use this for initialization
	void Start () {
			mLight = GameObject.Find("Camera").GetComponent<SpotLight>();
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
		startStory();
	}
	
	public void startCreatedBy(){
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/CreatedBy") as Texture);
		renderer.material.SetTextureScale("_DecalTex", new Vector2(1.0f,0.2f));
		renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,-0.05f));
		StartCoroutine("setTextureOffset");
	}
	
	IEnumerator setTextureOffset(){
		for (int i=0;i<103;i++){
			renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,0.01f*(i-20)));
			yield return new WaitForSeconds(0.15f);
		}	

		yield return new WaitForSeconds(4.0f);

		startResetPaint();

	}
	
	public void startStory(){
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/Story4") as Texture);
		renderer.material.SetTextureScale("_DecalTex", new Vector2(1.0f,0.2f));
		renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,-0.05f));
		StartCoroutine("setTextureOffset2");
	}

	public IEnumerator setTextureOffset2(){
		for (int i=0;i<88;i++){
			renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,0.01f*(i-5)));
			yield return new WaitForSeconds(0.2f);
		}

		startCreatedBy();
	}
	
	public void startResetPaint(){
		StartCoroutine("lightReset");
	}
	
	public IEnumerator lightReset(){
		mLight.startDimLight();
		yield return new WaitForSeconds(4.0f);
		mLight.Reset();
		yield return new WaitForSeconds(4.0f);
		resetPaint();
	}
	
	public void resetPaint(){
		renderer.material.SetTexture("_DecalTex", Resources.Load("Textures/PaintTheTown1") as Texture);
		renderer.material.SetTextureScale("_DecalTex", new Vector2(1.0f,1.0f));
		renderer.material.SetTextureOffset("_DecalTex",new Vector2(0f,0f));
	}
		
}
