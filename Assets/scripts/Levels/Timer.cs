using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour {

	public TMP_Text timerText; //GUI text to display the time
	private float startTime; //Time player started/reset the map
	private bool finished = false; //Check if player has crossed the finish line

	public Ghost ghostScript;
	public CameraController cameraController;

	private float timeScore; // The final time/score the player gets on the level
	float t;

	float twoStar;
	float threeStar;
	float fourStar;
	float fiveStar;

	public AudioSource failSound;

	public Image star1, star2, star3, star4, star5;
	public Sprite starSelected, starUnselected;

	//Finished screen variables
	public TMP_Text scoreNames;
	public TMP_Text scoreScores;
	public Sprite starSelectedEnd;
	public Image starEnd1, starEnd2, starEnd3, starEnd4, starEnd5;
	public UIManager UIManagerScript;
	public Animator openPanelAnim;
	public TMP_Text endScore;

	private string[] szSplited;
	private bool showLeaderboard = true;

	//Reset total drift score on reset
	public DriftScoreManager driftScoreManager;

	//Slow Car on Level complete
	Dot_Truck_Controller dotTruck;

	//End Screen Headings
	public TMP_Text leaderboardText, starsText;

	//Reset Button
	public Button playButton;

	//Get scene information
    Scene scene;

	//Start and stop fireworks
	public ParticleSystem[] fireworks;

	// Use this for initialization
	void Start () {

		//Set sound effects audio
		failSound.volume = PlayerPrefs.GetFloat("musicVolume");

		for (int i = 0; i < fireworks.Length; i++) {
			fireworks[i].Stop();
		}

		dotTruck = GameObject.FindWithTag("Player").GetComponent<Dot_Truck_Controller>();

		scene = SceneManager.GetActiveScene(); //Get scen name

		startTime = Time.time; //Set start time to current time

		twoStar = PlayerPrefs.GetFloat("twoStar", 0);
		threeStar = PlayerPrefs.GetFloat("threeStar", 0);
		fourStar = PlayerPrefs.GetFloat("fourStar", 0);
		fiveStar = PlayerPrefs.GetFloat("fiveStar", 0);

		star1.sprite = starSelected;
		star2.sprite = starSelected;
		star3.sprite = starSelected;
		star4.sprite = starSelected;
		star5.sprite = starSelected;

		if((Application.internetReachability != NetworkReachability.NotReachable) ||
			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)) {
			StartCoroutine(downloadWorldRecord());
		}

	}

	public IEnumerator downloadWorldRecord() {

        WWW wwwHighscores = new WWW("https://www.undivided.games/DriftWorlds/GetScore.php?level="+scene.name);
        yield return wwwHighscores;
        if (wwwHighscores.error != null) {
            print("There was an error getting the high score: " + wwwHighscores.error);
        } else {
            szSplited = wwwHighscores.text.Split(',');

            setScorePanel();
        }
	}

	public void insertNewHighscore() {
		if(timeScore < float.Parse(szSplited[2])) { //If score is number 1

			szSplited[8] = szSplited[5];
			szSplited[5] = szSplited[2];
			szSplited[2] = timeScore.ToString();

			szSplited[6] = szSplited[3];
			szSplited[3] = szSplited[0];
			szSplited[0] = "<color=orange>"+PlayerPrefs.GetString("Username")+"</color>";

			setScorePanel();

		} else if(timeScore < float.Parse(szSplited[5])) { //If score is number 1

			szSplited[8] = szSplited[5];
			szSplited[5] = timeScore.ToString();

			szSplited[6] = szSplited[3];
			szSplited[3] = "<color=orange>"+PlayerPrefs.GetString("Username")+"</color>";

			setScorePanel();

		} else if(timeScore < float.Parse(szSplited[8])) { //If score is number 1

			szSplited[8] = timeScore.ToString();
			szSplited[6] = "<color=orange>"+PlayerPrefs.GetString("Username")+"</color>";

			setScorePanel();
		}
	}

	public void setScorePanel() {
		scoreNames.text = szSplited[0]+"\n"+szSplited[3]+"\n"+szSplited[6];
		scoreScores.text = convertSecondsToMinutes(szSplited[2])+"\n"+convertSecondsToMinutes(szSplited[5])+"\n"+convertSecondsToMinutes(szSplited[8]);
	}

	public void changeScores(string buttonPressed) {

		if(buttonPressed == "lb") {
			showLeaderboard = true;
			leaderboardText.color = new Color32(255, 255, 255, 255);
			starsText.color = new Color32(156, 156, 156, 255);

		} else {
			showLeaderboard = false;
			leaderboardText.color = new Color32(156, 156, 156, 255);
			starsText.color = new Color32(255, 255, 255, 255);
		}

		if(showLeaderboard) {
			scoreNames.text = szSplited[0]+"\n"+szSplited[3]+"\n"+szSplited[6];
			scoreScores.text = convertSecondsToMinutes(szSplited[2])+"\n"+convertSecondsToMinutes(szSplited[5])+"\n"+convertSecondsToMinutes(szSplited[8]);
		} else {
			scoreNames.text = "5 Stars:\n4 Stars:\n3 Stars:";
			scoreScores.text = fiveStar.ToString("F3")+"\n"+fourStar.ToString("F3")+"\n"+threeStar.ToString("F3");
		}

	}

	string convertSecondsToMinutes(string theScore) {
		string minutes = ((int) float.Parse(theScore) / 60).ToString(); //Calculate minutes past
		string seconds = (float.Parse(theScore) % 60).ToString("f3"); //Calculate seconds pass to 2 decimal places
		return minutes + ":" + seconds; //Assign time to text gameObject
	}

	void Update () {
		if(!finished){
			t = Time.time - startTime;

			string minutes = ((int) t / 60).ToString(); //Calculate minutes past
			string seconds = (t % 60).ToString("f2"); //Calculate seconds pass to 2 decimal places

			timerText.text = minutes + ":" + seconds; //Assign time to text gameObject
			timeScore = t;
			changeStars();
		}
	}

	public void resetTimer() {
		//Stop fireworks
		for (int i = 0; i < fireworks.Length; i++) {
			fireworks[i].Stop();
		}

		dotTruck.slowCar(false);
		star1.sprite = starSelected;
		star2.sprite = starSelected;
		star3.sprite = starSelected;
		star4.sprite = starSelected;
		star5.sprite = starSelected;

		//Reset driftscore total
		driftScoreManager.resetTotal();

		cameraController.FinishedCamera(false);
		startTime = Time.time; //Set start time to current time
		finished = false; //Used to reset a finished time
		ghostScript.StopRecordingGhost(); //Reset Recording immediately

	}

	public void Finished() {
		playFireworks();

		dotTruck.slowCar(true);

		//We won't have highscores if there's no internet
		if((Application.internetReachability != NetworkReachability.NotReachable) ||
			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)) {
				insertNewHighscore(); //Check to see if we beat any of the top 3 scores
			}

		finished = true; //Pause timer at current time

		//Set stars on end panel based on score
		starEnd1.sprite = starSelectedEnd;
		if(timeScore < twoStar)	 {
			starEnd2.sprite = starSelectedEnd;
			if(PlayerPrefs.GetInt(scene.name+"-Stars", 0) < 2) {
				PlayerPrefs.SetInt(scene.name+"-Stars", 2);
			}
		}
		if(timeScore < threeStar) 	{
			starEnd3.sprite = starSelectedEnd;
			if(PlayerPrefs.GetInt(scene.name+"-Stars", 0) < 3) {
				PlayerPrefs.SetInt(scene.name+"-Stars", 3);
			}
		}
		if(timeScore < fourStar) {
			starEnd4.sprite = starSelectedEnd;
			if(PlayerPrefs.GetInt(scene.name+"-Stars", 0) < 4) {
				PlayerPrefs.SetInt(scene.name+"-Stars", 4);
			}
		}
		if(timeScore < fiveStar) {
			starEnd5.sprite = starSelectedEnd;
			if(PlayerPrefs.GetInt(scene.name+"-Stars", 0) < 5) {
				PlayerPrefs.SetInt(scene.name+"-Stars", 5);
			}
		}
		endScore.text = (timeScore % 60).ToString("f3"); //Set score in end panel text

		driftScoreManager.addToPlayerCash(); //Add total drift score to money

		UIManagerScript.EnableBoolAnimator(openPanelAnim); //Open end screen panel

		ghostScript.StopRecordingGhost(timeScore); //Stop recording and update ghost if highscore is better
		cameraController.FinishedCamera(true);
		//FindObjectOfType<PlayerPrefsController>().setPlayerScore(timeScore);

		playButton.onClick.RemoveAllListeners();
		playButton.onClick.AddListener(ClosePanel);
	}

	private void playFireworks() {
		for (int i = 0; i < fireworks.Length; i++) {
			StartCoroutine(playFireworksDelay(Random.Range(0, 3), fireworks[i]));
		}
	}

	IEnumerator playFireworksDelay(float time, ParticleSystem fw) {
		yield return new WaitForSeconds(time);
		fw.Play();
	}

	public void changeStars() {
		if(t < twoStar && star1.sprite.ToString().IndexOf("star-grey") != -1) {
			failSound.PlayOneShot(failSound.clip, 0.50f); //Fail audio
			star1.sprite = starUnselected;
		} else if(t > twoStar && star2.sprite.ToString().IndexOf("star-grey") != -1) {
			failSound.PlayOneShot(failSound.clip, 0.50f); //Fail audio
			star2.sprite = starUnselected;
		} else if(t > threeStar && star3.sprite.ToString().IndexOf("star_glow") != -1) {
			failSound.PlayOneShot(failSound.clip, 0.50f); //Fail audio
			star3.sprite = starUnselected;
		} else if(t > fourStar && star4.sprite.ToString().IndexOf("star_glow") != -1) {
			failSound.PlayOneShot(failSound.clip, 0.50f); //Fail audio
			star4.sprite = starUnselected;
		} else if(t > fiveStar && star5.sprite.ToString().IndexOf("star_glow") != -1) {
			failSound.PlayOneShot(failSound.clip, 0.50f); //Fail audio
			star5.sprite = starUnselected;
		}
	}

	void OnTriggerExit(Collider other) {
        UIManagerScript.DisableBoolAnimator(openPanelAnim);
    }

    void ClosePanel() {
        UIManagerScript.DisableBoolAnimator(openPanelAnim);
    }


}
