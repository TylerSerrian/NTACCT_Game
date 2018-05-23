using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTextScript : MonoBehaviour {

	private bool contact;
	private int numKeys = 0;
	private int numProj = 0;
	private int numHealth = 0;
	private int level = 1;

	// Use this for initialization
	void Start () {
		contact = false;
	}
	
	// Update is called once per frame
	void Update () {
		numKeys = GameObject.Find("M1Avatar").GetComponent<YBotSimpleControlScript>().getKeys();
		numProj = GameObject.Find("M1Avatar").GetComponent<YBotSimpleControlScript>().getProj();
		numHealth = GameObject.Find("M1Avatar").GetComponent<HealthScript>().getCurrentHealth();
		level = GameObject.Find("M1Avatar").GetComponent<YBotSimpleControlScript>().getLevel();
		if (contact && this.transform.gameObject.tag == "elevator" && numKeys == 3 && level == 1) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Press 'F' to beat level.";
		} else if (contact && this.transform.gameObject.tag == "Door" && numKeys == 3 && level == 2) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Door unlocked!";
		} else if (contact && this.transform.gameObject.tag == "projectile" && numProj >= 3) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Cannot hold more projectiles.";
		} else if (contact && this.transform.gameObject.tag == "projectile" && numProj < 3) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Press 'F' to pick projectile up.";
		} else if (contact && this.transform.gameObject.tag == "health" && numHealth >= 100) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Already at maximum health.";
		} else if (contact && this.transform.gameObject.tag == "health" && numHealth < 100) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Press 'F' to pick health up.";
		} else if (contact) {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
		} else {
			this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
	}
	
	/*void OnCollisionEnter(Collision collision)
    {
		if (collision.transform.gameObject.tag == "Player")
        {
            contact = true;
		}
						
    }*/

    //This is a physics callback
    /*void OnCollisionExit(Collision collision)
    {

		if (collision.transform.gameObject.tag == "Player")
        {
            contact = false;
		}
		
    }*/
	
	void OnTriggerEnter(Collider collision)
    {
		
		if (collision.transform.gameObject.tag == "player")
        {
            contact = true;
			numKeys = collision.transform.gameObject.GetComponent<YBotSimpleControlScript>().getKeys();
			numProj = collision.transform.gameObject.GetComponent<YBotSimpleControlScript>().getProj();
			numHealth = collision.transform.gameObject.GetComponent<HealthScript>().getCurrentHealth();
		}
						
    }

    //This is a physics callback
    void OnTriggerExit(Collider collision)
    {

		if (collision.transform.gameObject.tag == "player")
        {
            contact = false;
		}
		
    }
}
