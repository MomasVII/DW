using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatus : MonoBehaviour
{

    //Status text when enetering level area
    private string grassStatusText, desertStatusText, snowStatusText, lavaStatusText;

    void Start() {
        int totalStars = 0, starsTotal = 0, zeroLevels = 0, levelStars = 0;
        int enableScore = 0, unlocked = 0;

        string[] levels = new string[9] {"Grass", "Grass2", "Grass3", "Lava2", "Lava3", "Snow", "Snow2", "Desert", "Desert2"};
		foreach (string level in levels) {
			totalStars += PlayerPrefs.GetInt(level+"-Stars", 0);
        }

        /*---Grass status text---*/
        string[] grassLevels = new string[3] {"Grass", "Grass2", "Grass3"}; //AddLevel
		foreach (string grassLevel in grassLevels) {
			levelStars = PlayerPrefs.GetInt(grassLevel+"-Stars", 0);
            enableScore = PlayerPrefs.GetInt(grassLevel+"-StarsToUnlock", 0);
            if(totalStars >= enableScore) {
                unlocked++;
                if(levelStars == 0) {
                    zeroLevels++;
                }
            }
        }
        if(zeroLevels == 1) {
            grassStatusText = zeroLevels+" unplayed level";
        } else if(zeroLevels > 1) {
            grassStatusText = zeroLevels+" unplayed levels";
        } else {
            grassStatusText = unlocked+"/"+grassLevels.Length+" levels unlocked";
        }
        unlocked = 0;
        zeroLevels = 0;
        /*------*/

        /*---Snow status text---*/
        string[] snowLevels = new string[2] {"Snow", "Snow2"}; //AddLevel
		foreach (string snowLevel in snowLevels) {
			levelStars = PlayerPrefs.GetInt(snowLevel+"-Stars", 0);
            enableScore = PlayerPrefs.GetInt(snowLevel+"-StarsToUnlock", 0);
            if(totalStars >= enableScore) {
                unlocked++;
                if(levelStars == 0) {
                    zeroLevels++;
                }
            }
        }
        if(zeroLevels == 1) {
            snowStatusText = zeroLevels+" unplayed level";
        } else if(zeroLevels > 1) {
            snowStatusText = zeroLevels+" unplayed levels";
        } else {
            snowStatusText = unlocked+"/"+snowLevels.Length+" levels unlocked";
        }
        unlocked = 0;
        zeroLevels = 0;
        /*------*/

        /*---Lava status text---*/
        string[] lavaLevels = new string[3] {"Lava", "Lava2", "Lava3"}; //AddLevel
		foreach (string lavaLevel in lavaLevels) {
			levelStars = PlayerPrefs.GetInt(lavaLevel+"-Stars", 0);
            enableScore = PlayerPrefs.GetInt(lavaLevel+"-StarsToUnlock", 0);
            if(totalStars >= enableScore) {
                unlocked++;
                if(levelStars == 0) {
                    zeroLevels++;
                }
            }
        }
        if(zeroLevels == 1) {
            lavaStatusText = zeroLevels+" unplayed level";
        } else if(zeroLevels > 1) {
            lavaStatusText = zeroLevels+" unplayed levels";
        } else {
            lavaStatusText = unlocked+"/"+lavaLevels.Length+" levels unlocked";
        }
        unlocked = 0;
        zeroLevels = 0;
        /*------*/

        /*---Desert status text---*/
        string[] desertLevels = new string[2] {"Desert", "Desert2"}; //AddLevel
		foreach (string desertLevel in desertLevels) {
			levelStars = PlayerPrefs.GetInt(desertLevel+"-Stars", 0);
            enableScore = PlayerPrefs.GetInt(desertLevel+"-StarsToUnlock", 0);
            if(totalStars >= enableScore) {
                unlocked++;
                if(levelStars == 0) {
                    zeroLevels++;
                }
            }
        }
        if(zeroLevels == 1) {
            desertStatusText = zeroLevels+" unplayed level";
        } else if(zeroLevels > 1) {
            desertStatusText = zeroLevels+" unplayed levels";
        } else {
            desertStatusText = unlocked+"/"+desertLevels.Length+" levels unlocked";
        }
        /*------*/

    }

    public string getGrassStatus() {
        return grassStatusText;
    }

    public string getDesertStatus() {
        return desertStatusText;
    }

    public string getLavaStatus() {
        return lavaStatusText;
    }

    public string getSnowStatus() {
        return snowStatusText;
    }



}
