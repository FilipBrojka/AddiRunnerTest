﻿using System.Collections;
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
    public float StartingSpeed;
    public float SpeedIncrement;
    public float JumpForce;
    public float GravityForce;
    public float GlidingSpeed;

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

    private ScoreManager _scoreManager;
    private GameManager _gameManager;

    private Rigidbody2D _rigidbody;    

    private Transform _transform;

    private Vector3 _previousPosition;

    private nuitrack.Skeleton _skeleton;

    private nuitrack.Joint _leftShoulderJoint;
    private nuitrack.Joint _rightShoulderJoint;
    private nuitrack.Joint _leftHipJoint;
    private nuitrack.Joint _rightHipJoint;

    private Vector3 _leftShoulderStartingPosition;
    private Vector3 _rightShoulderStartingPosition;
    private Vector3 _leftHipStartingPosition;
    private Vector3 _rightHipStartingPosition;

    private float _currentDistanceCovered;
    private float _multyplierDistanceCovered;
    private float _currentSpeed;

    private bool _jump = false;
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
        //Move();
    }

    private void CheckForInput()
    {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Grounded)
        {
            _jump = true;
        }

        if((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && !Grounded)
        {
            _glide = true;
        }

        if(Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            _glide = false;
        }

        if (UseNuitrack)
        {
            if (Grounded && (_rightShoulderJoint.ToVector3().y > _rightShoulderStartingPosition.y + JumpThreshold || _leftShoulderJoint.ToVector3().y > _leftShoulderStartingPosition.y + JumpThreshold))
            {
                _jump = true;
            }

            if (!Grounded && _rightShoulderJoint.ToQuaternion().eulerAngles.z > 270 && _rightShoulderJoint.ToQuaternion().eulerAngles.z < 360 &&
                _leftShoulderJoint.ToQuaternion().eulerAngles.z > 0 && _leftShoulderJoint.ToQuaternion().eulerAngles.z < 90)
            {
                _glide = true;
            }

            if (_rightShoulderJoint.ToQuaternion().eulerAngles.z <= 270)
            {
                _glide = false;
            }

            if (_gameManager.GameState == GameManager.GameStateType.EndGame && _rightShoulderJoint.ToQuaternion().eulerAngles.z >= 330)
            {
                _gameManager.RestartLevel();
            }

            if(Grounded && (_rightHipJoint.ToVector3().y < _rightHipStartingPosition.y - CrouchThreshold || _leftHipJoint.ToVector3().y < _leftHipStartingPosition.y - CrouchThreshold))
            {
                _crouch = true;
            }
            else
            {
                _crouch = false;
            }
        }
    }

    private void Move()
    {
        if(Grounded)
        {
            AddiAC.SetBool("Falling", false);
            AddiAC.SetBool("Run", true);
        }
        else
        {
            AddiAC.SetBool("Run", false);
        }

        if (_gameManager.GameState == GameManager.GameStateType.Playing)
        {
            if(Physics2D.gravity != new Vector2(0.0f, GravityForce))
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
        if (_jump)
        {
            AddiAC.SetTrigger("Jump");
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce * Time.deltaTime);
            _jump = false;
        }
    }

    private void Glide()
    {
        if(_glide)
        {
            if (_rigidbody.velocity.y < 0.0f && !Grounded)
            {
                AddiAC.SetBool("Glide", true);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, GlidingSpeed * Time.deltaTime);
            }
            else
            {
                AddiAC.SetBool("Glide", false);
            }
        }
        else
        {
            AddiAC.SetBool("Glide", false);

            if(!Grounded)
            {
                AddiAC.SetBool("Falling", true);
            }
        }
    }

    private void Crouch()
    {
        if(_crouch)
        {
            print("CROUCHED!");
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

                _leftShoulderJoint = _skeleton.GetJoint(nuitrack.JointType.LeftShoulder);
                _rightShoulderJoint = _skeleton.GetJoint(nuitrack.JointType.RightShoulder);
                _leftHipJoint = _skeleton.GetJoint(nuitrack.JointType.LeftHip);
                _rightHipJoint = _skeleton.GetJoint(nuitrack.JointType.RightHip);

                if (!StartingPositionSet)
                {
                    _leftShoulderStartingPosition = _leftShoulderJoint.ToVector3();
                    _rightShoulderStartingPosition = _rightShoulderJoint.ToVector3();
                    _leftHipStartingPosition = _leftHipJoint.ToVector3();
                    _rightHipStartingPosition = _rightHipJoint.ToVector3();
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

    private void SpeedUp(float amount)
    {
        _currentSpeed += amount;
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
