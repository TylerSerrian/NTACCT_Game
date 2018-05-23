using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour {

	private int ballStrength = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.transform.gameObject.tag == "SecurityGuard" &&  collision.transform.gameObject.GetComponent<Rigidbody>() != null && collision.impulse.magnitude > 1f) {
			collision.transform.gameObject.GetComponent<GetParentObject>().GetParent().GetComponent<HealthScript>().damage(ballStrength);
			GameObject powder = Instantiate (Resources.Load ("ProjectileExplosion")) as GameObject;
			powder.transform.position = this.gameObject.transform.position;
			Destroy(this.gameObject);
			Destroy(powder, 3);
		}
	}
}
