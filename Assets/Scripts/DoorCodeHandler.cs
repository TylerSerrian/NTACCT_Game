using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class DoorCodeHandler : MonoBehaviour, ISubmitHandler, ICancelHandler {
	
	[Serializable]
	public class StringEvent : UnityEvent <string> {}



	CanvasScaler canvasScaler;

	Text msgDisplay;
	Text entered_1, entered_2, entered_3, entered_4;

	String startMsg = "Enter {0}-digit passcode.  You have {1} tries left.";
	String successMsg = "You are correct!";
	String failureMsgFormat = "Wrong!  You had {0} number{1} in the right place, and {2} right number{3} in the wrong place.";
	String shortEntryMsg = "Must enter {0} digits.";

	static int codeLen = 4;
	static string defaultCode = "1138";
	int[] codeArr = new int[codeLen];
	int currIdx = -1;
	static int[] defaultEnteredCode = {-1, -1, -1, -1};
	int[] enteredCode = defaultEnteredCode;
	bool win = false;
	bool lose = false;
	float timeToReset = 0;

	public string Code = defaultCode;
	public int MaxTries = 5;


	// Use this for initialization
	void Start () {
		canvasScaler = transform.GetComponent<CanvasScaler> ();
		msgDisplay = GameObject.Find ("MsgDisplay").GetComponent<Image> ().GetComponentInChildren<Text> ();
		entered_1 = GameObject.Find ("Entered-1").GetComponent<Image> ().GetComponentInChildren<Text> ();
		entered_2 = GameObject.Find ("Entered-2").GetComponent<Image> ().GetComponentInChildren<Text> ();
		entered_3 = GameObject.Find ("Entered-3").GetComponent<Image> ().GetComponentInChildren<Text> ();
		entered_4 = GameObject.Find ("Entered-4").GetComponent<Image> ().GetComponentInChildren<Text> ();

		canvasScaler.scaleFactor = 0.25f;

		DisplayMessage(String.Format(startMsg, (codeLen.ToString()), 
			(MaxTries == 0 ? "unlimited" : MaxTries.ToString())));

		// transfer public code to int array for internal use
		if (Code.Length != codeLen) {
			print("Code is incorrect length.  Setting to default of " + defaultCode);
			Code = defaultCode;
		}
		char[] codeCharArray = Code.ToCharArray();
		int idx = 0; 
		while (idx < codeLen) {
			if (!Char.IsDigit(codeCharArray[idx])) {
				print("Code had non-number characters. Setting to default of " + defaultCode);
				Code = defaultCode;
				// restart setting codeArray
				idx = -1;
				break;
			} else {
				codeArr[idx] = (int)Char.GetNumericValue(codeCharArray[idx]);
			}
			idx++;	
		}

		reset ();
	}

	public void reset() {
		enteredCode = new int[codeLen];
		for (int idx2 = 0; idx2 < codeLen; idx2++) enteredCode[idx2] = -1;
		this.currIdx = -1;
		DisplayCode (enteredCode);
	}
	
	// Update is called once per frame
	void Update () {
	}

	// FixedUpdate is called on a regular schedule
	void FixedUpdate () {
		if (win && Time.timeSinceLevelLoad - timeToReset >= 6) {
			SceneManager.LoadScene("MainMenu");
		} else if (lose && Time.timeSinceLevelLoad - timeToReset >= 4) {
			SceneManager.LoadScene("Level2");
		}
		
		if (Input.GetKeyDown (KeyCode.P)) {
			canvasScaler.scaleFactor = 1f;
			Destroy (GameObject.Find ("PressPCanvas"));
		} else if (Input.GetKeyDown (KeyCode.Keypad0) || Input.GetKeyDown (KeyCode.Alpha0)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (0);
		} else if (Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown (KeyCode.Alpha1)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (1);
		} else if (Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown (KeyCode.Alpha2)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (2);
		} else if (Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown (KeyCode.Alpha3)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (3);
		} else if (Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown (KeyCode.Alpha4)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (4);
		} else if (Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown (KeyCode.Alpha5)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (5);
		} else if (Input.GetKeyDown (KeyCode.Keypad6) || Input.GetKeyDown (KeyCode.Alpha6)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (6);
		} else if (Input.GetKeyDown (KeyCode.Keypad7) || Input.GetKeyDown (KeyCode.Alpha7)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (7);
		} else if (Input.GetKeyDown (KeyCode.Keypad8) || Input.GetKeyDown (KeyCode.Alpha8)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (8);
		} else if (Input.GetKeyDown (KeyCode.Keypad9) || Input.GetKeyDown (KeyCode.Alpha9)) {
			GetComponent<AudioSource> ().Play ();
			HandleNumPress (9);
		} else if (Input.GetButtonDown ("Submit")) {
			HandleButtonPress ("Submit");
		} else if (Input.GetButtonDown ("Cancel")) {
			HandleButtonPress ("Cancel");
		} 
	}

	void DisplayMessage(string message) {
		msgDisplay.text = message;
	}

	void DisplayCode(int[] dcode) {
		
		entered_1.text = (dcode[0] == -1 ? " " : dcode[0].ToString());
		entered_2.text = (dcode[1] == -1 ? " " : dcode[1].ToString());
		entered_3.text = (dcode[2] == -1 ? " " : dcode[2].ToString());
		entered_4.text = (dcode[3] == -1 ? " " : dcode[3].ToString());
	}

	public void HandleButtonPress (String buttonText) {
		
		GetComponent<AudioSource> ().Play ();

		if (buttonText.Equals("Submit")) {
			StringBuilder sbldr = new StringBuilder();
			bool correct = CheckIfCorrect (sbldr);
			DisplayMessage (sbldr.ToString());
			if (correct) {
				// TODO
				GameObject.Find("WinCanvas").GetComponent<Canvas>().enabled = true;
				win = true;
				timeToReset = Time.timeSinceLevelLoad;
			} else {
				currTry += 1;
				if ((MaxTries > 0) && (currTry > MaxTries)) {
					DisplayMessage(msgDisplay.text + "  You have failed too many times.");
					// TODO
					lose = true;
					timeToReset = Time.timeSinceLevelLoad;
				} else {
					// reset to enter again
					reset();
				}
			}
		} else if (buttonText.Equals("Cancel")) {
			DisplayMessage(String.Format(startMsg, (codeLen.ToString()), 
				(MaxTries == 0 ? "unlimited" : (MaxTries - this.currTry + 1).ToString())));
			reset ();
		} else {
			char[] chars = buttonText.ToCharArray ();
			int num = (int)Char.GetNumericValue (chars [0]);
			HandleNumPress (num);
		}
	}

	int currTry = 1;

	private void HandleNumPress(int num) {
		currIdx += 1;
		if (currIdx < codeLen) {
			enteredCode [currIdx] = num;
		} else {
			enteredCode [0] = enteredCode [1];
			enteredCode [1] = enteredCode [2];
			enteredCode [2] = enteredCode [3];
			enteredCode [3] = num;
			currIdx = (codeLen - 1);
		}
		DisplayCode (enteredCode);

	}

	private bool CheckIfCorrect(StringBuilder returnMsg) {
		if ((currIdx + 1) != codeLen) {
			returnMsg.Append(String.Format (shortEntryMsg, codeLen));
			return false;
		} else {
			int rrcount = 0;
			int rwcount = 0;
			List<int> ecCopy = new List<int> (codeArr);

			for (int idx = 0; idx < codeLen; idx++) {
				if (enteredCode [idx] == codeArr [idx]) {
					rrcount += 1;
				} else if (ecCopy.Remove (enteredCode [idx])) {
					ecCopy.Remove (enteredCode [idx]);
					rwcount += 1;
				}
			}

			if (rrcount == codeLen) {
				returnMsg.Append (successMsg);
				return true;
			} else {
				returnMsg.AppendFormat (failureMsgFormat, rrcount, (rrcount == 1 ? "" : "s"), 
					rwcount, (rwcount == 1 ? "" : "s"));
				return false;
			}
		}
	}


	// for debugging
	string ToString(int[] acode) {
		return (acode [0].ToString() + "-" +
			acode [1].ToString() + "-" +
			acode [2].ToString() + "-" +
			acode [3].ToString());
	}


	public void OnSubmit(BaseEventData eventData) {

	}

	public void OnCancel(BaseEventData eventData) {

	}

}
