using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    Made by team Not The Killjoys
    April Simmons
    Tyler Serrian
    Jarrett Serrian
*/
public class StartButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void onClick() {
		StartGame ();
	}
	
	public void StartGame() {
		Canvas titleCanvas = GameObject.Find ("TitleCanvas").GetComponent<Canvas> ();
		Text title = titleCanvas. GetComponentInChildren<Text>();

		Text introText = GameObject.Find ("IntroTextCanvas").GetComponent<Canvas> ().GetComponentInChildren<Text>();
		introText.transform.position = new Vector3 (title.transform.position.x,
			title.transform.position.y - .145f, title.transform.position.z);

		GameObject.Find ("PlayGameButton").transform.position = new Vector3 (title.transform.position.x,
			title.transform.position.y - .44f, title.transform.position.z);

		titleCanvas.enabled = false;
	}

	public void PlayGame() {
		SceneManager.LoadScene ("BasicOfficeScene");
	}
}
