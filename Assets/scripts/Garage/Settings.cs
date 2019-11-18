using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public GameObject carDetailsCanvas, carStatsCanvas, colorCanvas, showPanel;
    private bool isActive = true;

    //Flags for metrics
    public Image ausFlag, usaFlag;
    public Sprite ausSprite, usaSprite, ausGSSprite, usaGSSprite;

    //Quality/Audio settings slider
    public Slider qualitySlider, musicSlider, sfxSlider;

    //Change audio volume
    public AudioSource audioSource, sfx1, sfx2, sfx3, sfx4;

    //Get Quality scripts and change based on slider
    private Quality quality;
    private float qualityFloat;

    public void Start() {
        quality = GetComponent<Quality>();
        qualitySlider.value = PlayerPrefs.GetFloat("QualitySetting")/100;
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        changeMetric(PlayerPrefs.GetString("metrics"));

        //Set initial sound settings
        float sfxv = PlayerPrefs.GetFloat("sfxVolume");
        sfx1.volume = sfxv;
        sfx2.volume = sfxv;
        sfx3.volume = sfxv;
        sfx4.volume = sfxv;
    }

    public void showSettings() {
		isActive = !isActive;
        if(PlayerPrefs.HasKey("homeTutorial")) {
    		carDetailsCanvas.SetActive(isActive);
    		carStatsCanvas.SetActive(isActive);
    		colorCanvas.SetActive(isActive);
        }
        showPanel.SetActive(!isActive);
        if(isActive) {
            if(qualityFloat < 18) {
                quality.ChangeQuality(0);
            } else if(qualityFloat >= 18 && qualityFloat < 50) {
                quality.ChangeQuality(1);
            } else if(qualityFloat >= 50 && qualityFloat < 83) {
                quality.ChangeQuality(2);
            }  else if(qualityFloat > 83) {
                quality.ChangeQuality(3);
            }
        }
	}

    public void changeMetric(string myMetric) {
        PlayerPrefs.SetString("metrics", myMetric);
        if(myMetric == "kph"){
            ausFlag.sprite = ausSprite;
            usaFlag.sprite = usaGSSprite;
        } else if(myMetric == "mph"){
            usaFlag.sprite = usaSprite;
            ausFlag.sprite = ausGSSprite;
        }
    }

    public void changeQuality() {
        qualityFloat = qualitySlider.value*100;
        PlayerPrefs.SetFloat("QualitySetting", qualityFloat);
    }

    public void changeMusicVolume() {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        audioSource.volume = musicSlider.value;
    }

    public void changeSoundEffects() {
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        sfx1.volume = sfxSlider.value;
        sfx2.volume = sfxSlider.value;
        sfx3.volume = sfxSlider.value;
        sfx4.volume = sfxSlider.value;
    }


}
