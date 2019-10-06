using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Store : MonoBehaviour
{

	public GameObject carDetailsCanvas, carStatsCanvas, colorCanvas, showPanel;
	private bool isActive = true;

	// Use this for initialization
	void Start () {
	}

	public void showStore() {
		isActive = !isActive;
		carDetailsCanvas.SetActive(isActive);
		carStatsCanvas.SetActive(isActive);
		colorCanvas.SetActive(isActive);
		showPanel.SetActive(!isActive);
	}

}
