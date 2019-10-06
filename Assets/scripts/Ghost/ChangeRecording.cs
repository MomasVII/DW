using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRecording : MonoBehaviour {

	public Sprite personalUnselected;
	public Sprite personalSelected;
	public Sprite ghostUnselected;
	public Sprite ghostSelected;

	Image personalImageComponent;
	Image ghostImageComponent;

	string recordingPref;

	void Start()
	{
		personalImageComponent = GameObject.FindWithTag("Personal").GetComponent<Image>();
		ghostImageComponent = GameObject.FindWithTag("Ghost").GetComponent<Image>();

		recordingPref = PlayerPrefs.GetString("personalOrGhost");

		personalImageComponent.sprite = personalUnselected;
		ghostImageComponent.sprite = ghostUnselected;

		//Disable ghost recording if can't connet to network
		if((Application.internetReachability == NetworkReachability.NotReachable) ||
			(Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork &&
			Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork))
        {
			PlayerPrefs.SetString("personalOrGhost", "personal");
		}

		if(recordingPref == "personal") {
			personalImageComponent.sprite = personalSelected;
		} else if(recordingPref == "ghost") {
			ghostImageComponent.sprite = ghostSelected;
		}
	}

	public void changeRecordingType(string recordingType) {
		recordingPref = PlayerPrefs.GetString("personalOrGhost");

		if(recordingType == "personal") {
			if(recordingPref == "personal") {
				PlayerPrefs.SetString("personalOrGhost", "none");
				personalImageComponent.sprite = personalUnselected;
				ghostImageComponent.sprite = ghostUnselected;
			} else if(recordingPref == "ghost" || recordingPref == "none") {
				PlayerPrefs.SetString("personalOrGhost", "personal");
				personalImageComponent.sprite = personalSelected;
				ghostImageComponent.sprite = ghostUnselected;
			}
		} else if(recordingType == "ghost" && Application.internetReachability != NetworkReachability.NotReachable) {
			if(recordingPref == "ghost") {
				PlayerPrefs.SetString("personalOrGhost", "none");
				ghostImageComponent.sprite = ghostUnselected;
				personalImageComponent.sprite = personalUnselected;
			} else if(recordingPref == "personal" || recordingPref == "none") {
				PlayerPrefs.SetString("personalOrGhost", "ghost");
				ghostImageComponent.sprite = ghostSelected;
				personalImageComponent.sprite = personalUnselected;
			}
		}

	}



}
