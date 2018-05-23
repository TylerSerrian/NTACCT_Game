using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRoomEnterExit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider collision) {
	}

	void OnTriggerExit(Collider collision) {
		// Close the door on your way out.  Where you raised in a barn?
		if (collision.transform.name.StartsWith ("SecurityGuard")) {
			Detection detectionScript = collision.transform.GetComponent<Detection> ();
			if (detectionScript != null) {
				detectionScript.closeDoor ();
			}
		}

	}
}
