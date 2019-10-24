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

	GameObject car;
	Rigidbody rb;

	private DriftScoreManager driftScoreManager; //Don't update drift if car below x kph
	private Skidmarks skidmarksManager; //Don't update sound if car below x kph

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
		carsSpeed = Mathf.Round(((rb.velocity.magnitude - 0)*180)/(40 - 0));
		if(PlayerPrefs.GetString("metrics") == "mph") {
			carsSpeed = Mathf.Floor(carsSpeed*0.6f);
		}
		carSpeed.text = carsSpeed.ToString();
		carSpeedCircle.fillAmount = ((rb.velocity.magnitude - 0)*1)/(40 - 0);
		driftScoreManager.checkCarSpeed(carsSpeed);
		skidmarksManager.checkCarSpeed(carsSpeed);
	}

}
