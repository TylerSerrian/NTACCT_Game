using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunchScript : MonoBehaviour {

	public GameObject unit;
	private Animator anim;
	private bool isPunching = false;
	private bool enemyHit = false;
	private bool playerHit = false;
	private int strength = 25;

	// Use this for initialization
	void Start () {
		//anim = GameObject.Find("M1Avatar").GetComponent<Animator>();
		anim = unit.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPunching && Input.GetButtonDown("Punch")) {
			anim.SetTrigger("punch");
			isPunching = true;
		}
	}
	
	void OnTriggerEnter(Collider collision)
    {
		if (!enemyHit && isPunching && collision.transform.gameObject.tag == "SecurityGuard" && !collision.transform.gameObject.GetComponent<GetParentObject>().GetParent().GetComponent<HealthScript>().unitDead())
        {
			//anim.applyRootMotion = false;
			//collision.transform.gameObject.GetComponent<Animator>().applyRootMotion = false;
			if (!collision.transform.gameObject.GetComponent<GetParentObject>().GetParent().GetComponent<HandleIntruder>().isCaught()) {
				collision.transform.gameObject.GetComponent<GetParentObject>().GetParent().GetComponent<HealthScript>().damage(collision.transform.gameObject.GetComponent<HealthScript>().getMaxHealth());
			} else {
				collision.transform.gameObject.GetComponent<GetParentObject>().GetParent().GetComponent<HandleIntruder>().getFist().GetComponent<EnemyPunchScript>().EnemyCannotHit();
				collision.transform.gameObject.GetComponent<GetParentObject>().GetParent().GetComponent<HealthScript>().damage(strength);
			}
			
			//collision.transform.gameObject.GetComponent<Rigidbody>().AddForce(1000*new Vector3(0, -1000, 0));
			//collision.transform.gameObject.GetComponent<Rigidbody>().AddForce(-1 * collision.transform.gameObject.GetComponent<Rigidbody>().velocity);
			enemyHit = true;
		}				
    }
	
	public void Punch() {
		if (!isPunching) {
			anim.SetTrigger("punch");
			isPunching = true;
		}
	}
	
	public void StopPunch() {
		isPunching = false;
		enemyHit = false;
	}
	
	public void setPlayerHit() {
		playerHit = !playerHit;
	}
	
	public void PlayerCanHit() {
		playerHit = false;
	}
	
	public void PlayerCannotHit() {
		playerHit = true;
	}
}
