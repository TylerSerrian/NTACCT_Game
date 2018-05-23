using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonScript : MonoBehaviour {

	private bool restart = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (restart) {
			restart = false;
			Time.timeScale = Time.timeScale == 0 ? 1 : 0;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
	
	public void restartLevel() {
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		//SceneManager.LoadScene ("BasicOfficeScene");
		restart = true;
	}
}
