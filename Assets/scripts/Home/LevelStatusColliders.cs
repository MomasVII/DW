using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStatusColliders : MonoBehaviour
{
    public GameObject levelStatus, lsContent;
    public Text levelStatusText;

    private LevelStatus ls;

    void Start() {
        ls = lsContent.GetComponent<LevelStatus>();
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            if(this.name == "Grass") {
                levelStatusText.text = ls.getGrassStatus();
            } else if(this.name == "Snow") {
                levelStatusText.text = ls.getSnowStatus();
            } else if(this.name == "Lava") {
                levelStatusText.text = ls.getLavaStatus();
            } else if(this.name == "Desert") {
                levelStatusText.text = ls.getDesertStatus();
            }
            StartCoroutine(KillText());
        }
    }

    IEnumerator KillText() {
        levelStatus.gameObject.SetActive(true);
		yield return new WaitForSeconds(5);
		levelStatus.SetActive(false);
	}
}
