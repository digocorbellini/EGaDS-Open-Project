using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _fallSpeed = 20;
    [SerializeField] private float _wallSlide = 10;
    [SerializeField] private float _jumpHeight = 4;

    // time after wall jumping until left right movement is given back to player
    [SerializeField] private float _wallJumpTime = 0.5f;

    [Header("Ground and Wall Detection")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundRaycastLength = .5f;
    [SerializeField] private float _groundDetectionWidth = 1f;
    [SerializeField] private float _wallRaycastLength = 0.3f;
    [SerializeField] private float _wallDetectionWidth = 1f;
    [SerializeField] private float _wallDetectionHeight = 0.7f;

    public float MoveSpeed => _moveSpeed * (_abilityManager?.SpeedMultiplier ?? 1);
    public float FallSpeed => _fallSpeed * (_abilityManager?.FallSpeedMultiplier ?? 1);
    public float WallSlideSpeed => _wallSlide * (_abilityManager?.WallSlideMultiplier ?? 1);
    public float JumpHeight => _jumpHeight + (_abilityManager?.JumpHeightAddend ?? 0);
    public int AirJumpCount => _abilityManager?.AirJumpAddend ?? 0;

    public bool IsGrounded
    {
        get
        {
            Vector3 feetPosition = transform.position - new Vector3(0, transform.localScale.y/2, 0);
            Vector3 edgeTransform = new Vector3(_groundDetectionWidth/2, 0, 0);
            return Physics2D.Raycast(feetPosition, Vector2.down, _groundRaycastLength, _groundLayerMask) 
             || Physics2D.Raycast(feetPosition-edgeTransform, Vector2.down, _groundRaycastLength, _groundLayerMask)
             || Physics2D.Raycast(feetPosition+edgeTransform, Vector2.down, _groundRaycastLength, _groundLayerMask);
        }
    }

    private bool IsSideOnWall(Vector2 side)
    {
        Vector3 feetPosition = transform.position - new Vector3(0, transform.localScale.y/2, 0);
        Vector3 edgeTransform = new Vector3(side.x*_wallDetectionWidth/2, 0, 0);
        Vector3 heightTransform = new Vector3(0, _wallDetectionHeight, 0);
        return Physics2D.Raycast(feetPosition+edgeTransform, side, _wallRaycastLength, _groundLayerMask)
            || Physics2D.Raycast(feetPosition+edgeTransform+heightTransform, side, _wallRaycastLength, _groundLayerMask);
    }

    public bool IsLeftOnWall => IsSideOnWall(Vector2.left);
    public bool IsRightOnWall => IsSideOnWall(Vector2.right);
    public bool IsOnWall => IsLeftOnWall || IsRightOnWall;

    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private AbilityManager _abilityManager;

    private float _wallJumpTimer = -1;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _abilityManager = GetComponent<AbilityManager>();
    }
    
    private void Update()
    {
        if (_wallJumpTimer >= 0) _wallJumpTimer -= Time.deltaTime;

        if (_wallJumpTimer < 0)
        {
            var moveDirection = Input.GetAxisRaw("Horizontal");
            _rigidbody.velocity = new Vector2(moveDirection*MoveSpeed, _rigidbody.velocity.y);
        }

        if (Input.GetButtonDown("Jump"))
        {
            var jumpSpeed = Mathf.Sqrt(-2*Physics2D.gravity.y*_rigidbody.gravityScale*JumpHeight);
            if (IsGrounded)
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpSpeed);
            else if (IsOnWall)
            {
                var wallJumpVelocity = IsLeftOnWall ? MoveSpeed : -MoveSpeed;
                _rigidbody.velocity = new Vector2(wallJumpVelocity, jumpSpeed);
                _wallJumpTimer = _wallJumpTime;
            }
        }
    }

    void FixedUpdate()
    {
        // limit fall speed
        float fallSpeed = IsOnWall ? WallSlideSpeed : FallSpeed;
        if (_rigidbody.velocity.y < -fallSpeed)
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallSpeed);
    }

    void OnDrawGizmos()
    {
        // Draw ground detection raycasts
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 feetPosition = transform.position - new Vector3(0, transform.localScale.y/2, 0);
        Vector3 edgeTransform = new Vector3(_groundDetectionWidth/2, 0, 0);
        Gizmos.DrawRay(feetPosition, Vector2.down * _groundRaycastLength); 
        Gizmos.DrawRay(feetPosition-edgeTransform, Vector2.down * _groundRaycastLength);
        Gizmos.DrawRay(feetPosition+edgeTransform, Vector2.down * _groundRaycastLength);

        // Draw wall detection raycasts
        Vector3 sideTransform = new Vector3(_wallDetectionWidth/2, 0, 0);
        Vector3 heightTransform = new Vector3(0, _wallDetectionHeight, 0);
        Gizmos.color = IsLeftOnWall ? Color.green : Color.red;
        Gizmos.DrawRay(feetPosition-sideTransform, Vector2.left * _wallRaycastLength);
        Gizmos.DrawRay(feetPosition-sideTransform+heightTransform, Vector2.left * _wallRaycastLength);
        Gizmos.color = IsRightOnWall ? Color.green : Color.red;
        Gizmos.DrawRay(feetPosition+sideTransform, Vector2.right * _wallRaycastLength);
        Gizmos.DrawRay(feetPosition+sideTransform+heightTransform, Vector2.right * _wallRaycastLength);
    }
}
