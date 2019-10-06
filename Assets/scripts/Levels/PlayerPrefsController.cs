using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerPrefsController : MonoBehaviour {

	private float highscore;
	public Text timerText; //Highscore text
	Scene scene;

	// Use this for initialization
	void Start () {
		getHighScore();
	}

	void getHighScore() {
		scene = SceneManager.GetActiveScene();
		if(PlayerPrefs.HasKey(scene.name+"highscore")) {
			highscore = PlayerPrefs.GetFloat(scene.name+"highscore", 0);
			timerText.text = highscore.ToString("F3");
		} else {
			highscore = 0.0f;
			timerText.text = "";
		}
	}

	public void setPlayerScore(float score) {
		getHighScore();
		if(score < highscore || highscore <= 0) {
			PlayerPrefs.SetFloat(scene.name+"highscore", score);
			timerText.text = score.ToString("F3");
		}
	}

}
