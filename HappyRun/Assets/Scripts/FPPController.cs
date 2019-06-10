using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public class FPPController : MonoBehaviour
{
    [Header("Camera")]
    public Camera _camera;

    [Header("Audio Clips")]
    [Tooltip("The audio clip that is played while walking."), SerializeField]
    private AudioClip walkingSound;

    [Tooltip("The audio clip that is played while running."), SerializeField]
    private AudioClip runningSound;

    [Header("Movement Settings")]
    [Tooltip("How fast the player moves while walking and strafing."), SerializeField]
    private float walkingSpeed = 5f;

    [Tooltip("How fast the player moves while running."), SerializeField]
    private float runningSpeed = 9f;

    [Tooltip("Approximately the amount of time it will take for the player to reach maximum running or walking speed."), SerializeField]
    private float movementSmoothness = 0.125f;

    [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
    private float jumpForce = 35f;

    [Tooltip("Amount of force applied to the player when wall jumping."),SerializeField]
    private float wallJumpForce = 90f;

    [Header("Look Settings")]
    [Tooltip("Rotation speed of the fps controller."), SerializeField]
    private float mouseSensitivity = 4f;

    [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
    private float rotationSmoothness = 0.05f;

    [Tooltip("Minimum rotation of the arms and camera on the x axis."),
     SerializeField]
    private float minVerticalAngle = -90f;

    [Tooltip("Maximum rotation of the arms and camera on the axis."),
     SerializeField]
    private float maxVerticalAngle = 90f;

    [Tooltip("The names of the axes and buttons for Unity's Input Manager."), SerializeField]
    private FpsInput input;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private AudioSource _audioSource;
    private SmoothRotation _rotationX;
    private SmoothRotation _rotationY;
    private SmoothVelocity _velocityX;
    private SmoothVelocity _velocityZ;
    private bool _isGrounded;
    private bool isOnWall;
    private bool canWallJump;
    private bool isOnWallPlane;

    private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
    private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _collider = GetComponent<CapsuleCollider>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = walkingSound;
        _audioSource.loop = true;
        _rotationX = new SmoothRotation(RotationXRaw);
        _rotationY = new SmoothRotation(RotationYRaw);
        _velocityX = new SmoothVelocity();
        _velocityZ = new SmoothVelocity();
        Cursor.lockState = CursorLockMode.Locked;
        //ValidateRotationRestriction();

    }

    // Update is called once per frame
    void Update()
    {
        //OnCollisionStay();
        Jump();
    }

    private void FixedUpdate()
    {
        // FixedUpdate is used instead of Update because this code is dealing with physics and smoothing.
        // RotateCameraAndCharacter();
        MoveCharacter();
        RotateCamera();
        _isGrounded = false;
    }

    // private void OnCollisionEnter(Collision collision){
    //     Debug.Log("collision");
    //     if(collision.collider.CompareTag("cube")){
    //         var velocity = gameObject.GetComponent<Rigidbody>().velocity;
    //         Debug.Log(velocity);
    //     }
    // }

    /// Checks if the character is on the ground.
    private void OnCollisionStay(Collision collision)
    {


        var collider = collision.collider;

        if (collider.CompareTag("plane"))
        {
            var bounds = _collider.bounds;
            var extents = bounds.extents;
            var radius = extents.x - 0.01f;
            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
            if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
            for (var i = 0; i < _groundCastResults.Length; i++)
            {
                _groundCastResults[i] = new RaycastHit();
            }

            _isGrounded = true;
        }
        else if (collider.CompareTag("cube"))
        {
            Vector3 FPPPos = gameObject.GetComponent<Transform>().position;
            Vector3 cubePos = collider.GetComponent<Transform>().position;

            Vector3 FPPScale = gameObject.GetComponent<Transform>().localScale;
            float cubeHeight = collider.GetComponent<CapsuleCollider>().height;
            float 

            //Debug.Log("cube collision stay");
            isOnWall = true;
            if (canWallJump && input.Jump)
            {
                Debug.Log("fpp jump");
                WallJump();
                canWallJump = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {


        var collider = collision.collider;

        if (collider.CompareTag("cube"))
        {
            Debug.Log("cube collision out");

            canWallJump = true;
            isOnWall = false;
        }
    }

    private void RotateCamera()
    {
        if (_camera)
        {
            //float yRot = Input.GetAxis("Mouse X") * mouseSensitivity;
            //float xRot = Input.GetAxis("Mouse Y") * mouseSensitivity;
            float yRot = _rotationX.Update(RotationXRaw, rotationSmoothness);
            float xRot = _rotationY.Update(RotationYRaw, rotationSmoothness);
            this.transform.localRotation *= Quaternion.Euler(0f, yRot, 0f);
            _camera.transform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);
        }
    }

    private void MoveCharacter()
    {
        var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
        var worldDirection = transform.TransformDirection(direction);
        var velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed);
        var intersectsWall = CheckCollisionsWithWalls(velocity);
        if (intersectsWall)
        {
            _velocityX.Current = _velocityZ.Current = 0f;
            return;
        }

        var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
        var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
        var rigidbodyVelocity = _rigidbody.velocity;
        var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
        _rigidbody.AddForce(force, ForceMode.VelocityChange);

    }

    //to avoid cross wall
    private bool CheckCollisionsWithWalls(Vector3 velocity)
    {
        if (_isGrounded) return false;
        var bounds = _collider.bounds;
        var radius = _collider.radius;
        var halfHeight = _collider.height * 0.5f - radius * 1.0f;
        var point1 = bounds.center;
        point1.y += halfHeight;
        var point2 = bounds.center;
        point2.y -= halfHeight;
        Physics.CapsuleCastNonAlloc(point1, point2, radius, velocity.normalized, _wallCastResults,
            radius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
        var collides = _wallCastResults.Any(hit => hit.collider != null && hit.collider != _collider);
        if (!collides) return false;
        for (var i = 0; i < _wallCastResults.Length; i++)
        {
            _wallCastResults[i] = new RaycastHit();
        }



        return true;
    }

    private void Jump()
    {
        if (!_isGrounded || !input.Jump) return;
        _isGrounded = false;
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void WallJump()
    {
        if (!isOnWall || !input.Jump) return;
        canWallJump = false;
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// Returns the target rotation of the camera around the y axis with no smoothing.
    private float RotationXRaw
    {
        get { return input.RotateX * mouseSensitivity; }
    }

    /// Returns the target rotation of the camera around the x axis with no smoothing.
    private float RotationYRaw
    {
        get { return input.RotateY * mouseSensitivity; }
    }

    /// A helper for assistance with smoothing the camera rotation.
    private class SmoothRotation
    {
        private float _current;
        private float _currentVelocity;

        public SmoothRotation(float startAngle)
        {
            _current = startAngle;
        }

        /// Returns the smoothed rotation.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            set { _current = value; }
        }
    }

    /// A helper for assistance with smoothing the movement.
    private class SmoothVelocity
    {
        private float _current;
        private float _currentVelocity;

        /// Returns the smoothed velocity.
        public float Update(float target, float smoothTime)
        {
            return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
        }

        public float Current
        {
            set { _current = value; }
        }
    }

    /// Input mappings
    [Serializable]
    private class FpsInput
    {
        [Tooltip("The name of the virtual axis mapped to rotate the camera around the y axis."),
         SerializeField]
        private string rotateX = "Mouse X";

        [Tooltip("The name of the virtual axis mapped to rotate the camera around the x axis."),
         SerializeField]
        private string rotateY = "Mouse Y";

        [Tooltip("The name of the virtual axis mapped to move the character back and forth."),
         SerializeField]
        private string move = "Horizontal";

        [Tooltip("The name of the virtual axis mapped to move the character left and right."),
         SerializeField]
        private string strafe = "Vertical";

        [Tooltip("The name of the virtual button mapped to run."),
         SerializeField]
        private string run = "Fire3";

        [Tooltip("The name of the virtual button mapped to jump."),
         SerializeField]
        private string jump = "Jump";

        /// Returns the value of the virtual axis mapped to rotate the camera around the y axis.
        public float RotateX
        {
            get { return Input.GetAxisRaw(rotateX); }
        }

        /// Returns the value of the virtual axis mapped to rotate the camera around the x axis.        
        public float RotateY
        {
            get { return Input.GetAxisRaw(rotateY); }
        }

        /// Returns the value of the virtual axis mapped to move the character back and forth.        
        public float Move
        {
            get { return Input.GetAxisRaw(move); }
        }

        /// Returns the value of the virtual axis mapped to move the character left and right.         
        public float Strafe
        {
            get { return Input.GetAxisRaw(strafe); }
        }

        /// Returns true while the virtual button mapped to run is held down.          
        public bool Run
        {
            get { return Input.GetButton(run); }
        }

        /// Returns true during the frame the user pressed down the virtual button mapped to jump.          
        public bool Jump
        {
            get { return Input.GetButtonDown(jump); }
        }
    }
}

