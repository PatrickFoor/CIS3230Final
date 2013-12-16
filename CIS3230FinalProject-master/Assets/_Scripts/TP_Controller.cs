using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour {

	public static CharacterController CharacterController;
	public static TP_Controller Instance;

	// Use this for initialization
	void Awake () {
		CharacterController = (CharacterController) GetComponent("CharacterController");
		Instance = this;

        TP_Camera.UseExistingOrCreateNewMainCamera();

        SetupExtras();
	}

    void SetupExtras()
    {
        InventorySystem.UseExistingOrCreateInventory();
        Pause.UseExistingOrCreatePauseGUI();
    }
	
	// Update is called once per frame
	void Update () {
		if (Camera.main == null) {
			return;
		}
		
		GetLocomotionInput();
		HandleActionInput();

		TP_Motor.Instance.UpdateMotor();
	}

	//get input from keyboard/gamepad
	void GetLocomotionInput() {
		//deadZone is used to compensate for joysticks on gamepads
		var deadZone = 0.1f; 
		
		//Save vertical velocity first
		TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.MoveVector.y;
		//reset MoveVector in TP_Motor
		TP_Motor.Instance.MoveVector = Vector3.zero;

		//Check values in vertical and horizontal axis
		if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone) {
			//NOTE: Add to MoveVector ***NOT ASSIGN***
			TP_Motor.Instance.MoveVector += new Vector3(0,0, Input.GetAxis("Vertical"));
		}

		if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone) {
			//NOTE: Add to MoveVector ***NOT ASSIGN***
			TP_Motor.Instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"),0, 0);
		}

        //determine move direction (used to determine speed in TP_Motor)
        TP_Animator.Instance.DetermineCurrentMoveDirection();
	}
	
	void HandleActionInput() {
		if (Input.GetButton("Jump")) {
			Jump();	
		}
	}
	
	void Jump() {
		TP_Motor.Instance.Jump();	
	}
}
