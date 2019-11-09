using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public Slider slider;

    void Start() {
		StartCoroutine(LoadAsynchronously("Garage"));

        //Set car count to 0
        if(!PlayerPrefs.HasKey("carCount")) {
            PlayerPrefs.SetInt("carCount", 0);
        }


	}

	IEnumerator LoadAsynchronously(string sceneName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while(!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
