using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {
//-----------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------
//
//  Simple Clock Script / Andre "AEG" Bürger / VIS-Games 2012
//
//-----------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------
	public float val;
    public int minutes = 0;
    public int hour = 0;
	public float currTimeIntensity; 
    public float clockSpeed = 1.0f;     // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster
	public Light daylightSource;
    int seconds;
    float msecs;
    GameObject pointerSeconds;
    GameObject pointerMinutes;
    GameObject pointerHours;

	public Material BrightMorning, AfterNoon, Noon, Sunset, Night, MidNight, EarlyDusk;
	void Start()
	{
    pointerSeconds = transform.Find("rotation_axis_pointer_seconds").gameObject;
    pointerMinutes = transform.Find("rotation_axis_pointer_minutes").gameObject;
    pointerHours   = transform.Find("rotation_axis_pointer_hour").gameObject;

    msecs = 0.0f;
    seconds = 0;
	}	
	void Update()
	{
		msecs += Time.deltaTime * clockSpeed;
		if (msecs >= 1.0f) {
			msecs -= 1.0f;
			seconds++;
			if (seconds >= 60) {
				seconds = 0;
				minutes++;
				if (minutes > 60) {
					minutes = 0;
					hour++;
					if (hour >= 24)
						hour = 0;
				}
			}
		}


		float rotationSeconds = (360.0f / 60.0f) * seconds;
		float rotationMinutes = (360.0f / 60.0f) * minutes;
		float rotationHours = ((360.0f / 12.0f) * hour) + ((360.0f / (60.0f * 12.0f)) * minutes);

		pointerSeconds.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, rotationSeconds);
		pointerMinutes.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, rotationMinutes);
		pointerHours.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, rotationHours);

		if (hour <= 12) {
			currTimeIntensity = ((hour + minutes / 60) / 12.0f);
			daylightSource.intensity = currTimeIntensity;
		} else {
			currTimeIntensity = ((24 - hour + minutes / 60) / 12.0f);
			daylightSource.intensity = currTimeIntensity;
		}

		if (hour >= 0 && hour <= 3) {
			val = (3 - hour) * 60 + minutes;
			//Debug.Log ("MidNight");
			RenderSettings.skybox = MidNight;
			//RenderSettings.skybox.Lerp (RenderSettings.skybox, MidNight, val / 180.0f);
		} else if (hour >= 3 && hour <= 6) {
			val = (6 - hour) * 60 + minutes;
			//Debug.Log ("EarlyDusk");
			RenderSettings.skybox = EarlyDusk;

			//RenderSettings.skybox.Lerp (RenderSettings.skybox, EarlyDusk, val / 180.0f);
		} else if (hour >= 6 && hour <= 11) {
			val = (11 - hour) * 60 + minutes;
		//	Debug.Log ("BrightMorning");
			RenderSettings.skybox = BrightMorning;

			//RenderSettings.skybox.Lerp (RenderSettings.skybox, BrightMorning, val / 300.0f);
		} else if (hour >= 11 && hour <= 15) {
			//Debug.Log ("AfterNoon");
			val = (3 - hour) * 60 + minutes;
			RenderSettings.skybox = AfterNoon;

			//RenderSettings.skybox.Lerp (RenderSettings.skybox, AfterNoon, val / 240.0f);
		} else if (hour >= 15 && hour <= 18) {
			val = (3 - hour) * 60 + minutes;
			//Debug.Log ("Noon");

			RenderSettings.skybox.Lerp (RenderSettings.skybox, Noon, val / 180.0f);
		} else if (hour >= 18 && hour <= 20) {
			val = (3 - hour) * 60 + minutes;
			//Debug.Log ("Sunset");
 			RenderSettings.skybox = Sunset;

			//RenderSettings.skybox.Lerp (RenderSettings.skybox, Sunset, val / 120.0f);
		} else if (hour >= 20 && hour <= 0) {
			val = (3 - hour) * 60 + minutes;
			//Debug.Log ("Night");
			RenderSettings.skybox = Night;

			//RenderSettings.skybox.Lerp (RenderSettings.skybox, Night, val / 240.0f);
		}

	}
}
