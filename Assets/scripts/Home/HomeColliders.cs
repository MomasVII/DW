using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

using TMPro;

public class HomeColliders : MonoBehaviour {

    public string sceneName, levelName;
    public float twoStar, threeStar, fourStar, fiveStar;

    public TMP_Text levelText;
    public TMP_Text StarInfo;
    public TMP_Text personalBestText, worldRecordText;
    public GameObject modalPanelObject;
    public Button playButton;

    public Image star1, star2, star3, star4, star5;
    public Sprite starSelected, starUnselected;

    public UIManager UIManagerScript;
    public Animator openPanelAnim;

    //Online world record time
    private float timeTaken;
    private int WRcarUsed;
    private string WRghostID;

    public Text ErrorText;

    //Slow car on enter
    private Dot_Truck_Controller dotTruckController;

    //Disabled level objects
    public GameObject unlockedGO;
    public GameObject lockedGO;
    public TMP_Text starsNeeded;
    private bool unlocked = false;
    private int totalStars = 0;
    private int enableScore = 0;

    //Change camera on unlocking new level
    public Camera levelCamera;
    public GameObject theCanvas;
    //Allow touch after this many seconds
	private int allowTouch = 1;
    private bool panCamera = false;

    void Start() {
        dotTruckController = GameObject.FindWithTag("Player").GetComponent<Dot_Truck_Controller>();

        //Reset Stuff
        /*PlayerPrefs.SetInt(sceneName+"-Stars", 0);
        PlayerPrefs.DeleteKey(sceneName+"highscore");
        PlayerPrefs.DeleteKey("OldTotalStars");*/

        //Get stars to lock or unlock level
        string[] levels = new string[7] {"Grass", "Grass2", "Grass3", "Lava2", "Lava3", "Snow", "Desert"};
		foreach (string level in levels) {
			totalStars += PlayerPrefs.GetInt(level+"-Stars", 0);
        }
        enableScore = PlayerPrefs.GetInt(sceneName+"-StarsToUnlock", 0);

		if(totalStars >= enableScore || sceneName == "Garage") {
            unlocked = true;
        } else {
            unlocked = false;
        }

        if(!PlayerPrefs.HasKey("OldTotalStars")) {
            PlayerPrefs.SetInt("OldTotalStars", totalStars);
        }

        if(enableScore > PlayerPrefs.GetInt("OldTotalStars") && enableScore <= totalStars) {
            levelCamera.enabled = true;
            theCanvas.SetActive(false);
            PlayerPrefs.SetInt("OldTotalStars", totalStars);
            allowTouch = Mathf.FloorToInt(Time.time)+2;
            levelCamera.transform.position = new Vector3(this.transform.position.x + 30.0f, this.transform.position.y + 26.0f, this.transform.position.z);
            panCamera = true;
        }
    }

