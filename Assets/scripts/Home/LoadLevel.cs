using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {


	public void LoadALevel(string  levelName) {
		//SceneManager.LoadScene(levelName, LoadSceneMode.Single);
		LoadingScreen loadingscreen = GetComponent<LoadingScreen>();
		loadingscreen.LoadLevel(levelName);
	}
}
