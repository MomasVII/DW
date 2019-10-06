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

public class Muffler_mob : MonoBehaviour {

    RealisticEngineSound_mobile res;
    // master volume setting
    [Range(0.1f, 1.0f)]
    public float masterVolume = 1f;
    // audio mixer
    public AudioMixerGroup audioMixer;
    private AudioMixerGroup _audioMixer;
    // pitch multiplier
    [Range(0.5f, 2.0f)]
    public float pitchMultiplier = 1;
    // play time
    [Range(0.5f, 4)]
    public float playTime = 2;
    private float playTime_;
    // audio clips
    public AudioClip offClip;
    public AudioClip onClip;
    // audio sources
    private AudioSource offLoop;
    private AudioSource onLoop;
    // curve settings
    public AnimationCurve mufflerOffVolCurve;
    public AnimationCurve mufflerOnVolCurve;
    // private
    private float clipsValue;
    private int oneShotController = 0;
    private WaitForSeconds _playtime;

    void Start()
    {
        res = gameObject.transform.parent.GetComponent<RealisticEngineSound_mobile>();
        // audio mixer settings
        if (audioMixer != null) // user is using a seperate audio mixer for this prefab
        {
            _audioMixer = audioMixer;
        }
        else
        {
            if (res.audioMixer != null) // use engine sound's audio mixer for this prefab
            {
                _audioMixer = res.audioMixer;
                audioMixer = _audioMixer;
            }
        }
        playTime_ = playTime;
        UpdateWaitTime();
    }
    void Update()
    {
        clipsValue = res.engineCurrentRPM / res.maxRPMLimit; // calculate % percentage of rpm
        if (res.isCameraNear)
        {
            // play on loop
            if (res.gasPedalPressing)
            {
                oneShotController = 1; // prepare for one shoot
            }
            else
            {
                // play off loop
                if (oneShotController == 1)
                {
                    if (mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume > 0.09f)
                        oneShotController = 2; // one shot is played, do not play more
                    else
                        oneShotController = 0;
                }
            }
            // off loop
            if (mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume > 0.09f)
            {
                if (oneShotController == 2)
                {
                    if (offLoop == null)
                    {
                        CreateOff();
                    }
                    else
                    {
                        offLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                        offLoop.volume = mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume;
                    }
                }
            }
            else
            {
                if (offLoop != null)
                    Destroy(offLoop);
            }
            // on loop
            if (mufflerOnVolCurve.Evaluate(clipsValue) * masterVolume > 0.09f)
            {
                if (oneShotController == 1)
                {
                    if (onLoop == null)
                    {
                        CreateOn();
                    }
                    else
                    {
                        onLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
                        onLoop.volume = mufflerOnVolCurve.Evaluate(clipsValue) * masterVolume;
                    }
                }
            }
            else
            {
                if (onLoop != null)
                    Destroy(onLoop);
            }
        }
    }
    private void OnEnable() // if prefab got new audiomixer on runtime, it will use that after prefab got re-enabled
    {
        Start();
    }
    private void OnDisable()
    {
        if (onLoop != null)
            Destroy(onLoop);
        if (offLoop != null)
            Destroy(offLoop);
    }
    // create off loop
    void CreateOff()
    {
        if (offClip != null)
        {
            offLoop = gameObject.AddComponent<AudioSource>();
            offLoop.rolloffMode = res.audioRolloffMode;
            offLoop.minDistance = res.minDistance;
            offLoop.maxDistance = res.maxDistance;
            offLoop.spatialBlend = res.spatialBlend;
            offLoop.dopplerLevel = res.dopplerLevel;
            offLoop.volume = mufflerOffVolCurve.Evaluate(clipsValue) * masterVolume;
            offLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * 2;
            offLoop.clip = offClip;
            offLoop.loop = true;
            if (_audioMixer != null)
                offLoop.outputAudioMixerGroup = _audioMixer;
            offLoop.Play();
            StartCoroutine(Wait2());
        }
    }
    //  create on loop
    void CreateOn()
    {
        if (onClip != null)
        {
            onLoop = gameObject.AddComponent<AudioSource>();
            onLoop.rolloffMode = res.audioRolloffMode;
            onLoop.minDistance = res.minDistance;
            onLoop.maxDistance = res.maxDistance;
            onLoop.spatialBlend = res.spatialBlend;
            onLoop.dopplerLevel = res.dopplerLevel;
            onLoop.volume = mufflerOnVolCurve.Evaluate(clipsValue) * masterVolume;
            onLoop.pitch = res.medPitchCurve.Evaluate(clipsValue) * pitchMultiplier;
            onLoop.clip = onClip;
            onLoop.loop = true;
            if (_audioMixer != null)
                onLoop.outputAudioMixerGroup = _audioMixer;
            onLoop.Play();
            StartCoroutine(Wait1());
        }
    }
    private void UpdateWaitTime()
    {
        _playtime = new WaitForSeconds(playTime);
        playTime_ = playTime;
    }
    IEnumerator Wait1()
    {
        while (true)
        {
            yield return _playtime; // destroy audio playtime secconds later 
            oneShotController = 0;
            Destroy(onLoop);
            break;
        }
    }
    IEnumerator Wait2()
    {
        while (true)
        {
            yield return _playtime; // destroy audio playtime secconds later
            oneShotController = 0;
            Destroy(offLoop);
            break;
        }
    }
}
