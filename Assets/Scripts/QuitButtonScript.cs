using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
    Made by team Not The Killjoys
    April Simmons
    Tyler Serrian
    Jarrett Serrian
*/
public class QuitButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void QuitGame() {
		#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
       	#else
        	Application.Quit();
        #endif
	}

}
