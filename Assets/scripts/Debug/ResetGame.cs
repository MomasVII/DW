using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResetGame : MonoBehaviour {

    public MoneyController moneyController;

    public void resetTheGame() {

        GameDataController.purchaseCar(1, 30, 30, 30);

        //moneyController.updateDriftMoney(20000);

        //Reset database
        if(Application.internetReachability != NetworkReachability.NotReachable) {
            //StartCoroutine(resetDatabase());
        }

        PlayerPrefs.DeleteAll();

        string[] levels = new string[7] {"Grass", "Grass2", "Grass3", "Lava2", "Lava3", "Snow", "Desert"};
		foreach (string level in levels) {
            DeleteFile(level+"-record"); //Delete saved game Files
            DeleteFile(level+"-ghost"); //Delete saved game Files
        }

        //Application.Quit();
    }

    public IEnumerator resetDatabase() {

        WWW wwwHighscores = new WWW("https://www.undivided.games/DriftWorlds/ResetDatabase.php");
        while (!wwwHighscores.isDone) {
            yield return null;
        }
    }

    void DeleteFile(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;

        // check if file exists
        if ( !File.Exists( filePath ) )
        {
            Debug.Log("no " + fileName + " file exists"); //Debug.Log( "no " + fileName + " file exists" );
        }
        else
        {
            Debug.Log(fileName + " file exists, deleting..."); //Debug.Log( fileName + " file exists, deleting..." );

            File.Delete( filePath );

        }
    }

}
