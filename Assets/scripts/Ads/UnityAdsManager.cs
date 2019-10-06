using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : MonoBehaviour
{
    private static UnityAdsManager instance;
    public static UnityAdsManager Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<UnityAdsManager>();
                if(instance == null) {
                    instance = new GameObject("Spawned UnityAdsManager", typeof(UnityAdsManager)).GetComponent<UnityAdsManager>();
                }
            }
            return instance;
        }
        set {
            instance = value;
        }
    }

    [Header("Config")]
    [SerializeField] private string gameID = "";
    [SerializeField] private bool testMode = true;
    [SerializeField] private string rewardedVideoPlacementId;
    [SerializeField] private string regularPlacementId;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        Advertisement.Initialize(gameID, testMode);
    }

    public void ShowRegularAd(Action<ShowResult> callback) {
        if(Advertisement.IsReady(regularPlacementId)) {
                ShowOptions so = new ShowOptions();
                so.resultCallback = callback;
                Advertisement.Show(regularPlacementId, so);
        } else {
            Debug.Log("Ad Not Ready");
        }
    }

    public void ShowRewardedAd(Action<ShowResult> callback) {
        if(Advertisement.IsReady(rewardedVideoPlacementId)) {
                ShowOptions so = new ShowOptions();
                so.resultCallback = callback;
                Advertisement.Show(rewardedVideoPlacementId, so);
        } else {
            Debug.Log("Ad Not Ready");
        }
    }

}
