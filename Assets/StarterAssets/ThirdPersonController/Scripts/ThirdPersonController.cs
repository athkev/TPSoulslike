    using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
    */

namespace StarterAssets
{
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDSpeedX;
        private int _animIDSpeedY;
        private int _animIDStrafe;
        private int _animIDMoveInput;
        private int _animIDWallRunning;
        private int _animIDWallLeft;
        private int _animIDWallRunInit;
        private int _animIDWallJump;
        bool ableToMove = true;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private Rigidbody _rb;
        private InputManager _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        // extra jump counter
        public int _maxJumps = 2;
        private int _jumpsLeft;
        public bool _strafe = false;
        private float speedX;
        private float speedY;
        private Vector3 speedVector;

        private bool enableSprint = true;
        private bool enableJump = true;
        private bool enableMovement = true;
        private bool enableRotation = true;

        [Header("Wall Run")]
        public LayerMask wallRunnable;
        private bool isWallRunning = false;
        private Vector3 wallRunDirection;

        bool onLeftWall;
        bool onRightWall;
        bool currentlyOnLeft;
        RaycastHit leftWallHit;
        RaycastHit rightWallHit;
        Vector3 wallNormal;
        public float wallrunInitialJump = 5f;
        float wallVerticalVelocity;


        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _rb = GetComponent<Rigidbody>();
            _input = GetComponent<InputManager>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _jumpsLeft = _maxJumps;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            if (!isWallRunning)
            {
                GroundedCheck();
                JumpAndGravity();
            }
            CheckWallRun();

        }

        private void FixedUpdate()
        {
            if (isWallRunning)
            {
                HandleWallRun();
            }
            else
            {
                Move();
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDSpeedX = Animator.StringToHash("SpeedX");
            _animIDSpeedY = Animator.StringToHash("SpeedY");
            _animIDStrafe = Animator.StringToHash("Strafe");
            _animIDMoveInput = Animator.StringToHash("Move Input");
            _animIDWallRunning = Animator.StringToHash("WallRunning");
            _animIDWallLeft = Animator.StringToHash("WallRunningLeft");
            _animIDWallRunInit = Animator.StringToHash("WallRunInit");
            _animIDWallJump = Animator.StringToHash("WallJump");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }
        private void Move()
        {
            // set target speed based on move speed, sprint speed, and if sprint is pressed
            float targetSpeed = _input.sprint && AbleToSprint() ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error-prone and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            _speed = targetSpeed;
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalizing input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // calculate speed vector
            speedVector = Vector3.Lerp(speedVector, inputDirection.normalized * targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (!enableMovement) speedVector = Vector3.zero;

            _animator.SetBool(_animIDMoveInput, (_input.move != Vector2.zero) && ableToMove);
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                    _mainCamera.transform.eulerAngles.y;
            }
            else
            {
                _targetRotation = transform.eulerAngles.y;
            }

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            Vector3 moveDirection = Vector3.zero;
            speedX = speedVector.x;
            speedY = speedVector.z;
            if (!_strafe)
            {
                if (enableRotation && Grounded) transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                moveDirection = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * speedVector;
            }
            else
            {
                // should replace this line for aim IK lower body strafe
                if (enableRotation) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0), 15 * Time.deltaTime);
                moveDirection = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) * speedVector;
            }
            //switch to rb _controller.Move(moveDirection * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            if (Grounded) _rb.velocity = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);
            else AirMovement(inputDirection);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                _animator.SetFloat(_animIDSpeedX, speedX, .1f, Time.deltaTime);
                _animator.SetFloat(_animIDSpeedY, speedY, .1f, Time.deltaTime);
            }
        }

        private Vector3 airMovement;
        public float airboneAcceleration = 5f;
        private void AirMovement(Vector3 inputVector)
        {
            float x = _rb.velocity.x;
            float z = _rb.velocity.z;
            airMovement = _mainCamera.transform.TransformDirection(inputVector).normalized - _rb.velocity.normalized;

            if (Mathf.Abs(x + airMovement.x) < SprintSpeed || IsOppositeDirection(x, airMovement.x)) x += airMovement.x * Time.deltaTime * airboneAcceleration;
            if (Mathf.Abs(z + airMovement.z) < SprintSpeed || IsOppositeDirection(z, airMovement.z)) z += airMovement.z * Time.deltaTime * airboneAcceleration;
            _rb.velocity = new Vector3(x, _rb.velocity.y, z);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // reset jumps left
                _jumpsLeft = _maxJumps;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -10f;
                }

                // Jump
                if (Input.GetKeyDown(KeyCode.Space) && _jumpTimeoutDelta <= 0.0f && AbleToJump())
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    ResetVerticalSpeed();
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    _rb.AddForce(new Vector3(0, JumpHeight, 0), ForceMode.Impulse);
                    _animator.SetTrigger(_animIDJump);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // airborne jump check
                if (_jumpsLeft > 0 && Input.GetKeyDown(KeyCode.Space) && AbleToJump())
                {
                    ResetVerticalSpeed();
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    _rb.AddForce(new Vector3(0, JumpHeight, 0), ForceMode.Impulse);
                    _animator.SetTrigger(_animIDJump);
                    _jumpsLeft--;
                }


                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void CheckWallRun()
        {
            onLeftWall = Physics.Raycast(transform.position + new Vector3(0, 1, 0), isWallRunning? -transform.right : -_mainCamera.transform.right, out leftWallHit, 0.7f, wallRunnable);
            onRightWall = Physics.Raycast(transform.position + new Vector3(0, 1, 0), isWallRunning ? transform.right : _mainCamera.transform.right, out rightWallHit, 0.7f, wallRunnable);
            if ((onRightWall || onLeftWall) && !isWallRunning && _input.jump)
            {
                if (onRightWall) currentlyOnLeft = false;
                else currentlyOnLeft = true;
                _animator.SetBool(_animIDWallLeft, currentlyOnLeft);
                StartWallRun();
            }
        }
        private void StartWallRun()
        {
            Debug.Log("Start Wall Run");
            _animator.SetTrigger(_animIDWallRunInit);
            _animator.SetBool(_animIDWallRunning, true);


            isWallRunning = true;
            _jumpsLeft = _maxJumps;
            wallNormal = onLeftWall ? leftWallHit.normal : rightWallHit.normal;
            wallRunDirection = Vector3.Cross(wallNormal, Vector3.up);

            if (_rb.velocity.y >= 0f || Grounded)
            {
                wallVerticalVelocity = wallrunInitialJump;
            }
            else
            {
                wallVerticalVelocity = 0;
            }
            ResetVerticalSpeed();

            if (Vector3.Dot(wallRunDirection, transform.forward) < 0)
            {
                wallRunDirection = -wallRunDirection;
            }

            transform.rotation = Quaternion.LookRotation(wallRunDirection, Vector3.up);
            onLeftWall = Physics.Raycast(transform.position + new Vector3(0, 1, 0), isWallRunning ? -transform.right : -_mainCamera.transform.right, out leftWallHit, 0.7f, wallRunnable);
            onRightWall = Physics.Raycast(transform.position + new Vector3(0, 1, 0), isWallRunning ? transform.right : _mainCamera.transform.right, out rightWallHit, 0.7f, wallRunnable);
            float distance = currentlyOnLeft ? leftWallHit.distance : rightWallHit.distance;

            if (distance <= .5f)
            {
                //switch to rb _controller.Move((.5f - Vector3.Distance(transform.position, pos)) * wallNormal.normalized);
                transform.position = transform.position + wallNormal.normalized * (.5f - distance);
                Debug.Log("MOVED POSITION");
            }
        }
        private void ExitWallRun()
        {
            Debug.Log("Exit Wall Run");
            isWallRunning = false;
            _animator.SetBool(_animIDWallRunning, false);
            ResetVerticalSpeed();
        }
        private void HandleWallRun()
        {
            //if (_verticalVelocity > 0) _verticalVelocity += Gravity / 4 * Time.deltaTime;
            //else _verticalVelocity = 0;
            //switch to rb _controller.Move(wallRunDirection * 8 * Time.deltaTime + _verticalVelocity * Vector3.up * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(wallRunDirection, Vector3.up);
            if (wallVerticalVelocity >= 0)
            {
                Debug.Log("velocity over 0 , increasing it");
                wallVerticalVelocity += Gravity /2* Time.deltaTime;
            }
            else wallVerticalVelocity = 0f;
            _rb.velocity = new Vector3(wallRunDirection.x, 0, wallRunDirection.z) * 8 + Vector3.up * wallVerticalVelocity;

            // Check if the player wants to stop wall running
            if (!_input.jump)
            {
                //move towards where camera is facing
                ExitWallRun();
                JumpTowards(_mainCamera.transform.forward);
            }
            else if ((currentlyOnLeft && !onLeftWall) || (!currentlyOnLeft && !onRightWall))
            {
                //give previous momentum
                ExitWallRun();
            }
        }
        private void JumpTowards(Vector3 towards)
        {
            Debug.Log("Jump towards : " + towards);
            _rb.AddForce(towards * JumpHeight, ForceMode.VelocityChange);
            _animator.SetTrigger(_animIDWallJump);
            transform.rotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
        }

        private void OnDrawGizmos()
        {
            // Draw the grounded sphere
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded)
                Gizmos.color = transparentGreen;
            else
                Gizmos.color = transparentRed;

            // Draw the ray for left wall
            if (!onLeftWall) Gizmos.color = Color.red;
            else Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), isWallRunning ? -transform.right : -_mainCamera.transform.right * 0.7f);

            // Draw the ray for right wall
            if (!onRightWall) Gizmos.color = Color.red;
            else Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), isWallRunning ? transform.right : _mainCamera.transform.right * 0.7f);
        }
            
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        public void SetStrafe(bool enable)
        {
            _strafe = enable;
            _animator.SetBool(_animIDStrafe, enable);
            
            //SetSprint(!enable);
            //SetJump(!enable);
        }
        public void SetSprint(bool enable)
        {
            enableSprint = enable;
        }
        public void SetJump(bool enable)
        {
            enableJump = enable;
        }
        public void SetMovement(bool enable)
        {
            enableMovement = enable;
            enableJump = enable;
            ableToMove = enable;
        }
        public void SetAbleToMove()
        {
            if (!_animator.IsInTransition(6)) ableToMove = true;
        }
        public void SetRotation(bool enable)
        {
            enableRotation = enable;
        }
        public float GetInputAngle()
        {
            var inputDirection = _input.move;
            return Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
        }
        public Vector2 GetInput()
        {
            return _input.move;
        }
        private bool AbleToJump()
        {
            return enableJump && !_strafe;
        }
        private bool AbleToSprint()
        {
            return enableSprint && !_strafe;
        }
        public void ResetVerticalSpeed()
        {
            _verticalVelocity = 0;
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        }
        private bool IsOppositeDirection(float a, float b)
        {
            return (Mathf.Sign(a) >= 0 && Mathf.Sign(b) < 0) || (Mathf.Sign(a) < 0 && Mathf.Sign(b) >= 0);
        }
    }
}