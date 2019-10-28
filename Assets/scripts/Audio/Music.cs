using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    public AudioClip[] songs;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.volume = PlayerPrefs.GetFloat("musicVolume");
        audioSource.clip = songs[Random.Range(0, songs.Length)];
        audioSource.Play();
    }

}
