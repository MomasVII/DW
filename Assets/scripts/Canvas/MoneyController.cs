using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyController : MonoBehaviour {

	private int storePlayerCash;
	private int incrementalMoney = 0;
	private int nextUpdate = 1;

	public TMP_Text playerCash;

	bool isNet = true;

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
		if (((Application.internetReachability == NetworkReachability.NotReachable) ||
			(Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork &&
			Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)) &&
			PlayerPrefs.GetString("NoAds") != "NoAds") {
			isNet = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if(Time.time >= nextUpdate && isNet){
			// Change the next update (current second+1)
			nextUpdate=Mathf.FloorToInt(Time.time)+1;
			storePlayerCash += 5;
			incrementalMoney += 5;
			if(incrementalMoney < 2000) {
				updatePlayerMoney();
				saveMoney();
			}
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
