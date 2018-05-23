using UnityEngine;

public class Detection : MonoBehaviour
{
    // GENERAL SETTINGS
    [Header("General Settings")]
	[Tooltip("Whether the doors open automatically for the player.")]
	public bool AutoOpenDoors = false;
    [Tooltip("How close the player has to be in order to be able to open/close the door.")]
    public float Reach = 4.0F;
    [HideInInspector] public bool InReach;
	[Tooltip("The character the player has to type to open/close the door.")]
    public string Character = "e";

    // UI SETTINGS
    [Header("UI Settings")]
    [Tooltip("The image or text that will be shown whenever the player is in reach of the door.")]
    public GameObject TextPrefab; // A text element to display when the player is in reach of the door
    [HideInInspector] public GameObject TextPrefabInstance; // A copy of the text prefab to prevent data corruption
    [HideInInspector] public bool TextActive;

	// Hiding everything from the original asset to do with displaying the crosshair
    // [Tooltip("The image or text that is shown in the middle of the the screen.")]
    // public GameObject CrosshairPrefab;
    // [HideInInspector] public GameObject CrosshairPrefabInstance; // A copy of the crosshair prefab to prevent data corruption

    // DEBUG SETTINGS
    [Header("Debug Settings")]
    [Tooltip("The color of the debugray that will be shown in the scene view window when playing the game.")]
    public Color DebugRayColor;
    [Tooltip("The opacity of the debugray.")]
    [Range(0.0F, 1.0F)]
    public float Opacity = 1.0F;
	
	private bool doorClosed;


    void Start()
    {
		// This is from the original asset
        // gameObject.name = "Player";
        // gameObject.tag = "Player";

		// Hiding everything from the original asset to do with displaying the crosshair
        // if (CrosshairPrefab == null) Debug.Log("<color=yellow><b>No CrosshairPrefab was found.</b></color>"); // Return an error if no crosshair was specified
		// else 
        // {
        //    CrosshairPrefabInstance = Instantiate(CrosshairPrefab); // Display the crosshair prefab
        //    CrosshairPrefabInstance.transform.SetParent(transform, true); // Make the player the parent object of the crosshair prefab
        //}

        if (TextPrefab == null) Debug.Log("<color=yellow><b>No TextPrefab was found.</b></color>"); // Return an error if no text element was specified

        DebugRayColor.a = Opacity; // Set the alpha value of the DebugRayColor
		doorClosed = true;
    }

	GameObject lastDoor = null;
	public void closeDoor() {
		if (lastDoor != null) {
			Door doorScript = lastDoor.GetComponent<Door> ();
			if (doorScript.RotationPending == false && doorClosed == false)
			{
				StartCoroutine (doorScript.Move ());
				doorClosed = true;
				EventManager.TriggerEvent<DoorEvent, Vector3, string> (this.transform.position, "close");
			}
		}
	}

    void Update()
    {
        // Set origin of ray to center of GameObject and direction of ray to whatever direction the GameObject is facing
		float halfHeight = GetComponent<CapsuleCollider> ().bounds.size.y / 2f;
		Ray ray = new Ray (new Vector3 (transform.position.x, transform.position.y + halfHeight, transform.position.z), transform.forward);

		// All the Game Objects the raycasting hit
		// Cast ray from center of the screen towards where the player is looking
		RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Reach);
		bool found = false;
		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].collider.tag == "Door") {
				found = true;
				RaycastHit hit = hits [i];

				InReach = true;

				// Display the UI element when the player is in reach of the door
				if (TextActive == false && TextPrefab != null && this.transform.gameObject == GameObject.Find("M1Avatar")) {
					TextPrefabInstance = Instantiate (TextPrefab);
					TextActive = true;
					TextPrefabInstance.transform.SetParent (transform, true); // Make the player the parent object of the text element
				}

				// Give the object that was hit the name 'Door'
				GameObject currentDoor = hit.transform.gameObject;

				// Get access to the 'Door' script attached to the object that was hit
				Door doorScript = currentDoor.GetComponent<Door> ();

				if (Input.GetKey (Character) || AutoOpenDoors == true && doorScript.isOpenable()) {
					// Open/close the door by running the 'Move' function found in the 'Door' script
					if (doorScript.RotationPending == false)
					{
						StartCoroutine (doorScript.Move ());
						if(doorClosed == true)
						{
							EventManager.TriggerEvent<DoorEvent, Vector3, string> (this.transform.position, "open");
							doorClosed = false; 
						} else if (doorClosed == false)
						{
							EventManager.TriggerEvent<DoorEvent, Vector3, string> (this.transform.position, "close");
							doorClosed = true;
						}
					}
					lastDoor = currentDoor;
				}
			}
		}
		if (!found) {
                InReach = false;

                // Destroy the UI element when Player is no longer in reach of the door
			if (TextActive == true) {
                    DestroyImmediate(TextPrefabInstance);
                    TextActive = false;
            }
        }

        //Draw the ray as a colored line for debugging purposes.
        //Debug.DrawRay(ray.origin, ray.direction * Reach, DebugRayColor);
    }
}
