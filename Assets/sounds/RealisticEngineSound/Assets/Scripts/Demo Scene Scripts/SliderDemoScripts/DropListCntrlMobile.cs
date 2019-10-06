//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2019 Yugel Mobile__________//
//______________________________________________//
//_________ http://mobile.yugel.net/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropListCntrlMobile : MonoBehaviour { // this script was only made for slider demo scene

    public Dropdown rpmDropdownList;
    public GameObject gasPedalButton;
    public GameObject[] sounds;

    public void ControllRPM()
    {
        if (rpmDropdownList.value == 0) // controll rpm with gas pedal button
        {
            gasPedalButton.SetActive(true);
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].GetComponent<MobileSetRPMToSlider>().simulated = true;
            }
            gasPedalButton.GetComponent<CarSimulator>().rpm = sounds[0].GetComponent<MobileSetRPMToSlider>().rpmSlider.value;
        }
        if (rpmDropdownList.value == 1) // controll rpm with slider
        {
            gasPedalButton.SetActive(false);
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].GetComponent<MobileSetRPMToSlider>().simulated = false;
            }
        }
    }
}
