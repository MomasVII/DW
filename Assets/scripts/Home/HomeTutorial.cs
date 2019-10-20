using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HomeTutorial : MonoBehaviour {

	public GameObject lightsButton, resetButton;

	public GameObject controls, speedo, nextButton;

	//Speech Panel
	public GameObject speechPanel;

	public Camera mainCamera, levelCamera;

	//Speech variables
	private float delay = 0.005f; //0.04
	private string currentText = "";
	public TMP_Text characterSpeech;

	private bool typing = false, startTyping = true;
	private int speechIndex = 1;


	// Use this for initialization
	void Start () {

		if(!PlayerPrefs.HasKey("homeTutorial")) { //Check if playerpref car has been created
			speechPanel.SetActive(true);

			lightsButton.SetActive(false);
			resetButton.SetActive(false);
			controls.SetActive(false);
			speedo.SetActive(false);
			nextButton.SetActive(true);
		}

	}

	// Update is called once per frame
	void Update () {

		if(!PlayerPrefs.HasKey("homeTutorial")) { //Check if playerpref car has been created
			if(!typing && speechIndex == 1 && startTyping) {
				startTyping = false;
				typing = true;
				StartCoroutine(ShowText("Welcome to the world. Like I said there are dozens of levels to explore around here. Each provide there own unique obstacles and challenges. If you race well enough you'll get a nice payday so make sure you're on your A game. You will also get a star rating depending on your time which can be used to upgrade your car in the garage."));
			} else if(!typing && speechIndex == 2 && startTyping) {
				startTyping = false;
				typing = true;
				mainCamera.enabled = false;
				levelCamera.enabled = true;
				StartCoroutine(ShowText("Each level has an access panel like this. To enter a level just drive over a platform. It's that simple."));
			} else if(!typing && speechIndex == 3 && startTyping) {
				startTyping = false;
				typing = true;
				mainCamera.enabled = true;
				levelCamera.enabled = false;
				StartCoroutine(ShowText("Now, let me give you a run down of how your new car works."));
			} else if(!typing && speechIndex == 4 && startTyping) {
				startTyping = false;
				typing = true;
				resetButton.SetActive(true);
				//lightsButton.SetActive(true);
				StartCoroutine(ShowText("The buttons on your left will allow you to reset your car to the beginning and zoom and pan around the level. You can tap the middle of the screen to upright your car should you even end upside down. "));
			} else if(!typing && speechIndex == 5 && startTyping) {
				startTyping = false;
				typing = true;
				controls.SetActive(true);
				speedo.SetActive(true);
				StartCoroutine(ShowText("Here are your controls and speedometer. You can tap the speedo to change into Reverse and tapping it again will bring you back into Drive."));
			} else if(!typing && speechIndex == 6 && startTyping) {
				startTyping = false;
				typing = true;
				controls.SetActive(true);
				StartCoroutine(ShowText("That's enough for now. Have a drive around and explore. I won't be too far away. Come back when you earn some money."));
			} else if(!typing && speechIndex == 7 && startTyping) {
				startTyping = false;
				speechPanel.SetActive(false);
				PlayerPrefs.SetInt("homeTutorial", 1);
				nextButton.SetActive(false);
			}

		}

	}

	public void nextSpeech() {
		if (!typing) {
			startTyping = true;
			speechIndex++;
		}
	}

	IEnumerator ShowText(string myText) {
		for(int i = 0; i <= myText.Length; i++) {
			currentText = myText.Substring(0,i);
			characterSpeech.text = currentText;
			yield return new WaitForSeconds(delay);
		}
		typing = false;
	}
}
