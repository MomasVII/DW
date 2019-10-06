using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turns : MonoBehaviour {

	public RawImage m_RawImage;
    //Select a Texture in the Inspector to change to
	public Texture turn_Texture;

    private bool textOn = false;

	// Use this for initialization
	void Start () {
		//m_RawImage.enabled = false;
	}

	void Update () {
		if(textOn) {
			StartCoroutine(KillText()); //Remove image after x seconds
		}
	}

	IEnumerator KillText()
	{
		textOn = false;
		yield return new WaitForSeconds(2);
		m_RawImage.gameObject.SetActive(false);
	}


	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			m_RawImage.texture = turn_Texture;
			m_RawImage.gameObject.SetActive(true);
	        //Change the Texture to be the one you define in the Inspector
			textOn = true;
		}
	}
}
