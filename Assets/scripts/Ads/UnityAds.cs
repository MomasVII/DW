using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour
{
    public void PlayAd() {
        UnityAdsManager.Instance.ShowRegularAd(OnAdClosed);
    }

    public void PlayRewardedAd() {
        UnityAdsManager.Instance.ShowRewardedAd(OnAdClosed);
    }

    private void OnAdClosed(ShowResult result) {
        Debug.Log("Regular Ad Closed");
    }

    private void OnRewardedAdClosed(ShowResult result) {
        Debug.Log("Rewarded Ad Closed");
        switch(result) {
            case ShowResult.Finished:
                Debug.Log("Rewarded Ad Closed");
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
