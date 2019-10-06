using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFollow : MonoBehaviour {

	GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
	}

	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y+10, player.transform.position.z);
	}
}
