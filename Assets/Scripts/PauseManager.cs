using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
    Made by team Not The Killjoys
    April Simmons
    Tyler Serrian
    Jarrett Serrian
*/
public class PauseManager : MonoBehaviour {
	
	Canvas canvas;
	
	void Start()
	{
		canvas = GetComponent<Canvas>();
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			canvas.enabled = !canvas.enabled;
			Pause();
		}
	}
	
	public void Pause()
	{
		Time.timeScale = Time.timeScale == 0 ? 1 : 0;
		if (Time.timeScale == 0) {
			GameObject.Find("ResumeButton").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("ResumeButton"));
		}
		
	}

	public void Resume()
	{
	    canvas.enabled = !canvas.enabled;
        Pause();
	}
	
	public void Quit()
	{
		#if UNITY_EDITOR 
		EditorApplication.isPlaying = false;
		#else 
		Application.Quit();
		#endif
	}
}
