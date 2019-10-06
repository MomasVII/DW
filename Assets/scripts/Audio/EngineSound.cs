using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour {

	//Setting's Values
	public float[] MinRpmTable = {-100.0f, 75.0f, 112.0f, 166.9f, 222.4f, 278.3f, 333.5f, 388.2f, 435.5f, 483.3f, 538.4f, 594.3f, 643.6f, 692.8f, 741.9f, 790.0f};
	public float[] NormalRpmTable = {72.0f, 93.0f, 155.9f, 202.8f, 267.0f, 314.5f, 377.4f, 423.9f, 472.1f, 519.4f, 582.3f, 631.3f, 680.8f, 729.4f, 778.8f, 826.1f};
	public float[] MaxRpmTable = {92.0f, 136.0f, 182.9f, 247.4f, 294.3f, 357.5f, 403.6f, 452.5f, 499.3f, 562.5f, 612.3f, 661.6f, 708.8f, 758.9f, 806.0f, 1000.0f};
	public float[] PitchingTable = {0.12f, 0.12f, 0.12f, 0.12f, 0.11f, 0.10f, 0.09f, 0.08f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f, 0.06f};

	public float engineRPM;
	public float soundRPM;
	public float[] gearRatio;
    public int currentGear;
	public float gearUpRPM;
    public float gearDownRPM;
	public float RangeDivider = 4f;

	public float demoSoundRPM;

	//Used to move wheels and turn car on demo replayTime
	private Ghost ghostScript;

	//Get Wheel colliders for RPM
	private WheelCollider rearRight, rearLeft;

	public List<AudioSource> carSound;

	// Use this for initialization
	void Start () {

		ghostScript = GameObject.FindObjectOfType<Ghost>();

		GameObject car = GameObject.FindWithTag("Player");
		rearLeft = car.transform.Find("wheels/RearLeft").GetComponent<WheelCollider>();
		rearRight = car.transform.Find("wheels/RearRight").GetComponent<WheelCollider>();

		for(int i = 1; i <= 16; i++) {
			carSound.Add(GameObject.Find(string.Format("CarSound ({0})", i)).GetComponent<AudioSource>());
			carSound[i-1].Play();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if(ghostScript != null) {
			if(ghostScript.playingDemo()) {
				soundRPM = ghostScript.getDemoSoundRPM();
			} else {
				engineRPM = (((rearRight.rpm + rearLeft.rpm) / 2) * gearRatio[currentGear]);
				soundRPM = Mathf.Round(engineRPM * (1000 / 420));
			}
		} else {
			engineRPM = (((rearRight.rpm + rearLeft.rpm) / 2) * gearRatio[currentGear]);
			soundRPM = Mathf.Round(engineRPM * (1000 / 420));
		}
		//soundRPM += 50;
		AutoGears();

		if(this.transform.parent.tag != "Ghost") {
			//PlayCarSound();
		}
	}

	public float getSoundRPM() {
		return soundRPM;
	}

	void AutoGears()
    {

        int AppropriateGear = currentGear;

        if (engineRPM >= gearUpRPM) {

            for (var i = 0; i < gearRatio.Length; i++) {
                if (rearRight.rpm * gearRatio[i] < gearUpRPM) {
                    AppropriateGear = i;
                    break;
                }
            }
            currentGear = AppropriateGear;
            //BacFire_Sound.PlayOneShot(BacFire_Sound.clip);
        }

        if (engineRPM <= gearDownRPM) {
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

	void PlayCarSound() {
		//Set Volume By Rpm's
        for (int i = 0; i < 16; i++) {
            if (i == 0) {
                //Set carSound[0]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[0].volume = 50.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[0].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[0].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[0].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[0].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[0].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[0].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 1) {
                //Set carSound[1]
                if (soundRPM < MinRpmTable[i]) { // < 75 Minimum RMP to make any noise
                    carSound[1].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) { // Between 75 - 93
                    float Range = NormalRpmTable[i] - MinRpmTable[i]; // 93 - 75 = 18
                    float ReducedRPM = soundRPM - MinRpmTable[i]; // Between 0 and 18
                    carSound[1].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[1].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[1].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[1].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[1].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[1].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 2) {
                //Set carSound[2]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[2].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[2].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[2].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[2].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[2].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[2].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[2].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 3) {
                //Set carSound[3]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[3].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[3].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[3].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[3].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[3].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[3].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[3].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 4) {
                //Set carSound[4]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[4].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[4].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[4].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[4].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[4].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[4].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[4].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 5) {
                //Set carSound[5]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[5].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[5].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[5].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[5].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[5].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[5].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[5].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 6) {
                //Set carSound[6]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[6].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[6].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[6].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[6].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[6].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[6].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[6].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 7) {
                //Set carSound[7]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[7].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[7].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[7].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[7].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[7].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[7].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[7].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 8) {
                //Set carSound[8]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[8].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[8].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[8].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[8].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[8].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[8].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[8].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 9) {
                //Set carSound[9]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[9].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[9].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[9].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[9].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[9].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[9].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[9].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 10) {
                //Set carSound[8]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[10].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[10].volume = ((ReducedRPM / Range) * 2f) - 1f;
                    //carSound[10].volume = 0.0f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[10].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[10].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[10].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[10].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[10].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 11) {
                //Set carSound[11]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[11].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[11].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[11].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[11].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[11].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[11].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[11].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 12) {
                //Set carSound[12]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[12].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[12].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[12].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[12].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[12].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[12].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[12].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 13) {
                //Set carSound[13]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[13].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[13].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[13].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[13].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[13].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[13].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[13].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 14) {
                //Set carSound[14]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[14].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[14].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[14].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[14].volume = 1f;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[14].pitch = 1f + PitchMath;
                } else if (soundRPM > MaxRpmTable[i]) {
                    float Range = (MaxRpmTable[i + 1] - MaxRpmTable[i]) / RangeDivider;
                    float ReducedRPM = soundRPM - MaxRpmTable[i];
                    carSound[14].volume = 1f - ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    //carSound[14].pitch = 1f + PitchingTable[i] + PitchMath;
                }
            } else if (i == 15) {
                //Set carSound[14]
                if (soundRPM < MinRpmTable[i]) {
                    carSound[15].volume = 0.0f;
                } else if (soundRPM >= MinRpmTable[i] && soundRPM < NormalRpmTable[i]) {
                    float Range = NormalRpmTable[i] - MinRpmTable[i];
                    float ReducedRPM = soundRPM - MinRpmTable[i];
                    carSound[15].volume = ReducedRPM / Range;
                    float PitchMath = (ReducedRPM * PitchingTable[i]) / Range;
                    carSound[15].pitch = 1f - PitchingTable[i] + PitchMath;
                } else if (soundRPM >= NormalRpmTable[i] && soundRPM <= MaxRpmTable[i]) {
                    float Range = MaxRpmTable[i] - NormalRpmTable[i];
                    float ReducedRPM = soundRPM - NormalRpmTable[i];
                    carSound[15].volume = 1f;
                    float PitchMath = (ReducedRPM * (PitchingTable[i] + 0.1f)) / Range;
                    carSound[15].pitch = 1f + PitchMath;
                }
            }
        }

	}
}
