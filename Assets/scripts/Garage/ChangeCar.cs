using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;

public class ChangeCar : MonoBehaviour {

	public Vector3 startPosition;

	int currentCar, selectedCar; //Hold position of current car and previous car

	GameObject[] theCar; //Array holding the cars in the scene

	bool changeCar = false;

	public Image speedLine, speedUnderline;
	public Image accelerationLine, accelerationUnderline;
	public Image handlingLine, handlingUnderline;

	public TMP_Text carPrice;
	public GameObject carPriceGO;
	Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>();

	//Hide when car is purchased
	public GameObject BuyGO;
	public Button keyImage;

	//Change player cash
	public TMP_Text playerCash;

	//Engine Start Audio
	public AudioSource engineStartSource;

	//Spray Paint Audio
	public AudioSource sprayPaintSource;

	//Cash Audio
	public AudioSource cashSource;

	//Car Color
	Color newColor;

	//Check if this is first time in the garage
	private bool carSet = true;

	//First time garage script
	GarageTutorial garageTutorial;

	//Character Animation
	public GameObject character;
	private Animator anim;

	//colors
	float transitionTime = 45.0f; //Time it takes to paint a car
	public GameObject colorsParent;
	public GameObject paintGO, paintSpecGO, paintMoney, paintSpecMoney;
	private string selectedColor;
	private string globalColorName; //A way to have the selected color name on hand

	Dictionary <string, string> myColors = new Dictionary <string, string> ();
	Dictionary <string, string> myColorsSpec = new Dictionary <string, string> ();

	//CHange car suspension
	ChangeWheelColliders changeWheelColliders;

	//Upgrade Car
	public GameObject confirmDialog;
	private string statChose;
	public TMP_Text starNumber;

	//Update player cash
	private MoneyController moneyController;

	//Car stats limits
	private int handlingMin = 15, handlingMax = 25;
	private int speedMin = 60, speedMax = 100;
	private int accellerationMin = 80, accellerationMax = 120;

