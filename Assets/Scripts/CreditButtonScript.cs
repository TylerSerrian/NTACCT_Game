using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Made by team Not The Killjoys
    April Simmons
    Tyler Serrian
    Jarrett Serrian
*/
public class CreditButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void onClick() {
		GameObject.Find("CreditText").GetComponent<CreditScroll>().PlayCredits();
	}
}
