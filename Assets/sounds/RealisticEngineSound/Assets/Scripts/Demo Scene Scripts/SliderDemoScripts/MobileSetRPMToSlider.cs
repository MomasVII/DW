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

public class MobileSetRPMToSlider : MonoBehaviour { // this script was only made for slider demo scene

    RealisticEngineSound_mobile res_mob;
    public GameObject controllerGameobject; // GameObject with Realistic Engine Sound script
    public Slider rpmSlider; // UI slider to set RPM
    public Slider pitchSlider; // UI sliter to set maximum pitch
    public Text pitchText; // UI text to show pitch multiplier value
    public Text rpmText; // UI text to show current RPM
    public Toggle ReversingCheckbox; // UI toggle for is reversing
    public bool simulated = true;
    public GameObject gasPedalButton;
    CarSimulator carSimulator;
    private int _rpm;
    private float _pitch;

    private void Start()
    {
        res_mob = controllerGameobject.GetComponent<RealisticEngineSound_mobile>();
        rpmSlider.maxValue = res_mob.maxRPMLimit; // set UI slider's max value to Realistic Engine Sound script's setted maximum RPM
        carSimulator = gasPedalButton.GetComponent<CarSimulator>();
        // save values
        if (!simulated)
            _rpm = (int)rpmSlider.value;
        else
            _rpm = (int)carSimulator.rpm;
        _pitch = pitchSlider.value;
    }
    public void SetRPM()
    {
        if (res_mob != null)
            res_mob.engineCurrentRPM = rpmSlider.value; // set prefab's current RPM to slider value
    }
    private void Update() // creates GC because of updating Text.text, this garbage can't be reduced. Not recomended to use this script for mobile games. This script was only made for this demo scene!
    {
        if (!simulated)
        {
            if (_rpm != (int)rpmSlider.value) // rpm is changed, update values and ui
            {
                rpmText.text = "Engine RPM: " + (int)rpmSlider.value; // show current RPM - this creates garbage
                res_mob.engineCurrentRPM = (int)rpmSlider.value; // set prefab's current RPM to slider value
                _rpm = (int)rpmSlider.value; // save value
            }
        }
        else
        {
            if (_rpm != (int)carSimulator.rpm) // rpm is changed, update values and ui
            {
                rpmText.text = "Engine RPM: " + (int)carSimulator.rpm; // show current RPM - this creates garbage
                res_mob.engineCurrentRPM = (int)carSimulator.rpm; // set rpm for res prefabs
                rpmSlider.value = (int)carSimulator.rpm; // set ui sliders value to rpm
                _rpm = (int)carSimulator.rpm; // save value
            }
        }
        if (_pitch != pitchSlider.value)
        {
            res_mob.pitchMultiplier = pitchSlider.value; // set pitch multiplier value for res prefabs
            pitchText.text = "" + pitchSlider.value; // set pitch multiplier value for ui text
            _pitch = pitchSlider.value; // save value
        }
    }
    public void ReverseGearCheckbox() // enable/disable reverse gear
    {
        if (res_mob != null)
        {
            if (res_mob.enableReverseGear) // turn off gas pedal pressing
            {
                res_mob.enableReverseGear = false;
                ReversingCheckbox.gameObject.SetActive(false);
                ReversingCheckbox.isOn = false;
            }
            else // turn on gas pedal pressing
            {
                res_mob.enableReverseGear = true;
                ReversingCheckbox.gameObject.SetActive(true);
            }
        }
    }
    public void Reversing() // enable/disable reversing sound
    {
        if (res_mob != null)
        {
            if (res_mob.isReversing) // turn off reversing sound
                res_mob.isReversing = false;
            else
                res_mob.isReversing = true;
        }
    }
    public void RPMLimit() // enable/disable rpm limit
    {
        if (res_mob != null)
        {
            if (res_mob.useRPMLimit) // turn off rpm limit
                res_mob.useRPMLimit = false;
            else
                res_mob.useRPMLimit = true;
        }
    }
}
