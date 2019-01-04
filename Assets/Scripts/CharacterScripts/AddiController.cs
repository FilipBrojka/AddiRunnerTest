using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddiController : MonoBehaviour
{
    public LayerMask GroundMask;
    public Transform GroundCheckTransform;

    [Space(10.0f)]
    public float StartingSpeed;
    public float SpeedIncrement;
    public float JumpForce;
    public float GravityForce;
    public float GlidingSpeed;

    [Space(10.0f)]
    public float GroundCheckRadius;

    [Space(10.0f)]
    public bool Grounded = false;
    public bool IsGliding = false;

    private Rigidbody2D _rigidbody;
    private Animator _addiAC;

    private Transform _transform;

    private float _currentSpeed;

    private bool _jump = false;
    private bool _glide = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _addiAC = GetComponent<Animator>();

        _transform = transform;
    }

    private void Start()
    {
        Physics2D.gravity = new Vector2(0.0f, GravityForce);

        _currentSpeed = StartingSpeed;
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
        if(Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            _jump = true;
        }

        if(Input.GetKey(KeyCode.Space) && !Grounded)
        {
            _glide = true;
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            _glide = false;
        }
    }

    private void Move()
    {
        _rigidbody.velocity = new Vector2(_currentSpeed * Time.deltaTime, _rigidbody.velocity.y);
    }

    private void Jump()
    {
        if (_jump)
        {
            _addiAC.SetTrigger("Jump");
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
                _addiAC.SetBool("Glide", true);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, GlidingSpeed * Time.deltaTime);
            }
            else
            {
                _addiAC.SetBool("Glide", false);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GroundCheckTransform.position, GroundCheckRadius);
    }
}
