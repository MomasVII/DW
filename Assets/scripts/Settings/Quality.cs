using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quality : MonoBehaviour {

    public GameObject[] mediumQualityObjects;
    public GameObject[] highQualityObjects;
    int qualityLevel = 1;

    void Start() {
        ChangeAdditionalQuality();
    }

    public void ChangeAdditionalQuality() {

        //Get the quality level from Unity as a number
        qualityLevel = QualitySettings.GetQualityLevel();

        //Low Quality
        if(qualityLevel == 1) {

            foreach (GameObject mediumQualityObject in mediumQualityObjects)
            {
                mediumQualityObject.SetActive(false);
            }

            foreach (GameObject highQualityObject in highQualityObjects)
            {
                highQualityObject.SetActive(false);
            }

        //Medium Quality
        } else if(qualityLevel == 2) {

            foreach (GameObject mediumQualityObject in mediumQualityObjects)
            {
                mediumQualityObject.SetActive(true);
            }

            foreach (GameObject highQualityObject in highQualityObjects)
            {
                highQualityObject.SetActive(false);
            }

        //High Quality
        } else if(qualityLevel == 4) {

            foreach (GameObject mediumQualityObject in mediumQualityObjects)
            {
                mediumQualityObject.SetActive(true);
            }

            foreach (GameObject highQualityObject in highQualityObjects)
            {
                highQualityObject.SetActive(true);
            }
        }
    }

    // Use this for initialization
    public void ChangeQuality(int q) {

        //Set quality
        QualitySettings.SetQualityLevel(q, true);

        PlayerPrefs.SetInt("QualitySetting", q);

        //Show and hide elements based on quality settings
        ChangeAdditionalQuality();
    }
}
