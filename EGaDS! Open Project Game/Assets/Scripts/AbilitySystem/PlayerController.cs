using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Facings { LEFT = -1, RIGHT = 1 };

    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _fallSpeed = 20;
    [SerializeField] private float _wallSlide = 8;
    [SerializeField] private float _jumpHeight = 4;

    // time after wall jumping until left right movement is given back to player
    [SerializeField] private float _wallJumpTime = 0.3f;

    [Header("Ground and Wall Detection")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundRaycastLength = .5f;
    [SerializeField] private float _groundDetectionWidth = .8f;
    [SerializeField] private float _wallRaycastLength = .3f;
    [SerializeField] private float _wallDetectionWidth = .8f;
    [SerializeField] private float _wallDetectionHeight = .7f;


    /// <summary>
    /// The maximum left-right movement speed of the player, in tiles per second
    /// </summary>
    public float MoveSpeed => _moveSpeed * (_abilityManager?.SpeedMultiplier ?? 1);

    /// <summary>
    /// The maximum fall speed of the player, in tiles per second
    /// </summary>
    public float FallSpeed => _fallSpeed * (_abilityManager?.FallSpeedMultiplier ?? 1);

    /// <summary>
    /// The maximum wall slide speed of the player, in tiles per second
    /// </summary>
    public float WallSlideSpeed => _wallSlide * (_abilityManager?.WallSlideMultiplier ?? 1);

    /// <summary>
    /// The jump height of the player in number of tiles
    /// </summary>
    public float JumpHeight => _jumpHeight + .5f + (_abilityManager?.JumpHeightAddend ?? 0);

    /// <summary>
    /// The number of air jumps the player can do
    /// </summary>
    public int AirJumpCount => _abilityManager?.AirJumpAddend ?? 0;


    /// <summary>
    /// The direction the player is facing (either left or right)
    /// </summary>
    public Facings Facing { get; private set; } = Facings.RIGHT;


    /// <summary>
    /// True when the player is on the ground
    /// </summary>
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

    // Performs a raycast on the specified side of the player
    // Use Vector.left or Vector.right as input
    private bool IsSideOnWall(Vector2 side)
    {
        Vector3 feetPosition = transform.position - new Vector3(0, transform.localScale.y/2, 0);
        Vector3 edgeTransform = new Vector3(side.x*_wallDetectionWidth/2, 0, 0);
        Vector3 heightTransform = new Vector3(0, _wallDetectionHeight, 0);
        return Physics2D.Raycast(feetPosition+edgeTransform, side, _wallRaycastLength, _groundLayerMask)
            || Physics2D.Raycast(feetPosition+edgeTransform+heightTransform, side, _wallRaycastLength, _groundLayerMask);
    }

    /// <summary>
    /// True when the player is on a wall to their left side
    /// </summary>
    public bool IsLeftOnWall => IsSideOnWall(Vector2.left);

    /// <summary>
    /// True when the player is on a wall to their right side
    /// </summary>
    public bool IsRightOnWall => IsSideOnWall(Vector2.right);

    /// <summary>
    /// True when the player is on a wall
    /// </summary>
    public bool IsOnWall => IsLeftOnWall || IsRightOnWall;


    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private AbilityManager _abilityManager;

    private float _wallJumpTimer = -1;
    private float _currentAirJumpCount = 0;


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

            if (moveDirection != 0) // update facing
                Facing = moveDirection == 1 ? Facings.RIGHT : Facings.LEFT;
        }

        if (Input.GetButtonDown("Jump"))
        {
            var jumpSpeed = Mathf.Sqrt(-2*Physics2D.gravity.y*_rigidbody.gravityScale*JumpHeight);

            if (IsGrounded) // perform ground jump
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpSpeed);
            
            else if (IsOnWall) // perform wall jump
            {
                Facing = IsLeftOnWall ? Facings.RIGHT : Facings.LEFT;
                var wallJumpVelocity = (int)Facing * MoveSpeed;
                _rigidbody.velocity = new Vector2(wallJumpVelocity, jumpSpeed);
                _wallJumpTimer = _wallJumpTime;
            }
            
            else if (_currentAirJumpCount < AirJumpCount) // perform air jump
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpSpeed);
                _currentAirJumpCount++;
            }
        }

        // reset air jump count
        if (IsGrounded || IsOnWall) _currentAirJumpCount = 0;

        // Update direction player is facing based on Facing value
        transform.localScale = new Vector3((int)Facing, 1, 1);
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
