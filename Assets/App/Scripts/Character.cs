using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Character : MonoBehaviour
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float SwingSpeed;
    [SerializeField] private float ClimbSpeed;
    [SerializeField] private GameObject PunchParticlePrefab;

    private bool _isInUse = true;
    private bool _isJumpKeyPressed;
    private Rigidbody2D _rigidBody;
    private DistanceJoint2D _distanceJoint2D;
    private Animator _animator;
    private Vector3 _storedVelocity;
    private float _swinging;
    private bool _punching;
    
    [HideInInspector] public bool UsingRope;
    private bool _isPunched;
    private float _punchedTimer;
    public bool FirstUse = true;

    public Action PunchedOtherCharacter;
    
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _distanceJoint2D = GetComponent<DistanceJoint2D>();
        _animator = GetComponent<Animator>();
        _distanceJoint2D.enabled = false;
    }

    public void SwitchToCharacter(bool useThisCharacter)
    {
        if (_isInUse == useThisCharacter)
            return;
        
        if (!useThisCharacter)
        {
            _storedVelocity = _rigidBody.velocity;
            _rigidBody.bodyType = RigidbodyType2D.Static;
            _distanceJoint2D.enabled = false;
            if(!FirstUse)
                _animator.enabled = false;

            FirstUse = false;
        }
        else
        {
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            _rigidBody.velocity = _storedVelocity;
            _animator.enabled = true;
        }
        
        _isInUse = useThisCharacter;
    }
    
    private void Update()
    {
        if (_isPunched)
        {
            _punchedTimer -= Time.deltaTime;
            if (_punchedTimer <= 0)
                _isPunched = false;

            return;
        }
        
        if (_punching && !CollidingWithOtherCharacter())
        {
            _punching = false;
        }
        
        if (_isInUse)
        {
            if (Input.GetKey(KeyCode.G) && CollidingWithOtherCharacter() && !_punching)
            {
                _punching = true;
                PunchedOtherCharacter?.Invoke();
            }
            
            if (UsingRope)
            {
                _distanceJoint2D.enabled = true;
                _swinging = Input.GetAxis("Horizontal") * SwingSpeed;
                var otherPosition = _distanceJoint2D.connectedBody.transform.position;

                var climbVel = Input.GetAxis("Vertical") * Time.deltaTime * ClimbSpeed;

                var ropeLength = Vector2.Distance(transform.position, otherPosition);

                if((climbVel > 0 && !CollidingWithOtherCharacter()) || (climbVel < 0 && !IsGrounded()))
                    ropeLength -= climbVel;
                
                _distanceJoint2D.distance = ropeLength;
                
                return;
            }

            _swinging = 0;
            _distanceJoint2D.enabled = false;
            
            transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * MoveSpeed, 0, 0);

            _isJumpKeyPressed = Input.GetKey(KeyCode.UpArrow);
            
            if (Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded())
                _rigidBody.AddForce(new Vector2(0, 4), ForceMode2D.Impulse);

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                transform.localScale = new Vector3(-0.02f, 0.02f, 0);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                transform.localScale = new Vector3(0.02f, 0.02f, 0);
        }
    }

    private void FixedUpdate()
    {
        if (!Mathf.Approximately(_swinging, 0))
        {
            _rigidBody.AddForce(new Vector2(_swinging, 0), ForceMode2D.Impulse);
        }

        if(!_isJumpKeyPressed && Vector2.Dot(_rigidBody.velocity, Vector2.up) > 0 && !UsingRope && !_isPunched)
            _rigidBody.AddForce(new Vector2(0, -40));
    }

    private bool IsGrounded()
    {
        LayerMask mask = LayerMask.GetMask("Ground", "Character");
        var hits = Physics2D.OverlapCircleAll(transform.position, 0.05f, mask);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && IsWithinAngleDown(hit.transform.position, 55))
                return true;
        }

        return false;
    }

    private bool IsWithinAngleDown(Vector2 objectPosition, float angle)
    {
        var hitAngle =
            90 + Mathf.Atan2(objectPosition.y - transform.position.y, objectPosition.x - transform.position.x) *
            Mathf.Rad2Deg;

        return hitAngle < angle && hitAngle > -angle;
    }
    
    private bool CollidingWithOtherCharacter()
    {
        LayerMask mask = LayerMask.GetMask("Character");
        var hits = Physics2D.OverlapCircleAll(transform.position, 0.2f, mask);
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
                return true;
        }

        return false;
    }

    public void PunchMe(bool left)
    {
        _isPunched = true;
        _punchedTimer = 0.4f;
        _rigidBody.AddForce(new Vector2(left ? -2f : 2f, 3f), ForceMode2D.Impulse);
        Instantiate(PunchParticlePrefab, transform.position, Quaternion.identity, transform);
    }
}