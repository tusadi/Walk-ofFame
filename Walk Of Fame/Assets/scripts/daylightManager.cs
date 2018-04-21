using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject))]

public class daylightManager : MonoBehaviour {

	SteamVR_TrackedObject trackedObj;
	SteamVR_Controller.Device device;

	SteamVR_TrackedController ctrlr;

	public int offsetHour = 6;
	public Clock clockObj;
	public float angle = 0;

	void Start(){
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
		ctrlr = GetComponent<SteamVR_TrackedController> ();
	}


	void Update(){

		device = SteamVR_Controller.Input ((int)trackedObj.index);


		if (device.GetAxis ().magnitude > 0) {
			angle = -(float)(Mathf.Atan2 (device.GetAxis ().y, device.GetAxis ().x) * Mathf.Rad2Deg - 90) % 360;

			if (angle > 0) {
				clockObj.hour = (int)(angle / 360 * 24);
			} else {
				clockObj.hour = (int)((360 + angle) / 360 * 24);
			}
		}

	}

}
