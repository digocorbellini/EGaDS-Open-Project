using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _fallSpeed = 10;
    [SerializeField] private float _wallSlide = 5;
    [SerializeField] private float _jumpHeight = 4;

    [Header("Ground and Wall Detection")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundRaycastLength = 1.5f;
    [SerializeField] private float _groundDetectionWidth = 1f;
    [SerializeField] private float _wallRaycastLength = 1f;
    [SerializeField] private float _wallDetectionHeight = 1.5f;

    public float MoveSpeed => _moveSpeed * (_abilityManager?.SpeedMultiplier ?? 1);
    public float FallSpeed => _fallSpeed * (_abilityManager?.FallSpeedMultiplier ?? 1);
    public float WallSlideSpeed => _wallSlide * (_abilityManager?.WallSlideMultiplier ?? 1);
    public float JumpHeight => _jumpHeight + (_abilityManager?.JumpHeightAddend ?? 0);
    public int AirJumpCount => _abilityManager?.AirJumpAddend ?? 0;

    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private AbilityManager _abilityManager;

    private float _moveDirection;
    private bool _isJumping;

    public bool IsGrounded
    {
        get
        {
            Vector3 edgeTransform = new Vector3(_groundDetectionWidth/2, 0, 0);
            return Physics2D.Raycast(transform.position, Vector2.down, _groundRaycastLength, _groundLayerMask) 
             || Physics2D.Raycast(transform.position-edgeTransform, Vector2.down, _groundRaycastLength, _groundLayerMask)
             || Physics2D.Raycast(transform.position+edgeTransform, Vector2.down, _groundRaycastLength, _groundLayerMask);
        }
    }

    public bool IsOnWall
    {
        get
        {
            Vector3 feetPosition = transform.position - new Vector3(0, transform.localScale.y/2, 0);
            Vector3 heightTransform = new Vector3(0, _wallDetectionHeight, 0);
            return Physics2D.Raycast(feetPosition, Vector2.left, _wallRaycastLength, _groundLayerMask)
             || Physics2D.Raycast(feetPosition+heightTransform, Vector2.left, _wallRaycastLength, _groundLayerMask)
             || Physics2D.Raycast(feetPosition, Vector2.right, _wallRaycastLength, _groundLayerMask)
             || Physics2D.Raycast(feetPosition+heightTransform, Vector2.right, _wallRaycastLength, _groundLayerMask);
        }
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _abilityManager = GetComponent<AbilityManager>();
    }
    
    private void Update()
    {
        
        _moveDirection = Input.GetAxisRaw("Horizontal");
        // be wary of setting the player's position directly. Can cause clipping and getting into unwanted areas.
        // better to set the player's position.


        _isJumping = _isJumping || Input.GetButtonDown("Jump") && (IsGrounded || IsOnWall);
    }

    void FixedUpdate()
    {
        // TODO: the *10 here is probably tile size. use a constant somewhere instead

        _rigidbody.velocity = new Vector2(_moveDirection*10, _rigidbody.velocity.y);

        if (_isJumping)
        {
            Debug.Log("Jump");
            var jumpSpeed = Mathf.Sqrt(-2*Physics2D.gravity.y*_rigidbody.gravityScale*JumpHeight);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpSpeed);
            _isJumping = false;
        }

        // limit fall speed
        float fallSpeed = FallSpeed;
        if (_rigidbody.velocity.y < -fallSpeed)
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallSpeed);
    }

    void OnDrawGizmos()
    {
        // Draw ground detection raycasts
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 edgeTransform = new Vector3(_groundDetectionWidth/2, 0, 0);
        Gizmos.DrawRay(transform.position, Vector2.down * _groundRaycastLength); 
        Gizmos.DrawRay(transform.position-edgeTransform, Vector2.down * _groundRaycastLength);
        Gizmos.DrawRay(transform.position+edgeTransform, Vector2.down * _groundRaycastLength);

        // Draw wall detection raycasts
        Gizmos.color = IsOnWall ? Color.green : Color.red;
        Vector3 feetPosition = transform.position - new Vector3(0, transform.localScale.y/2, 0);
        Vector3 heightTransform = new Vector3(0, _wallDetectionHeight, 0);
        Gizmos.DrawRay(feetPosition, Vector2.left * _wallRaycastLength);
        Gizmos.DrawRay(feetPosition+heightTransform, Vector2.left * _wallRaycastLength);
        Gizmos.DrawRay(feetPosition, Vector2.right * _wallRaycastLength);
        Gizmos.DrawRay(feetPosition+heightTransform, Vector2.right * _wallRaycastLength);
    }
}
