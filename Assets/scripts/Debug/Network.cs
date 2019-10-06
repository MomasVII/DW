using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Network : MonoBehaviour {

	public GameObject networkIndicator;
	private bool continueCoroutine = true;

	void Start () {
		StartCoroutine(checkNetwork());
	}

	IEnumerator checkNetwork() {
        while(continueCoroutine) {
			if((Application.internetReachability != NetworkReachability.NotReachable) ||
	            (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
	            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)) {
					networkIndicator.SetActive(true);
			} else {
				networkIndicator.SetActive(false);
			}
        	yield return new WaitForSeconds(1f);
        }
    }
}
