using UnityEngine;

[RequireComponent(typeof(KinematicBody))]
public class KinematicPlayer : MonoBehaviour
{
    [Header("Walk")]
	[SerializeField] private float speed = 12.0f;

    [Header("Falling")]
    [SerializeField] private float gravity = 35.0f;
    [SerializeField] private float snapForce = 10.0f;

    private Vector3 _moveDirection;

    private KinematicBody _kinematicBody;
    private PlayerInput _playerInput;
    private Camera _camera;

    private void Start()
    {
        _kinematicBody = GetComponent<KinematicBody>();
        _playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        //  Always deriv your motion from the kinematic body velocity
        //  otherwise your movement will be incorrect, like when you
        //  jump against a ceiling and don't fall since your motion 
        //  still being applied against its direction.
        _moveDirection = _kinematicBody.Velocity;
        //print(_kinematicBody.Acceleration);
        Walk();

        SnapForce();

        Gravity();

        Move();
    }


    private void Walk()
    {
        var input = _playerInput.GetMovementInput();
        var camForward = _camera.transform.forward;
        var camRight = _camera.transform.right;

        //Walk relative to camera's rotation
        var desiredMoveDirection = camRight * input.x + camForward * input.z;

        _moveDirection = desiredMoveDirection * speed;
        _moveDirection.y = _kinematicBody.Velocity.y;
    }

    private void SnapForce()
    {
        if(_kinematicBody.IsGrounded)
        {
            //  If te kinematic body is grounded you should always apply velocity downward
            //  to keep the isGrounded status as true and avoid the "bunny hop" effect when
            //  walking down steep ground surfaces.
            _moveDirection.y = -snapForce;
        }
    }

    private void Gravity()
    {
        _moveDirection.y -= gravity * Time.deltaTime;
    }


    private void Move()
    {
        _kinematicBody.Move(_moveDirection);
    }    
}
