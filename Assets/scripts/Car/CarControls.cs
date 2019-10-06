using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class CarControls : MonoBehaviour {

	Dot_Truck_Controller dotTruck;
	GameObject car;

	public Image SpeedCircle, SpeedCircleOutline;
    public Sprite SpeedCircleImg, SpeedCircleOutlineImg, ReverseCircleImg, ReverseCircleOutlineImg;
	public TMP_Text reverseText;

	void Start () {
		car = GameObject.FindWithTag("Player");
		dotTruck = car.GetComponent<Dot_Truck_Controller>();
	}

	public void reverseCar() {
		dotTruck.reversed = !dotTruck.reversed;
		if(dotTruck.reversed){
			SpeedCircle.sprite = ReverseCircleImg;
			SpeedCircleOutline.sprite = ReverseCircleOutlineImg;
			reverseText.text = "R";
		} else {
			SpeedCircle.sprite = SpeedCircleImg;
			SpeedCircleOutline.sprite = SpeedCircleOutlineImg;
			reverseText.text = "KPH";
		}
	}

	public void carLights() {
		dotTruck.lightsOn = !dotTruck.lightsOn;
	}

	public void TurnLeft() {
		dotTruck.turnLeft = true;
	}
	public void TurnRight() {
		dotTruck.turnRight = true;
	}
	public void Accelerate() {
		dotTruck.accelerate = true;
	}

	public void TurnLeftOff() {
		dotTruck.turnLeft = false;
	}
	public void TurnRightOff() {
		dotTruck.turnRight = false;
	}
	public void AccelerateOff() {
		dotTruck.accelerate = false;
	}


}
