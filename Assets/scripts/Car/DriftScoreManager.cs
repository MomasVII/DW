using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DriftScoreManager : MonoBehaviour {

	public TMP_Text driftScoreText, totalDriftScoreText;
	int driftScore = 0;
	int totalDrift = 0; //Total points acquired over the level drifitng
	float increaseFont = 0.50f;
	bool allowDrift = true;
	bool allowDriftReversed = true;

	//Used to set drift money to player cash
	public MoneyController moneyController;

	private int findDemo;

	public void Start() {
		findDemo = GameObject.FindGameObjectsWithTag("Demo").Length;
	}

	public void checkCarSpeed(float carSpeed) {
		if(carSpeed < 40) {
			allowDrift = false;
		} else {
			allowDrift = true;
		}
	}

	public void checkReverse(bool isReversed) {
		if(isReversed) {
			allowDriftReversed = false;
		} else {
			allowDriftReversed = true;
		}
	}

	public void resetTotal() {
		driftScoreText.text = "";
		findDemo = GameObject.FindGameObjectsWithTag("Demo").Length;
		Debug.Log("In here");
		totalDrift = 0;
	}

	public void addToPlayerCash() {
		moneyController.updateDriftMoney(totalDrift);
	}

	public void updateScore(byte intensity) {
		if(findDemo == 1) {
			if (intensity > 100 && allowDrift && allowDriftReversed) {
				driftScore++;

				if(driftScore > 200 && driftScore < 300) { driftScoreText.color = new Color32(255, 138, 0, 255); }
				else if(driftScore >= 300) { driftScoreText.color = new Color32(255, 0, 0, 255); }

				if(driftScoreText.fontSize < 70) {
					driftScoreText.fontSize += increaseFont;
				}
				if(driftScore > 0) {
					driftScoreText.gameObject.SetActive(true);
					driftScoreText.text = "+"+driftScore;
				}
				if(increaseFont > 0.10) {
					increaseFont -= 0.01f;
				}

			} else if (intensity < 100) {
				totalDrift += driftScore;
				totalDriftScoreText.text = "$"+totalDrift.ToString("n0");
				driftScore = 0;
			}

			if (intensity < 25) {
				driftScoreText.color = new Color32(255, 255, 255, 255);
				driftScoreText.fontSize = 13;
				driftScoreText.gameObject.SetActive(false);
			}
		}

	}
}
