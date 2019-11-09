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

    private ParticleSystem wildfireLeft, wildfireRight;

    private Rigidbody carRB;

    // Start is called before the first frame update
    void Start() {
        car = GameObject.FindWithTag("Player");
        wildfireLeft = car.transform.GetChild(3).transform.GetChild(0).GetComponent<ParticleSystem>();
        wildfireRight = car.transform.GetChild(3).transform.GetChild(1).GetComponent<ParticleSystem>();
        wildfireLeft.Stop();
        wildfireRight.Stop();

        rearLeft = car.transform.Find("wheels/RearLeft").GetComponent<WheelCollider>();
		rearRight = car.transform.Find("wheels/RearRight").GetComponent<WheelCollider>();

        carRB = car.GetComponent<Rigidbody>();

		StartCoroutine("addNos");
    }

    IEnumerator addNos() {
        WaitForSeconds delay = new WaitForSeconds(1f);
	    for(;;) {
                if(rearRight.isGrounded || rearLeft.isGrounded) {
                 fillPercent += 0.02f;
                 flameColored.fillAmount = fillPercent;
                 if(fillPercent > 0.99f) {
                     flameGlow.SetActive(true);
                 }
                 if(fillPercent > 1) { fillPercent = 1; }
             }
    	         // execute block of code here
	         yield return delay;
	    }
	}

    // Update is called once per frame
    void FixedUpdate() {
        if(nosing && fillPercent > 0.1f && (rearRight.isGrounded || rearLeft.isGrounded)) {
            carRB.AddForce(car.transform.forward * startForce, ForceMode.Acceleration);
            startForce++;
            if(startForce > maxForce) { startForce = maxForce; }
            fillPercent -= 0.01f;
            if(fillPercent < 0) { fillPercent = 0; }
            wildfireLeft.Play();
            wildfireRight.Play();
            flameColored.fillAmount = fillPercent;
        } else {
            if (wildfireLeft.isPlaying) {
                wildfireLeft.Stop();
                wildfireRight.Stop();
            }
            startForce = 10;
        }
    }

    public void StartNos() {
        nosing = true;
        flameGlow.SetActive(false);
    }

    public void StopNos() {
        nosing = false;
    }

    public void refill() {
        fillPercent = 1.0f;
        flameGlow.SetActive(true);
    }

}
