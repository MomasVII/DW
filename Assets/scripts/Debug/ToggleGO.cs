using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleGO : MonoBehaviour {

	public Button toggleButton;
	public GameObject GObject;
	public TMP_Text onOffText;

	public void Start() {
		toggleButton.onClick.RemoveAllListeners();
		toggleButton.onClick.AddListener(ToggleGameObject);
	}

	public void ToggleGameObject() {
		GObject.SetActive(!GObject.activeInHierarchy);
		if(GObject.activeSelf) {
			onOffText.text = "On";
		} else {
			onOffText.text = "Off";
		}
	}

}
