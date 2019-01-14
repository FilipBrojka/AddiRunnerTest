using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddiController : MonoBehaviour
{
    public static AddiController instance;

    public bool UseNuitrack = false;

    [Space(10.0f)]
    public LayerMask GroundMask;
    public Transform GroundCheckTransform;

    [Space(10.0f)]
    public Animator AddiAC;

    [Space(10.0f)]
    public CapsuleCollider2D AddiCollider;
    public Vector2 RunColliderOffset;
    public Vector2 RunColliderSize;
    public Vector2 CrouchColliderOffset;
    public Vector2 CrouchColliderSize;

    [Space(10.0f)]
    public float StartingSpeed;
    public float SpeedIncrement;    
    public float GravityForce;
    public float FastFallGravityForce;
    public float GlidingSpeed;    
    public float JumpForce = 750;

    [Space(10.0f)]
    public float JumpThreshold = 10.0f;
    public float CrouchThreshold = 10.0f;    
    public bool StartingPositionSet = false;

    [Space(10.0f)]
    public List<int> SpeedIncrementDistance;
    public int SpeedMultyplier;

    [Space(10.0f)]
    public float GroundCheckRadius;

    [Space(10.0f)]
    public bool Grounded = false;

    [Space(20.0f)]
    public AudioSource RunAudioSource;
    public AudioSource JumpAndGlideAudioSource;
    public AudioSource CrouchAudioSource;

    [Space(10.0f)]
    public AudioClip JumpAudioClip;
    public AudioClip GlideAudioClip;
    public AudioClip FallingAudioClip;
    public AudioClip CrouchAudioClip;

    private ScoreManager _scoreManager;
    private GameManager _gameManager;

    private Rigidbody2D _rigidbody;    

    private Transform _transform;

    private Vector3 _previousPosition;

    private nuitrack.Skeleton _skeleton;

    private nuitrack.Joint _leftAnkleJoint;
    private nuitrack.Joint _rightAnkleJoint;

    private nuitrack.Joint _leftHipJoint;
    private nuitrack.Joint _rightHipJoint;

    private nuitrack.Joint _leftShoulderJoint;
    private nuitrack.Joint _rightShoulderJoint;

    private nuitrack.Joint _headJoint;

    private Vector3 _leftHipStartingPosition;
    private Vector3 _rightHipStartingPosition;

    private float _currentDistanceCovered;
    private float _multyplierDistanceCovered;
    private float _currentSpeed;
    private float _playerHeight;

    private bool _Jump = false;    
    private bool _glide = false;
    private bool _crouch = false;
    private bool _userDetected = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        AddiAC = GetComponent<Animator>();

        _transform = transform;
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
        _scoreManager = _gameManager.Score;

        Physics2D.gravity = new Vector2(0.0f, GravityForce);

        _currentSpeed = StartingSpeed;

        _previousPosition = _transform.position;
    }

    private void Update()
    {
        CheckIfUserDetected();
        CheckIfGrounded();
        CheckForInput();
    }

    private void FixedUpdate()
    {
        Jump();
        Glide();
        Crouch();
        FallFast();
        Move();
    }

    private void MessureHeight()
    {
        float leftHeight = Vector3.Distance(_leftAnkleJoint.ToVector3(), _headJoint.ToVector3());
        float rightHeight = Vector3.Distance(_rightAnkleJoint.ToVector3(), _headJoint.ToVector3());

        _playerHeight = (leftHeight + rightHeight) / 2000f;
    }

    private void CheckForInput()
    {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Grounded)
        {
            _Jump = true;
        }

        if((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && !Grounded)
        {
            _glide = true;
        }

        if(Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            _glide = false;
        }

        if(Grounded && Input.GetKey(KeyCode.LeftControl))
        {
            _crouch = true;
        }

        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            _crouch = false;
        }

        if (UseNuitrack)
        {
            if (Grounded && (_rightHipJoint.ToVector3().y > _rightHipStartingPosition.y + JumpThreshold * _playerHeight || _leftHipJoint.ToVector3().y > _leftHipStartingPosition.y + JumpThreshold * _playerHeight))
            {
                _Jump = true;
            }

            if (!Grounded && (_rightShoulderJoint.ToQuaternion().eulerAngles.z > 270 && _rightShoulderJoint.ToQuaternion().eulerAngles.z < 360) ||
                (_rightShoulderJoint.ToQuaternion().eulerAngles.z >= 0 && _rightShoulderJoint.ToQuaternion().eulerAngles.z < 60))
            {
                if((_leftShoulderJoint.ToQuaternion().eulerAngles.z > 0 && _leftShoulderJoint.ToQuaternion().eulerAngles.z < 90) ||
                    (_leftShoulderJoint.ToQuaternion().eulerAngles.z <= 360 && _leftShoulderJoint.ToQuaternion().eulerAngles.z > 300))
                {
                    _glide = true;
                }
            }

            if ((_rightShoulderJoint.ToQuaternion().eulerAngles.z <= 270 && _rightShoulderJoint.ToQuaternion().eulerAngles.z > 60)
                || (_leftShoulderJoint.ToQuaternion().eulerAngles.z >= 90 && _leftShoulderJoint.ToQuaternion().eulerAngles.z <= 300))
            {
                _glide = false;
            }

            if (_gameManager.GameState == GameManager.GameStateType.EndGame && _rightShoulderJoint.ToQuaternion().eulerAngles.z >= 300)
            {
                _gameManager.RestartLevel();
            }

            if(_rightHipJoint.ToVector3().y < _rightHipStartingPosition.y - CrouchThreshold * _playerHeight || _leftHipJoint.ToVector3().y < _leftHipStartingPosition.y - CrouchThreshold * _playerHeight)
            {
                _crouch = true;
            }
            else
            {
                _crouch = false;
            }

            //print("LEFT: " + _leftShoulderJoint.ToQuaternion().eulerAngles.z + " , RIGHT: " + _rightShoulderJoint.ToQuaternion().eulerAngles.z);
        }
    }

    private void Move()
    {
        if(_gameManager.GameState == GameManager.GameStateType.EndGame)
        {
            RunAudioSource.Stop();
        }        

        if (_gameManager.GameState == GameManager.GameStateType.Playing)
        {
            if (Grounded)
            {
                AddiAC.SetBool("Falling", false);
                AddiAC.SetBool("Run", true);

                if (!RunAudioSource.isPlaying)
                {
                    RunAudioSource.Play();
                }
            }
            else
            {
                AddiAC.SetBool("Run", false);

                if (RunAudioSource.isPlaying)
                {
                    RunAudioSource.Stop();
                }
            }

            if (!_crouch && _rigidbody.velocity.y >= 0.0f && Physics2D.gravity != new Vector2(0.0f, GravityForce))
            {
                Physics2D.gravity = new Vector2(0.0f, GravityForce);
            }

            _rigidbody.velocity = new Vector2(_currentSpeed * Time.deltaTime, _rigidbody.velocity.y);

            _currentDistanceCovered += _transform.position.x - _previousPosition.x;
            _multyplierDistanceCovered += _transform.position.x - _previousPosition.x;

            if (_currentDistanceCovered >= 1.0f)
            {
                _scoreManager.AddToTotalScore(1);
                _currentDistanceCovered -= 1;
            }

            _previousPosition = _transform.position;
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
            Physics2D.gravity = Vector2.zero;
        }

        if (SpeedMultyplier > 0)
        {
            if (_multyplierDistanceCovered >= SpeedIncrementDistance[SpeedMultyplier] + SpeedIncrementDistance[SpeedMultyplier - 1])
            {
                SpeedMultyplier++;
                IncreaseMovingSpeed(SpeedIncrement);
            }
        }
        else
        {
            if(_multyplierDistanceCovered >= SpeedIncrementDistance[SpeedMultyplier])
            {
                SpeedMultyplier++;
                IncreaseMovingSpeed(SpeedIncrement);
            }
        }
    }

    private void Jump()
    {
        if (_gameManager.GameState == GameManager.GameStateType.Playing)
        {
            if (_Jump)
            {
                AddiAC.SetTrigger("Jump");
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce * Time.deltaTime);
                _Jump = false;

                JumpAndGlideAudioSource.loop = false;
                JumpAndGlideAudioSource.clip = JumpAudioClip;
                JumpAndGlideAudioSource.Play();
            }
        }
    }

    private void Glide()
    {
        if(_glide && !_crouch)
        {
            if (_rigidbody.velocity.y < 0.0f && !Grounded)
            {
                AddiAC.SetBool("Glide", true);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, GlidingSpeed * Time.deltaTime);

                if (JumpAndGlideAudioSource.clip != GlideAudioClip)
                {
                    JumpAndGlideAudioSource.clip = GlideAudioClip;
                    JumpAndGlideAudioSource.Stop();
                }

                if (!JumpAndGlideAudioSource.isPlaying && _gameManager.GameState == GameManager.GameStateType.Playing)
                {
                    JumpAndGlideAudioSource.Play();
                }

            }
            else
            {
                AddiAC.SetBool("Glide", false);

                if (JumpAndGlideAudioSource.clip == GlideAudioClip)
                {
                    JumpAndGlideAudioSource.Stop();
                }
            }
        }
        else
        {
            AddiAC.SetBool("Glide", false);

            if(!Grounded)
            {
                AddiAC.SetBool("Falling", true);

                if(JumpAndGlideAudioSource.clip != FallingAudioClip && _rigidbody.velocity.y < 0.0f)
                {
                    JumpAndGlideAudioSource.Stop();
                    JumpAndGlideAudioSource.clip = FallingAudioClip;
                    JumpAndGlideAudioSource.Play();
                }
            }

            if (JumpAndGlideAudioSource.clip == GlideAudioClip)
            {
                JumpAndGlideAudioSource.Stop();
            }
        }
    }

    private void FallFast()
    {
        if (_crouch)
        {
            if (_rigidbody.velocity.y < 0.0f && !Grounded)
            {
                if(Physics2D.gravity.y != FastFallGravityForce)
                {
                    Physics2D.gravity = new Vector2(0.0f, FastFallGravityForce);
                }
            }
        }
    }

    private void Crouch()
    {
        if (_gameManager.GameState == GameManager.GameStateType.Playing)
        {
            if (_crouch && !_glide && !AddiAC.GetBool("Falling"))
            {
                if (Grounded)
                {
                    if (!AddiAC.GetBool("Crouching"))
                    {
                        AddiAC.SetBool("Crouching", true);

                        CrouchAudioSource.Play();
                        RunAudioSource.pitch = 1.25f;
                    }

                    if (AddiCollider.size == RunColliderSize)
                    {
                        AddiCollider.offset = CrouchColliderOffset;
                        AddiCollider.size = CrouchColliderSize;
                    }
                }
            }
            else
            {
                AddiAC.SetBool("Crouching", false);

                CrouchAudioSource.Stop();
                RunAudioSource.pitch = 1.0f;

                if (AddiCollider.size == CrouchColliderSize)
                {
                    AddiCollider.offset = RunColliderOffset;
                    AddiCollider.size = RunColliderSize;
                }
            }
        }
    }

    private void CheckIfUserDetected()
    {
        if (UseNuitrack)
        {
            if (CurrentUserTracker.CurrentUser != 0)
            {                
                _userDetected = true;
                _skeleton = CurrentUserTracker.CurrentSkeleton;

                _leftAnkleJoint = _skeleton.GetJoint(nuitrack.JointType.LeftAnkle);
                _rightAnkleJoint = _skeleton.GetJoint(nuitrack.JointType.RightAnkle);

                _leftHipJoint = _skeleton.GetJoint(nuitrack.JointType.LeftHip);
                _rightHipJoint = _skeleton.GetJoint(nuitrack.JointType.RightHip);

                _leftShoulderJoint = _skeleton.GetJoint(nuitrack.JointType.LeftShoulder);
                _rightShoulderJoint = _skeleton.GetJoint(nuitrack.JointType.RightShoulder);
                
                _headJoint = _skeleton.GetJoint(nuitrack.JointType.Head);

                if (!StartingPositionSet)
                {
                    _leftHipStartingPosition = _leftHipJoint.ToVector3();
                    _rightHipStartingPosition = _rightHipJoint.ToVector3();

                    MessureHeight();

                    StartingPositionSet = true;
                }
            }
            else
            {
                _userDetected = false;
            }
        }
    }

    private void CheckIfGrounded()
    {
        Grounded = Physics2D.OverlapCircle(GroundCheckTransform.position, GroundCheckRadius, GroundMask);
    }

    public void IncreaseMovingSpeed(float amount)
    {
        _currentSpeed += amount;
    }

    public void StartPlaying()
    {
        _gameManager.GameState = GameManager.GameStateType.Playing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GroundCheckTransform.position, GroundCheckRadius);
    }
}
