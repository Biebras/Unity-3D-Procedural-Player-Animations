using UnityEngine;

/// <summary>
/// This code was made by marmitoTH.
/// The code can be accessed: https://github.com/marmitoTH/Unity-Kinematic-Body
/// </summary>

[RequireComponent(typeof(KinematicBody))]
public class KinematicPlayer : MonoBehaviour
{
	public float speed = 12.0f;
	public float jumpSpeed = 15.0f;
	public float gravity = 35.0f;
    public float snapForce = 10.0f;

    private KinematicBody kinematicBody;
    private PlayerInput playerInput;
    private Camera _camera;

    private void Start()
    {
        kinematicBody = GetComponent<KinematicBody>();
        playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        //  Always deriv your motion from the kinematic body velocity
        //  otherwise your movement will be incorrect, like when you
        //  jump against a ceiling and don't fall since your motion 
        //  still being applied against its direction.
        Vector3 moveDirection = kinematicBody.velocity;

        if (kinematicBody.isGrounded)
        {
            var input = playerInput.GetMovementInput();
            var camForward = _camera.transform.forward;
            var camRight = _camera.transform.right;

            var desiredMoveDirection = camRight * input.x + camForward * input.z;
            moveDirection = desiredMoveDirection * speed;

            //  If te kinematic body is grounded you should always apply velocity downward
            //  to keep the isGrounded status as true and avoid the "bunny hop" effect when
            //  walking down steep ground surfaces.
            moveDirection.y = -snapForce;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        // Call the Move method from Kinematic Body with the desired motion.
        kinematicBody.Move(moveDirection);
    }
}
