using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFollowHome : MonoBehaviour
{
    GameObject player;
    public GameObject snowParticles;
    bool entered = false;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
	}

    void FixedUpdate() {
        if(entered) {
            snowParticles.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+10, player.transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            entered = true;
            snowParticles.GetComponent<ParticleSystem>().Play();
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "Player"){
            entered = false;
            snowParticles.GetComponent<ParticleSystem>().Stop();
        }
    }
}
