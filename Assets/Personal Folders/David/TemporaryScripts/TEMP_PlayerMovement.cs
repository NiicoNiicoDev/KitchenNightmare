using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TEMPORARY SCRIPT USED FOR TESTING. DO NOT USE IN MAIN SCENES
public class TEMP_PlayerMovement : MonoBehaviour
{
	//used to control speed of player movement, default of 2 m/s
	[SerializeField] private float speed = 2;

	//used as a position from which to check player gounding
	[SerializeField] private Transform groundChecker;

	//how large of a radius around the groundChecker to check for grounding
	[SerializeField] private float groundDistance = 0.1f;

	//which layer to check for grounding on
	[SerializeField] private LayerMask ground;

	//how high should the player jump. An abirtary value.
	[SerializeField] private float jumpHeight = 2f;

	//reference to the character controller component on the player
	private CharacterController controller;

	//stores the input values to move character
	private Vector3 move;

	//stores the velocity for jumping
	private Vector3 velocity;

	//true or false depending on if the character is grounded
	private bool bIsGrounded;

	void Start()
	{
		//gets a reference to the character controller on the object
		controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		//checks around the transform of the GroundChecker to see if any objects have the ground layer
		//if it detects a ground layer we know the character is on the ground.
		bIsGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);

		//if it is grounded and velocity is negative then set velocity to 0. Stops the character from moving down.
		if (bIsGrounded && velocity.y < 0)
		{
			velocity.y = 0f;
		}

		//Creates a new vector based on the input from the horizontal and vertical axis
		//(Vertical = W&S and Horizontal = A&D)
		move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		//applies the movement from the input to the character controller.
		controller.Move(move * Time.deltaTime * speed);

		//if move is not 0, then change the forward direction to face the direction of movement.
		if (move != Vector3.zero)
		{
			transform.forward = move;
		}

		//if the jump button is pressed and the character is grounded.
		if (Input.GetButtonDown("Jump") && bIsGrounded)
		{
			//apply jump calculation to velocity in the up direction.
			velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
		}

		//adds gravity to the character.
		velocity.y += Physics.gravity.y * Time.deltaTime;

		//applies final movement calculation to the character controller.
		controller.Move(velocity * Time.deltaTime);

	}
}
