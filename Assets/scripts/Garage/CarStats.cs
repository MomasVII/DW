using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarStats : MonoBehaviour {

	public TMP_Text starNumber;

	public GameObject carDetailsCanvas, carStatsCanvas, colorCanvas, showPanel;
	private bool isActive = true;

	// Use this for initialization
	void Start () {
		getTotalStars();
	}

	public void getTotalStars() {
		string[] levels = new string[7] {"Grass", "Grass2", "Grass3", "Lava2", "Lava3", "Snow", "Desert"};
		int totalStars = 0;
		foreach (string level in levels) {
			totalStars += PlayerPrefs.GetInt(level+"-Stars", 0);
        }
		totalStars -= PlayerPrefs.GetInt("Spent");
		starNumber.text = totalStars.ToString();
	}

	public void showStars() {
		isActive = !isActive;
		carDetailsCanvas.SetActive(isActive);
		carStatsCanvas.SetActive(isActive);
		colorCanvas.SetActive(isActive);
		showPanel.SetActive(!isActive);
	}


}
