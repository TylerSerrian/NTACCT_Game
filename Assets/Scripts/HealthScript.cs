using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour {

	public GameObject unit;
	private Animator anim;
	public GameObject hips;

	public int maxHealth;
	private int currentHealth;
	
	private bool isDead = false;

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		anim = unit.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void damage(int d) {
		currentHealth -= d;
		print("Oof! " + currentHealth);
		if (unit.tag == "player") {
			GameObject.Find("NumHealth").GetComponent<Text>().text = "Health: " + currentHealth + "/" + maxHealth;
			EventManager.TriggerEvent<PlayerHitEvent, Vector3> (this.transform.position);
		}
		
		if (currentHealth <= 0 && !isDead) {
			print("I am slain!");
			Ragdoll();
			isDead = true;
			
			if (unit.tag == "SecurityGuard") {
				unit.GetComponent<HandleIntruder>().getFist().GetComponent<EnemyPunchScript>().setDead();
				unit.GetComponent<HandleIntruder>().setDead();
				EventManager.TriggerEvent<GuardDeathEvent, Vector3> (this.transform.position);
			} else if (unit.tag == "player") {
				//player death sound
			}
		} else {
			// implement recoil from being hit
			anim.SetTrigger("hit");
			if (unit.tag == "SecurityGuard")
			{
				EventManager.TriggerEvent<EnemyHitEvent, Vector3> (this.transform.position);
			}
		}
	}
	
	public void heal(int h) {
		if (currentHealth + h <= maxHealth) {
			currentHealth += h;
		} else {
			currentHealth = maxHealth;
		}
		GameObject.Find("NumHealth").GetComponent<Text>().text = "Health: " + currentHealth + "/" + maxHealth;
	}
	
	void Ragdoll() {
		anim.enabled = false;
		//GameObject hips = GameObject.Find ("M1Avatar/mixamorig:Hips");
		Rigidbody[] rb = hips.transform.GetComponentsInChildren<Rigidbody> ();
		foreach (Rigidbody r in rb) {
			r.velocity = new Vector3 (0, 0, 0);
		}
		//SceneManager.LoadScene ("demo");
		//isDead = true;
		//timeOfDeath = Time.timeSinceLevelLoad;
		if (unit.tag == "player") {
			unit.GetComponent<YBotSimpleControlScript>().Die();
		}
	}
	
	public int getMaxHealth() {
		return maxHealth;
	}
	
	public int getCurrentHealth() {
		return currentHealth;
	}
	
	public bool unitDead() {
		return isDead;
	}	
	
	public void revive() {
		currentHealth = maxHealth;
		isDead = false;
		unit.GetComponent<HandleIntruder>().getFist().GetComponent<EnemyPunchScript>().revive();
		unit.GetComponent<HandleIntruder>().revive();
	}
}
