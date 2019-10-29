using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

	public GameObject loadingScreen;
    public Slider slider;

	public void LoadLevel(string levelName) {
		StartCoroutine(LoadAsynchronously(levelName));
		System.GC.Collect();
	}

	IEnumerator LoadAsynchronously(string sceneName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadingScreen.SetActive(true);
        while(!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
