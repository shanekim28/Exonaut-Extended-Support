using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSWalker : MonoBehaviour
{
	private float speed = 6f;

	private float jumpSpeed = 8f;

	private float gravity = 20f;

	private Vector3 moveDirection = Vector3.zero;

	private bool grounded;

	private void FixedUpdate()
	{
		if (grounded)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			if (moveDirection == Vector3.zero)
			{
			}
			moveDirection = base.transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"))
			{
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		CharacterController characterController = (CharacterController)GetComponent("CharacterController");
		CollisionFlags collisionFlags = characterController.Move(moveDirection * Time.deltaTime);
		grounded = ((collisionFlags & CollisionFlags.Below) != 0);
	}
}