	// Use this for initialization
	void Start () {

		myColors.Add("Red", "205000000");
		myColors.Add("Orange", "255161000");
		myColors.Add("Yellow", "255236000");
		myColors.Add("Green", "000168019");
		myColors.Add("Blue", "000102169");
		myColors.Add("Purple", "156000255");
		myColors.Add("Pink", "255000184");
		myColors.Add("Brown", "154048000");
		myColors.Add("White", "255255255");
		myColors.Add("Silver", "133133133");
		myColors.Add("Black", "000000000");

		myColorsSpec.Add("RedSpec", "205000000");
		myColorsSpec.Add("OrangeSpec", "255161000");
		myColorsSpec.Add("YellowSpec", "255236000");
		myColorsSpec.Add("GreenSpec", "000168019");
		myColorsSpec.Add("BlueSpec", "000102169");
		myColorsSpec.Add("PurpleSpec", "156000255");
		myColorsSpec.Add("PinkSpec", "255000184");
		myColorsSpec.Add("BrownSpec", "154048000");
		myColorsSpec.Add("WhiteSpec", "255255255");
		myColorsSpec.Add("SilverSpec", "133133133");
		myColorsSpec.Add("BlackSpec", "000000000");

		garageTutorial = GetComponent<GarageTutorial>();

		changeWheelColliders = GetComponent<ChangeWheelColliders>();

		playerCash.text = "$"+PlayerPrefs.GetInt("playerCash", 0).ToString("n0");
		moneyController = GetComponent<MoneyController>();

		if(PlayerPrefs.HasKey("playerCar")) {
			anim = character.GetComponent<Animator>();
			anim.Play("Lean", 0, 0);
			character.transform.position = new Vector3(261.834f, 23.417f, 206.374f);
			currentCar = PlayerPrefs.GetInt("playerCar", 0);
			selectedCar = currentCar;
		} else {
			currentCar = 23;
			selectedCar = 23;
			carSet = false;
		}

		theCar = new GameObject[40];

	    theCar[currentCar] = Instantiate(Resources.Load("Car"+selectedCar, typeof(GameObject)), startPosition, transform.rotation) as GameObject;

		//Disable dust and flames
		theCar[currentCar].transform.GetChild(2).gameObject.SetActive(false);
		theCar[currentCar].transform.GetChild(3).gameObject.SetActive(false);

		if(carSet) {
			ownedCar();
		}

		ChangeWheelColliders.changeWheelCollider(theCar[currentCar]);

		/*Set Up array of car variables
		Speed 70-100
		Acceleration 90-120
		Handling 30-45
		*/
		dict.Add("1", new Dictionary<string, object>()
		{
			{ "Price", 15400 },
			{ "Speed", 70 },
			{ "Acceleration", 90 },
			{ "Handling", 17 }
		});

		dict.Add("2", new Dictionary<string, object>()
		{
			{ "Price", 17990 },
			{ "Speed", 70 },
			{ "Acceleration", 92 },
			{ "Handling", 18 }
		});

		dict.Add("3", new Dictionary<string, object>()
		{
			{ "Price", 25880 },
			{ "Speed", 73 },
			{ "Acceleration", 94 },
			{ "Handling", 19 }
		});

		dict.Add("4", new Dictionary<string, object>()
		{
			{ "Price", 26000 },
			{ "Speed", 78 },
			{ "Acceleration", 99 },
			{ "Handling", 21 }
		});

		dict.Add("5", new Dictionary<string, object>()
		{
			{ "Price", 35670 },
			{ "Speed", 76 },
			{ "Acceleration", 97 },
			{ "Handling", 22 }
		});
		dict.Add("6", new Dictionary<string, object>()
		{
			{ "Price", 42000 },
			{ "Speed", 84 },
			{ "Acceleration", 100 },
			{ "Handling", 22 }
		});

		dict.Add("7", new Dictionary<string, object>()
		{
			{ "Price", 46590 },
			{ "Speed", 82 },
			{ "Acceleration", 95 },
			{ "Handling", 21 }
		});

		dict.Add("8", new Dictionary<string, object>()
		{
			{ "Price", 48099 },
			{ "Speed", 85 },
			{ "Acceleration", 100 },
			{ "Handling", 19 }
		});

		dict.Add("9", new Dictionary<string, object>()
		{
			{ "Price", 52590 },
			{ "Speed", 79 },
			{ "Acceleration", 100 },
			{ "Handling", 23 }
		});

		dict.Add("10", new Dictionary<string, object>()
		{
			{ "Price", 59670 },
			{ "Speed", 84 },
			{ "Acceleration",98 },
			{ "Handling", 23 }
		});

		dict.Add("11", new Dictionary<string, object>()
		{
			{ "Price", 64909 },
			{ "Speed", 77 },
			{ "Acceleration", 103 },
			{ "Handling", 24 }
		});

		dict.Add("12", new Dictionary<string, object>()
		{
			{ "Price", 70900 },
			{ "Speed", 79 },
			{ "Acceleration", 99 },
			{ "Handling", 24 }
		});

		dict.Add("13", new Dictionary<string, object>()
		{
			{ "Price", 75650 },
			{ "Speed", 80 },
			{ "Acceleration", 112 },
			{ "Handling", 25 }
		});

		dict.Add("14", new Dictionary<string, object>()
		{
			{ "Price", 80000 },
			{ "Speed", 85 },
			{ "Acceleration", 106 },
			{ "Handling", 25 }
		});

		dict.Add("15", new Dictionary<string, object>()
		{
			{ "Price", 88600 },
			{ "Speed", 94 },
			{ "Acceleration", 112 },
			{ "Handling", 23 }
		});

		dict.Add("16", new Dictionary<string, object>()
		{
			{ "Price", 100000 },
			{ "Speed", 95 },
			{ "Acceleration", 108 },
			{ "Handling", 25 }
		});

		dict.Add("17", new Dictionary<string, object>()
		{
			{ "Price", 120000 },
			{ "Speed", 93 },
			{ "Acceleration", 104 },
			{ "Handling", 25 }
		});

		dict.Add("18", new Dictionary<string, object>()
		{
			{ "Price", 154670 },
			{ "Speed", 93 },
			{ "Acceleration", 115 },
			{ "Handling", 22 }
		});

		dict.Add("19", new Dictionary<string, object>()
		{
			{ "Price", 459850 },
			{ "Speed", 93 },
			{ "Acceleration", 112 },
			{ "Handling", 23 }
		});

		dict.Add("20", new Dictionary<string, object>()
		{
			{ "Price", 1000000 },
			{ "Speed", 90 },
			{ "Acceleration", 114 },
			{ "Handling", 24 }
		});

		dict.Add("21", new Dictionary<string, object>()
		{
			{ "Price", 1200500 },
			{ "Speed", 92 },
			{ "Acceleration", 110 },
			{ "Handling", 21 }
		});

		dict.Add("22", new Dictionary<string, object>()
		{
			{ "Price", 1500000 },
			{ "Speed", 95 },
			{ "Acceleration", 116 },
			{ "Handling", 25 }
		});

		dict.Add("23", new Dictionary<string, object>()
		{
			{ "Price", 2000000 },
			{ "Speed", 100 },
			{ "Acceleration", 120 },
			{ "Handling", 25 }
		});

		dict.Add("24", new Dictionary<string, object>()
		{
			{ "Price", 20000 },
			{ "Speed", 100 },
			{ "Acceleration", 120 },
			{ "Handling", 25 }
		});

		dict.Add("25", new Dictionary<string, object>()
		{
			{ "Price", 20000 },
			{ "Speed", 100 },
			{ "Acceleration", 120 },
			{ "Handling", 24 }
		});

		dict.Add("26", new Dictionary<string, object>()
		{
			{ "Price", 20000 },
			{ "Speed", 100 },
			{ "Acceleration", 120 },
			{ "Handling", 25 }
		});

		dict.Add("27", new Dictionary<string, object>()
		{
			{ "Price", 20000 },
			{ "Speed", 100 },
			{ "Acceleration", 120 },
			{ "Handling", 24 }
		});

		dict.Add("28", new Dictionary<string, object>()
		{
			{ "Price", 20000 },
			{ "Speed", 100 },
			{ "Acceleration", 120 },
			{ "Handling", 24 }
		});

		//Set initial main car color
		setCarColors();

		/*
		Speed 70-100
		Acceleration 90-120
		Handling 20-35
		*/

		// Retrieving values
		int carsPrice = System.Convert.ToInt32(dict[selectedCar.ToString()]["Price"].ToString());
		carPrice.text = "$"+carsPrice.ToString("n0");

		getCarStat(selectedCar, "speed");
		getCarStat(selectedCar, "acceleration");
		getCarStat(selectedCar, "handling");

	}

