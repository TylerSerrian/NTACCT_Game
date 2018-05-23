using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{

    public AudioClip soccerAudio;
	public AudioClip barbellAudio;
	public AudioClip metalAudio;
	public AudioClip canAudio;
	public AudioClip blockAudio;
	public AudioClip obeliskAudio;
	public AudioClip pyramidAudio;
    public AudioClip grassAudio;
	public AudioClip sandAudio;
	public AudioClip groundAudio;
	public AudioClip doorOpeningAudio;
	public AudioClip doorClosingAudio;
	public AudioClip chairAudio;
	public AudioClip playerHitAudio;
	public AudioClip ballThrownAudio;
	public AudioClip enemyHitAudio;
	public AudioClip guardDeathAudio;
	public AudioClip bodyFoundAudio;
	public AudioClip intruderDetectedAudio;

	private UnityAction<Vector3, string> collisionEventListener;

	private UnityAction<Vector3, string> playerLandsEventListener;

	private UnityAction<Vector3, string> footStepEventListener;
	
	private UnityAction<Vector3, string> doorEventListener;
	
	private UnityAction<Vector3> playerHitEventListener;
	
	private UnityAction<Vector3> ballThrownEventListener;
	
	private UnityAction<Vector3> enemyHitEventListener;
	
	private UnityAction<Vector3> guardDeathEventListener;
	
	private UnityAction<Vector3> bodyFoundEventListener;
	
	private UnityAction<Vector3> intruderDetectedEventListener;

    void Awake()
    {

		collisionEventListener = new UnityAction<Vector3, string>(collisionEventHandler);

		playerLandsEventListener = new UnityAction<Vector3, string>(playerLandsEventHandler);

		footStepEventListener = new UnityAction<Vector3, string>(footStepEventHandler);
		
		doorEventListener = new UnityAction<Vector3, string>(doorEventHandler);
		
		playerHitEventListener = new UnityAction<Vector3>(playerHitEventHandler);
		
		ballThrownEventListener = new UnityAction<Vector3>(ballThrownEventHandler);
		
		enemyHitEventListener = new UnityAction<Vector3>(enemyHitEventHandler);
		
		guardDeathEventListener = new UnityAction<Vector3>(guardDeathEventHandler);
		
		bodyFoundEventListener = new UnityAction<Vector3>(bodyFoundEventHandler);
		
		intruderDetectedEventListener = new UnityAction<Vector3>(intruderDetectedEventHandler);
    }


    // Use this for initialization
    void Start()
    {


        			
    }


    void OnEnable()
    {

		EventManager.StartListening<CollisionEvent, Vector3, string>(collisionEventListener);
		EventManager.StartListening<PlayerLandsEvent, Vector3, string>(playerLandsEventListener);
		EventManager.StartListening<footStepEvent, Vector3, string>(footStepEventListener);
		EventManager.StartListening<DoorEvent, Vector3, string>(doorEventListener);
		EventManager.StartListening<PlayerHitEvent, Vector3>(playerHitEventListener);
		EventManager.StartListening<BallThrownEvent, Vector3>(ballThrownEventListener); 
		EventManager.StartListening<EnemyHitEvent, Vector3>(enemyHitEventListener); 
		EventManager.StartListening<GuardDeathEvent, Vector3>(guardDeathEventListener);
		EventManager.StartListening<BodyFoundEvent, Vector3>(bodyFoundEventListener); 
		EventManager.StartListening<IntruderDetectedEvent, Vector3>(intruderDetectedEventListener); 
    }

    void OnDisable()
    {

		EventManager.StopListening<CollisionEvent, Vector3, string>(collisionEventListener);
		EventManager.StopListening<PlayerLandsEvent, Vector3, string>(playerLandsEventListener);
		EventManager.StopListening<footStepEvent, Vector3, string>(footStepEventListener);
		EventManager.StopListening<DoorEvent, Vector3, string>(doorEventListener);
		EventManager.StopListening<PlayerHitEvent, Vector3>(playerHitEventListener);
		EventManager.StopListening<BallThrownEvent, Vector3>(ballThrownEventListener); 
		EventManager.StopListening<EnemyHitEvent, Vector3>(enemyHitEventListener); 
		EventManager.StopListening<GuardDeathEvent, Vector3>(guardDeathEventListener);
		EventManager.StopListening<BodyFoundEvent, Vector3>(bodyFoundEventListener); 
		EventManager.StopListening<IntruderDetectedEvent, Vector3>(intruderDetectedEventListener); 
    }


	
    // Update is called once per frame
    void Update()
    {
    }


 

	void collisionEventHandler(Vector3 worldPos, string surfaceName)
    {
		if (surfaceName == "soccer") {
			AudioSource.PlayClipAtPoint (this.soccerAudio, worldPos);
		} else if (surfaceName == "barbell") {
			AudioSource.PlayClipAtPoint (this.barbellAudio, worldPos);
		} else if (surfaceName == "metal") {
			AudioSource.PlayClipAtPoint (this.metalAudio, worldPos);
		} else if (surfaceName == "can") {
				AudioSource.PlayClipAtPoint (this.canAudio, worldPos);
		} else if (surfaceName == "block") {
			AudioSource.PlayClipAtPoint (this.blockAudio, worldPos);
		} else if (surfaceName == "obelisk") {
			AudioSource.PlayClipAtPoint (this.obeliskAudio, worldPos);
		} else if (surfaceName == "pyramid") {
			AudioSource.PlayClipAtPoint (this.pyramidAudio, worldPos);
		} else if (surfaceName == "chair") {
			AudioSource.PlayClipAtPoint (this.chairAudio, worldPos);
		}
    }

	void playerLandsEventHandler(Vector3 worldPos, string surfaceName)
    {
		//if (surfaceName == "ground") {
			AudioSource.PlayClipAtPoint (this.groundAudio, worldPos);
		//} else if (surfaceName == "sand") {
		//	AudioSource.PlayClipAtPoint (this.sandAudio, worldPos);
		//}
    }

	void footStepEventHandler(Vector3 worldPos, string surfaceName)
	{
		//if (surfaceName == "ground") {
			AudioSource.PlayClipAtPoint (this.groundAudio, worldPos);
		//} else if (surfaceName == "sand") {
		//	AudioSource.PlayClipAtPoint (this.sandAudio, worldPos);
		//}
	}
	
	void doorEventHandler(Vector3 worldPos, string openOrClose)
	{
		if(openOrClose == "open") 
		{
			AudioSource.PlayClipAtPoint (this.doorOpeningAudio, worldPos);
		} else if (openOrClose == "close")
		{
			AudioSource.PlayClipAtPoint (this.doorClosingAudio, worldPos);
		}
	}
	
	void playerHitEventHandler(Vector3 worldPos)
	{
		AudioSource.PlayClipAtPoint(this.playerHitAudio, worldPos);
	}
	
	void ballThrownEventHandler(Vector3 worldPos)
	{
		AudioSource.PlayClipAtPoint(this.ballThrownAudio, worldPos);
	}

	void enemyHitEventHandler(Vector3 worldPos)
	{
		AudioSource.PlayClipAtPoint(this.enemyHitAudio, worldPos);
	}
	
	void guardDeathEventHandler(Vector3 worldPos)
	{
		AudioSource.PlayClipAtPoint(this.guardDeathAudio, worldPos);
	}
	
	void bodyFoundEventHandler(Vector3 worldPos)
	{
		AudioSource.PlayClipAtPoint(this.bodyFoundAudio, worldPos);
	}
	
	void intruderDetectedEventHandler(Vector3 worldPos)
	{
		AudioSource.PlayClipAtPoint(this.intruderDetectedAudio, worldPos);
	}

}
