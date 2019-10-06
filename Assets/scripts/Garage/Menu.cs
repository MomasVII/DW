using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject locationGO, PostProcessing, buttonGO, MenuGO, adGo;
    public TMP_Text locationText;

    public Image ausFlag, usaFlag, DWLogo;
    public Sprite ausSprite, usaSprite, ausGSSprite, usaGSSprite;
    public Button nextButton;

    public GameObject tapToContinue;

    //Canvas Items
    public GameObject topBar, carStats, colors, prevNext;

    GarageTutorial garageTutorial;

    int cur_time;
    int ad_time;

    void Start()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        ad_time = PlayerPrefs.GetInt("GarageAdPlayed");
        ad_time = ad_time+(24*3600);

        garageTutorial = GetComponent<GarageTutorial>();
        PlayerPrefs.SetString("metrics", "kph");
        if(PlayerPrefs.HasKey("metrics")) {
            tapToContinue.SetActive(true);
            locationGO.SetActive(false);
            nextButton.gameObject.SetActive(false);
            if(PlayerPrefs.GetInt("loadGarage") == 1) { //Loading garage from Home, show all canvas elements
                MenuGO.SetActive(false);
                PostProcessing.SetActive(false);
                buttonGO.SetActive(false);
                topBar.SetActive(true);
                carStats.SetActive(true);
                colors.SetActive(true);
                prevNext.SetActive(true);
                tapToContinue.SetActive(false);

                if(cur_time > ad_time && ((Application.internetReachability != NetworkReachability.NotReachable) ||
        			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
        			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork))) {
                    adGo.SetActive(true);
                }
            }
        }
        PlayerPrefs.SetInt("loadGarage", 0);
    }

    public void setLocation(string myMetric) {
        PlayerPrefs.SetString("metrics", myMetric);
        nextButton.interactable = true;
        if(myMetric == "kph"){
            ausFlag.sprite = ausSprite;
            usaFlag.sprite = usaGSSprite;
        } else if(myMetric == "mph"){
            usaFlag.sprite = usaSprite;
            ausFlag.sprite = ausGSSprite;
        }

    }

    public void startGame() {
        DWLogo.CrossFadeAlpha(0, 1.0f, false);
        locationText.CrossFadeAlpha(0, 1.0f, false);
        ausFlag.CrossFadeAlpha(0, 1.0f, false);
        usaFlag.CrossFadeAlpha(0, 1.0f, false);
        PostProcessing.SetActive(false);
        buttonGO.SetActive(false);

        StartCoroutine(HideCrossFadedItems(1.0f));

        //If not the first time they've played the game show everything
        if(PlayerPrefs.HasKey("playerCar")) {
            topBar.SetActive(true);
            carStats.SetActive(true);
            colors.SetActive(true);
            prevNext.SetActive(true);
            if(cur_time > ad_time && ((Application.internetReachability != NetworkReachability.NotReachable) ||
                (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork))) {
                adGo.SetActive(true);
            }
        } else { //Start tutorial if never played before
            garageTutorial.StartTutorial();
            topBar.SetActive(true);
            prevNext.SetActive(true);
            PlayerPrefs.SetInt("Spent", -1);
        }
    }

    IEnumerator HideCrossFadedItems(float time)
    {
        yield return new WaitForSeconds(time);

        MenuGO.SetActive(false);
    }



}
