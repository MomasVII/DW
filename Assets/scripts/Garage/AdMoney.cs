using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdMoney : MonoBehaviour
{
	public AudioSource success;
	public GameObject adGO;

	//Used to set Ad money to player cash
	private MoneyController moneyController;

	void Start() {
		success.volume = PlayerPrefs.GetFloat("musicVolume");
	}

    public void PlayRewardedAd() {
        UnityAdsManager.Instance.ShowRewardedAd(OnRewardedAdClosed);
    }

    private void OnRewardedAdClosed(ShowResult result) {
        switch(result) {
            case ShowResult.Finished:
                Debug.Log("Rewarded Ad Finished");
				moneyController = GetComponent<MoneyController>();
				moneyController.updateDriftMoney(2000);
				success.PlayOneShot(success.clip, 1.0f);
				System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
				PlayerPrefs.SetInt("GarageAdPlayed", cur_time);
				adGO.SetActive(false);
                break;
            case ShowResult.Skipped:
                Debug.Log("Rewarded Ad Skipped");
                break;
            case ShowResult.Failed:
                Debug.Log("Rewarded Ad Failed");
                break;
        }
    }
}
