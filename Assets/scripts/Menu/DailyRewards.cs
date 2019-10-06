using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyRewards : MonoBehaviour
{

    Menu menu;
    public GameObject DailyRewardPanel;
    public TMP_Text rewardText, collectText;

    //Play audio on reward shown
    public AudioSource success;
    //Play audio when collecting money
    public AudioSource cashSource;

    //Hide DW logo
    public Image DWLogo;

    //Money earnt through reward
    int rewardMoney;

    public GameObject tapToContinue;

    //Used to set Ad money to player cash
	private MoneyController moneyController;

    public void CheckDailyReward() {
        menu = GetComponent<Menu>();
        tapToContinue.SetActive(false);

        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        if(PlayerPrefs.HasKey("DailyReward")) {
            int dr_time = PlayerPrefs.GetInt("DailyReward");
            dr_time = dr_time+(3600*24); //dr_time = dr_time+(3600*24);
            if(cur_time > dr_time) {
                if(cur_time < dr_time+(3600*24)) {
                    PlayerPrefs.SetInt("DailyRewardMultiplier", PlayerPrefs.GetInt("DailyRewardMultiplier")+1);
                } else {
                    PlayerPrefs.SetInt("DailyRewardMultiplier", 1);
                }
                rewardMoney = 1000*PlayerPrefs.GetInt("DailyRewardMultiplier");
                if(rewardMoney > 7000) {
                    rewardMoney = 7000;
                }
                rewardText.text = "+$" + rewardMoney;
                collectText.text = "Daily Login Reward! (x" + PlayerPrefs.GetInt("DailyRewardMultiplier") + ")";

                DWLogo.CrossFadeAlpha(0, 1.0f, false);
                success.PlayOneShot(success.clip, 1.0f);
                DailyRewardPanel.SetActive(true);
                PlayerPrefs.SetInt("DailyReward", cur_time);
            } else {
                DailyRewardPanel.SetActive(false);
                menu.startGame();
            }
        } else {
            PlayerPrefs.SetInt("DailyReward", cur_time);
            PlayerPrefs.SetInt("DailyRewardMultiplier", 1);
            DailyRewardPanel.SetActive(false);
            menu.startGame();
        }
    }

    public void CollectReward() {
        moneyController = GetComponent<MoneyController>();
        moneyController.updateDriftMoney(rewardMoney);
        cashSource.PlayOneShot(cashSource.clip, 1.0f);

        DailyRewardPanel.SetActive(false);
        menu.startGame();
    }

}
