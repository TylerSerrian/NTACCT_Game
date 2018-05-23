using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Made by team Not The Killjoys
    April Simmons
    Tyler Serrian
    Jarrett Serrian
*/
public class CreditScroll : MonoBehaviour {

	Vector3 initPos;
	public bool isPlaying;
	public GameObject skipButton;

	// Use this for initialization
	void Start () {
		initPos = transform.position;
		isPlaying = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + .001f, transform.position.z);
			//transform.position = new Vector3(transform.position.x, transform.position.y + .004f, transform.position.z);  //debug fast scroll
		}
	}
	
	void OnCollisionEnter(Collision c)
	{
		transform.position = initPos;
		isPlaying = false;
		GameObject.Find("TitleCanvas").GetComponent<Canvas>().enabled = true;
		//GameObject.Find("SkipButton").SetActive(false);
		skipButton.SetActive(false);
	}
	
	public void PlayCredits()
	{
		isPlaying = true;
		GameObject.Find("TitleCanvas").GetComponent<Canvas>().enabled = false;
		//GameObject.Find("SkipButton").SetActive(true);
		skipButton.SetActive(true);

	}

	public void SkipCredits()
	{
	    transform.position = initPos;
     	isPlaying = false;
        GameObject.Find("TitleCanvas").GetComponent<Canvas>().enabled = true;
        //GameObject.Find("SkipButton").SetActive(false);
        skipButton.SetActive(false);
	}
}
