using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crash : MonoBehaviour
{

    public AudioClip crash1, crash2, crash3;
    AudioSource audio;

    void Start() {
        audio = GetComponent<AudioSource>();
        audio.volume = PlayerPrefs.GetFloat("sfxVolume");
    }

    void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Player") {
            int randomClip = Random.Range(0, 2);
            if(col.rigidbody.velocity.magnitude < 1.2) {
                randomClip = 2;
            }
            if(randomClip <= 0){
                audio.clip = crash1;
            } else if (randomClip == 1){
                audio.clip = crash2;
            } else {
                audio.clip = crash3;
            }
            audio.Play();
        }
    }
}
