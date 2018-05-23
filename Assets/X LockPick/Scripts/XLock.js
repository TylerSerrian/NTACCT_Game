#pragma strict
// ******************************X Lock by Sabin T. ****** FREE CODE! I grant ***************************************/
//  *******************************you permission to do whatever you want! :)*****************************************

public var XLockPickToolX: GameObject;
public var XLockPickToolY: GameObject;
public var XLOCKSYSTEM:GameObject;
public var Xlight : GameObject;
public var Ylight : GameObject;
public var XUnlockEffect: GameObject;
public var customGuiStyle : GUIStyle;
var ShowInstructions : boolean;
// ******************************************************************************************************************
function Start () {
// ******************************************************************************************************************
}
// ******************************************************************************************************************
function Update () {
var checkX : XLockPickToolXAxis;
checkX = XLockPickToolX.GetComponent("XLockPickToolXAxis");
var xunlocked :boolean;
xunlocked = checkX.XisUNLOCKED;
var checkY : XLockPickToolYAxis;
checkY = XLockPickToolY.GetComponent("XLockPickToolYAxis");
var yunlocked :boolean;
yunlocked = checkY.YisUNLOCKED;
if ( xunlocked == true){
	Debug.Log ("X UNLOCKED");
	Xlight.SetActive (true);
	}
else {
	Xlight.SetActive (false);
	}

if ( yunlocked == true){
	Debug.Log ("Y UNLOCKED");
	Ylight.SetActive (true);
	}
else {
	Ylight.SetActive (false);
	}
if ( xunlocked == true && yunlocked== true){
		Debug.Log ("BOTH X AND Y UNLOCKED!!!");
		Xlight.SetActive (true);
		Xlight.SetActive (true);
		if(Input.GetMouseButtonDown(0)){
		
			if (XUnlockEffect){
				XUnlockEffect.SetActive (true);
				}
			XLOCKSYSTEM.SetActive (false);
			Application.Quit();			
			
		
			}
		}
// ******************************************************************************************************************
}
// ******************************************************************************************************************

function OnGUI(){
GUI.Box(new Rect(((Screen.width / 2)-265),  20,580, 40), "[SPACEBAR] to change lockpicking setup...",customGuiStyle);


if (ShowInstructions){
	
	    		GUI.Box(new Rect(30,  420, 480, 40), "Control the tools by moving the mouse",customGuiStyle);
				GUI.Box(new Rect(80, 461, 380, 40), "UP or DOWN and LEFT or RIGHT ",customGuiStyle);
	   		
}
var checkX : XLockPickToolXAxis;
checkX = XLockPickToolX.GetComponent("XLockPickToolXAxis");
var xunlocked :boolean;
xunlocked = checkX.XisUNLOCKED;
var checkY : XLockPickToolYAxis;
checkY = XLockPickToolY.GetComponent("XLockPickToolYAxis");
var yunlocked :boolean;
yunlocked = checkY.YisUNLOCKED;
if (xunlocked == true && yunlocked== true){
	
	    			GUI.Box(new Rect(100, 420, 320, 40), "Click to unlock!",customGuiStyle);
	    			
	ShowInstructions = false;
	}
if (xunlocked == false || yunlocked== false){
	ShowInstructions = true;
	
	}
	
// ******************************************************************************************************************
}	
// ******************************************************************************************************************