using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataController : MonoBehaviour
{

	public static SaveData saveData;

	private void Awake()
	{
		LoadData();
	}

	[ContextMenu("Save Data")]
	public static void SaveGame()
	{
		var data = JsonUtility.ToJson(saveData);
		PlayerPrefs.SetString("GameData", data);
		Debug.Log(PlayerPrefs.GetString("GameData"));
	}

	[ContextMenu("Load Data")]
	public void LoadData()
	{

		if(PlayerPrefs.HasKey("GameData")) {
			var data = PlayerPrefs.GetString("GameData");
			saveData = JsonUtility.FromJson<SaveData>(data);
		}
	}

	public static void ResetData() {
		saveData.CarList.Clear();
		SaveGame();
		purchaseCar(1, 70, 30, 30);
	}

	public static string getCarsColor(int carColour)
	{
		foreach (var sd in saveData.CarList) {
			if(sd.carId == carColour) { //Find car we are referring to
				if(sd.currentColor != null) { //Check we have set a color
					return sd.currentColor;
				}
			}
		}
		return "none";
	}
	public static string getCarsSpecColor(int carColour)
	{
		foreach (var sd in saveData.CarList) {
			if(sd.carId == carColour) { //Find car we are referring to
				if(sd.currentSpecColor != null) { //Check we have set a color
					return sd.currentSpecColor;
				}
			}
		}
		return "none";
	}

	public static bool getCars(int currentCarId)
	{
		if (saveData.CarList == null)
			return false;

		if (saveData.CarList.Any(t => t.carId == currentCarId))
			return true;

		return false;
	}

	public static bool getColor(int currentCarId, string currentColor)
	{
		if (saveData.CarList == null)
			return false;

		foreach (var sd in saveData.CarList) {
			if(sd.carId == currentCarId) { //If we own the car we are painting
				if(sd.carColors != null) { //If there are any colors in the list
					if(sd.carColors.Contains(currentColor)) { //if our list of colors does include the color we are painting
						return true;
					}
				}
			}
		}
		return false;
	}

	public static void purchaseCar(int myCarId, int mySpeed, int myAcceleration, int myHandling) {
		if (saveData.CarList == null)
			saveData.CarList = new List<CarListData>();

		var carListData = new CarListData() { carId = myCarId, speed = mySpeed, acceleration = myAcceleration, handling = myHandling };
		saveData.CarList.RemoveAll(t => t.carId == carListData.carId);
		saveData.CarList.Add(carListData);
		SaveGame();
	}

	public static void purchaseColor(string myCarColor, int selectedCar, string colorType) {
		if (saveData.CarList == null)
			saveData.CarList = new List<CarListData>();

		foreach (var sd in saveData.CarList) {
			if(sd.carId == selectedCar) {
				if(sd.carColors != null) {
					if(!sd.carColors.Contains(myCarColor)) {
						sd.carColors.Add(myCarColor);
					}
					if(colorType == "Spec") {
						sd.currentSpecColor = myCarColor;
					} else {
						sd.currentColor = myCarColor;
					}
				}
			}
		}
		SaveGame();
	}

	public static void upgradeCar(int currentCarId, string upgradeType) {
		if (saveData.CarList == null)
			saveData.CarList = new List<CarListData>();

		foreach (var sd in saveData.CarList) {
			if(sd.carId == currentCarId) { //If we own the car we are painting
				if(upgradeType == "speed") {
					sd.speed += 1;
					Debug.Log("up Speed");
				} else if(upgradeType == "acceleration") {
					sd.acceleration += 1;
					Debug.Log("up Acceleration");
				} else if(upgradeType == "handling") {
					sd.handling += 1;
					Debug.Log("up Handling");
				}
			}
		}
		SaveGame();
	}

	public static int getCarStats(int currentCarId, string upgradeType) {
		if (saveData.CarList == null)
			saveData.CarList = new List<CarListData>();

		foreach (var sd in saveData.CarList) {
			if(sd.carId == currentCarId) {
				if(upgradeType == "speed") {
					return sd.speed;
				} else if(upgradeType == "acceleration") {
					return sd.acceleration;
				} else if(upgradeType == "handling") {
					return sd.handling;
				}
			}
		}
		return 0;

	}


}
