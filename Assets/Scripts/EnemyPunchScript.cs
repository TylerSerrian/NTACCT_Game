using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunchScript : MonoBehaviour {

	public GameObject unit;
	private Animator anim;
	private bool isPunching = false;
	private bool playerHit = false;
	private bool enemyHit = false;
	private int strength = 25;
	private bool playerDead = false;
	private bool isDead = false;

	// Use this for initialization
	void Start () {
		//anim = GameObject.Find("M1Avatar").GetComponent<Animator>();
		anim = unit.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnTriggerEnter(Collider collision)
    {
		//print("plz halp");
		if (!isDead && !playerDead && !playerHit && !enemyHit && isPunching && collision.transform.gameObject.tag == "player")
        {
			//anim.applyRootMotion = false;
			//collision.transform.gameObject.GetComponent<Animator>().applyRootMotion = false;
			collision.transform.gameObject.GetComponent<YBotSimpleControlScript>().getFist().GetComponent<PlayerPunchScript>().PlayerCannotHit();
            collision.transform.gameObject.GetComponent<HealthScript>().damage(strength);
			playerDead = collision.transform.gameObject.GetComponent<YBotSimpleControlScript>().PlayerDead();
			//collision.transform.gameObject.GetComponent<Rigidbody>().AddForce(1000*new Vector3(0, -1000, 0));
			//collision.transform.gameObject.GetComponent<Rigidbody>().AddForce(-1 * collision.transform.gameObject.GetComponent<Rigidbody>().velocity);
			playerHit = true;
		}				
    }
	
	public void Punch() {
		if (!isPunching) {
			isPunching = true;
			anim.SetTrigger("punch");
		}
	}
	
	public void StopPunch() {
		isPunching = false;
		playerHit = false;
	}
	
	public void setDead() {
		isDead = true;
	}
	
	public void EnemyCanHit() {
		enemyHit = false;
	}
	
	public void EnemyCannotHit() {
		enemyHit = true;
	}
	
	public void revive() {
		isDead = false;
		enemyHit = false;
		isPunching = false;
	}
}
