using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using TMPro;

[System.Serializable]
public class WB_Vector3 {

	private float x;
	private float y;
	private float z;

	public WB_Vector3() { }
	public WB_Vector3(Vector3 vec3) {
		this.x = vec3.x;
		this.y = vec3.y;
		this.z = vec3.z;
	}

	public static implicit operator WB_Vector3(Vector3 vec3) {
		return new WB_Vector3(vec3);
	}
	public static explicit operator Vector3(WB_Vector3 wb_vec3) {
		return new Vector3(wb_vec3.x, wb_vec3.y, wb_vec3.z);
	}
}

[System.Serializable]
public class WB_Quaternion {

    private float w;
	private float x;
	private float y;
	private float z;

	public WB_Quaternion() { }
	public WB_Quaternion(Quaternion quat3) {
		this.x = quat3.x;
		this.y = quat3.y;
		this.z = quat3.z;
        this.w = quat3.w;
	}

	public static implicit operator WB_Quaternion(Quaternion quat3) {
		return new WB_Quaternion(quat3);
	}
	public static explicit operator Quaternion(WB_Quaternion wb_quat3) {
		return new Quaternion(wb_quat3.x, wb_quat3.y, wb_quat3.z, wb_quat3.w);
	}
}

[System.Serializable]
public class GhostShot
{
    public float timeMark = 0.0f;       // mark at which the position and rotation are of af a given shot

    private WB_Vector3 _posMark;
    public Vector3 posMark {
		get {
			if (_posMark == null) {
				return Vector3.zero;
			} else {
				return (Vector3)_posMark;
			}
		}
		set {
			_posMark = (WB_Vector3)value;
		}
	}

    private WB_Quaternion _rotMark;
    public Quaternion rotMark {
		get {
			if (_rotMark == null) {
				return Quaternion.identity;
			} else {
				return (Quaternion)_rotMark;
			}
		}
		set {
			_rotMark = (WB_Quaternion)value;
		}
	}

	public bool acceleration, turnLeft, turnRight;
	public float carSoundRPM;

	//Wheel Rotation
	private WB_Quaternion _wheelRotMarkLeft;
    public Quaternion wheelrotMarkLeft {
		get {
			if (_wheelRotMarkLeft == null) {
				return Quaternion.identity;
			} else {
				return (Quaternion)_wheelRotMarkLeft;
			}
		}
		set {
			_wheelRotMarkLeft = (WB_Quaternion)value;
		}
	}
	//Wheel Rotation
	private WB_Quaternion _wheelRotMarkRight;
    public Quaternion wheelrotMarkRight {
		get {
			if (_wheelRotMarkRight == null) {
				return Quaternion.identity;
			} else {
				return (Quaternion)_wheelRotMarkRight;
			}
		}
		set {
			_wheelRotMarkRight = (WB_Quaternion)value;
		}
	}
    //public GhostShot() { }
}

public class Ghost : MonoBehaviour {

    private List<GhostShot> framesList;
    private List<GhostShot> lastReplayList = null;
	private List<GhostShot> saveReplayList = null;
	private List<GhostShot> demoReplayList = null;

  	GameObject car;

    private float replayTimescale = 1;
    private int replayIndex = 0;
    private float recordTime = 0.0f;
    private float replayTime = 0.0f;

    //Check whether we should be recording or not
    bool startRecording = true, recordingFrame = false;

	//Get player car number
	int playerCar = 0;

    //Ghost and Demo Cars
    GameObject theGhost, theDemo;

	//Get scene information
    Scene scene;

	//Debug testing info
	public Text ErrorLogText;
	public Text DownloadingText;
	public GameObject downloadingPanel;

	public TMP_Text personalHighscoreText;
	public TMP_Text globalHighscoreText;
	private float personalHighscore;
	private float globalHighscore = 0;

	//Check whether we cre showing the intro demo
	private bool runningDemo = false;
	public ResetPlayer resetPlayer; // Used to reset demo car
	public GameObject canvasTop, canvasLeft, canvasBottom, canvasTapToContinue;

