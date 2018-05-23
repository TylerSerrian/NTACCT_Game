using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGAudioEventMgrScript : MonoBehaviour {

	[Header("Intruder Clips")]
	[Tooltip("Clip to play when intruder is sighted.")]
	public AudioClip IntruderFound;
	[Tooltip("Clip to play when a security guard finds another security guard on the floor.")]
	public AudioClip SeeFallenComrade;
	[Tooltip("Clip to play when guard loses intruder.")]
	public AudioClip LostIntruder;
	[Tooltip("Clip to play when guard finds another guard dead.")]
	public AudioClip FoundGuardDead;
	public AudioClip[] Replies;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
