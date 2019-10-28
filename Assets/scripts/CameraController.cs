using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	GameObject car;
    public Camera orthCamera;

	Vector3 lastPosition = Vector3.zero;
	private float speed, speedTo;
	private float nextUpdate = 1f;

	private bool finished = false;

	private float Zoom1;
    private float Zoom2;

	public float posX = 5f, posY = 6f, posZ = 5, orthS = 20f;

    //private float duration = 0.5f;
    //private float elapsed = 0.0f;

	private bool transition;
	private float carPos;

	public string tagString;

	//Stack overflow camera variables
	Vector3 camVelocity = Vector3.zero;
	Vector3 camSpeedOffset = Vector3.zero;
	Vector3 startPosition;
	public float cameraSmoothTime = 0.1f;
	public float maxCameraOffsetVelocity = 4f;

	private Vector3 touchStart;
	public float zoomOutMin = 1;
	public float zoomOutMax = 8;

	//Hide these on panning
	public GameObject reset, controls, money;

	//Camera variables
	private Vector3 camSpeedOffsetTarget;

	public void Start() {
		car = GameObject.FindWithTag(tagString);
		transform.position = new Vector3(car.transform.position.x-posX+carPos, car.transform.position.y+posY, car.transform.position.z-posZ);
		transform.LookAt(car.transform);
		startPosition = transform.position;
	}

	public void FixedUpdate() {
		//Get car speed
		if(Time.time >= nextUpdate){
			speedTo = (car.transform.position - lastPosition).magnitude;
			nextUpdate=Mathf.FloorToInt(Time.time)+0.1f;
			if(speedTo > speed) {
				if(speed+0.001f > speedTo) {
					speed += 0.0001f;
				} else {
					speed += 0.001f;
				}
			} else if(speedTo < speed) {
				if(speed-0.001f < speedTo) {
					speed -= 0.0001f;
				} else {
					speed -= 0.001f;
				}
			}
		}
		lastPosition = car.transform.position;
	}

	void Update ()
	{
		carPos = car.transform.forward.x * 125;
		if(tagString != "Demo") {
			carPos = 0; // Remove for moving camera
		}
		carPos = 0;
		if(!finished) {
			/*transform.position = new Vector3(car.transform.position.x-posX+carPos, car.transform.position.y+posY, car.transform.position.z-posZ) + car.transform.forward * (Mathf.Clamp(speed, 0, 0.70f)*30);
			orthCamera.orthographicSize = orthS+(Mathf.Clamp(speed, 0, 0.70f)*30);*/

			if(tagString == "Player") {
				camSpeedOffsetTarget = car.transform.forward * (Mathf.Clamp(speed, 0, 0.70f)*20);
				camSpeedOffset = Vector3.SmoothDamp(camSpeedOffset, camSpeedOffsetTarget,
	        	ref camVelocity, cameraSmoothTime, maxCameraOffsetVelocity);

				transform.position = new Vector3(
			        car.transform.position.x-posX+carPos,
			        car.transform.position.y+posY,
			        car.transform.position.z-posZ)
			      + camSpeedOffset;

				 orthCamera.orthographicSize = 20.0f+(Mathf.Clamp(speed, 0, 0.70f)*20);
			} else if(tagString == "Demo") {
				transform.position = new Vector3(car.transform.position.x-5f+carPos, car.transform.position.y+6.0f, car.transform.position.z-5);
				orthCamera.orthographicSize = 20.0f+(Mathf.Clamp(speed, 0, 0.70f)*10);
				transition = false;
				transform.LookAt(car.transform);
			}  else if(tagString == "Top") {
				if(Input.GetMouseButtonDown(0)) {
					touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				}
				if(Input.touchCount == 2) {
					Touch touchZero = Input.GetTouch(0);
					Touch touchOne = Input.GetTouch(1);

					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

					float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

					float difference = currentMagnitude - prevMagnitude;

					zoom(difference * 0.10f);
				} else if(Input.GetMouseButton(0)) {
					Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
					Camera.main.transform.position += direction;
				}

				//zoom(Input.GetAxis("Mouse ScrollWheel")); Scroll wheel

			}


			//elapsed = 0.0f;
			transition = false;
		} else {
			transition = true;
			/*transform.LookAt(car.transform);
			transform.Translate(Vector3.right * Time.deltaTime * 4);*/
		}

		if (transition) {
			/*orthCamera.orthographicSize = Mathf.Lerp(Zoom1, Zoom2, elapsed);
			elapsed += duration * Time.deltaTime; //Here

			if (elapsed > 10.0f) {
				transition = false;
			}*/
		}
	}

	void zoom(float increment) {
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
	}

	public void FinishedCamera(bool isFinished) {
		finished = isFinished;
	}

	public void DetachCamera() {
		if(tagString == "Top") {
			tagString = "Player";
			controls.SetActive(true);
			reset.SetActive(true);
			money.SetActive(true);
		} else {
			tagString = "Top";
			controls.SetActive(false);
			reset.SetActive(false);
			money.SetActive(false);
		}
	}

	public void ChangeCamera() {
		car = GameObject.FindWithTag("Player");
		tagString = "Player";
		transform.position = startPosition;
	}

}
