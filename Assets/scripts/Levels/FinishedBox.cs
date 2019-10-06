using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedBox : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		//Make sure ghost doesn't trigger this
		if(other.gameObject.tag == "Player") {
			GameObject.Find("Brain").SendMessage("Finished");
		}
	}

}