	void Update() {
		//Debug.Log("Color: " + GameDataController.getCarsColor(selectedCar) + " " + PlayerPrefs.GetString("Car"+selectedCar+"Colour"));
		//Debug.Log("SpecColor: " + GameDataController.getCarsSpecColor(selectedCar) + " " + PlayerPrefs.GetString("Car"+selectedCar+"SpecColour"));
		if(changeCar) {
			Debug.Log(PlayerPrefs.GetString("GameData"));
			Destroy(theCar[currentCar]);
			currentCar = selectedCar;
			theCar[selectedCar] = Instantiate(Resources.Load("Car"+selectedCar, typeof(GameObject)), startPosition, transform.rotation) as GameObject;

			//Disable dust, make car bouncy and turn off trigger collider
			theCar[currentCar].transform.GetChild(2).gameObject.SetActive(false);
			theCar[currentCar].transform.GetChild(3).gameObject.SetActive(false);
			ChangeWheelColliders.changeWheelCollider(theCar[currentCar]);
			theCar[selectedCar].GetComponent<Collider>().isTrigger = false;

			//Change Car Price
			int currentCarPrice =  System.Convert.ToInt32(dict[selectedCar.ToString()]["Price"].ToString());
			carPrice.text = "$"+currentCarPrice.ToString("n0");

			if(!GameDataController.getCars(selectedCar)) {
				//Get values for stats
				//((input - min) * 100) / (max - min)
				speedLine.fillAmount = getPercentageOf(float.Parse(dict[selectedCar.ToString()]["Speed"].ToString()), speedMin, speedMax) / 100;
				accelerationLine.fillAmount = getPercentageOf(float.Parse(dict[selectedCar.ToString()]["Acceleration"].ToString()), accellerationMin, accellerationMax) / 100;
				handlingLine.fillAmount = getPercentageOf(float.Parse(dict[selectedCar.ToString()]["Handling"].ToString()), handlingMin, handlingMax) / 100;

				speedUnderline.fillAmount = getPercentageOf(float.Parse(dict[selectedCar.ToString()]["Speed"].ToString()), speedMin, speedMax) / 100 + 0.03f;
				accelerationUnderline.fillAmount = getPercentageOf(float.Parse(dict[selectedCar.ToString()]["Acceleration"].ToString()), accellerationMin, accellerationMax) / 100 + 0.03f;
				handlingUnderline.fillAmount = getPercentageOf(float.Parse(dict[selectedCar.ToString()]["Handling"].ToString()), handlingMin, handlingMax) / 100 + 0.03f;
			} else {
				//Get saved car stats
				getCarStat(selectedCar, "speed");
				getCarStat(selectedCar, "acceleration");
				getCarStat(selectedCar, "handling");
			}
			setCarColors(); //Change car to selected color

			changeCar = false;
		}
	}

