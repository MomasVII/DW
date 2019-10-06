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

public class SetRPMToSlider : MonoBehaviour { // this script was only made for slider demo scene

    RealisticEngineSound res;
    public GameObject controllerGameobject; // GameObject with Realistic Engine Sound script
    public Slider rpmSlider; // UI slider to set RPM
    public Slider pitchSlider; // UI sliter to set maximum pitch
    public Text pitchText; // UI text to show pitch multiplier value
    public Text rpmText; // UI text to show current RPM
    public Toggle ReversingCheckbox; // UI toggle for is reversing
    public Toggle gasPedalCheckbox;
    public bool simulated = true; // simulating rpm value with gas pedal button
    public GameObject gasPedalButton;
    CarSimulator carSimulator;
    private int _rpm;
    private float _pitch;

    private void Start()
    {
        res = controllerGameobject.GetComponent<RealisticEngineSound>();
        rpmSlider.maxValue = res.maxRPMLimit; // set UI slider's max value to Realistic Engine Sound script's setted maximum RPM
        carSimulator = gasPedalButton.GetComponent<CarSimulator>();
        // save values
        if (!simulated)
            _rpm = (int)rpmSlider.value;
        else
            _rpm = (int)carSimulator.rpm;
        _pitch = pitchSlider.value;
    }
    private void Update() // creates GC because of updating Text.text, this garbage can't be reduced. Not recomended to use this script for mobile games. This script was only made for this demo scene!
    {
        if (!simulated) // rpm is set by hand with slider
        {
            if (_rpm != (int)rpmSlider.value) // rpm is changed, update values and ui
            {
                rpmText.text = "Engine RPM: " + (int)rpmSlider.value; // show current RPM - this creates garbage
                res.engineCurrentRPM = (int)rpmSlider.value; // set prefabs's current RPM to slider value
                _rpm = (int)rpmSlider.value; // save value
            }
        }
        else // rpm is simulated with gas pedal button
        {
            if (_rpm != (int)carSimulator.rpm) // rpm is changed, update values and ui
            {
                rpmText.text = "Engine RPM: " + (int)carSimulator.rpm; // show current RPM - this creates garbage
                res.engineCurrentRPM = (int)carSimulator.rpm; // set rpm for res prefabs
                rpmSlider.value = (int)carSimulator.rpm; // set ui sliders value to rpm

                res.gasPedalPressing = carSimulator.gasPedalPressing; // set gas pressing value for res prefabs   
                gasPedalCheckbox.isOn = carSimulator.gasPedalPressing; // set gas pressing ui checkbox value

                _rpm = (int)carSimulator.rpm; // save value
            }
        }
        if (_pitch != pitchSlider.value)
        {
            res.pitchMultiplier = pitchSlider.value; // set pitch multiplier value for res prefabs
            pitchText.text = "" + pitchSlider.value; // set pitch multiplier value for ui text
            _pitch = pitchSlider.value; // save value
        }
    }
    public void GasPedalCheckbox() // press gas pedal
    {
        if (res != null)
        {
            if (res.gasPedalPressing) // turn off gas pedal pressing
                res.gasPedalPressing = false;
            else
                res.gasPedalPressing = true;
        }
    }
    public void ReverseGearCheckbox() // enable/disable reverse gear
    {
        if (res != null)
        {
            if (res.enableReverseGear) // turn off gas pedal pressing
            {
                res.enableReverseGear = false;
                ReversingCheckbox.gameObject.SetActive(false);
                ReversingCheckbox.isOn = false;
            }
            else // turn on gas pedal pressing
            {
                res.enableReverseGear = true;
                ReversingCheckbox.gameObject.SetActive(true);
            }
        }
    }
    public void Reversing() // enable/disable reversing sound
    {
        if (res != null)
        {
            if (res.isReversing) // turn off reversing sound
                res.isReversing = false;
            else
                res.isReversing = true;
        }
    }
    public void RPMLimit() // enable/disable rpm limit
    {
        if (res != null)
        {
            if (res.useRPMLimit) // turn off rpm limit
                res.useRPMLimit = false;
            else
                res.useRPMLimit = true;
        }
    }
}
