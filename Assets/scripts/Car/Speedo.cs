using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class Speedo : MonoBehaviour {

	public TMP_Text carSpeed;
	public Image carSpeedCircle;

	//Car text kph or mph
	public TMP_Text carMetric;

	float carsSpeed = 0.0f;

	private GameObject car;
	private Rigidbody rb;

	private DriftScoreManager driftScoreManager; //Don't update drift if car below x kph
	private Skidmarks skidmarksManager; //Don't update sound if car below x kph

	private bool greaterThan = true;
	private int greaterThan2 = 3;
	private float mag;

	void Start() {
		driftScoreManager = GameObject.FindObjectOfType<DriftScoreManager>();
		skidmarksManager = GameObject.FindObjectOfType<Skidmarks>();

		if(PlayerPrefs.GetString("metrics") == "mph") {
			carMetric.text = "mph";
		} else {
			carMetric.text = "kph";
		}
		car = GameObject.FindWithTag("Player");
		rb = car.GetComponent<Rigidbody>();
	}

	public void FixedUpdate() {
		mag = rb.velocity.magnitude;
		carsSpeed = Mathf.Round((mag*180)/40);
		if(PlayerPrefs.GetString("metrics") == "mph") {
			carsSpeed = Mathf.Floor(carsSpeed*0.6f);
		}
		carSpeed.text = carsSpeed.ToString();
		carSpeedCircle.fillAmount = (mag*1)/40;

		//Run if speed changes from above or below 45
		if(checkSpeed > 45 && !greaterThan) {
			driftScoreManager.checkCarSpeed(true);
			greaterThan = true;
		} else if(checkSpeed < 45 && greaterThan) {
			driftScoreManager.checkCarSpeed(false);
			greaterThan = false;
		}

		if(checkSpeed < 10 && greaterThan2 != 0) {
			skidmarksManager.checkCarSpeed(carsSpeed);
			greaterThan2 = 0;
		} else if(checkSpeed < 45 && checkSpeed >= 10) {
			skidmarksManager.checkCarSpeed(carsSpeed);
		} else if(checkSpeed >= 45 && greaterThan2 != 2) {
			skidmarksManager.checkCarSpeed(carsSpeed);
			greaterThan2 = 2;
		}

	}

}
