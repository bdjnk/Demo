using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tracker : MonoBehaviour
{
	private GameData gameData;
	
	private void Awake()
	{
		gameData = GameObject.FindGameObjectWithTag("Master").GetComponent<GameData>();
		gameData.players.Add(gameObject);
	}
}