	//Allow touch after this many seconds
	private int allowTouch = 1;

	//Used only to get sound Rpm
	private RealisticEngineSound realisticEngineSound;
	private Dot_Truck_Controller dotTruckController;
	private Transform wheelRecordRight, wheelRecordLeft, wheelRotationBL, wheelRotationBR, wheelRotationFL, wheelRotationFR;

	public GameObject newRecord;

	//int rank;

    // Use this for initialization
    void Start () {

		//Get rank for ghost information
		//rank = PlayerPrefs.GetInt("Rank");

		scene = SceneManager.GetActiveScene(); //Get scene name

		//Get Demo recording
		Debug.Log("Getting pre saved Demo Ghost");
		TextAsset textAsset = Resources.Load("Demos/" + scene.name + "Demo") as TextAsset;
		Stream stream = new MemoryStream(textAsset.bytes);
		BinaryFormatter formatter = new BinaryFormatter();
		//MyClass myInstance = formatter.Deserialize(stream) as MyClass;
		demoReplayList = (List<GhostShot>)formatter.Deserialize(stream);
		stream.Close();
		////////////////////

		allowTouch = Mathf.FloorToInt(Time.time)+2; //Allow touch after 2 seconds

		canvasTop.SetActive(false); //Hide HUD while demo finishes playing
		canvasLeft.SetActive(false);
		canvasBottom.SetActive(false);

		theDemo = GameObject.FindWithTag("Demo"); //Get Demo car
		//Get demo wheels to rotate
		wheelRotationBL = theDemo.transform.Find("Wheel_Back_L");
		wheelRotationBR = theDemo.transform.Find("Wheel_Back_R");
		wheelRotationFL = theDemo.transform.Find("Wheel_L");
		wheelRotationFR = theDemo.transform.Find("Wheel_R");

		runningDemo = true; //Play Demo run through of level first

		//Get User Record Highscore
		if(PlayerPrefs.HasKey(scene.name+"highscore")) {
			personalHighscore = PlayerPrefs.GetFloat(scene.name+"highscore");
			personalHighscoreText.text = "0:"+string.Format("{0:0.000}", personalHighscore);
		} else {
			personalHighscore = 99999;
			personalHighscoreText.text = "None";
		}

		//Get World Record Highscore
		if(PlayerPrefs.HasKey("ghostTimeTaken")) {
			globalHighscore = PlayerPrefs.GetFloat("ghostTimeTaken");
			globalHighscoreText.text = "0:"+string.Format("{0:0.000}", globalHighscore);
		} else {
			globalHighscore = 99999;
			globalHighscoreText.text = "None";
		}

		playerCar = PlayerPrefs.GetInt("playerCar", 0); //Get player car number
        car = GameObject.FindWithTag("Player"); // Get player car
		wheelRecordLeft = car.transform.Find("Wheel_L"); //Get wheel to record
		wheelRecordRight = car.transform.Find("Wheel_Back_R"); //Get wheel to record

		realisticEngineSound = car.transform.GetChild(0).GetComponent<RealisticEngineSound>(); //Get sound to save to ghost
		dotTruckController = car.GetComponent<Dot_Truck_Controller>(); //Get motor to spin ghost wheels

 		//If we have set the game to play your own personal ghost
		if(PlayerPrefs.GetString("personalOrGhost") == "personal") {

			LogError("Starting Personal Ghost");

			//Duplicate the car and add ghost shader to it
			CreateGhost(playerCar);

			//Check if Ghost file exists. If it does load it
	        if(File.Exists(Application.persistentDataPath + "/" + scene.name+"-ghost")) {
	            BinaryFormatter bf = new BinaryFormatter();
	            FileStream file = File.Open(Application.persistentDataPath + "/" + scene.name+"-ghost", FileMode.Open);
	            lastReplayList = (List<GhostShot>)bf.Deserialize(file);
	            file.Close();
	        } else {
				LogError("No Local File Exist");
			}

		//If we have set the ghost to be world record make sure we have all the info for it first
		} else if(PlayerPrefs.GetString("personalOrGhost") == "ghost" && PlayerPrefs.HasKey("ghostTimeTaken") && PlayerPrefs.HasKey("ghostGhostID") && PlayerPrefs.HasKey("ghostCarUsed")) {

			LogError("Loading Global Ghost");

			//Create a ghost as the world record holders car
			CreateGhost(PlayerPrefs.GetInt("ghostCarUsed"));

	        //Download ghost file from web server
			StartCoroutine(DownloadFile());

		} else {
			LogError("Does not want to race a ghost");
		}
    }

