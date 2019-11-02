using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DriftScoreManager : MonoBehaviour {

	public TMP_Text driftScoreText, totalDriftScoreText;
	private int driftScore = 0;
	private int totalDrift = 0; //Total points acquired over the level drifitng
	private float increaseFont = 0.50f;
	private bool allowDrift = true;
	private bool allowDriftReversed = true;

	//Used to set drift money to player cash
	public MoneyController moneyController;

	private int findDemo;

	//Colors
	Color firstColor;
	Color secondColor;
	Color whiteColor;

	public void Start() {
		findDemo = GameObject.FindGameObjectsWithTag("Demo").Length;

		//Colors
		firstColor = new Color32(255, 138, 0, 255);
		secondColor = new Color32(255, 0, 0, 255);
		whiteColor = new Color32(255, 255, 255, 255);
	}

	public void checkCarSpeed(bool carSpeed) {
			allowDrift = carSpeed;
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
		totalDrift = 0;
	}

	public void addToPlayerCash() {
		moneyController.updateDriftMoney(totalDrift);
	}

	public void updateScore(byte intensity) {
		if(findDemo == 1) {
			if (intensity > 100 && allowDrift && allowDriftReversed) {
				driftScore++;

				if(driftScoreText.fontSize < 70) {
					driftScoreText.fontSize += increaseFont;
				} else {
					if(driftScore > 200 && driftScore < 300 && driftScoreText.color != firstColor) {
						driftScoreText.color = firstColor;
					} else if(driftScore >= 300 && driftScoreText.color != secondColor) {
						driftScoreText.color = secondColor;
					}
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

			if (intensity < 25 && driftScoreText.color != whiteColor) {
				driftScoreText.color = whiteColor;
				driftScoreText.fontSize = 13;
				driftScoreText.gameObject.SetActive(false);
			}
		}

	}
}
