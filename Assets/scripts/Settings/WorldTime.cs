using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : MonoBehaviour
{

	private int nextUpdate = 1;
    private int myWorldTime;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("worldTime")) {
            myWorldTime = PlayerPrefs.GetInt("worldTime");
        } else {
            myWorldTime = 0;
        }
    }

    // Update is called once per frame
    void Update () {
		if(Time.time >= nextUpdate){
			// Change the next update (current second+1)
			nextUpdate = Mathf.FloorToInt(Time.time)+1;
            myWorldTime++;
            PlayerPrefs.SetInt("worldTime", myWorldTime);

            /*System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            PlayerPrefs.SetInt("worldDate", cur_time);*/
         }
	}
}