	public void buyCar() {
		int currentCarPrice =  System.Convert.ToInt32(dict[selectedCar.ToString()]["Price"].ToString());
		if(PlayerPrefs.GetInt("playerCash", 0) > currentCarPrice) {
			//I can afford this car
			if(!carSet) {
				garageTutorial.firstCarBought();
			}

			//Set playerCash
			moneyController.updateDriftMoney(-Mathf.Abs(currentCarPrice));

			GameDataController.purchaseCar(selectedCar, (int)dict[selectedCar.ToString()]["Speed"], (int)dict[selectedCar.ToString()]["Acceleration"], (int)dict[selectedCar.ToString()]["Handling"]);
			ownedCar();
			cashSource.PlayOneShot(cashSource.clip, 0.50f); //Cash audio

		} else {
			Debug.Log("I cannot afford this car");
		}
	}

	public static Vector4 hexColor(float r, float g, float b, float a){
         Vector4 color = new Vector4(r/255, g/255, b/255, a/255);
         return color;
 	}

	public void chageTheCarColor(string colorName) {
		sprayPaintSource.PlayOneShot(sprayPaintSource.clip, 0.50f); //Spray paint audio

		//Use the word to find the Hex code
		KeyValuePair <string, string> results = myColors.FirstOrDefault (v => v.Key.Equals (colorName));
		globalColorName = colorName;
		string stats = results.Value;

		selectedColor = stats; //variable we send to savedata to assign to our car

		//If car is owned and color has not been purchased show buy screen
		if(!BuyGO.active && !GameDataController.getColor(selectedCar, colorName)) {
			paintMoney.SetActive(true);
			paintGO.SetActive(true);
		} else if(!BuyGO.active && GameDataController.getColor(selectedCar, colorName)) { //If car is owned and color has been purchased select color
			selectColorButton(globalColorName, myColors);
			GameDataController.purchaseColor(globalColorName, selectedCar, "Stock");
			PlayerPrefs.SetString("Car"+selectedCar+"Colour", stats); //Set the carColour playerpref
			paintGO.SetActive(false);
			paintMoney.SetActive(false);
		}

		newColor = hexColor(int.Parse(stats.Substring(0,3)), int.Parse(stats.Substring(3,3)), int.Parse(stats.Substring(6,3)), 255);
		StartCoroutine(ColorChange());
	}

