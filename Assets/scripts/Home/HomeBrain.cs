using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeBrain : MonoBehaviour {

    public Vector3 startPosition;
    public float yAngle;

    //Get Quality Change Script
    public Quality QualityScript;

    //preload used shaders
    public ShaderVariantCollection shaderCollection;

	// Use this for initialization
	void Awake () {

        int rank = (PlayerPrefs.GetInt("Speed") + PlayerPrefs.GetInt("Acceleration")) - 160;
        rank = 0;
        PlayerPrefs.SetInt("Rank", rank);

        shaderCollection.WarmUp();

        if(!PlayerPrefs.HasKey("personalOrGhost")) {
            PlayerPrefs.SetString("personalOrGhost", "none");
        }

        //Remove data gathered from home colliders
        if(PlayerPrefs.HasKey("ghostTimeTaken")) {
            PlayerPrefs.DeleteKey("ghostTimeTaken");
            PlayerPrefs.DeleteKey("ghostGhostID");
    		PlayerPrefs.DeleteKey("ghostCarUsed");
        }

        if(!PlayerPrefs.HasKey("playerCar")) {
            GameDataController.purchaseCar(1, 70, 90, 30);
            PlayerPrefs.SetInt("playerCar", 1);
        }

        int playerCar = PlayerPrefs.GetInt("playerCar", 0);

	    //Instantiate(Resources.Load("Car"+playerCar, typeof(GameObject)), startPosition, transform.rotation);
        GameObject theCar = Instantiate(Resources.Load("Car"+playerCar, typeof(GameObject)), startPosition, transform.rotation * Quaternion.Euler (0f, yAngle, 0f)) as GameObject;
        if(PlayerPrefs.HasKey("Car"+playerCar+"Colour")) {
            string stats = PlayerPrefs.GetString("Car"+playerCar+"Colour");
            Color newColor = hexColor(int.Parse(stats.Substring(0,3)), int.Parse(stats.Substring(3,3)), int.Parse(stats.Substring(6,3)), 255);
            theCar.GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", newColor);
        }

        if(PlayerPrefs.HasKey("Car"+playerCar+"SpecColour")) {
            string specStats = PlayerPrefs.GetString("Car"+playerCar+"SpecColour");
            Color newSpecColor = hexColor(int.Parse(specStats.Substring(0,3)), int.Parse(specStats.Substring(3,3)), int.Parse(specStats.Substring(6,3)), 255);
            theCar.GetComponent<Transform>().GetChild(1).GetComponent<Renderer>().material.SetColor("_SpecColor", newSpecColor);
        }

	}

    public static Vector4 hexColor(float r, float g, float b, float a){
        Vector4 color = new Vector4(r/255, g/255, b/255, a/255);
        return color;
 	}

}
