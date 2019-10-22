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
using UnityEngine.Audio;

public class RealisticEngineSound : MonoBehaviour {

    // master volume setting
    [Range(0.1f, 1.0f)]
    public float masterVolume = 1f;
    public AudioMixerGroup audioMixer;

    public float engineCurrentRPM = 0.0f;
    public bool gasPedalPressing = false;
    [Range(0.0f, 1.0f)]
    public float gasPedalValue = 1; // (simulated or not simulated) 0 = not pressing = 0 engine volume, 0.5 = halfway pressing (half engine volume), 1 = pedal to the metal (full engine volume)
    // enum for gas pedal setting
    public enum GasPedalValue { Simulated, NotSimulated } // NotSimulated setting is recommended for joystick controlled games
    public GasPedalValue gasPedalValueSetting = new GasPedalValue();
    //
    [Range(1.0f, 15.0f)]
    public float gasPedalSimSpeed = 5.5f; // simulates how fast the player hit the gas pedal
    public float maxRPMLimit = 7000;
    [Range(0.0f, 5.0f)]
    public float dopplerLevel = 1;
    [Range(0.0f, 1.0f)]
    [HideInInspector] // remove this line if you want to set custom values for spatialBlend
    public float spatialBlend = 1f; // this value should always be 1. If you want custom value, remove spatialBlend = 1f; line from Start()
    [Range(0.1f, 2.0f)]
    public float pitchMultiplier = 1.0f; // pitch value multiplier
    public AudioReverbPreset reverbZoneSetting;
    private AudioReverbPreset reverbZoneControll;

    [Range(0.0f, 0.25f)]
    public float optimisationLevel = 0.01f; // audio source with volume level below this value will be destroyed
    public AudioRolloffMode audioRolloffMode = AudioRolloffMode.Custom;
    // play sounds within this distances
    public float minDistance = 1; // within the minimum distance the audiosources will cease to grow louder in volume
    public float maxDistance = 50; // maxDistance is the distance a sound stops attenuating at
    // other settings
    public bool isReversing = false; // is car in reverse gear
    public bool useRPMLimit = true; // enable rpm limit at maximum rpm
    public bool enableReverseGear = true; // enable wistle sound for reverse gear

    // hiden public stuff
    [HideInInspector]
    public float carCurrentSpeed; // needed for straight cut gearbox script
    [HideInInspector]
    public float carMaxSpeed; // needed for straight cut gearbox script
    [HideInInspector]
    public bool isShifting = false; // needed for shifting sounds script

    // idle clip sound
    public AudioClip idleClip;
    public AnimationCurve idleVolCurve;
    public AnimationCurve idlePitchCurve;
    // low rpm clip sounds
    public AudioClip lowOffClip;
    public AudioClip lowOnClip;
    public AnimationCurve lowVolCurve;
    public AnimationCurve lowPitchCurve;
    // medium rpm clip sounds
    public AudioClip medOffClip;
    public AudioClip medOnClip;
    public AnimationCurve medVolCurve;
    public AnimationCurve medPitchCurve;
    // high rpm clip sounds
    public AudioClip highOffClip;
    public AudioClip highOnClip;
    public AnimationCurve highVolCurve;
    public AnimationCurve highPitchCurve;
    // maximum rpm clip sound - if RPM limit is enabled
    public AudioClip maxRPMClip;
    public AnimationCurve maxRPMVolCurve;
    // reverse gear clip sound
    public AudioClip reversingClip;
    public AnimationCurve reversingVolCurve;
    public AnimationCurve reversingPitchCurve;

    // idle audio source
    private AudioSource engineIdle;

    // low rpm audio sources
    private AudioSource lowOff;
    private AudioSource lowOn;

    // medium rpm audio sources
    private AudioSource medOff;
    private AudioSource medOn;

    // high rpm audio sources
    private AudioSource highOff;
    private AudioSource highOn;

    //maximum rpm audio source
    private AudioSource maxRPM;

    // reverse gear audio source
    private AudioSource reversing;
    //private settings
    private float clipsValue;
    private float clipsValue2;
    // get camera for optimisation
    public Camera mainCamera;
    [HideInInspector]
    public bool isCameraNear; // tells is the camera near

    // shake engine sound settings
    public enum EngineShake { Off, Random, AllwaysOn }
    public EngineShake engineShakeSetting = new EngineShake();

