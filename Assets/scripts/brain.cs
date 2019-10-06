using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class brain : MonoBehaviour {

    public Vector3 startPosition;
    public float yAngle;

    int playerCar;

    //public ShaderVariantCollection shaderCollection;

    //Demo cars
    private GameObject demoCar;

    public GameObject adGo;

	// Use this for initialization
	void Awake () {
        //shaderCollection.WarmUp();
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        int ad_time = PlayerPrefs.GetInt("LevelAdPlayed");
        ad_time = ad_time+(4*60); //2 minutes between ad showing
        if(cur_time > ad_time && ((Application.internetReachability != NetworkReachability.NotReachable) ||
            (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork))) {
            adGo.SetActive(true);
        } else {
            adGo.SetActive(false);
        }

        if(PlayerPrefs.HasKey("playerCar")) {
            playerCar = PlayerPrefs.GetInt("playerCar", 0);
        } else {
            playerCar = 1;
        }

	    demoCar = Instantiate(Resources.Load("Car"+Random.Range(1, 19), typeof(GameObject)), startPosition, transform.rotation) as GameObject;
        demoCar.gameObject.tag = "Demo";

        //Rotate car to start pos
        //demoCar.transform.Rotate(xAngle, yAngle, zAngle, Space.Self);

        //Instantiate(Resources.Load("Car"+playerCar, typeof(GameObject)), new Vector3(0,0,0), transform.rotation);
        GameObject theCar = Instantiate(Resources.Load("Car"+playerCar, typeof(GameObject)), new Vector3(-100f,0,-100f), transform.rotation * Quaternion.Euler (0f, yAngle, 0f)) as GameObject;
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
