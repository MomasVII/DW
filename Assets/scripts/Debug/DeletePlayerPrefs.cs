using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePlayerPrefs : MonoBehaviour {

	public Text errorLogText;

	// Update is called once per frame
	public void DeleteAPlayerPref (string playerPrefKey) {
		PlayerPrefs.DeleteKey(playerPrefKey);
		if(PlayerPrefs.HasKey(playerPrefKey)) {
		    errorLogText.text += "\nError Deleting PlayerPref...";
        } else {
			errorLogText.text += "\nDeleted "+playerPrefKey;
        }

	}
}