    void FixedUpdate () {
        if(panCamera) {
            levelCamera.transform.Translate(Vector3.right * Time.deltaTime * 4);
            levelCamera.transform.LookAt(this.transform);
            if(Time.time >= allowTouch){
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)){
                    levelCamera.enabled = false;
                    theCanvas.SetActive(true);
                    panCamera = false;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {

        //playButton.interactable = false;
        dotTruckController.slowCar(true);

        if(other.gameObject.tag == "Player") {
            if (sceneName != "" && sceneName != null)
            {
                if(sceneName == "Garage") {
                    worldRecordText.text = "Garage";
                    playButton.interactable = true;
                } else if((Application.internetReachability != NetworkReachability.NotReachable) ||
        			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
        			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)) {
                    StartCoroutine(downloadWorldRecordGhost());
                } else {
                    worldRecordText.text = "No Net";
                    playButton.interactable = true;
                }

                //If they have unlocked this map
                if(unlocked) {
                    unlockedGO.SetActive(true);
                    lockedGO.SetActive(false);
                } else {
                    starsNeeded.text = totalStars + "/" + enableScore;
                    unlockedGO.SetActive(false);
                    lockedGO.SetActive(true);
                }

                UIManagerScript.EnableBoolAnimator(openPanelAnim);
                this.levelText.text = levelName;
                this.StarInfo.text = threeStar.ToString("F3")+"\n"+fourStar.ToString("F3")+"\n"+fiveStar.ToString("F3");
                PlayerPrefs.SetFloat("twoStar", twoStar);
                PlayerPrefs.SetFloat("threeStar", threeStar);
                PlayerPrefs.SetFloat("fourStar", fourStar);
                PlayerPrefs.SetFloat("fiveStar", fiveStar);

                float highscore = 999;
                //Set Player PB Text
                if(PlayerPrefs.HasKey(sceneName+"highscore")) {
        			highscore = PlayerPrefs.GetFloat(sceneName+"highscore", 0);
                    if(highscore < 60) {
        			    this.personalBestText.text = "PB: 0:"+highscore.ToString("F3");
                    } else {
                        this.personalBestText.text = "PB: "+highscore.ToString("F3");
                    }

                    //Set Stars
                    star1.sprite = starSelected;
                    if(highscore < twoStar) {
                        star2.sprite = starSelected;
                    }
                    if(highscore < threeStar) {
                        star3.sprite = starSelected;
                    }
                    if(highscore < fourStar) {
                        star4.sprite = starSelected;
                    }
                    if(highscore < fiveStar) {
                        star5.sprite = starSelected;
                    }
        		} else {
                    this.personalBestText.text = "0:00.000";
                }

                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(LoadLevel);
                playButton.onClick.AddListener(ClosePanel);

            }
        }
    }

    public IEnumerator downloadWorldRecordGhost() {

        ErrorText.text += "\nDownloading World Record Ghost";

        WWW wwwHighscores = new WWW("https://www.undivided.games/DriftWorlds/GetScore.php?level="+sceneName);
        while (!wwwHighscores.isDone) {
            yield return null;
        }

        if (wwwHighscores.error != null)
        {
            ErrorText.text += "\nThere was an error getting the high score: " + wwwHighscores.error;
        } else {
            string[] szSplited = wwwHighscores.text.Split(',');
            ErrorText.text += "\nHighscore returned: "+szSplited[0]+" "+szSplited[1]+" "+szSplited[2];

            WRghostID = szSplited[0];
            WRcarUsed = int.Parse(szSplited[1]);
            timeTaken = float.Parse(szSplited[2]);
        }

        if(timeTaken < 60) {
            worldRecordText.text = "WR: 0:"+string.Format("{0:0.000}", timeTaken);
        } else {
            //worldRecordText.text = "WR: "+string.Format("{0:0.000}", timeTaken);
            string minutes = ((int) timeTaken / 60).ToString(); //Calculate minutes past
			string seconds = (timeTaken % 60).ToString("f3"); //Calculate seconds pass to 2 decimal places

			worldRecordText.text = "WR: " + minutes + ":" + seconds; //Assign time to text gameObject
        }
        //Set ghost prefs for selected level
        PlayerPrefs.SetFloat("ghostTimeTaken", timeTaken);
        PlayerPrefs.SetFloat("thirdPlaceGhost", timeTaken); //Get third place ghost score so we can update top 3 inside the level
        PlayerPrefs.SetString("ghostGhostID", WRghostID);
        PlayerPrefs.SetInt("ghostCarUsed", WRcarUsed);

        playButton.interactable = true;
        //Enable play button
	}

    void OnTriggerExit(Collider other) {
        closePanel();
        star1.sprite = starUnselected;
        star2.sprite = starUnselected;
        star3.sprite = starUnselected;
        star4.sprite = starUnselected;
        star5.sprite = starUnselected;
    }

    public void closePanel() {
        dotTruckController.slowCar(false);
        worldRecordText.text = "Loading";
        playButton.interactable = false;
        UIManagerScript.DisableBoolAnimator(openPanelAnim);
    }

    void ClosePanel() {
        modalPanelObject.SetActive(false);
    }

    void LoadLevel() {
        // Show an ad:
        if((Application.internetReachability != NetworkReachability.NotReachable) ||
			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) &&
			PlayerPrefs.GetString("NoAds") != "NoAds") {
			UnityAdsManager.Instance.ShowRegularAd(OnAdClosed);
		} else {
            if(sceneName == "Garage") {
                PlayerPrefs.SetInt("loadGarage", 1);
            }
            LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
    		loadingscreen.LoadLevel(sceneName);
		}
    }

    private void OnAdClosed(ShowResult result) {
        if(sceneName == "Garage") {
            PlayerPrefs.SetInt("loadGarage", 1);
        }
        LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
		loadingscreen.LoadLevel(sceneName);
    }


}
