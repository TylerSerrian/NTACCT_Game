using UnityEngine;
using System.Collections;

public class XLSceneManager : MonoBehaviour {
	// **************************************************************
	public  GameObject sLock1; // Lock 1 3D Model
	public  GameObject sLock2;// Lock 2 3D Model

	public  GameObject XTool1; //  X axis tool 1 3D Model
	public  GameObject XTool2;
	public  GameObject YTool1;
	public  GameObject YTool2;

	public  bool ssetup;

	// Use this for initialization
	void Start () {
		ssetup = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Jump")) {
			Debug.Log ("SPACE");
			if (ssetup ){
				sLock1.SetActive(true);
				sLock2.SetActive(false);

				XTool1.SetActive(true);
				YTool1.SetActive(true);

				XTool2.SetActive(false);
				YTool2.SetActive(false);


				ssetup = !ssetup;
			}
			else if (!ssetup) {
				sLock1.SetActive(false);
				sLock2.SetActive(true);

				XTool1.SetActive(false);
				YTool1.SetActive(false);
				
				XTool2.SetActive(true);
				YTool2.SetActive(true);


				ssetup = !ssetup;


			
			
			}
		}



	}
// **************************************************************

}
