using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathManager : MonoBehaviour {

	public GameObject[] pathBeacons;
	int targetPos = 5;
	public GameObject currArrowLocation;
	Vector3 offset;
	// Use this for initialization
	void Start () {
		offset = gameObject.transform.position - currArrowLocation.transform.position;
		offset = new Vector3 (offset.x, 0, offset.z);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target = new Vector3 (pathBeacons [targetPos].transform.position.x, gameObject.transform.position.y, pathBeacons [targetPos].transform.position.z);
		gameObject.transform.rotation = Quaternion.LookRotation (target - gameObject.transform.position, Vector3.up);
		gameObject.transform.position = new Vector3 (currArrowLocation.transform.position.x, gameObject.transform.position.y, currArrowLocation.transform.position.z) + offset;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag.Equals ("path")) {
			targetPos = (targetPos + 1) % pathBeacons.Length;
			Debug.Log ("Reached!");
		}
	}
}
