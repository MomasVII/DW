using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyController : MonoBehaviour {

	private int storePlayerCash;
	private int incrementalMoney = 0;

	public TMP_Text playerCash;

	private bool isNet = true;

	// Use this for initialization
	void Awake () {
		if(!PlayerPrefs.HasKey("playerCash")) {
			PlayerPrefs.SetInt("playerCash", 20000);
		}
		storePlayerCash = PlayerPrefs.GetInt("playerCash", 0);
		incrementalMoney = 0;
		updatePlayerMoney();
	}

	void Start() {
		//Don't incremenet money if there is no internet
		if ((Application.internetReachability == NetworkReachability.NotReachable) ||
			(Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork &&
			Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)) {
			isNet = false;
		} else {
			 StartCoroutine("payMoney");
		}
	}

	IEnumerator payMoney() {
		WaitForSeconds delay = new WaitForSeconds(1f);
	     for(;;) {
			 storePlayerCash += 5000;
 			incrementalMoney += 5000;
 			if(incrementalMoney < 200000) {
 				updatePlayerMoney();
 				saveMoney();
 			}
	         // execute block of code here
	         yield return delay;
	     }
	}

	public void updateDriftMoney(int driftAmount) {
		storePlayerCash += driftAmount;
	}

	public void saveMoney() {
		PlayerPrefs.SetInt("playerCash", storePlayerCash);
	}

	public void updatePlayerMoney() {
		playerCash.text = "$"+storePlayerCash.ToString("n0");
	}

}
