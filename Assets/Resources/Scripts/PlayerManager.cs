using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	private GameObject master;
	//private GameData gameData;
	
	public bool ready = false;
		
	[RPC] private void AreWeThereYet(NetworkPlayer netPlayer) { networkView.RPC("GoGoGo", netPlayer); }
	[RPC] private void GoGoGo() { ready = true; }
	
	private void Awake()
	{
		master = GameObject.FindGameObjectWithTag("Master");
		//gameData = master.GetComponent<GameData>();
	}
	
	private void Start()
	{
	}
	
	public void Enable(bool state)
	{
		Screen.showCursor = !state;
		
		foreach (MouseLook mouseLook in GetComponentsInChildren<MouseLook>())
		{
			mouseLook.enabled = state;
		}
		GetComponent<CharacterMotor>().enabled = state;
		GetComponent<FPSInputController>().enabled = state;
		
		GetComponentInChildren<Camera>().enabled = state;
		GetComponentInChildren<PG_Gun>().enabled = state;
		
		enabled = state;
		
		master.GetComponent<UpgradeManager>().enabled = true;
	}
	
	public void JoinTeam()
	{
		Texture color = master.GetComponent<GameData>().GetTeam();
		networkView.RPC("SetColor", RPCMode.AllBuffered, color.name);
	}
	
	[RPC]
	private void SetColor(string color)
	{
		GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", Resources.Load("Textures/"+color) as Texture);
		GetComponentInChildren<PG_Gun>().shot = Resources.Load("Prefabs/"+color+"Shot") as GameObject;
		tag = color;
	}
	
	private void Update(){}
}