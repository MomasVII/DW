using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Advertisements;

public class GarageCollider : MonoBehaviour
{

    public TMP_Text forestStars, lavaStars, snowStars, desertStars, carCount;
    public Button playButton;

    public UIManager UIManagerScript;
    public Animator openPanelAnim;
    public GameObject modalPanelObject;

    public GameObject GarageOBJ, LockedOBJ, LevelsOBJ;

    //Slow car on enter
    private Dot_Truck_Controller dotTruckController;

    // Start is called before the first frame update
    void Start()
    {
        carCount.text = "Unlocked "+PlayerPrefs.GetInt("carCount")+"/21 Cars";

        dotTruckController = GameObject.FindWithTag("Player").GetComponent<Dot_Truck_Controller>();

        //Get stars for each section
        int totalStars = 0, starsTotal = 0;
        string[] grassLevels = new string[3] {"Grass", "Grass2", "Grass3"}; //AddLevel
		foreach (string grassLevel in grassLevels) {
			totalStars += PlayerPrefs.GetInt(grassLevel+"-Stars", 0);
            starsTotal += 5;
        }
        forestStars.text = totalStars+"/"+starsTotal;

        totalStars = 0;
        starsTotal = 0;
        string[] lavaLevels = new string[2] {"Lava2", "Lava3"};
		foreach (string lavaLevel in lavaLevels) {
			totalStars += PlayerPrefs.GetInt(lavaLevel+"-Stars", 0);
            starsTotal += 5;
        }
        lavaStars.text = totalStars+"/"+starsTotal;

        totalStars = 0;
        starsTotal = 0;
        string[] snowLevels = new string[1] {"Snow"};
		foreach (string snowLevel in snowLevels) {
			totalStars += PlayerPrefs.GetInt(snowLevel+"-Stars", 0);
            starsTotal += 5;
        }
        snowStars.text = totalStars+"/"+starsTotal;

        totalStars = 0;
        starsTotal = 0;
        string[] desertLevels = new string[2] {"Desert", "Desert2"};
		foreach (string desertLevel in desertLevels) {
			totalStars += PlayerPrefs.GetInt(desertLevel+"-Stars", 0);
            starsTotal += 5;
        }
        desertStars.text = totalStars+"/"+starsTotal;


    }

    void OnTriggerEnter(Collider other) {
        //playButton.interactable = false;
        GarageOBJ.SetActive(true);
        LockedOBJ.SetActive(false);
        LevelsOBJ.SetActive(false);

        dotTruckController.slowCar(true);
        if(other.gameObject.tag == "Player") {
            UIManagerScript.EnableBoolAnimator(openPanelAnim);

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(LoadLevel);
            playButton.onClick.AddListener(ClosePanel);
        }
    }

    void OnTriggerExit(Collider other) {
        closePanel();
    }

    public void closePanel() {
        dotTruckController.slowCar(false);
        UIManagerScript.DisableBoolAnimator(openPanelAnim);
        GarageOBJ.SetActive(false);
    }

    void ClosePanel() {
        modalPanelObject.SetActive(false);
    }

    void LoadLevel() {
        // Show an ad:
        if(((Application.internetReachability != NetworkReachability.NotReachable) ||
			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)) &&
			PlayerPrefs.GetString("NoAds") != "NoAds") {
			UnityAdsManager.Instance.ShowRegularAd(OnAdClosed);
		} else {
            PlayerPrefs.SetInt("loadGarage", 1);
            LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
    		loadingscreen.LoadLevel("Garage");
		}
    }

    private void OnAdClosed(ShowResult result) {
        PlayerPrefs.SetInt("loadGarage", 1);
        LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
		loadingscreen.LoadLevel("Garage");
    }

}
