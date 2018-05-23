using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class YBotSimpleControlScript : MonoBehaviour
{
    private Animator anim;	
    private Rigidbody rbody;

    private Transform leftFoot;
    private Transform rightFoot;

    public int groundContacts = 0;
   
    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;

    private float forwardSpeedLimit = 1f;

	private bool leftFootStep = false;
	private bool rightFootStep = false;

	private bool jumping = false;
	private bool throwing = false;

	private float jumpForwardSpeedScalar = 1f;
	private float jumpVertSpeed = 5f;
	private int lastForwardSign = 0; // just added defaults to stop warning message on build
	private Vector3 lastVelocity = Vector3.one; // just added defaults to stop warning message on build
	
	private GameObject playerFist;

	public string surfaceType;

	GameObject ball;

	int layerMask;

	float timeOfDeath;
	float deathTimer = 5;
	bool isDead = false;
	
	bool win = false;
	float timeOfWin;
	float winTimer = 5;
	int level = 1;
	
	bool contact = false;
	GameObject item;
	int numKeys = 0;
	int numProjectiles = 3;

    public bool IsGrounded
    {
        get { return groundContacts > 0; }
    }


    void Awake()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

    }


    // Use this for initialization
    void Start()
    {
		if (SceneManager.GetActiveScene().name == "Level2") {
			level = 2;
		}
		layerMask = ~(1 << LayerMask.NameToLayer("Ragdoll"));
		print (layerMask);
		//example of how to get access to certain limbs
        leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

		//prevPos = rbody.position;

		playerFist = GameObject.Find ("M1Avatar/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/playerPunchCollider");
        if (leftFoot == null || rightFoot == null)
            Debug.Log("One of the feet could not be found");

    }





    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
	
        //GetAxisRaw() so we can do filtering here instead of the InputManager
        float h = Input.GetAxisRaw("Horizontal");// setup h variable as our horizontal input axis
        float v = Input.GetAxisRaw("Vertical");	// setup v variables as our vertical input axis


        //enforce circular joystick mapping which should coincide with circular blendtree positions
        Vector2 vec = Vector2.ClampMagnitude(new Vector2(h, v), 1.0f);

        h = vec.x;
        v = vec.y;


        //BEGIN ANALOG ON KEYBOARD DEMO CODE
        /*if (Input.GetKey(KeyCode.Q))
            h = -0.5f;
        else if (Input.GetKey(KeyCode.E))
            h = 0.5f;*/

        if (Input.GetKeyUp(KeyCode.Alpha1))
            forwardSpeedLimit = 0.1f;
        else if (Input.GetKeyUp(KeyCode.Alpha2))
            forwardSpeedLimit = 0.2f;
        else if (Input.GetKeyUp(KeyCode.Alpha3))
            forwardSpeedLimit = 0.3f;
        else if (Input.GetKeyUp(KeyCode.Alpha4))
            forwardSpeedLimit = 0.4f;
        else if (Input.GetKeyUp(KeyCode.Alpha5))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha6))
            forwardSpeedLimit = 0.6f;
        else if (Input.GetKeyUp(KeyCode.Alpha7))
            forwardSpeedLimit = 0.7f;
        else if (Input.GetKeyUp(KeyCode.Alpha8))
            forwardSpeedLimit = 0.8f;
        else if (Input.GetKeyUp(KeyCode.Alpha9))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha0))
            forwardSpeedLimit = 1.0f;
        //END ANALOG ON KEYBOARD DEMO CODE  


        //do some filtering of our input as well as clamp to a speed limit
        filteredForwardInput = Mathf.Clamp(Mathf.Lerp(filteredForwardInput, v, 
                Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);
        
        filteredTurnInput = Mathf.Lerp(filteredTurnInput, h, 
            Time.deltaTime * turnInputFilter);
                                                    
        //finally pass the processed input values to the animator
        anim.SetFloat("velx", filteredTurnInput);	// set our animator's float parameter 'Speed' equal to the vertical input axis				
        anim.SetFloat("vely", filteredForwardInput); // set our animator's float parameter 'Direction' equal to the horizontal input axis		

		bool isFalling = !IsGrounded;

		if (isFalling)
		{
			const float rayOriginOffset = 1f; //origin near bottom of collider, so need a fudge factor up away from there
			const float rayDepth = 1f; //how far down will we look for ground?
			const float totalRayLen = rayOriginOffset + rayDepth;

			Ray ray = new Ray(this.transform.position + Vector3.up * rayOriginOffset, Vector3.down);

			//visualize ray in the editor
			//I'm using DrawLine because Debug.DrawRay() doesn't allow setting ray length past a certain size
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * totalRayLen, Color.green); 

			RaycastHit hit;


			//Cast ray and look for ground. If ground is close, then transition out of falling animation
			if (Physics.Raycast(ray, out hit, totalRayLen, layerMask))
			{
				if (hit.collider.gameObject.CompareTag("grass") || hit.collider.gameObject.CompareTag("sand") || hit.collider.gameObject.CompareTag("Untagged"))
				{
					if (hit.collider.gameObject.CompareTag("grass")) {
						surfaceType = "grass";
					} else if (hit.collider.gameObject.CompareTag("sand")) {
						surfaceType = "sand";
					}

					isFalling = false; //turning falling back off because we are close to the ground

					//draw an X that denotes where ray hit
					const float ZBufFix = 0.01f;
					const float edgeSize = 0.2f;
					Color col = Color.red;

					Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.forward * edgeSize, col);
					Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.left * edgeSize, col);
					Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.right * edgeSize, col);
					Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.back * edgeSize, col);
				}
			}
		}

		if (!jumping) {
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
					if (hit.collider.gameObject.CompareTag ("ground")) {
						surfaceType = "ground";
					} else if (hit.collider.gameObject.CompareTag ("sand")) {
						surfaceType = "sand";
					}
				}
				EventManager.TriggerEvent<footStepEvent, Vector3, string> (leftFoot.transform.position, surfaceType);
				EventManager.TriggerEvent<ParticleEffectEvent, Vector3, string> (leftFoot.transform.position, surfaceType);
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
					if (hit.collider.gameObject.CompareTag ("ground")) {
						surfaceType = "ground";
					} else if (hit.collider.gameObject.CompareTag ("sand")) {
						surfaceType = "sand";
					}
				}
				EventManager.TriggerEvent<footStepEvent, Vector3, string> (rightFoot.transform.position, surfaceType);
				EventManager.TriggerEvent<ParticleEffectEvent, Vector3, string> (rightFoot.transform.position, surfaceType);
				rightFootStep = true;
				leftFootStep = false;
			} else if (anim.GetFloat ("foot") >= -0.1 && anim.GetFloat ("foot") <= 0.1) {
				rightFootStep = false;
				leftFootStep = false;
			}
		}

		if (anim.GetFloat ("ground") > 0.9 && !isFalling) {
			jumping = false;
			rightFootStep = true;
			leftFootStep = true;
		} else if (anim.GetFloat ("ground") < 0) {
			jumping = true;
		}

		//print (anim.velocity);

		anim.SetBool("isFalling", isFalling);

        if (numProjectiles > 0 && !throwing && Input.GetButtonDown("Fire1")) { //normally left-ctrl on keyboard
            anim.SetTrigger("throw");
			throwing = true;
		}
		/*if (Input.GetButtonDown ("Jump") && !jumping && !isFalling) { //normally space on keyboard
			jumping = true;
			lastVelocity = anim.velocity;
			if (v < 0) {
				lastForwardSign = -1;
				jumpForwardSpeedScalar = 3f;
			} else {
				lastForwardSign = 1;
				jumpForwardSpeedScalar = 1f;
			}
			anim.SetTrigger ("jump");
			//jumping = true;
		}*/
		/*if (Input.GetButtonDown ("Fire2")) {
			Ragdoll ();
		}*/
		
		if (Input.GetKeyDown(KeyCode.F) && contact) {
			if (item.tag == "item") {
				print("Key picked up.");
				Destroy(item);
				item = null;
				contact = false;
				numKeys++;
				GameObject.Find("NumKeys").GetComponent<Text>().text = "Keys: " + numKeys + "/3";
			} else if (item.tag == "projectile" && numProjectiles < 3) {
				print("Projectile picked up.");
				Destroy(item);
				item = null;
				contact = false;
				numProjectiles++;
				GameObject.Find("NumProjectiles").GetComponent<Text>().text = "Projectiles: " + numProjectiles + "/3";
			} else if (item.tag == "health" && GetComponent<HealthScript>().getCurrentHealth() < GetComponent<HealthScript>().getMaxHealth()) {
				print("Health picked up.");
				Destroy(item);
				item = null;
				contact = false;
				GetComponent<HealthScript>().heal(25);
			} else if (!win && item.tag == "elevator" && numKeys == 3 && level == 1) {
				GameObject.Find("WinCanvas").GetComponent<Canvas>().enabled = true;
				win = true;
				timeOfWin = Time.timeSinceLevelLoad;
			} else if (item.tag == "tablet") {
				SceneManager.LoadScene("DoorCodeScene");
			}
		}
			
		if (isDead && Time.timeSinceLevelLoad - timeOfDeath >= deathTimer) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			isDead = false;
		}
		
		if (win && Time.timeSinceLevelLoad - timeOfWin >= winTimer) {
			SceneManager.LoadScene("Level2");
			win = false;
			numKeys = 0;
			GetComponent<HealthScript>().heal(100);
			numProjectiles = 3;
			level = 2;
		}

		if (!isFalling) {
			jumping = false;
		}
		//if (!isFalling)
			//jumping = false;
		//prevPos = rbody.position;

		//print (vel);
		//prevPos = transform.position;
    }


    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

		if (collision.transform.gameObject.tag == "grass" || collision.transform.gameObject.tag == "sand")
        {
            ++groundContacts;

            //Debug.Log("Player hit the ground at: " + collision.impulse.magnitude);

            if (collision.impulse.magnitude > 100f)
            {               
				EventManager.TriggerEvent<PlayerLandsEvent, Vector3, string>(collision.contacts[0].point, surfaceType);
				EventManager.TriggerEvent<ParticleEffectEvent, Vector3, string> (collision.contacts[0].point, surfaceType);
            }
        }
						
    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {

		if (collision.transform.gameObject.tag == "grass" || collision.transform.gameObject.tag == "sand")
            --groundContacts;
    }

	void OnTriggerEnter(Collider collision)
    {
		
		if (collision.transform.gameObject.tag == "item" || collision.transform.gameObject.tag == "projectile" || collision.transform.gameObject.tag == "health" || collision.transform.gameObject.tag == "elevator" || collision.transform.gameObject.tag == "tablet")
        {
            contact = true;
			item = collision.transform.gameObject;
		} else if (level == 2 && collision.transform.gameObject.tag == "Door" && numKeys == 3) {
			collision.transform.gameObject.GetComponent<Door>().makeOpenable();
		}
						
    }

    //This is a physics callback
    void OnTriggerExit(Collider collision)
    {

		if (collision.transform.gameObject.tag == "item" || collision.transform.gameObject.tag == "elevator")
        {
            contact = false;
			item = null;
		}
		
    }

    void OnAnimatorMove()
    {
		//rbody.velocity = (anim.deltaPosition / Time.deltaTime);
        if (IsGrounded)
        {
         	//use root motion as is if on the ground		
            this.transform.position = anim.rootPosition;

        }
        else
        {
            //Simple trick to keep model from climbing other rigidbodies that aren't the ground
            this.transform.position = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);
        }

        //use rotational root motion as is
        this.transform.rotation = anim.rootRotation;
        				
    }

	void StartJump() {
		
		/*print (rbody.position);
		print (prevPos);
		vel = (rbody.position - prevPos) / Time.fixedDeltaTime;
		print (vel);*/
	}

	/*void Jump() {
		//rbody.AddForce (0, 5, 0, ForceMode.Impulse);
		rbody.velocity = new Vector3(0, 5, 0);
		//print (rbody.velocity.magnitude);

	}*/

	//Animation callback for jumping animation
	public void Jump() {

		//turn off root motion so that AddForce() will work and the character will preserve momentum
		//we'll turn it back on once the char lands
		anim.applyRootMotion = false;

		//optionally change physics material to one with no friction for better jumping against ledges
		//we'll switch it back once the char lands
		//capsule.material = noFrictionPhysicsMaterial;

		//calculate a jump launch vector:
		//lastForwardSign is used to determine if jump is forward or backward
		//I get this from the sign of the user input Y-axis at the time jump anim initiated
		//but you could also figure it out from lastVelocity and transform.forward
		//jumpForwardSpeedScalar is just a way to tweak the jump speed relative to lastVelocity
		//lastVelocity is char velocity at the time right before the jump animation was initiated
		//jumpVertSpeed is used for vertical component of the jump
		Vector3 launchV = lastForwardSign*jumpForwardSpeedScalar*lastVelocity.magnitude*transform.forward + jumpVertSpeed*Vector3.up;

		//do the actual jump. ForceMode.VelocityChange allows the applied force to be independent of char mass
		rbody.AddForce(launchV, ForceMode.VelocityChange);

		//Also trigger a sound effect and/or particle effect, etc.
		//EventManager.TriggerEvent<JumpEvent, Vector3>(transform.position);

	}

	void ThrowStart() {
		ball = Instantiate (Resources.Load ("SplitMetalBall")) as GameObject;
		//SphereCollider sc = GameObject.Find ("M1Avatar/Body").GetComponent(typeof(SphereCollider)) as SphereCollider;
		ball.transform.parent = GameObject.Find ("M1Avatar/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand").transform;
		//ball.transform.parent = sc.transform;
		//ball.transform.localPosition = new Vector3 ((float) -0.25, (float) -0.35, 0);
		ball.transform.localPosition = new Vector3 (0, (float) 0.1, 0);
		numProjectiles--;
		GameObject.Find("NumProjectiles").GetComponent<Text>().text = "Projectiles: " + numProjectiles + "/3";
		//GameObject ball1 = Instantiate (Resources.Load ("SplitMetalBall 1")) as GameObject;
		//ball1 = ball;
		//print (sc.transform.position);
		/*Rigidbody rb = ball.GetComponent(typeof(Rigidbody)) as Rigidbody;
		rb.useGravity = false;*/
	}

	void Throw() {
		EventManager.TriggerEvent<BallThrownEvent, Vector3>(this.transform.position);
		ball.transform.parent = null;
		Rigidbody rb = ball.GetComponent <Rigidbody>();
		rb.isKinematic = false;
		rb.AddForce (rbody.transform.forward * 1000);
		
		Destroy(ball, 1.5f);
		throwing = false;
	}

	void Ragdoll() {
		anim.enabled = false;
		GameObject hips = GameObject.Find ("M1Avatar/mixamorig:Hips");
		Rigidbody[] rb = hips.transform.GetComponentsInChildren<Rigidbody> ();
		foreach (Rigidbody r in rb) {
			r.velocity = new Vector3 (0, 0, 0);
		}
		//SceneManager.LoadScene ("demo");
		isDead = true;
		timeOfDeath = Time.timeSinceLevelLoad;
	}
	
	public void Die() {
		isDead = true;
		GameObject.Find("DeathMessage").GetComponent<Text>().enabled = true;
		timeOfDeath = Time.timeSinceLevelLoad;
	}
	
	void EndPunch() {
		playerFist.transform.GetComponent<PlayerPunchScript>().StopPunch();
	}
	
	public int getKeys() {
		return numKeys;
	}
	
	public int getProj() {
		return numProjectiles;
	}
	
	public void makeRagdoll() {
		if (!isDead) {
			Ragdoll();
		}
	}
	
	public bool PlayerDead() {
		return isDead;
	}
	
	void startHit() {
		gameObject.GetComponent<Animator>().applyRootMotion = false;
		anim.SetFloat("velx", 0);	// set our animator's float parameter 'Speed' equal to 0				
        anim.SetFloat("vely", 0); // set our animator's float parameter 'Direction' equal to the horizontal input axis		
		gameObject.GetComponent<Rigidbody>().AddForce (-1 * gameObject.GetComponent<Rigidbody>().transform.forward * 13000);
	}
	
	void canHit() {
		gameObject.GetComponent<Animator>().applyRootMotion = true;
		playerFist.transform.GetComponent<PlayerPunchScript>().PlayerCanHit();
	}
	
	public GameObject getFist() {
		return playerFist;
	}
	
	public int getLevel() {
		return level;
	}
}