	IEnumerator ColorChange() {
        //Infinite loop will ensure our coroutine runs all game
        while(theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.color != newColor) {

            float transitionRate = 0; //Create and set transitionRate to 0. This is necessary for our next while loop to function

            while(transitionRate < 20) {

                theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.color, newColor, Time.deltaTime * transitionRate));
                transitionRate += Time.deltaTime * transitionTime; // Increment transitionRate over the length of transitionTime
                yield return null; // wait for a frame then loop again

            }
			//Turn off car select
        }
    }

	public void chageTheCarSpecularColor(string colorName) {
		sprayPaintSource.PlayOneShot(sprayPaintSource.clip, 0.50f); //Spray paint audio

		//Use the word to find the Hex code
		KeyValuePair <string, string> results = myColorsSpec.FirstOrDefault (v => v.Key.Equals (colorName));
		globalColorName = colorName;
		string stats = results.Value;

		selectedColor = stats; //variable we send to savedata to assign to our car

		//If car is owned and color has not been purchased show buy screen
		if(!BuyGO.active && !GameDataController.getColor(selectedCar, colorName)) {
			paintSpecMoney.SetActive(true);
			paintSpecGO.SetActive(true);
		} else if(!BuyGO.active && GameDataController.getColor(selectedCar, colorName)) { //If car is owned and color has been purchased select color
			selectColorButton(globalColorName, myColorsSpec);
			PlayerPrefs.SetString("Car"+selectedCar+"SpecColour", stats); //Set the carColour playerpref
			paintSpecGO.SetActive(false);
			paintSpecMoney.SetActive(false);
		}

		newColor = hexColor(int.Parse(stats.Substring(0,3)), int.Parse(stats.Substring(3,3)), int.Parse(stats.Substring(6,3)), 255);
		//theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", newColor);
		StartCoroutine(SpecularChange());
	}

	IEnumerator SpecularChange() {
        //Infinite loop will ensure our coroutine runs all game
        while(theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.GetColor("_SpecColor") != newColor) {

            float transitionRate = 0; //Create and set transitionRate to 0. This is necessary for our next while loop to function
            while(transitionRate < 20) {

                theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_SpecColor", Color.Lerp(theCar[currentCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.GetColor("_SpecColor"), newColor, Time.deltaTime * transitionRate));
                transitionRate += Time.deltaTime * transitionTime; // Increment transitionRate over the length of transitionTime
                yield return null; // wait for a frame then loop again

            }
			//Turn off car select
        }
    }

	public void purchaseTheColor(string colorType) {
		cashSource.PlayOneShot(cashSource.clip, 0.50f); //Cash audio

		if(colorType == "Spec") {
			GameDataController.purchaseColor(globalColorName, selectedCar, "Spec"); //Buy color
			PlayerPrefs.SetString("Car"+selectedCar+"SpecColour", selectedColor);
			moneyController.updateDriftMoney(-500);
			selectColorButton(globalColorName, myColorsSpec);
			paintSpecMoney.SetActive(false);
			paintSpecGO.SetActive(false);
		} else if (colorType == "Stock") {
			GameDataController.purchaseColor(globalColorName, selectedCar, "Stock"); //Buy color
			moneyController.updateDriftMoney(-250);
			PlayerPrefs.SetString("Car"+selectedCar+"Colour", selectedColor);
			selectColorButton(globalColorName, myColors);
			paintMoney.SetActive(false);
			paintGO.SetActive(false);
		}

		//Hide lock icon
		Transform result = colorsParent.transform.Find(globalColorName);
		result.GetChild(0).gameObject.SetActive(false);

		backColor();
	}

	public void setColorPadlocks(int selectedCar, Dictionary<string, string> c) {
		Transform result;
		foreach(KeyValuePair<string, string> entry in c)
		{
			result = colorsParent.transform.Find(entry.Key);
			if(!GameDataController.getColor(selectedCar, entry.Key) || selectedCar == 9999){
				result.GetChild(0).gameObject.SetActive(true);
			} else {
				result.GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	//Set buy and carprice to false, enable turn on and colors
	public void ownedCar() {
		BuyGO.SetActive(false);
		carPriceGO.SetActive(false);
		keyImage.interactable = true;
		colorsParent.SetActive(true);
		setColorPadlocks(selectedCar, myColors); //Show padlocks on colors
		setColorPadlocks(selectedCar, myColorsSpec); //Show padlocks on colors
		//selectColorButton(GameDataController.getCarsColor(selectedCar), myColors);
	}

	//Set buy car price to true, turn off start car and colors
	public void unownedCar() {
		BuyGO.SetActive(true);
		carPriceGO.SetActive(true);
		keyImage.interactable = false;
		colorsParent.SetActive(false);
	}

	public void specificCar(int carChosen) {
		carPriceGO.SetActive(false);
		paintGO.SetActive(false);
		paintSpecGO.SetActive(false);
		paintSpecMoney.SetActive(false);
		paintMoney.SetActive(false);
		selectedCar = carChosen;
		if(!GameDataController.getCars(selectedCar)) {
			unownedCar();
		} else {
			ownedCar();
		}
		changeCar = true;
	}
	public void nextCar() {
		if((selectedCar+1) <= dict.Count) {
			carPriceGO.SetActive(false);
			paintGO.SetActive(false);
			paintSpecGO.SetActive(false);
			paintSpecMoney.SetActive(false);
			paintMoney.SetActive(false);
			selectedCar++;
			if(!GameDataController.getCars(selectedCar)) {
				unownedCar();
			} else {
				ownedCar();
			}
			changeCar = true;
		}
	}
	public void previousCar() {
		if((selectedCar-1) >= 1) {
			carPriceGO.SetActive(false);
			paintGO.SetActive(false);
			paintSpecGO.SetActive(false);
			paintMoney.SetActive(false);
			paintSpecMoney.SetActive(false);
			selectedCar--;
			if(!GameDataController.getCars(selectedCar)) {
				unownedCar();
			} else {
				ownedCar();
			}
			changeCar = true;
		}
	}
	public void selectCar() {
		engineStartSource.PlayOneShot(engineStartSource.clip, 1.0f);
		PlayerPrefs.SetInt("playerCar", selectedCar);
		PlayerPrefs.SetInt("Speed", GameDataController.getCarStats(selectedCar, "speed"));
		PlayerPrefs.SetInt("Acceleration", GameDataController.getCarStats(selectedCar, "acceleration"));
		PlayerPrefs.SetInt("Handling", GameDataController.getCarStats(selectedCar, "handling"));

		if(
			((Application.internetReachability != NetworkReachability.NotReachable) ||
			(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)) &&
			PlayerPrefs.GetString("NoAds") != "NoAds"
		) {
			UnityAdsManager.Instance.ShowRegularAd(OnAdClosed);
		} else {
			LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
			loadingscreen.LoadLevel("Home");
		}
	}

	private void OnAdClosed(ShowResult result) {
        LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
		loadingscreen.LoadLevel("Home");
    }

	public void backColor() {
		if(!GameDataController.getCars(selectedCar)) { //If unowned car show price and buy buttons
			BuyGO.SetActive(true);
			carPriceGO.SetActive(true);
		} else { //Else hide price and paint buttons
			carPriceGO.SetActive(false);
		}
	}

	public void selectColorButton(string myColor, Dictionary<string, string> c) {
		GameObject result;
		foreach(KeyValuePair<string, string> entry in c)
		{
			result = GameObject.Find(entry.Key);
			if(myColor == entry.Key){
				result.GetComponent<Button>().interactable = false;
			} else {
				result.GetComponent<Button>().interactable = true;
			}
		}
	}

	public void hideConfirm() {
		confirmDialog.SetActive(false);
		speedLine.enabled = true;
		accelerationLine.enabled = true;
		handlingLine.enabled = true;
	}

	public void showConfirm(string stat) {
		//show confirm screen
		if(getTotalStars() > PlayerPrefs.GetInt("Spent")) {
			speedLine.enabled = true;
			accelerationLine.enabled = true;
			handlingLine.enabled = true;
			if(stat == "speed") {
				speedLine.enabled = false;
			} else if(stat == "acceleration") {
				accelerationLine.enabled = false;
			} else if(stat == "handling") {
				handlingLine.enabled = false;
			}
			confirmDialog.SetActive(true);
			statChose = stat;
		}
	}

	public void upgradeCar() {
		GameDataController.upgradeCar(selectedCar, statChose);
		getCarStat(selectedCar, statChose);

		PlayerPrefs.SetInt("Spent", PlayerPrefs.GetInt("Spent")+1);
		getTotalStars();

		if(!carSet) {
			garageTutorial.firstStatUpgraded();
			carSet = true;
		}
		hideConfirm();
	}

	public int getTotalStars() {
		string[] levels = new string[7] {"Grass", "Grass2", "Grass3", "Lava2", "Lava3", "Snow", "Desert"};
		int totalStars = 0;
		foreach (string level in levels) {
			if(PlayerPrefs.HasKey(level+"-Stars")) {
				totalStars += PlayerPrefs.GetInt(level+"-Stars", 0);
			}
        }
		int totalStarsFull = totalStars;
		totalStars -= PlayerPrefs.GetInt("Spent");
		starNumber.text = totalStars.ToString();
		return totalStarsFull;
	}

	public void getCarStat(int carId, string statType) {
		int carStat = GameDataController.getCarStats(carId, statType);
		if(statType == "speed") {
			//((input - min) * 100) / (max - min)
			speedLine.fillAmount = getPercentageOf(carStat, speedMin, speedMax) / 100;
			speedUnderline.fillAmount = getPercentageOf(carStat, speedMin, speedMax) / 100 + 0.03f;

		} else if(statType == "acceleration") {
			accelerationLine.fillAmount = getPercentageOf(carStat, accellerationMin, accellerationMax) / 100;
			accelerationUnderline.fillAmount = getPercentageOf(carStat, accellerationMin, accellerationMax) / 100 + 0.03f;
		} else if(statType == "handling") {
			handlingLine.fillAmount = getPercentageOf(carStat, handlingMin, handlingMax) / 100;
			handlingUnderline.fillAmount = getPercentageOf(carStat, handlingMin, handlingMax) / 100 + 0.03f;
		}
	}

	private float getPercentageOf(float input, int min, int max) {
		return ((input - min) * 100) / (max - min);
	}

	private void setCarColors() {

		/*string lastColourCheck =  GameDataController.getCarsColor(selectedCar);
		KeyValuePair <string, string> results = myColors.FirstOrDefault (v => v.Key.Equals (lastColourCheck));
		string statsCurrent = results.Value;

		string lastSpecColourCheck =  GameDataController.getCarsSpecColor(selectedCar);
		KeyValuePair <string, string> resultsSpec = myColors.FirstOrDefault (v => v.Key.Equals (lastSpecColourCheck));
		string statsCurrentSpec = resultsSpec.Value;

		Debug.Log(lastColourCheck + " " + lastSpecColourCheck);
		if(statsCurrent.Length == 9) {
			PlayerPrefs.SetString("Car"+selectedCar+"Colour", statsCurrent);
		} else {
			PlayerPrefs.DeleteKey("Car"+selectedCar+"Colour");
		}
		if(statsCurrentSpec.Length == 9) {
			PlayerPrefs.SetString("Car"+selectedCar+"SpecColour", statsCurrentSpec);
		} else {
			PlayerPrefs.DeleteKey("Car"+selectedCar+"SpecColour");
		}*/

		if(PlayerPrefs.HasKey("Car"+selectedCar+"Colour")) {
            string stats = PlayerPrefs.GetString("Car"+selectedCar+"Colour");
            Color newColor = hexColor(int.Parse(stats.Substring(0,3)), int.Parse(stats.Substring(3,3)), int.Parse(stats.Substring(6,3)), 255);
			theCar[selectedCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", newColor);
        }
		//Set initial specular car color
		if(PlayerPrefs.HasKey("Car"+selectedCar+"SpecColour")) {
            string specStats = PlayerPrefs.GetString("Car"+selectedCar+"SpecColour");
            Color newSpecColor = hexColor(int.Parse(specStats.Substring(0,3)), int.Parse(specStats.Substring(3,3)), int.Parse(specStats.Substring(6,3)), 255);
            theCar[selectedCar].GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_SpecColor", newSpecColor);
        }
	}

}
