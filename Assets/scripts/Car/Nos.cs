using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nos : MonoBehaviour {

    public Image flameColored;
    public GameObject flameGlow;

    GameObject car;

    private bool nosing = false;
    private int maxForce = 10;
    private int startForce = 10;
    private float fillPercent = 1f;

    WheelCollider rearLeft, rearRight;

    //Count seconds;
    private int nextUpdate = 1;

    private ParticleSystem wildfireLeft, wildfireRight;

    // Start is called before the first frame update
    void Start() {
        car = GameObject.FindWithTag("Player");
        wildfireLeft = car.transform.GetChild(3).transform.GetChild(0).GetComponent<ParticleSystem>();
        wildfireRight = car.transform.GetChild(3).transform.GetChild(1).GetComponent<ParticleSystem>();
        wildfireLeft.Stop();
        wildfireRight.Stop();

        rearLeft = car.transform.Find("wheels/RearLeft").GetComponent<WheelCollider>();
		rearRight = car.transform.Find("wheels/RearRight").GetComponent<WheelCollider>();

    }

    // Update is called once per frame
    void FixedUpdate() {
        if(nosing && fillPercent > 0.1f && (rearRight.isGrounded || rearLeft.isGrounded)) {
            car.GetComponent<Rigidbody>().AddForce(car.transform.forward * startForce, ForceMode.Acceleration);
            startForce++;
            if(startForce > maxForce) { startForce = maxForce; }
            fillPercent -= 0.01f;
            if(fillPercent < 0) { fillPercent = 0; }
            wildfireLeft.Play();
            wildfireRight.Play();
        } else {
            wildfireLeft.Stop();
            wildfireRight.Stop();
            startForce = 10;
            if(Time.time >= nextUpdate) {
                nextUpdate=Mathf.FloorToInt(Time.time)+1;
                fillPercent += 0.02f;
            }
        }
        if( fillPercent > 0.99f) {
            flameGlow.SetActive(true);
        } else {
            flameGlow.SetActive(false);
        }
        flameColored.fillAmount = fillPercent;
        if(fillPercent > 1) { fillPercent = 1; }
    }

    public void StartNos() {
        nosing = true;
    }

    public void StopNos() {
        nosing = false;
    }

    public void refill() {
        fillPercent = 1.0f;
    }

}
