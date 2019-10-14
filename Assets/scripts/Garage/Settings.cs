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

    //Quality settings slider
    public Slider qualitySlider;

    //Get Quality scripts and change based on slider
    public Quality quality;

    public void Start() {
        quality = GetComponent<Quality>();
        qualitySlider.value = PlayerPrefs.GetFloat("quality")/100;
        changeMetric(PlayerPrefs.GetString("metrics"));
    }

    public void showSettings() {
		isActive = !isActive;
		carDetailsCanvas.SetActive(isActive);
		carStatsCanvas.SetActive(isActive);
		colorCanvas.SetActive(isActive);
		showPanel.SetActive(!isActive);
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
        float qualityFloat = qualitySlider.value*100;
        PlayerPrefs.SetFloat("quality", qualityFloat);
        if(qualityFloat < 25) {
            quality.ChangeQuality(1);
        } else if(qualityFloat >= 25 && qualityFloat < 50) {
            quality.ChangeQuality(2);
        } else if(qualityFloat >= 50 && qualityFloat < 75) {
            quality.ChangeQuality(3);
        }  else if(qualityFloat > 75) {
            quality.ChangeQuality(4);
        }
    }

}
