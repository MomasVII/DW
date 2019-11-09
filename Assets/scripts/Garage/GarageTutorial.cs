using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarageTutorial : MonoBehaviour {

	public GameObject exitButton, topMoney, noAds, topGold, settings;

	public GameObject bank, carInfo, adButton;

	public GameObject carStats, carColors, keys, nextButton, prevButton, nextSpeechButton;

	//Speech Panel
	public GameObject speechPanel, inputField;
	public TMP_Text mainInputField, errorText;

	//Speech variables
	private float delay = 0.05f; //0.04
	private string currentText = "";
	public TMP_Text characterSpeech;

	private bool typing = false, startTyping = true;
	private int speechIndex = 1;

	//Character Animation
	public GameObject character;
	private Animator anim;

	private string scriptUsername;

	//Get Garage Script to change car
	private ChangeCar changecar;

	bool startTutorial = false;

	public void StartTutorial () {
		PlayerPrefs.SetString("NoAds", "NoAds");

		changecar = transform.GetComponent<ChangeCar>();
		anim = character.GetComponent<Animator>();
		speechPanel.SetActive(true);

		exitButton.SetActive(false);
		topMoney.SetActive(false);
		settings.SetActive(false);
		noAds.SetActive(false);
		topGold.SetActive(false);

		bank.SetActive(false);
		carInfo.SetActive(false);
		adButton.SetActive(false);

		carStats.SetActive(false);
		carColors.SetActive(false);

		nextButton.SetActive(false);
		prevButton.SetActive(false);
		keys.SetActive(false);

		startTutorial = true;
	}

	// Update is called once per frame
	void Update () {

		if(startTutorial) { //Check if playerpref car has been created
			if(!typing && speechIndex == 1 && startTyping) {
				anim.Play("Waving", 0, 0);
				startTyping = false;
				typing = true;
				StartCoroutine(ShowText("Hey, welcome to my garage. My name is Kate and you must be?... "));
				nextSpeechButton.SetActive(true);
				inputField.SetActive(true);
		        PlayerPrefs.SetString("metrics", "kph");
			} else if(!typing && speechIndex == 2 && startTyping) {
				StartCoroutine(saveUsername());
				startTyping = false;
				typing = true;
				errorText.text = "";
				StartCoroutine(ShowText("So you're the driver I've heard so much about. I'm glad you've decided to join our team."));
				inputField.SetActive(false);
			} else if(!typing && speechIndex == 3 && startTyping) {
				startTyping = false;
				typing = true;
				StartCoroutine(ShowText("So what's the job? I just need you to race. That's it. There are dozens of tracks around here with plenty of prize money to be won. If you keep setting track records I'll keep paying you. As simple as that."));
			} else if(!typing && speechIndex == 4 && startTyping) {
				startTyping = false;
				typing = true;
				anim.Play("Happy-Idle4", 0, 0);
				StartCoroutine(ShowText("Before you go anywhere I'm going to have to set you up with a better car then what you've rolled in on. Don't get too excited though, this one behind me is too much for you to handle. If you're around long enough you might just get to take it out for a spin."));
			} else if(!typing && speechIndex == 5 && startTyping) {
				startTyping = false;
				typing = true;
				anim.Play("Happy-Hand-Gesture", 0, 0);
				StartCoroutine(ShowText("Here's a bit of extra cash to get you started."));
				bank.SetActive(true);
			} else if(!typing && speechIndex == 6 && startTyping) {
				startTyping = false;
				speechPanel.SetActive(false);
				nextButton.SetActive(true);
				prevButton.SetActive(true);
				carInfo.SetActive(true);
				nextSpeechButton.SetActive(false);
				changecar.specificCar(1);
			} else if(!typing && speechIndex == 7 && startTyping) {
				speechPanel.SetActive(true);
				nextSpeechButton.SetActive(true);
				startTyping = false;
				typing = true;
				nextButton.SetActive(false);
				prevButton.SetActive(false);
				StartCoroutine(ShowText("Not bad. It could use a little more work. Let me know what you would like me to improve to give you a bit more of an edge out there."));
			} else if(!typing && speechIndex == 8 && startTyping) {
				startTyping = false;
				speechPanel.SetActive(false);
				carStats.SetActive(true);
				nextSpeechButton.SetActive(false);
			} else if(!typing && speechIndex == 9 && startTyping) {
				nextSpeechButton.SetActive(true);
				speechPanel.SetActive(true);
				startTyping = false;
				typing = true;
				StartCoroutine(ShowText("Looking good. Remember every star you earn out there can be used to improve your cars performance. When you earn a few more come back and cash them in for new upgrades."));
			} else if(!typing && speechIndex == 10 && startTyping) {
				anim.Play("Happy-Idle2", 0, 0);
				speechPanel.SetActive(true);
				nextSpeechButton.SetActive(false);
				keys.SetActive(true);
				Button keybutton = keys.GetComponent<Button>();
				keybutton.interactable = true;
				startTyping = false;
				typing = true;
				StartCoroutine(ShowText("Bring it around the front and let's see what you can do."));
			}
		}
	}

	public void nextSpeech() {
		if (!typing) {
			startTyping = true;
			if(speechIndex == 1) {
				string playerName = mainInputField.text.Trim((char)8203);
				if(playerName == "" || playerName == null) {
					errorText.text = "Please enter a username";
				} else {
					//If first speech check entered username
				    WWW wwwcheckUsername = new WWW("https://www.undivided.games/DriftWorlds/CheckUsername.php?user=" + playerName);
				    while (!wwwcheckUsername.isDone) { }

			        if (wwwcheckUsername.error != null) {
			            Debug.Log("There was an error checking the Username: " + wwwcheckUsername.error);
			        } else {
						Debug.Log(wwwcheckUsername.text);
						if(wwwcheckUsername.text != "X") {
							//Show error text
							errorText.text = wwwcheckUsername.text;
						} else {
							PlayerPrefs.SetString("Username", playerName);
							scriptUsername = playerName;
							speechIndex++;
						}
			        }
				}
			} else {
				speechIndex++;
				errorText.text = "";
			}
		}
	}

	public void firstCarBought() {
		speechIndex = 7;
		startTyping = true;
		typing = false;
		speechPanel.SetActive(true);
		carStats.SetActive(false);
	}

	public void firstStatUpgraded() {
		speechIndex = 9;
		startTyping = true;
		typing = false;
		speechPanel.SetActive(true);
		carStats.SetActive(false);
	}

	IEnumerator ShowText(string myText) {
		WaitForSeconds localdelay = new WaitForSeconds(delay);
		for(int i = 0; i <= myText.Length; i++) {
			currentText = myText.Substring(0,i);
			characterSpeech.text = currentText;
			yield return localdelay;
		}
		typing = false;
	}

	private string secretKey = "insertsecretcodehere";
	public IEnumerator saveUsername() {

		string hash = Md5Sum(scriptUsername + secretKey);

		WWWForm form = new WWWForm();
		form.AddField("username", scriptUsername);
		form.AddField("hashPost", hash);
		WWW www = new WWW("https://www.undivided.games/DriftWorlds/SaveUser.php", form);
		yield return www;
	}

	public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

}