	// Update is called once per frame
	void FixedUpdate () {

		if(car != null && !runningDemo) {
			if (startRecording) {
				newRecord.SetActive(false); //Show new record banner
				startRecording = false;
				StartRecording();
			} else if (recordingFrame) {
				RecordFrame();
			}
			if (lastReplayList != null) {
				MoveGhost(lastReplayList);
			}
		} else if (runningDemo) {
			if (demoReplayList != null) {
				MoveGhost(demoReplayList); //demoReplayList
			}
			if(Time.time >= allowTouch){
				canvasTapToContinue.SetActive(true);
				if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)){
					canvasTop.SetActive(true); //Turn HUD on
					canvasLeft.SetActive(true);
					canvasBottom.SetActive(true);
					canvasTapToContinue.SetActive(false);
					runningDemo = false; //Stop playing demo
					GameObject.Find("Demo_Camera").SendMessage("ChangeCamera"); //Change camera to player car
					Destroy(theDemo); //Kill the demo car
					resetPlayer.resetPlayer(); //Reset the player
				}
			}
		}

	}

	public IEnumerator DownloadFile()
    {
		WWW www = new WWW("https://www.undivided.games/DriftWorlds/Ghosts/" + scene.name + "ghost");
		downloadingPanel.SetActive(true);
		while (!www.isDone) {
			DownloadingText.text = "Downloaded Ghost: " + (www.progress*100).ToString("F3") + "%...";
            yield return null;
        }
		downloadingPanel.SetActive(false);

		//Write downloaded file to new local file
		string fullPath = Application.persistentDataPath + "/" + scene.name + "-record";
        File.WriteAllBytes (fullPath, www.bytes);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/" + scene.name + "-record", FileMode.Open);
		lastReplayList = (List<GhostShot>)bf.Deserialize(file);
		file.Close();

		LogError("Downloading World Record Finished");
    }

	private void RecordFrame() {
		recordTime += Time.smoothDeltaTime * 1000;
        GhostShot newFrame = new GhostShot()
        {
            timeMark = recordTime,
			posMark = car.transform.position,
			rotMark = car.transform.rotation,
			carSoundRPM = realisticEngineSound.getRPM(),
			wheelrotMarkRight = wheelRecordRight.rotation,
			wheelrotMarkLeft = wheelRecordLeft.rotation
        };
        framesList.Add(newFrame);
	}

	public void StartRecording() {
        framesList = new List<GhostShot>();
        replayIndex = 0;
        recordTime = Time.time * 1000;
        recordingFrame = true;
    }

	public void StopRecordingGhost(float score) {
		recordingFrame = false;

		//New global highscore which also means new personal highscore
		if(score < globalHighscore) {

			newRecord.SetActive(true); //Show new record banner

			//Create new ghost with current records car
			/*if(PlayerPrefs.GetString("personalOrGhost") == "personal") {
				Destroy(theGhost);
				CreateGhost(playerCar);
			}*/

			LogError("Setting Global Highscore");

			globalHighscore = score;
			personalHighscore = score;

			//Set player prefs highscore to newly achieved highscore
			PlayerPrefs.SetFloat(scene.name+"highscore", score);

			personalHighscoreText.text = "0:"+string.Format("{0:0.000}", score);
			globalHighscoreText.text = "0:"+string.Format("{0:0.000}", score);

			lastReplayList = new List<GhostShot>(framesList);
			saveReplayList = new List<GhostShot>(framesList);
			SaveGhostToFile();
			UploadGhost();

		//New Personal High Score
		} else if(score < personalHighscore) {

			newRecord.SetActive(true);

			//Create new ghost with current records car
			/*if(PlayerPrefs.GetString("personalOrGhost") == "ghost") {
				Destroy(theGhost);
				CreateGhost(playerCar);
			}*/

			LogError("Setting Personal Highscore");

			personalHighscore = score;

			personalHighscoreText.text = "0:"+string.Format("{0:0.000}", score);

			//Set player prefs highscore to newly achieved highscore
			PlayerPrefs.SetFloat(scene.name+"highscore", score);

			//If racing personal ghost set new ghost to personal record just achieved
			if(PlayerPrefs.GetString("personalOrGhost") == "personal") {
				lastReplayList = new List<GhostShot>(framesList);
			}
			saveReplayList = new List<GhostShot>(framesList);
            SaveGhostToFile();
			if(score < PlayerPrefs.GetFloat("thirdPlaceGhost")) {
				StartCoroutine(saveGhostDataToDB());
			}

        } else if(score < PlayerPrefs.GetFloat("thirdPlaceGhost")) {
			StartCoroutine(saveGhostDataToDB());
		}
	}

    public void StopRecordingGhost() {
        startRecording = true;
    }

    public void MoveGhost(List<GhostShot> theReplay)
    {
        replayIndex++;

        if (replayIndex < theReplay.Count)
        {
            GhostShot frame = theReplay[replayIndex];
            DoLerp(theReplay[replayIndex - 1], frame);
            replayTime += Time.smoothDeltaTime * 1000 * replayTimescale;
        }
    }

    private void DoLerp(GhostShot a, GhostShot b)
    {
		if(!runningDemo) {
			//if(GameObject.FindWithTag("Ghost") != null) {
		        theGhost.transform.position = Vector3.Slerp(a.posMark, b.posMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
		        theGhost.transform.rotation = Quaternion.Slerp(a.rotMark, b.rotMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));

				//Wheel Rotation
				wheelRotationBL.rotation = Quaternion.Slerp(a.wheelrotMarkRight, b.wheelrotMarkRight, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
				wheelRotationBR.rotation = Quaternion.Slerp(a.wheelrotMarkRight, b.wheelrotMarkRight, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
				wheelRotationFL.rotation = Quaternion.Slerp(a.wheelrotMarkLeft, b.wheelrotMarkLeft, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
				wheelRotationFR.rotation = Quaternion.Slerp(a.wheelrotMarkLeft, b.wheelrotMarkLeft, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
			//}
		} else if(runningDemo) {
			//if(GameObject.FindWithTag("Demo") != null) {
		        theDemo.transform.position = Vector3.Slerp(a.posMark, b.posMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
		        theDemo.transform.rotation = Quaternion.Slerp(a.rotMark, b.rotMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));

				//Wheel Rotation
				wheelRotationBL.rotation = Quaternion.Slerp(a.wheelrotMarkRight, b.wheelrotMarkRight, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
				wheelRotationBR.rotation = Quaternion.Slerp(a.wheelrotMarkRight, b.wheelrotMarkRight, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
				wheelRotationFL.rotation = Quaternion.Slerp(a.wheelrotMarkLeft, b.wheelrotMarkLeft, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
				wheelRotationFR.rotation = Quaternion.Slerp(a.wheelrotMarkLeft, b.wheelrotMarkLeft, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
			//}
		}
    }

    public void SaveGhostToFile()
    {
		Debug.Log("Saving file");
        // Prepare to write
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + scene.name+"-ghost");

        // Write data to disk
        bf.Serialize(file, saveReplayList);
        file.Close();

    }

	public void UploadGhost () {
        StartCoroutine(UploadAFile());
		StartCoroutine(saveGhostDataToDB());
    }

	public IEnumerator UploadAFile() {
		byte[] bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + scene.name+"-ghost");
        // Create a Web Form, this will be our POST method's data
        var form = new WWWForm();
        form.AddBinaryData("theFile", bytes, scene.name + "ghost");

		//POST the file to DB
        WWW w = new WWW("https://www.undivided.games/DriftWorlds/UploadGhost.php", form);
		downloadingPanel.SetActive(true);
		while (!w.isDone) {
			DownloadingText.text = "Uploading Ghost: " + (w.progress*100).ToString("F3") + "%...";
            yield return null;
        }
		downloadingPanel.SetActive(false);

        //yield return w;

        if (w.error != null)
        {
            Debug.Log("UploadAFile Log !Null: "+w.error);
        }
        else
        {
            Debug.Log("UploadAFile Log Null: "+w.text);
        }
    }

	public IEnumerator saveGhostDataToDB() {

		string name = PlayerPrefs.GetString("Username");

		WWWForm form = new WWWForm();
		form.AddField("namePost", name);
		form.AddField("scorePost", personalHighscore.ToString());
		form.AddField("carusedPost", playerCar.ToString());
		form.AddField("level", scene.name);
		//form.AddField("rank", rank);
		WWW www = new WWW("https://www.undivided.games/DriftWorlds/SaveScore.php", form);
		yield return www;

		if (www.error != null)
        {
            LogError("UploadAFile Log !Null: "+www.error);
        }
        else
        {
            LogError("UploadAFile Log Null: "+www.text);
        }

		LogError("Uploaded score: "+name+" "+personalHighscore.ToString()+" "+playerCar+" "+scene.name);

	}

    public void CreateGhost(int carUsed)
    {
		//Check if ghost exists or not, no reason to destroy and create it everytime.
		if(GameObject.FindWithTag("Ghost") == null) {
	        theGhost = Instantiate(Resources.Load("Car"+carUsed, typeof(GameObject))) as GameObject;
	        theGhost.gameObject.tag = "Ghost";

	        //Disable controller script and RigidBody
	        theGhost.GetComponent<Dot_Truck_Controller>().enabled = false;
	        theGhost.GetComponent<Rigidbody>().isKinematic = true;

			Destroy (theGhost.GetComponent<Transform>().GetChild(0).gameObject); //Delete Audio Gameobject

	        foreach (Transform child in theGhost.transform)
	        {
	            if (child.name == "Dust")
	            {
	              //Delete Dust Particles
	              Destroy(child.gameObject);
	            }
	            else if (child.name == "wheels")
	            {
	                //Delete wheel colliders
	                Destroy(child.gameObject);
	            }
				else if (child.name == "WildFire")
	            {
	                //Delete wheel colliders
	                Destroy(child.gameObject);
	            }
				else if (child.name.Contains("Interior"))
	            {
	                //Delete interior
	                Destroy(child.gameObject);
	            }
				else if (child.name.Contains("break"))
	            {
	                //Delete break pads
	                Destroy(child.gameObject);
	            }
	            else if (child.name.Contains("Body"))
	            {
	                //Turn off mesh collider
	                child.gameObject.GetComponent<Collider>().enabled = false;

	                //Change to ghost transparent material
	                MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
	                mr.material = Resources.Load("Ghost_Shader", typeof(Material)) as Material;
	            }
	            else if (child.name.Contains("Taillight") || child.name.Contains("Headlight"))
	            {
	                //Delete Lights
	                Destroy(child.gameObject);
	            }
	            else if (child.name.Contains("Matte") || child.name == "Wheel_Back_L" || child.name == "Wheel_Back_R" || child.name == "Wheel_L" || child.name == "Wheel_R")
	            {
	                //Change to ghost transparent material
	                MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
	                mr.material = Resources.Load("Ghost_Shader", typeof(Material)) as Material;
	            }
	        }
		}
    }

	void LogError(string errorString) {
		Debug.Log(errorString);
		ErrorLogText.text += "\n"+errorString;
	}

	public bool playingDemo() {
		return runningDemo;
	}

	public float getDemoSoundRPM() {
		return demoReplayList[replayIndex].carSoundRPM;
	}

}
