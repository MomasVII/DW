using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFollow : MonoBehaviour {

	GameObject player;

	void Start () {
		player = GameObject.FindWithTag("Player");
	}

	void FixedUpdate () {
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y+10, player.transform.position.z);
	}
}
