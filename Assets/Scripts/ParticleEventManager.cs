using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleEventManager : MonoBehaviour
{

	public GameObject GrassParticles;
	public GameObject PS1;

	private GameObject Particles;

	private UnityAction<Vector3,string> ParticleEventListener;

	void Awake()
	{
		ParticleEventListener = new UnityAction<Vector3,string>(ParticleEventHandler);
	}


	// Use this for initialization
	void Start()
	{



	}


	void OnEnable()
	{
		EventManager.StartListening<ParticleEffectEvent, Vector3, string>(ParticleEventListener);
	}

	void OnDisable()
	{
		EventManager.StopListening<ParticleEffectEvent, Vector3, string>(ParticleEventListener);
	}



	// Update is called once per frame
	void Update()
	{
		if (Particles != null) {
			Destroy (Particles, (float) 0.3);
		}
	}
		
	void ParticleEventHandler(Vector3 worldPos, string surfaceName)
	{
		if (surfaceName == "grass") {
			Particles = Instantiate (GrassParticles) as GameObject;
			Particles.transform.position = worldPos;

		} else if (surfaceName == "sand") {
			Particles = Instantiate (PS1) as GameObject;
			Particles.transform.position = worldPos;
		}
	}

}