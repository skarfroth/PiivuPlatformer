using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Vector3 move;
    private bool groundedPlayer;
    private bool shouldJump = false;
    private readonly float playerSpeed = 4.0f;
    private readonly float jumpHeight = 2.0f;
    private readonly float gravityValue = -9.81f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        MoveLogic();
        JumpLogic();
    }

    private void OnMove(InputValue value)
    {
        Vector3 playerInputValue = value.Get<Vector2>();
        move = playerInputValue;
    }

    private void MoveLogic()
    {
        controller.Move(AdjustVelocityToSlope(playerSpeed * Time.deltaTime * move));
    }

    private void OnJump()
    {
        if (groundedPlayer)
            shouldJump = true;
    }

    private void JumpLogic()
    {
        if (shouldJump && groundedPlayer)
        {
            shouldJump = false;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        Ray ray = new(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            Vector3 adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }
}
