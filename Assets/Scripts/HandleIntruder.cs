using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HandleIntruder : MonoBehaviour {

	private AINavSteeringController aiSteer;
	private NavMeshAgent agent;
	private AudioSource audioPlayer;
	private Logger logger;
	private Animator anim;	
	private Rigidbody rigidBody;
	private SGAudioEventMgrScript sgAudioEventMgrScript;

	// If an intruder is found, this will contain the intruder
	private Transform intruder;
	private bool caught = false;
	private bool isDead = false;
	private bool revivingFallenComrade = false;
	private float timeFoundFallenComrade = 0f;
	private const float timeToReviveFallenComrade = 20f;


	[Tooltip("Used for throwing punches.")]
	public GameObject fistCollider;

	[Tooltip("sightDistance: How far the security guard can 'see'.")]
	public float sightDistance = 15f;
	[Tooltip("fightDistance: How close the intruder must be for the security guard to start fighting him/her.")]
	public float fightDistance = 1.5f;
	[Tooltip("intruderTag: The tag a transform must have to be considered an intruder.")]
	public string intruderTag = "player";
	[Tooltip("intruderCheckStartingPoint: The starting point for the 'rays' that constitute the security guard's sight.")]
	public Transform intruderCheckStartingPoint;

	public Transform leftFoot;
    public Transform rightFoot;
	private bool leftFootStep = false;
	private bool rightFootStep = false;
	private int layerMask;


	[Header("For Spawning Guards")]
	public Transform SecurityGuardPrefab;

	private Vector3[] roundsWaypoints = {};
	private static int spawnedGuardCount = 1;
	private static System.Object synchronizedLockKey = new System.Object();
	private bool handlingLostIntruder = false;

	private struct SecurityGuardInfo {
		public Vector3 startingPosition;
		public Quaternion startingRotation;
		public Vector3[] sgiRoundsWaypoints;
		public string guardName;
		public SecurityGuardInfo(string name, Vector3 position, Quaternion rotation, Vector3[] waypointsSGIArg) {
			this.guardName = name;
			this.startingPosition = position;
			this.startingRotation = rotation;
			this.sgiRoundsWaypoints = waypointsSGIArg;
		}

		public static Vector3[] frack_waypoints = {
			new Vector3 (7.6f, 0.5f, 11.2f), 
			new Vector3 (8.2f, 0.5f, 35.6f), 
			new Vector3 (10.5f, 0.5f, 42.1f), 
			new Vector3 (22.7f, 0.5f, 37.4f), 
			new Vector3 (30.0f, 0.5f, 17.0f), 
			new Vector3 (51.8f, 0.5f, 12.0f), 
			new Vector3 (51.7f, 0.5f, 35.9f), 
			new Vector3 (34.8f, 0.5f, 41.9f), 
			new Vector3 (30.1f, 0.5f, 29.5f)
		};
		public static Vector3[] frick_waypoints = {
			new Vector3 (30.1f, 0.5f, 29.5f), 
			new Vector3 (34.8f, 0.5f, 41.9f), 
			new Vector3 (51.7f, 0.5f, 35.9f), 
			new Vector3 (51.8f, 0.5f, 12.0f), 
			new Vector3 (30.0f, 0.5f, 17.0f), 
			new Vector3 (22.7f, 0.5f, 37.4f), 
			new Vector3 (10.5f, 0.5f, 42.1f), 
			new Vector3 (8.2f, 0.5f, 35.6f), 
			new Vector3 (7.6f, 0.5f, 11.2f)
		};
		public static SecurityGuardInfo[] guard_infos = {
			new SecurityGuardInfo("SecurityGuard-frack", new Vector3(30.2f, 0.0f, 21.3f), 
				new Quaternion(0.0f, 1.0f, 0.0f, 0.0f), frack_waypoints
			),
			new SecurityGuardInfo("SecurityGuard-frick", new Vector3(30.0f, 0.0f, 24.1f), 
				new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), frick_waypoints
			)
		};

		public static Transform spawnSecurityGuard(Transform securityGuardPrefab) {
			int rnd_idx = Random.Range(0, guard_infos.Length);
			SecurityGuardInfo guard_info = guard_infos [rnd_idx];
			// Create from prefab
			Transform security_guard = Instantiate(securityGuardPrefab, guard_info.startingPosition, guard_info.startingRotation);
			security_guard.name = guard_info.guardName;
			HandleIntruder handleIntruderScript = security_guard.GetComponent<HandleIntruder>();
			handleIntruderScript.roundsWaypoints = guard_info.sgiRoundsWaypoints;
			return security_guard;
		}
	}



	// Use this for initialization
	void Start () {
		
		aiSteer = GetComponent<AINavSteeringController>();
		agent = GetComponent<NavMeshAgent>();
		audioPlayer = GetComponent<AudioSource> ();	
		anim = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();


		layerMask = ~(1 << LayerMask.NameToLayer("Ragdoll"));

		GameObject sgAudioEventMgr = GameObject.Find ("SecurityGuardAudioEventManager");
		if (sgAudioEventMgr != null) {
			sgAudioEventMgrScript = sgAudioEventMgr.GetComponent<SGAudioEventMgrScript> ();
		}


		GameObject lm = GameObject.Find ("LoggerManager");
		if (lm == null) {
			print ("Logger Manager not found.");
		} else {
			this.logger = lm.GetComponent<Logger> ();
			if (this.logger != null)
				this.logger.Log ("Started Security Guard: " + transform.name);
		}

		if (this.roundsWaypoints.Length == 0) {
			if (transform.name.EndsWith ("frack")) {
				this.roundsWaypoints = SecurityGuardInfo.frack_waypoints;
				print ("Created: " + transform.name);
			} else if (transform.name.EndsWith ("frick")) {
				this.roundsWaypoints = SecurityGuardInfo.frick_waypoints;
				print ("Created: " + transform.name);
			}
		} else {
			// You were generated in code from a prefab
			lock(synchronizedLockKey) {
				transform.name = transform.name + "-" + spawnedGuardCount;
				spawnedGuardCount += 1;
			}
			print("Spawned: " + transform.name);
		}
		aiSteer.setWaypoints (this.roundsWaypoints);

	}

	// Update is called once per frame; FixedUpdate is called on a regular schedule
	void FixedUpdate () {
		
		if (anim.GetFloat ("foot") > 0.6 && !leftFootStep) {
				const float rayOriginOffset = 1f; //origin near bottom of collider, so need a fudge factor up away from there
				const float rayDepth = 1f; //how far down will we look for ground?
				const float totalRayLen = rayOriginOffset + rayDepth;

				Ray ray = new Ray(leftFoot.transform.position + Vector3.up * rayOriginOffset, Vector3.down);

				//visualize ray in the editor
				//I'm using DrawLine because Debug.DrawRay() doesn't allow setting ray length past a certain size
				Debug.DrawLine(ray.origin, ray.origin + ray.direction * totalRayLen, Color.green); 

				RaycastHit hit;


				//Cast ray and look for ground. If ground is close, then transition out of falling animation
				if (Physics.Raycast (ray, out hit, totalRayLen, layerMask)) {
					//print (hit.collider.gameObject.gameObject.name);
					//if (hit.collider.gameObject.CompareTag ("ground")) {
					//	surfaceType = "ground";
					//} else if (hit.collider.gameObject.CompareTag ("sand")) {
					//	surfaceType = "sand";
					//}
				}
				//EventManager.TriggerEvent<footStepEvent, Vector3, string> (leftFoot.transform.position, surfaceType);
				EventManager.TriggerEvent<footStepEvent, Vector3, string> (leftFoot.transform.position, "office");
				//EventManager.TriggerEvent<ParticleEffectEvent, Vector3, string> (leftFoot.transform.position, surfaceType);
				leftFootStep = true;
				rightFootStep = false;
		} else if (anim.GetFloat ("foot") < -0.6 && !rightFootStep) {
			const float rayOriginOffset = 1f; //origin near bottom of collider, so need a fudge factor up away from there
			const float rayDepth = 1f; //how far down will we look for ground?
			const float totalRayLen = rayOriginOffset + rayDepth;

			Ray ray = new Ray(rightFoot.transform.position + Vector3.up * rayOriginOffset, Vector3.down);

			//visualize ray in the editor
			//I'm using DrawLine because Debug.DrawRay() doesn't allow setting ray length past a certain size
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * totalRayLen, Color.green); 

			RaycastHit hit;


			//Cast ray and look for ground. If ground is close, then transition out of falling animation
			if (Physics.Raycast (ray, out hit, totalRayLen, layerMask)) {
				//print (hit.collider.gameObject.gameObject.name);
				//if (hit.collider.gameObject.CompareTag ("ground")) {
				//	surfaceType = "ground";
				//} else if (hit.collider.gameObject.CompareTag ("sand")) {
				//	surfaceType = "sand";
				//}
			}
			//EventManager.TriggerEvent<footStepEvent, Vector3, string> (rightFoot.transform.position, surfaceType);
			EventManager.TriggerEvent<footStepEvent, Vector3, string> (leftFoot.transform.position, "office");
			//EventManager.TriggerEvent<ParticleEffectEvent, Vector3, string> (rightFoot.transform.position, surfaceType);
			rightFootStep = true;
			leftFootStep = false;
		} else if (anim.GetFloat ("foot") >= -0.1 && anim.GetFloat ("foot") <= 0.1) {
			rightFootStep = false;
			leftFootStep = false;
		}

		if (isDead || handlingLostIntruder) {
			return;
		}

		if (!this.revivingFallenComrade || ((this.timeFoundFallenComrade > 0f) && 
											((this.timeFoundFallenComrade + timeToReviveFallenComrade) < Time.timeSinceLevelLoad))) {
			this.revivingFallenComrade = false;
			this.timeFoundFallenComrade = 0f;
			checkForFallenComrades ();
		}

		// If the guard is actively pursuing/fighting an intruder ...
		if (intruder != null) {
			float distance = Vector3.Distance (transform.position, intruder.position);
			// print ("FixedUpdate: Intruder " + intruder.name + " is " + distance + "f units away.");
			if (distance < this.fightDistance) { // fight
				// print ("In FixedUpdate : About to fight");
				agent.isStopped = true;
				fight ();
			} else if (distance <= (this.sightDistance + 1)) { // chase
				// print ("In FixedUpdate : Chasing");
				aiSteer.neverReachWaypoint = true;
				aiSteer.setWaypoint (intruder.transform);
				agent.isStopped = false;
			} else { // lost them
				// print ("In FixedUpdate : Lost 'em");
				StartCoroutine(handleLostIntruder());
			}
		} else {
			// print ("In FixedUpdate : Checking for intruder");
			checkForIntruder (); // this will set intruder variable
			if (!isDead && intruder != null) {
				StartCoroutine(handleIntruder());
				caught = true;
			}
		}
	}

	IEnumerator handleLostIntruder() {
		handlingLostIntruder = true;

		// Stop and look around
		agent.isStopped = true;
		anim.SetTrigger ("lookAround");

		// Play lost intruder audio and wait
		anim.SetBool("talk", true);
		yield return new WaitWhile (() => audioPlayer.isPlaying);
		audioPlayer.PlayOneShot (this.sgAudioEventMgrScript.LostIntruder);
		yield return new WaitWhile (() => audioPlayer.isPlaying);
		anim.SetBool("talk", false);

		// Play replies from other guards
		int numRepliesNeeded = (int)Mathf.Round (Mathf.Sqrt (spawnedGuardCount));
		for (int replyIdx = 0; replyIdx < numRepliesNeeded; replyIdx++) {
			int audioIdx = Random.Range (0, this.sgAudioEventMgrScript.Replies.Length);
			audioPlayer.PlayOneShot (this.sgAudioEventMgrScript.Replies [audioIdx]);
		}

		// Spawn one new guard and wait
		SecurityGuardInfo.spawnSecurityGuard (this.SecurityGuardPrefab);
		yield return new WaitForSeconds (2);

		// Continue On Rounds
		aiSteer.mecanimInputForwardSpeedCap = 0.5f;
		aiSteer.mecanimMaxSpeed = 5f;
		aiSteer.neverReachWaypoint = false;
		aiSteer.setWaypoints (this.roundsWaypoints);
		agent.isStopped = false;
		intruder = null;
		handlingLostIntruder = false;
	}

	private void checkForIntruder() {
		// After checking forward, rays will be built by
		// going back and forth and adding to the left, then right, etc.
		bool checkLeft = false;
		// The distance between rays on the x and z axis
		float stepSize = 0.0625f;
		// Endpoints of the rays that constitute "sight"
		Vector3 endpoint = new Vector3 (transform.forward.x,
			                   transform.forward.y, transform.forward.z);
		
		for (int idx = 0; idx < 17; idx++) {
			if (checkLeft) {
				endpoint.x += (idx * stepSize);
				endpoint.z -= stepSize;
			} else {
				endpoint.x -= (idx * stepSize);
				// endpoint.z stays same
			}
			checkLeft = !checkLeft;
			// print ("\tendpoint = " + endpoint.ToString("F4"));

			Ray ray = new Ray (intruderCheckStartingPoint.position, endpoint);
			// Debug.DrawRay (ray.origin, ray.direction * this.sightDistance, Color.red);

			RaycastHit hit; // Variable reading information about the collider hit

			// Cast ray from center of the screen towards where the player is looking
			if (Physics.Raycast (ray, out hit, this.sightDistance)) {
				// print ("Raycast found: " + hit.transform);
				if (hit.transform.tag == intruderTag) {
					intruder = hit.transform;
					if (this.logger != null)
						this.logger.Log ("Security Guard: " + transform.name + 
							" found intruder: " + intruder.name);
					return;
				}
			}
		}
	}


	IEnumerator handleIntruder() {

		yield return new WaitWhile (() => audioPlayer.isPlaying);
		audioPlayer.PlayOneShot (this.sgAudioEventMgrScript.IntruderFound);
		//audioPlayer.PlayOneShot (HeyYouAudioClip);
		//EventManager.TriggerEvent<IntruderDetectedEvent, Vector3> (this.transform.position);

		agent.isStopped = true;
		transform.LookAt (intruder.position);
		float distance = Vector3.Distance (transform.position, intruder.position);
		// print ("handleIntruder: Intruder " + intruder.name + " is " + distance + "f units away.");

		aiSteer.neverReachWaypoint = true;
		aiSteer.setWaypoint (intruder.transform);
		aiSteer.mecanimInputForwardSpeedCap = 1f;
		aiSteer.mecanimMaxSpeed = 4f;

		if (distance <= this.fightDistance) {
			fight ();
		} else { // chase
			agent.isStopped = false;
		}
	}

	public void fight() {
		//print("We're fighting now.");
		if (!GameObject.Find("M1Avatar").GetComponent<YBotSimpleControlScript>().PlayerDead()) {
			fistCollider.GetComponent<EnemyPunchScript>().Punch();
		}
	}
	
	void EndPunch() {
		fistCollider.transform.GetComponent<EnemyPunchScript>().StopPunch();
	}
	
	void startHit() {
		anim.applyRootMotion = false;
		rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		rigidBody.AddForce (-1 * gameObject.GetComponent<Rigidbody>().transform.forward * 13000);
	}
	
	void canHit() {
		anim.applyRootMotion = true;
		rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ 
		  | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		fistCollider.GetComponent<EnemyPunchScript>().EnemyCanHit();
	}
	
	public bool isCaught() {
		return caught;
	}
	
	public void setDead() {
		isDead = true;
	}


	public GameObject getFist() {
		return fistCollider;
	}

	private void checkForFallenComrades() {
		// After checking forward, rays will be built by
		// going back and forth and adding to the left, then right, etc.
		bool checkLeft = false;
		// The distance between rays on the x and z axis
		float stepSize = 0.0625f;
		// Endpoints of the rays that constitute "sight"
		Vector3 endpoint = new Vector3 (transform.forward.x,
			/*transform.forward.y*/ 0, transform.forward.z);

		for (int hitIdx = 0; hitIdx < 17; hitIdx++) {
			if (checkLeft) {
				endpoint.x += (hitIdx * stepSize);
				endpoint.z -= stepSize;
			} else {
				endpoint.x -= (hitIdx * stepSize);
				// endpoint.z stays same
			}

			checkLeft = !checkLeft;
			// print ("\tendpoint = " + endpoint.ToString("F4"));
			Ray ray = new Ray (intruderCheckStartingPoint.position, endpoint);
			// Debug.DrawRay (ray.origin, ray.direction * this.sightDistance, Color.blue);
			RaycastHit hit; // Variable reading information about the collider hit
			// Cast ray from center of the screen towards where the player is looking
			if (Physics.Raycast (ray, out hit, this.sightDistance)) {
				if ((hit.transform.tag == "SecurityGuard") && (!hit.transform.IsChildOf(this.transform)) 
					&& !this.revivingFallenComrade && 
					// If it doesn't have the HandleIntruder, it's not at the right level
					hit.transform.GetComponent<HandleIntruder> () != null &&
					hit.transform.GetComponent<HandleIntruder> ().isDead) {
					this.revivingFallenComrade = true;
					this.timeFoundFallenComrade = Time.timeSinceLevelLoad;
					
					//NEVER MIND HE IS DEAD
					//audioPlayer.PlayOneShot (HeyBuddyAudioClip);
					//EventManager.TriggerEvent<BodyFoundEvent, Vector3> (this.transform.position);
					// revive your buddy
					//buddy.GetComponent<HealthScript>().revive();
					//buddy.GetComponent<Animator>().enabled = true;
					//buddy.GetComponent<HandleIntruder> ().continueOnRounds ();
					// and then continue on yourself
					//agent.isStopped = false;

					StartCoroutine(handleFoundDeadGuard(hit.transform));
					return;
				}
			}
		}
	}


	public void revive() {
		isDead = false;
		caught = false;
	}


	IEnumerator handleFoundDeadGuard(Transform buddy) {
		print ("Security Guard: " + transform.name +
		" found another guard: " + buddy.name + " dead on the floor.");
		audioPlayer.PlayOneShot (this.sgAudioEventMgrScript.SeeFallenComrade);
		transform.LookAt (buddy);
		// Move to 1 unity away from your buddy
		aiSteer.setWaypoint (buddy);
		aiSteer.neverReachWaypoint = true;
		while (Vector3.Distance (transform.position, buddy.position) > 2f) {
			yield return new WaitForSeconds (2);
		}
		agent.isStopped = true;
		// Play animation and text for talking on walkie talkie
		anim.SetBool ("talk", true);
		yield return new WaitWhile (() => audioPlayer.isPlaying);
		audioPlayer.PlayOneShot (this.sgAudioEventMgrScript.FoundGuardDead);
		yield return new WaitWhile (() => audioPlayer.isPlaying);
		anim.SetBool ("talk", false);
		// Play replies from other guards
		int numRepliesNeeded = (int)Mathf.Round (Mathf.Sqrt (spawnedGuardCount));
		for (int replyIdx = 0; replyIdx < numRepliesNeeded; replyIdx++) {
			int audioIdx = Random.Range (0, this.sgAudioEventMgrScript.Replies.Length);
			audioPlayer.PlayOneShot (this.sgAudioEventMgrScript.Replies [audioIdx]);
		}
		// Send your buddy away for body pickup
		Destroy (buddy.gameObject);
		// Spawn 2 more guards; with a wait in between so they don't spawn on top of each other
		SecurityGuardInfo.spawnSecurityGuard (this.SecurityGuardPrefab);
		yield return new WaitForSeconds (3);
		SecurityGuardInfo.spawnSecurityGuard (this.SecurityGuardPrefab);
		yield return new WaitForSeconds (2);
		aiSteer.neverReachWaypoint = false;
		aiSteer.setWaypoints (this.roundsWaypoints);
		agent.isStopped = false;
	}

}
