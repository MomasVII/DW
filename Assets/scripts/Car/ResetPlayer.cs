using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayer : MonoBehaviour {

	private Vector3 playerStartPos;
	private float y;
	private Vector3 v3;

	private GameObject car; //Use Car position for resetting
	private GameObject demoCar; //Use for initial position
	public Timer timerScript;

	//Refill Nos on restart
	public Nos nos;

	// Use this for initialization
	void Start () {
		if(GameObject.FindGameObjectsWithTag("Demo").Length == 0) {
			demoCar = GameObject.FindWithTag("Player");
		} else {
			demoCar = GameObject.FindWithTag("Demo");
		}

		car = GameObject.FindWithTag("Player");

		//Get rotation the player starts at
		y = car.transform.eulerAngles.y;

		playerStartPos = demoCar.transform.position; //Set start pos to variables above
	}

	void OnTriggerEnter(Collider other)
    {
		//If player touches reset boundary
		if(other.gameObject.tag == "Player") {
			resetPlayer();
	    }
    }

	/*public void ChangeResetPosition(string resetString) {
		if(resetString == "grass") {
			playerStartPos = new Vector3(293, 126, 208);
			y = -206f;
		} else if(resetString == "snow") {
			playerStartPos = new Vector3(270, 126, 208);
			y = 100f;
		} else if(resetString == "lava") {
			playerStartPos = new Vector3(270, 126, 208);
			y = 100f;
		} else {
			playerStartPos = new Vector3(270, 126, 208);
			y = 100f;
		}
	}*/

	//Set player location to the start position
    public void resetPlayer() {
		if(timerScript != null) {
			timerScript.resetTimer();
		}
		if(nos != null) {
			nos.refill();
		}
		GameObject resetMe = car;
		resetMe.GetComponent<Rigidbody>().velocity = Vector3.zero;
		resetMe.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		resetMe.transform.eulerAngles = new Vector3(0, y, 0);
		resetMe.transform.position = playerStartPos;
    }

	public void uprightPlayer() {
		GameObject resetMe = car;
		Debug.Log("test");
		resetMe.GetComponent<Rigidbody>().velocity = Vector3.zero;
		resetMe.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

		resetMe.transform.eulerAngles = new Vector3(0, resetMe.transform.eulerAngles.y, 0);
		resetMe.transform.position = new Vector3(resetMe.transform.position.x, resetMe.transform.position.y+5, resetMe.transform.position.z);
	}
}
