using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableLevels : MonoBehaviour {

	public int enableScore;
	public string levelName;

	public static Vector4 hexColor(float r, float g, float b, float a){
         Vector4 color = new Vector4(r/255, g/255, b/255, a/255);
         return color;
 	}

	// Use this for initialization
	void Start () {

		PlayerPrefs.SetInt(levelName+"-StarsToUnlock", enableScore);

		string[] levels = new string[6] {"Grass", "Grass2", "Grass3", "Lava2", "Lava3", "Snow"};
		int totalStars = 0;
		foreach (string level in levels) {
			totalStars += PlayerPrefs.GetInt(level+"-Stars", 0);
        }
		if(totalStars < enableScore) {
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(false);
			Color newColor = hexColor(111, 111, 111, 157);
			transform.GetComponent<Transform>().GetChild(2).GetComponent<Renderer>().material.SetColor("_Color", newColor);
		}
	}

}
