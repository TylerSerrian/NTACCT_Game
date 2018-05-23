using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class Logger : MonoBehaviour {

	// Code based on: https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-

	string path = "Logs/";
	string filePrefix = "test-log-";
	string extension = ".txt";
	StreamWriter writer;

	// By creating the log in initialization, then other scripts 
	// can call the Logger in their Start methods
	void Awake () {
		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}
		string filename = path + filePrefix + currentTimeAsString() + extension;
		writer = new StreamWriter(filename, true);
		Log("Starting new game.");
	}

	static string currentTimeAsString() {
		return formatDateTime(DateTime.Now);
	}

	static string formatDateTime(DateTime dt) {
		return dt.ToString("yyyy-MM-dd--hh-mm-ss");
	}

	static string formatTime(DateTime dt) {
		return dt.ToString("hh:mm:ss");
	}

	public void Log(string message) {
		writer.WriteLine(currentTimeAsString() + ": " + message);
	}

	void OnDisable() {
		Log ("Total time in game: " + Time.realtimeSinceStartup);
		writer.Close();		
	}
}