    public enum ShakeLenghtType { Fix, Random }
    [HideInInspector]
    public ShakeLenghtType shakeLenghtSetting = new ShakeLenghtType();

    [HideInInspector] //[Range(10,100)]
    public float shakeLength = 50f;
    [HideInInspector] //[Range(0.3f, 0.9f)]
    public float shakeVolumeChange = 0.35f;
    [HideInInspector] //[Range(0.1f, 0.9f)]
    public float randomChance = 0.5f;

    private float _endRange = 1;
    private float shakeVolumeChangeDetect; // detect value change on runtime
    private float _oscillateRange;
    private float _oscillateOffset;
    private float lenght = 0; // shakingOn time
    private float randomShakingValue = 0;
    private float randomShakingValue2 = 0;
    private WaitForSeconds _wait;
    private bool alreadyDestroyed = false; // prevent asking to destroy already destroyed audio sources when camera is far away

    private WheelCollider rearRight, rearLeft;
    public float normalisePRM = 4.5f;
    public float[] gearRatio;
    public int currentGear;
    public float gearUpRPM;
    public float gearDownRPM;

    //Used to move wheels and turn car on demo replayTime
	private Ghost ghostScript;

    void AutoGears()
    {

        int AppropriateGear = currentGear;

        if (engineCurrentRPM >= gearUpRPM) {

            for (var i = 0; i < gearRatio.Length; i++) {
                if (rearRight.rpm * gearRatio[i] < gearUpRPM) {
                    AppropriateGear = i;
                    break;
                }
            }
            currentGear = AppropriateGear;
            //BacFire_Sound.PlayOneShot(BacFire_Sound.clip);
        }

        if (engineCurrentRPM <= gearDownRPM) {
            AppropriateGear = currentGear;
            for (var j = gearRatio.Length - 1; j >= 0; j--) {
                if (rearRight.rpm * gearRatio[j] > gearDownRPM) {
                    AppropriateGear = j;
                    break;
                }
            }
            currentGear = AppropriateGear;
        }
    }

    public float getRPM() {
		return engineCurrentRPM;
	}

    private void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("sfxVolume");
        ghostScript = GameObject.FindObjectOfType<Ghost>();
        GameObject car = GameObject.FindWithTag("Player");
		rearLeft = car.transform.Find("wheels/RearLeft").GetComponent<WheelCollider>();
		rearRight = car.transform.Find("wheels/RearRight").GetComponent<WheelCollider>();

		engineCurrentRPM = ((rearRight.rpm + rearLeft.rpm) / 2);

        spatialBlend = 1f; // remove this line if you want to set custom values for spatialBlend
        _wait = new WaitForSeconds(0.15f); // setup wait for secconds
        // if res scipts are far away from camera do not need to create audio sources, this audio sources already won't be heard because of the distance
        if (mainCamera == null)
            mainCamera = Camera.main;


        clipsValue = engineCurrentRPM / maxRPMLimit; // calculate % percentage of rpm

