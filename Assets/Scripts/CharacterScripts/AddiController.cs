using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddiController : MonoBehaviour
{
    public static AddiController instance;

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

    private float _currentDistanceCovered;
    private float _multyplierDistanceCovered;
    private float _currentSpeed;

    private bool _jump = false;
    private bool _glide = false;

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
        CheckIfGrounded();
        CheckForInput();
    }

    private void FixedUpdate()
    {
        Jump();
        Glide();
        Move();
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
                _scoreManager.AddToDistanceScore(1);
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
