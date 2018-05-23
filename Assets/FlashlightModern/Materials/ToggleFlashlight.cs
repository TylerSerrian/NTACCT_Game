using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFlashlight : MonoBehaviour
{
    int flashlightState = 1;
    public GameObject lightOnPrefab;
    public GameObject lightOffPrefab;
    public AudioClip switchSound;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// if (Input.GetKeyDown(KeyCode.F)) { // press F to toggle
		//	toggleFlashlight();
		// }
	}

	public void toggleFlashlight() {
		// toggle 1-0=1 , 1-1=0 
        flashlightState = 1-flashlightState; 

		// get audiosource
        AudioSource audio = GetComponent<AudioSource>(); 
		// play the clip
        audio.Play(); 

		// change depending on state
		if (flashlightState==1) { 
			lightOnPrefab.SetActive(true);
			lightOffPrefab.SetActive(false);
        }
        if (flashlightState == 0) {
			lightOnPrefab.SetActive(false);
			lightOffPrefab.SetActive(true);
        }
	}
}