        if (mainCamera != null)
        {
            if (Vector3.Distance(mainCamera.transform.position, gameObject.transform.position) <= maxDistance)
            {
                isCameraNear = true;
                // create and start playing audio sources
                // idle
                if (idleVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (engineIdle == null)
                        CreateIdle();
                }
                //
                // low rpm
                if (lowVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (gasPedalPressing)
                    {
                        if (lowOn == null)
                            CreateLowOn();
                    }
                    else
                    {
                        if (lowOff == null)
                            CreateLowOff();
                    }
                }
                //
                // medium rpm
                if (medVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (gasPedalPressing)
                    {
                        if (medOn == null)
                            CreateMedOn();
                    }
                    else
                    {
                        if (medOff == null)
                            CreateMedOff();
                    }
                }
                //
                // high rpm
                if (highVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (gasPedalPressing)
                    {
                        if (highOn == null)
                            CreateHighOn();
                    }
                    else
                    {
                        if (highOff == null)
                            CreateHighOff();
                    }
                }
                //
                // rpm limiting
                if (maxRPMVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (useRPMLimit) // if rpm limit is enabled, create audio source for it
                    {
                        if (maxRPM == null)
                            CreateRPMLimit();
                    }
                }
                //
                // reversing gear sound
                if (enableReverseGear)
                {
                    if (isReversing)
                    {
                        if (reversingVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                        {
                            if (reversing == null)
                                CreateReverse();
                        }
                    }
                    else
                    {
                        if (reversing != null)
                            Destroy(reversing);
                    }
                }
                reverbZoneControll = reverbZoneSetting;
                SetReverbZone();
            }
            else
            {
                isCameraNear = false;
            }
        }
        UpdateStartRange();
    }
    private void Update()
    {
        //AutoGears();
        if(ghostScript != null) {
			if(ghostScript.playingDemo()) {
				engineCurrentRPM = ghostScript.getDemoSoundRPM();
			} else {
                if(rearRight != null && rearLeft != null) {
    				engineCurrentRPM = ((((rearRight.rpm + rearLeft.rpm) / 2)*normalisePRM));
                }
			}
		} else {
            if(rearRight != null && rearLeft != null) {
                engineCurrentRPM = ((((rearRight.rpm + rearLeft.rpm) / 2)*normalisePRM));
            }
		}

        if(engineCurrentRPM <= 0) {
            engineCurrentRPM = Mathf.Abs(engineCurrentRPM);
        }
        if (isCameraNear) //Are we close enough to hear the audio should always be true
        {
            if (shakeVolumeChangeDetect != shakeVolumeChange)
                UpdateStartRange();
            if (engineShakeSetting == EngineShake.Off)
            {
                clipsValue = engineCurrentRPM / maxRPMLimit;
                if (gasPedalPressing)
                    gasPedalValue = Mathf.Lerp(gasPedalValue, 1, Time.deltaTime * gasPedalSimSpeed);
                else
                    gasPedalValue = Mathf.Lerp(gasPedalValue, 0, Time.deltaTime * gasPedalSimSpeed);
            }
            if (engineShakeSetting == EngineShake.AllwaysOn)
            {
                if (gasPedalPressing)
                {
                    if (lenght < 1) // shaking sound for short time
                    {
                        if (shakeLenghtSetting == ShakeLenghtType.Fix)
                        {
                            gasPedalValue = _oscillateOffset + Mathf.Sin(Time.time * (shakeLength * clipsValue)) * _oscillateRange;
                            clipsValue2 = (engineCurrentRPM / maxRPMLimit) + Mathf.Sin(Time.time * shakeLength) * (_oscillateRange / 10);
                        }
                        if (shakeLenghtSetting == ShakeLenghtType.Random)
                        {
                            gasPedalValue = _oscillateOffset + Mathf.Sin(Time.time * (Random.Range(10, 100) * clipsValue)) * _oscillateRange;
                            clipsValue2 = (engineCurrentRPM / maxRPMLimit) + Mathf.Sin(Time.time * Random.Range(10, 100)) * (_oscillateRange / 10);
                        }
                        lenght = lenght + Random.Range(0.01f, 0.12f);

                        clipsValue = clipsValue2;
                    }
                    else // end shaking
                    {
                        gasPedalValue = Mathf.Lerp(gasPedalValue, 1, Time.deltaTime * gasPedalSimSpeed);
                        clipsValue = engineCurrentRPM / maxRPMLimit;
                    }
                }
                else
                {
                    gasPedalValue = Mathf.Lerp(gasPedalValue, 0, Time.deltaTime * gasPedalSimSpeed);
                    clipsValue = engineCurrentRPM / maxRPMLimit;
                    lenght = 0;
                }
            }
            if (engineShakeSetting == EngineShake.Random)
            {
                if (gasPedalPressing)
                {
                    randomShakingValue2 = 0;
                    if (randomShakingValue == 0)
                        randomShakingValue = Random.Range(0.1f, 1f);
                    if (randomShakingValue < randomChance)
                    {
                        if (lenght < 1) // shaking sound for short time
                        {
                            if (shakeLenghtSetting == ShakeLenghtType.Fix)
                            {
                                gasPedalValue = _oscillateOffset + Mathf.Sin(Time.time * (shakeLength * clipsValue)) * _oscillateRange;
                                clipsValue2 = (engineCurrentRPM / maxRPMLimit) + Mathf.Sin(Time.time * shakeLength) * (_oscillateRange / 10);
                            }
                            if (shakeLenghtSetting == ShakeLenghtType.Random)
                            {
                                gasPedalValue = _oscillateOffset + Mathf.Sin(Time.time * (Random.Range(10, 100) * clipsValue)) * _oscillateRange;
                                clipsValue2 = (engineCurrentRPM / maxRPMLimit) + Mathf.Sin(Time.time * Random.Range(10, 100)) * (_oscillateRange / 10);
                            }
                            lenght = lenght + Random.Range(0.01f, 0.12f);

                            clipsValue = clipsValue2;
                        }
                        else // end shaking
                        {
                            gasPedalValue = Mathf.Lerp(gasPedalValue, 1, Time.deltaTime * gasPedalSimSpeed);
                            clipsValue = engineCurrentRPM / maxRPMLimit;
                        }
                    }
                    else
                    {
                        gasPedalValue = Mathf.Lerp(gasPedalValue, 1, Time.deltaTime * gasPedalSimSpeed);
                        clipsValue = engineCurrentRPM / maxRPMLimit;
                    }
                }
                else
                {
                    clipsValue = engineCurrentRPM / maxRPMLimit;
                    randomShakingValue = 0;
                    if (randomShakingValue2 == 0)
                        randomShakingValue2 = Random.Range(0.1f, 1f);
                    lenght = 0;
                    gasPedalValue = Mathf.Lerp(gasPedalValue, 0, Time.deltaTime * gasPedalSimSpeed);
                }
            }
            //
            // idle
            if (idleClip != null)
            {
                if (idleVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                {
                    if (engineIdle == null)
                        CreateIdle();
                    else
                    {
                        engineIdle.volume = idleVolCurve.Evaluate(clipsValue) * masterVolume;
                        engineIdle.pitch = idlePitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                    }
                }
                else
                {
                    Destroy(engineIdle);
                }
            }
            //
            // low rpm
            if (lowVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
            {
                if (gasPedalPressing)
                {
                    if (lowOnClip != null)
                    {
                        if (lowOn == null)
                            CreateLowOn();
                        else
                        {
                            lowOn.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                            lowOn.pitch = lowPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            if (lowOff != null)
                            {
                                lowOff.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                                lowOff.pitch = lowPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                if (lowOff.volume < 0.1f)
                                    Destroy(lowOff);
                            }
                        }
                    }
                }
                else
                {
                    if (lowOffClip != null)
                    {
                        if (lowOff == null)
                            CreateLowOff();
                        else
                        {
                            if (!isReversing)
                            {
                                lowOff.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                                lowOff.pitch = lowPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            }
                        }
                        if (lowOn != null)
                        {
                            lowOn.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                            lowOn.pitch = lowPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            if (lowOn.volume < 0.1f)
                                Destroy(lowOn);
                        }
                    }
                }
            }
            else
            {
                if (lowOn != null)
                    Destroy(lowOn);
                if (lowOff != null)
                    Destroy(lowOff);
            }
            //
            // medium rpm
            if (medVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
            {
                if (gasPedalPressing)
                {
                    if (medOnClip != null)
                    {
                        if (medOn == null)
                            CreateMedOn();
                        else
                        {
                            medOn.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                            medOn.pitch = medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                        }
                        if (medOff != null)
                        {
                            medOff.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                            medOff.pitch = medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            if (medOff.volume < 0.1f)
                                Destroy(medOff);
                        }
                    }
                }
                else // gas pedal is released
                {
                    if (medOffClip != null)
                    {
                        if (medOff == null)
                            CreateMedOff();
                        else
                        {
                            if (!isReversing)
                            {
                                medOff.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                                medOff.pitch = medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            }
                        }
                        if (medOn != null)
                        {
                            medOn.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                            medOn.pitch = medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            if (medOn.volume < 0.1f)
                                Destroy(medOn);
                        }
                    }
                }
            }
            else
            {
                if (medOn != null)
                    Destroy(medOn);
                if (medOff != null)
                    Destroy(medOff);
            }
            //
            // high rpm
            if (highVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
            {
                if (gasPedalPressing)
                {
                    if (highOnClip != null)
                    {
                        if (highOn == null)
                            CreateHighOn();
                        else
                        {
                            if (!isReversing)
                            {
                                if (maxRPM != null)
                                {
                                    if (maxRPM.volume < 0.95f)
                                        highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                    else
                                        highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                                }
                                else
                                {
                                    highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                }
                                highOn.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            }
                        }
                        if (!isReversing)
                        {
                            if (highOff != null)
                            {
                                highOff.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                                highOff.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                if (highOff.volume < 0.1f)
                                    Destroy(highOff);
                            }
                        }
                    }
                }
                else // gas pedal is released
                {
                    if (highOffClip != null)
                    {
                        if (highOff == null)
                            CreateHighOff();
                        else
                        {
                            if (!isReversing)
                            {
                                highOff.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
                                highOff.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            }
                        }
                        if (!isReversing)
                        {
                            if (highOn != null)
                            {
                                if (maxRPM != null)
                                {
                                    if (maxRPM.volume < 0.95f)
                                        highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                    else
                                        highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                                }
                                else
                                {
                                    highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                }
                                highOn.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                if (highOn.volume < 0.1f)
                                    Destroy(highOn);
                            }
                        }
                    }
                }
            }
            else
            {
                if (highOn != null)
                    Destroy(highOn);
                if (highOff != null)
                    Destroy(highOff);
            }
            //
            // rpm limiting
            if (maxRPMVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
            {
                if (maxRPMClip != null)
                {
                    if (useRPMLimit) // if rpm limit is enabled, create audio source for it
                    {
                        if (maxRPM == null)
                            CreateRPMLimit();
                        else
                        {
                            maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue) * masterVolume;
                            maxRPM.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                        }
                    }
                }
                else // missing rpm limit audio clip
                {
                    useRPMLimit = false;
                }
            }
            else
            {
                Destroy(maxRPM);
            }
            //
            // reversing gear sound
            if (enableReverseGear)
            {
                if (reversingClip != null)
                {
                    if (isReversing)
                    {
                        if (reversingVolCurve.Evaluate(clipsValue) * masterVolume > optimisationLevel)
                        {
                            if (reversing == null)
                                CreateReverse();
                            else
                            {
                                if (gasPedalPressing)
                                {
                                    if (highOn == null)
                                        CreateHighOn();
                                    else
                                    {
                                        if (maxRPM != null)
                                        {
                                            if (maxRPM.volume < 0.95f)
                                                highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                            else
                                                highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                                        }
                                        else
                                        {
                                            highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                        }
                                        highOn.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                    }
                                    if (highOff != null)
                                    {
                                        highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                                        highOff.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                        if (highOff.volume < 0.1f)
                                            Destroy(highOff);
                                    }
                                }
                                else // gas pedal is released
                                {
                                    if (highOff == null)
                                        CreateHighOff();
                                    else
                                    {
                                        highOff.volume = highVolCurve.Evaluate(clipsValue) * (1 - gasPedalValue);
                                        highOff.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                    }
                                    if (highOn != null)
                                    {
                                        if (maxRPM != null)
                                        {
                                            if (maxRPM.volume < 0.95f)
                                                highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                            else
                                                highOn.volume = (highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue) / 3.3f; // max rpm is playing
                                        }
                                        else
                                        {
                                            highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
                                        }
                                        highOn.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                                        if (highOn.volume < 0.1f)
                                            Destroy(highOn);
                                    }
                                }
                                // set reversing sound to setted settings
                                reversing.volume = reversingVolCurve.Evaluate(clipsValue) * masterVolume;
                                reversing.pitch = reversingPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                            }
                        }
                        else
                        {
                            if (reversing != null)
                                Destroy(reversing);
                        }
                    }
                    else
                    {
                        if (reversing != null)
                            Destroy(reversing);
                    }
                }
                else
                {
                    isReversing = false;
                    enableReverseGear = false; // disable reversing sound because there is no audio clip for it
                }
            }
            else
            {
                if (isReversing != false)
                    isReversing = false;
            }
        }
        else
        {
            if (!alreadyDestroyed) // stop asking for destroy if it already done
            {
                DestroyAll(); // camera is far away, destroy all audio sources
                alreadyDestroyed = true; // destroy is done stop asking for destroy
            }
        }
    }
    private void FixedUpdate()
    {
        if (mainCamera != null)
        {
            if (Vector3.Distance(mainCamera.transform.position, gameObject.transform.position) > maxDistance)
            {
                isCameraNear = false;
            }
            else
            {
                isCameraNear = true;
                if (alreadyDestroyed) // reset stop asking for destroy if it already done
                    alreadyDestroyed = false;
            }

            if (!enableReverseGear)
            {
                if (reversing != null)
                {
                    Destroy(reversing); // looks like someone disabled reversing on runtime, destroy this audio source
                }
            }
            // rpm limiting
            if (!useRPMLimit) // rpm limit is disabled in runtime, destroy it's audio source
            {
                if (maxRPM != null)
                    Destroy(maxRPM);
            }

            // reverb setting is changed
            if (reverbZoneControll != reverbZoneSetting)
                SetReverbZone();
        }
        else // missing main camera
        {
            isCameraNear = false;
        }
    }
    private void OnEnable() // recreate all audio sources if Realistic Engine Sound's script is reEnabled
    {
        StartCoroutine(WaitForStart());
        SetReverbZone();
    }
    private void OnDisable() // destroy audio sources if Realistic Engine Sound's script is disabled
    {
        DestroyAll();
    }
    private void DestroyAll() // destroy audio sources if Realistic Engine Sound's script is disabled or too far from camera
    {
        if (engineIdle != null)
            Destroy(engineIdle);
        if (lowOn != null)
            Destroy(lowOn);
        if (lowOff != null)
            Destroy(lowOff);
        if (medOn != null)
            Destroy(medOn);
        if (medOff != null)
            Destroy(medOff);
        if (highOn != null)
            Destroy(highOn);
        if (highOff != null)
            Destroy(highOff);
        if (useRPMLimit)
        {
            if (maxRPM != null)
                Destroy(maxRPM);
        }
        if (enableReverseGear)
        {
            if (reversing != null)
                Destroy(reversing);
        }
        if (gameObject.GetComponent<AudioReverbZone>() != null)
            Destroy(gameObject.GetComponent<AudioReverbZone>());
    }
    private void UpdateStartRange()
    {
        _oscillateRange = (_endRange - (1-shakeVolumeChange)) / 2;
        _oscillateOffset = _oscillateRange + (1-shakeVolumeChange);
        shakeVolumeChangeDetect = shakeVolumeChange; // detect value change on runtime
    }
    void SetReverbZone()
    {
        if (reverbZoneSetting == AudioReverbPreset.Off)
        {
            if (gameObject.GetComponent<AudioReverbZone>() != null)
                Destroy(gameObject.GetComponent<AudioReverbZone>());
        }
        else
        {
            if (gameObject.GetComponent<AudioReverbZone>() == null)
            {
                gameObject.AddComponent<AudioReverbZone>();
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = reverbZoneSetting;
            }
            else
            {
                gameObject.GetComponent<AudioReverbZone>().reverbPreset = reverbZoneSetting;
            }
        }
        reverbZoneControll = reverbZoneSetting;
    }
    IEnumerator WaitForStart()
    {
        while (true)
        {
            yield return _wait; // this is needed to avoid duplicate audio sources when gameobject is just enabled
            if (engineIdle == null)
                Start();
            break;
        }
    }
    // create audio sources
    // idle
    void CreateIdle()
    {
        if (idleClip != null)
        {
            engineIdle = gameObject.AddComponent<AudioSource>();
            engineIdle.spatialBlend = spatialBlend;
            engineIdle.rolloffMode = audioRolloffMode;
            engineIdle.dopplerLevel = dopplerLevel;
            engineIdle.volume = idleVolCurve.Evaluate(clipsValue) * masterVolume;
            engineIdle.pitch = idlePitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            engineIdle.minDistance = minDistance;
            engineIdle.maxDistance = maxDistance;
            engineIdle.clip = idleClip;
            engineIdle.loop = true;
            engineIdle.Play();
            if (audioMixer != null)
                engineIdle.outputAudioMixerGroup = audioMixer;
        }
    }
    // low
    void CreateLowOff()
    {
        if (lowOffClip != null)
        {
            lowOff = gameObject.AddComponent<AudioSource>();
            lowOff.spatialBlend = spatialBlend;
            lowOff.rolloffMode = audioRolloffMode;
            lowOff.dopplerLevel = dopplerLevel;
            lowOff.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
            lowOff.pitch = lowPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            lowOff.minDistance = minDistance;
            lowOff.maxDistance = maxDistance;
            lowOff.clip = lowOffClip;
            lowOff.loop = true;
            lowOff.Play();
            if (audioMixer != null)
                lowOff.outputAudioMixerGroup = audioMixer;
        }
    }
    void CreateLowOn()
    {
        if (lowOnClip != null)
        {
            lowOn = gameObject.AddComponent<AudioSource>();
            lowOn.spatialBlend = spatialBlend;
            lowOn.rolloffMode = audioRolloffMode;
            lowOn.dopplerLevel = dopplerLevel;
            lowOn.volume = lowVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
            lowOn.pitch = lowPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            lowOn.minDistance = minDistance;
            lowOn.maxDistance = maxDistance;
            lowOn.clip = lowOnClip;
            lowOn.loop = true;
            lowOn.Play();
            if (audioMixer != null)
                lowOn.outputAudioMixerGroup = audioMixer;
        }
    }
    // medium
    void CreateMedOff()
    {
        if (medOffClip != null)
        {
            medOff = gameObject.AddComponent<AudioSource>();
            medOff.spatialBlend = spatialBlend;
            medOff.rolloffMode = audioRolloffMode;
            medOff.dopplerLevel = dopplerLevel;
            medOff.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
            medOff.pitch = medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            medOff.minDistance = minDistance;
            medOff.maxDistance = maxDistance;
            medOff.clip = medOffClip;
            medOff.loop = true;
            medOff.Play();
            if (audioMixer != null)
                medOff.outputAudioMixerGroup = audioMixer;
        }
    }
    void CreateMedOn()
    {
        if (medOnClip != null)
        {
            medOn = gameObject.AddComponent<AudioSource>();
            medOn.spatialBlend = spatialBlend;
            medOn.rolloffMode = audioRolloffMode;
            medOn.dopplerLevel = dopplerLevel;
            medOn.volume = medVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
            medOn.pitch = medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            medOn.minDistance = minDistance;
            medOn.maxDistance = maxDistance;
            medOn.clip = medOnClip;
            medOn.loop = true;
            medOn.Play();
            if (audioMixer != null)
                medOn.outputAudioMixerGroup = audioMixer;
        }
    }
    // high
    void CreateHighOff()
    {
        if (highOffClip != null)
        {
            highOff = gameObject.AddComponent<AudioSource>();
            highOff.spatialBlend = spatialBlend;
            highOff.rolloffMode = audioRolloffMode;
            highOff.dopplerLevel = dopplerLevel;
            highOff.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * (1 - gasPedalValue);
            highOff.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            highOff.minDistance = minDistance;
            highOff.maxDistance = maxDistance;
            highOff.clip = highOffClip;
            highOff.loop = true;
            highOff.Play();
            if (audioMixer != null)
                highOff.outputAudioMixerGroup = audioMixer;
        }
    }
    void CreateHighOn()
    {
        if (highOnClip != null)
        {
            highOn = gameObject.AddComponent<AudioSource>();
            highOn.spatialBlend = spatialBlend;
            highOn.rolloffMode = audioRolloffMode;
            highOn.dopplerLevel = dopplerLevel;
            highOn.volume = highVolCurve.Evaluate(clipsValue) * masterVolume * gasPedalValue;
            highOn.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            highOn.minDistance = minDistance;
            highOn.maxDistance = maxDistance;
            highOn.clip = highOnClip;
            highOn.loop = true;
            highOn.Play();
            if (audioMixer != null)
                highOn.outputAudioMixerGroup = audioMixer;
        }
    }
    // rpm limit
    void CreateRPMLimit()
    {
        if (maxRPMClip != null)
        {
            maxRPM = gameObject.AddComponent<AudioSource>();
            maxRPM.spatialBlend = spatialBlend;
            maxRPM.rolloffMode = audioRolloffMode;
            maxRPM.dopplerLevel = dopplerLevel;
            maxRPM.volume = maxRPMVolCurve.Evaluate(clipsValue) * masterVolume;
            maxRPM.pitch = highPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            maxRPM.minDistance = minDistance;
            maxRPM.maxDistance = maxDistance;
            maxRPM.clip = maxRPMClip;
            maxRPM.loop = true;
            maxRPM.Play();
            if (audioMixer != null)
                maxRPM.outputAudioMixerGroup = audioMixer;
        }
    }
    // reversing
    void CreateReverse()
    {
        if (reversingClip != null)
        {
            reversing = gameObject.AddComponent<AudioSource>();
            reversing.spatialBlend = spatialBlend;
            reversing.rolloffMode = audioRolloffMode;
            reversing.dopplerLevel = dopplerLevel;
            reversing.volume = reversingVolCurve.Evaluate(clipsValue) * masterVolume;
            reversing.pitch = reversingPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            reversing.minDistance = minDistance;
            reversing.maxDistance = maxDistance;
            reversing.clip = reversingClip;
            reversing.loop = true;
            reversing.Play();
            if(audioMixer != null)
            reversing.outputAudioMixerGroup = audioMixer;
        }
    }
}
