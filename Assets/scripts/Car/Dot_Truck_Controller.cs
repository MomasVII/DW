using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn;
}

public class Dot_Truck_Controller : MonoBehaviour {

	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;

	private DriftScoreManager driftScoreManager;


	//Car control variables
	public Image SpeedCircle, SpeedCircleOutline;
    public Sprite SpeedCircleImg, SpeedCircleOutlineImg, ReverseCircleImg, ReverseCircleOutlineImg;
	public TMP_Text reverseText;

	public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

	float brakeTorque = 0.0f;

	float smooth = 2.0f;
	//float tiltAngle = 60.0f;

	public Light left_light;
	public Light right_light;

	public Light left_headlight;
	public Light right_headlight;

	public bool reversed = false;
	public bool lightsOn = false;

	public bool turnLeft = false,
				turnRight = false,
				accelerate = false;

	//Dottruck variables
	private Quaternion target;
	private float steering;
	private bool checkReverse = false;

	//Give acces to get function
	private float motor;

	void Start() {
		driftScoreManager = GameObject.FindObjectOfType<DriftScoreManager>();
		maxMotorTorque = PlayerPrefs.GetInt("Speed");
		maxSteeringAngle = PlayerPrefs.GetInt("Handling");
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.mass = 210 - PlayerPrefs.GetInt("Acceleration");

		left_headlight.enabled = false;
		right_headlight.enabled = false;
	}

	public void FixedUpdate()
	{

		target = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

		motor = maxMotorTorque * Input.GetAxis("Vertical");
		steering = maxSteeringAngle * Input.GetAxis("Horizontal");

		if(accelerate) {

			motor = maxMotorTorque * 1;

			//If we're accelerating disable the brake lights
			left_light.enabled = false;
			right_light.enabled = false;
		} else {
			left_light.enabled = true;
			right_light.enabled = true;
		}

		if(turnLeft) {
			steering = maxSteeringAngle * -1;
		}

		if(turnRight) {
			steering = maxSteeringAngle * 1;
		}

		//If reversing, negate the motor speed and turn on brake lights
		if(reversed) {
			motor = -Mathf.Abs(motor);
			left_light.enabled = true;
			right_light.enabled = true;
			if(!checkReverse) {
				driftScoreManager.checkReverse(true);
				checkReverse = true;
			}
		} else {
			if(checkReverse) {
				driftScoreManager.checkReverse(false);
				checkReverse = false;
			}
		}

		foreach (Dot_Truck truck_Info in truck_Infos)
		{
			if (truck_Info.steering == true) {
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn)?-1:1)*steering;
			}

			if (truck_Info.motor == true) {
				truck_Info.leftWheel.motorTorque = motor;
				truck_Info.rightWheel.motorTorque = motor;
			}

			truck_Info.leftWheel.brakeTorque = brakeTorque;
			truck_Info.rightWheel.brakeTorque = brakeTorque;

			VisualizeWheel(truck_Info);
		}

		/*if(lightsOn) {
			left_headlight.enabled = true;
			right_headlight.enabled = true;
		} else {
			left_headlight.enabled = false;
			right_headlight.enabled = false;
		}*/

	}

	public void slowCar(bool ShouldISlow) {
		if(ShouldISlow) {
			brakeTorque = 80;
		} else {
			brakeTorque = 0;
		}
	}

	public void reverseCar() {
		reversed = !reversed;
		if(reversed){
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
		lightsOn = !lightsOn;
	}

	public void TurnLeft() {
		turnLeft = true;
	}
	public void TurnRight() {
		turnRight = true;
	}
	public void Accelerate() {
		accelerate = true;
	}

	public void TurnLeftOff() {
		turnLeft = false;
	}
	public void TurnRightOff() {
		turnRight = false;
	}
	public void AccelerateOff() {
		accelerate = false;
	}


}
