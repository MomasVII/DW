using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoresPanel : MonoBehaviour {

	public string sceneName;
	public string levelName;

	public TMP_Text highscore;
	public TMP_Text levelTMP;

	public Image star1, star2, star3, star4, star5;
	public Sprite starSelected;

	// Use this for initialization
	void Start () {
		highscore.text = PlayerPrefs.GetFloat(sceneName+"highscore").ToString();
		levelTMP.text = levelName;
		int starNumber = PlayerPrefs.GetInt(sceneName+"-Stars", 0);
		if(starNumber >= 1) { star1.sprite = starSelected; }
		if(starNumber >= 2) { star2.sprite = starSelected; }
		if(starNumber >= 3) { star3.sprite = starSelected; }
		if(starNumber >= 4) { star4.sprite = starSelected; }
		if(starNumber >= 5) { star5.sprite = starSelected; }
	}

}
