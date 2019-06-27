/* File Info 
 * Author:      yebin 
 * CreateTime:  2019/6/10 下午3:49:30 
 * LastEditor:  xiaoming
 * ModifyTime:  2019/6/11 上午11:17:24
 * Description: 
*/
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

    [Tooltip("检测是否在地面的灵敏度，即投影的胶囊体的高度。值越大灵敏度越大"), SerializeField]
    [Range(0.2f, 1f)]
    private float checkGroundSensitivity = 0.7f;

    [Header("Jump")]
    [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
    private float jumpForce = 35f;

    [Header("Wall Jump")]
    [Tooltip("Amount of force applied to the player when wall jumping."), SerializeField]
    private float wallJumpUpForce = 90f;

    [Tooltip("Amount of force applied to the player when wall jumping."), SerializeField]
    private float wallJumpOutForce = 90f;

    [Header("Glide")]
    [Tooltip("滑翔时向上的加速度"), SerializeField]
    private float glideAcceleration = 5f;

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

    [Header("AccelerationBlock")]
    [Tooltip("加速块法线方向上提供的动力大小"), SerializeField]
    private float AccelerationUpForce = 60f;

    [Tooltip("加速块在玩家运动方向提供的动力大小"), SerializeField]
    private float AccelerationForwardForce = 40f;

    [Tooltip("The names of the axes and buttons for Unity's Input Manager."), SerializeField]
    private FpsInput input;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    ///private BoxCollider _collider;
    private AudioSource _audioSource;
    private SmoothRotation _rotationX;
    private SmoothRotation _rotationY;
    private SmoothVelocity _velocityX;
    private SmoothVelocity _velocityZ;

    private bool _isGround;//是否在地面
    private bool canWallJump;//能否蹭墙
    private Vector3 CollisionDir;//当前碰撞物体的法向量
    private Vector3 CollisionDir2;//上一次碰撞物体的法向量
    private Collider collider1;//两个碰撞体来检测蹭墙的交替
    private Collider collider2;

    //CheckIfOnGround使用
    private CapsuleCollider capsuleCollider;
    private Vector3 pointBottom, pointTop;
    private float c_radius;

    private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
    private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];

    // Start is called before the first frame update
    void Start()
    {
        //初始化FPP的rigidbody属性
        _rigidbody = GetComponent<Rigidbody>();
        //_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        _collider = GetComponent<CapsuleCollider>();

        //初始化FPP的声音属性
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = walkingSound;
        _audioSource.loop = true;

        //初始化运动参数
        _rotationX = new SmoothRotation(RotationXRaw);
        _rotationY = new SmoothRotation(RotationYRaw);
        _velocityX = new SmoothVelocity();
        _velocityZ = new SmoothVelocity();
        Cursor.lockState = CursorLockMode.Locked;
        //ValidateRotationRestriction();

        //限定角度
        maxVerticalAngle = maxVerticalAngle > 90 ? 90 : (maxVerticalAngle < 0 ? 0 : maxVerticalAngle);
        minVerticalAngle = minVerticalAngle < -90 ? -90 : (minVerticalAngle > 0 ? 0 : minVerticalAngle);

        //CheckIfOnGround使用
        c_radius = _collider.radius / 3;

    }

    private void FixedUpdate()
    {
        RotateCamera();
        CheckIfOnGround();
    }

    /// Checks if the character is on the ground or the wall
    private void OnCollisionEnter(Collision collision)
    {
        collider1 = collision.collider;
        ContactPoint con = collision.contacts[0];

        //碰撞点法向量小于0.65,且更新collider才能蹭墙跳
        if (collider1 != collider2)
        {
            collider2 = collider1;
            CollisionDir2 = CollisionDir = con.normal;
            if (CollisionDir.y < 0.65)
            {
                canWallJump = true;
            }
        }
        else //同一个碰撞体
        {
            if (CollisionDir2.y < 0.65 && con.normal.y < 0.65)
            {
                //两次碰撞点间水平点乘小于0
                float dd = Vector3.Dot(CollisionDir2, con.normal);
                //if(dd>0) Debug.Log(">>>");
                //else Debug.Log("<<<");
                if (Vector3.Dot(CollisionDir2, con.normal) < 0)
                    canWallJump = true;
            }
            CollisionDir2 = con.normal;
        }
        /* 保留的另一种加速块写法
        if(collider.gameObject.tag == "AccelerationBlock")
        {
            Vector3 m_volocity = new Vector3(_rigidbody.velocity.x, 0,_rigidbody.velocity.z);
            _rigidbody.AddForce(AccelerationForwardForce * m_volocity + AccelerationUpForce * CollisionDir, ForceMode.Impulse);
        }*/
    }
    private void OnCollisionStay(Collision collision)
    {
        Collider collider = collision.collider;
        ContactPoint con = collision.contacts[0];
        CollisionDir = con.normal;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CollisionDir.y);
        //if (canWallJump) Debug.Log("canWallJump");
        //else Debug.Log("XXXX");
        //if (_isGround) Debug.Log("Ground");
        //else Debug.Log("Air");
        if (input.Jump)
        {
            if (canWallJump) //防止一帧两种跳同时发生  !_isGround &&
            {
                WallJump(CollisionDir);
                //CollisionDir.y = 1;
            }
            else if (_isGround) Jump();

        }

        if (!_isGround && CollisionDir.y < 0.4f)//在空中撞墙了
        {
            _velocityX.Current = _velocityZ.Current = 0f;
            return;
        }
        else MoveCharacter();

        if (input.isGlide) Glide();
    }

    private void OnCollisionExit(Collision collision)
    {
        CollisionDir.y = 1;
        canWallJump = false;
    }

    private void Glide()
    {
        if (_isGround || _rigidbody.velocity.y >= 0) return;
        _rigidbody.AddForce(Vector3.up * glideAcceleration, ForceMode.Acceleration);
        // Debug.Log("正在滑翔。");
    }

    void CheckIfOnGround()
    {
        pointBottom = transform.position - transform.up * _collider.height / (2f - checkGroundSensitivity);
        pointTop = transform.position;
        LayerMask ignoreMask = ~(1 << 10);

        var colliders = Physics.OverlapCapsule(pointBottom, pointTop, c_radius, ignoreMask);
        //Debug.DrawLine(pointBottom, pointTop, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + transform.forward * c_radius, Color.yellow);
        if (colliders.Length != 0)
        {
            _isGround = true;
        }
        else
        {
            _isGround = false;
        }
    }

    private void RotateCamera()
    {
        if (_camera)
        {
            Quaternion cameraRotation = _camera.GetComponent<Transform>().rotation;
            float camXAngle = cameraRotation.eulerAngles.x;

            //将camXAngle转换至[-180,180]区间
            if (camXAngle < 180) camXAngle = -camXAngle;
            else camXAngle = 360 - camXAngle;

            float yRot = _rotationX.Update(RotationXRaw, rotationSmoothness);
            float xRot = _rotationY.Update(RotationYRaw, rotationSmoothness);
            this.transform.localRotation *= Quaternion.Euler(0f, yRot, 0f);

            camXAngle += xRot;

            //判断旋转角度
            if (camXAngle < maxVerticalAngle && camXAngle > minVerticalAngle) _camera.transform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);

        }
    }

    private void MoveCharacter()
    {

        var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
        var worldDirection = transform.TransformDirection(direction);
        //if (input.Run==true) Debug.Log("Run!!!!");
        //else Debug.Log("no");
        var velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed);
        var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
        var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
        var rigidbodyVelocity = _rigidbody.velocity;
        var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
        if (!_isGround) force = force / 2;
        //if(force.magnitude>1) Debug.Log("Moving");
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void Jump()
    {
        _isGround = false;
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void WallJump(Vector3 outDirection)
    {
        Debug.Log("蹭墙！");
        //if (CollisionDir.y > 0.5) return; //再次判断，防止大延迟walljump
        canWallJump = false;
        _rigidbody.AddForce(Vector3.up * wallJumpUpForce, ForceMode.Impulse);

        //提供反射冲量
        _rigidbody.AddForce(outDirection * wallJumpOutForce, ForceMode.Impulse);
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

        [Tooltip("The name of the virtual button mapped to glide."), // 定义滑翔按钮，类似于jump
         SerializeField]
        private string glide = "Glide";

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

        // 当玩家按下滑翔按钮时，返回true
        public bool isGlide
        {
            get { return Input.GetButton(glide); }
        }
    }
}

