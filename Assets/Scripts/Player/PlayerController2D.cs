using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    private static readonly float SlopeLimitTangant = Mathf.Tan(75f * Mathf.Deg2Rad);
    private const float skinWidth = .2f;
    private float _verticalDistanceBetweenRays, _horizontalDistanceBetweenRays;
    private float _jumpIn;

    
    private const int totalHorizontalRays = 7;
    private const int totalVerticalRays = 4;

    private GameObject lastStandingOn;
    public GameObject standingOn { get; private set; }
    public PlayerControllerParameter2D Parameters { get { return _overrideParameters ?? defaultParameters; } }
    public PlayerControllerState2D State { get; private set; }

    public Vector2 Velocity { get { return _velocity; } }
    public Vector3 PlatformVelocity { get; private set; }
    private Vector2 _velocity;

    private Vector3 activeGlobalPlatformPoint, activeLocalPlatformPoint;
    private Vector3 _raycastTopLeft, _raycastBottomRight, _raycastBottomLeft;
    private Vector3 _localScale;
    
    public LayerMask platformMask;

    public PlayerControllerParameter2D defaultParameters;
    private PlayerControllerParameter2D _overrideParameters;


    public bool CanJump
    { 
        get
        { 
            if (Parameters.jumpRestrictions == PlayerControllerParameter2D.JumpBehavior.canJumpAnywhere)
                return _jumpIn <= 0;

            if (Parameters.jumpRestrictions == PlayerControllerParameter2D.JumpBehavior.canJumpGrounded)
                return State.isGrounded;
            else
                return false;
        }
    }
    public bool HandleCollisions { get; set; }    

    private Transform _transform;
    private BoxCollider2D _boxCollider;

    public void Awake()
    {
        HandleCollisions = true;
        State = new PlayerControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * skinWidth);
        _horizontalDistanceBetweenRays = colliderWidth / (totalVerticalRays - 1);

        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * skinWidth);

        _verticalDistanceBetweenRays = colliderHeight / (totalHorizontalRays - 1);
    }

    public void AddForce(Vector2 force)
    {
        _velocity = force;
    }

    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }

    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }

    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }

    public void Jump()
    {
        AddForce(new Vector2(0, Parameters.jumpMagnitude));
        _jumpIn = Parameters.jumpFrequncy;
    }

    public void LateUpdate()
    {
        Move(Velocity * Time.deltaTime);
        _jumpIn -= Time.deltaTime;
        _velocity.y += Parameters.Gravity * Time.deltaTime;
    }

    private void Move(Vector2 deltaMovement)
    {
        var wasGrounded = State.isCollidingBelow;
        State.reset();

        if (HandleCollisions)
        {
            HandlePlatform();
            CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGrounded)
                HandleVerticalSlope(ref deltaMovement);

            if (Mathf.Abs(deltaMovement.x) > .001f)
                MoveHorizontal(ref deltaMovement);

            MoveVerticaly(ref deltaMovement);

            HorizontalPlacement(ref deltaMovement, true);
            HorizontalPlacement(ref deltaMovement, false);
        }

        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if (State.isMovingUpSlope)
            _velocity.y = 0;

        if (standingOn != null)
        {
            activeGlobalPlatformPoint = transform.position;
            activeLocalPlatformPoint = standingOn.transform.InverseTransformPoint(transform.position);

            if (lastStandingOn != standingOn)
            {
                if (lastStandingOn != null)
                    lastStandingOn.SendMessage("Controller2D", this, SendMessageOptions.DontRequireReceiver);

                standingOn.SendMessage("Controller2DEnter", this, SendMessageOptions.DontRequireReceiver);

                lastStandingOn = standingOn;
            } else if (standingOn != null)
                standingOn.SendMessage("Controller2DStay", this, SendMessageOptions.DontRequireReceiver);
        } else if (lastStandingOn != null)
        {
            lastStandingOn.SendMessage("Controller2DExit", this, SendMessageOptions.DontRequireReceiver);
            lastStandingOn = null;
        }
    }

    private void HandlePlatform()
    {
        if (standingOn != null)
        {
            var newGlobalPlatformPoint = standingOn.transform.TransformPoint(activeLocalPlatformPoint);
            var moveDistance = newGlobalPlatformPoint - activeGlobalPlatformPoint;

            if (moveDistance != Vector3.zero)
                transform.Translate(moveDistance, Space.World);

            PlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / Time.deltaTime;
        } else
            PlatformVelocity = Vector3.zero;

        standingOn = null;
    }

    private void HorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
    {
        var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
        var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        if (isRight)
            rayOrigin.x -= (halfWidth - skinWidth);
        else
            rayOrigin.x += (halfWidth - skinWidth);

        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        var offset = 0f;

        for (var i = 1; i < totalHorizontalRays - 1; i++)
        {
            var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));

            //Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, platformMask);

            if (!raycastHit)
                continue;
            offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
        }

        deltaMovement.x += offset;
    }

    private void CalculateRayOrigins()
    {
        var sizeBoxCollider = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        var centerBoxCollider = new Vector2(_boxCollider.center.x * _localScale.x, _boxCollider.center.y * _localScale.y);

        _raycastTopLeft = _transform.position + new Vector3(centerBoxCollider.x - sizeBoxCollider.x + skinWidth, centerBoxCollider.y + sizeBoxCollider.y - skinWidth);
        _raycastBottomRight = _transform.position + new Vector3(centerBoxCollider.x + sizeBoxCollider.x - skinWidth, centerBoxCollider.y - sizeBoxCollider.y + skinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(centerBoxCollider.x - sizeBoxCollider.x + skinWidth, centerBoxCollider.y - sizeBoxCollider.y + skinWidth);
    }

    private void MoveHorizontal(ref Vector2 deltaMovement)
    {
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        for (var i = 0; i < totalHorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

            //Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, platformMask);

            if (!rayCastHit)
                continue;

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;

            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight)
            {
                deltaMovement.x -= skinWidth;
                State.isCollidingRight = true;
            } else
            {
                deltaMovement.x += skinWidth;
                State.isCollidingLeft = true;
            }
            if (rayDistance < skinWidth + .0001f)
                break;

        }
    }

    private void MoveVerticaly(ref Vector2 deltaMovement)
    {
        var isMovingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
        var rayDirection = isMovingUp ? Vector2.up : -Vector2.up;
        var rayOriginin = isMovingUp ? _raycastTopLeft : _raycastBottomLeft;

        rayOriginin.x += deltaMovement.x;

        var standingDistance = float.MaxValue;

        for (var i = 0; i < totalVerticalRays; i++)
        {
            var rayVector = new Vector2(rayOriginin.x + (i * _horizontalDistanceBetweenRays), rayOriginin.y);

            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var rayHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, platformMask);

            if (!rayHit)
                continue;

            if (!isMovingUp)
            {
                var verticalDistanceToHit = _transform.position.y - rayHit.point.y;

                if (verticalDistanceToHit < standingDistance)
                {
                    standingDistance = verticalDistanceToHit;
                    standingOn = rayHit.collider.gameObject;
                }
            }

            deltaMovement.y = rayHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isMovingUp)
            {
                deltaMovement.y -= skinWidth;
                State.isCollidingAbove = true;
            } else
            {
                deltaMovement.y += skinWidth;
                State.isCollidingBelow = true;
            }

            if (!isMovingUp && deltaMovement.y > .0001f)
                State.isMovingUpSlope = true;

            if (rayDistance < skinWidth + .0001f)
                break;
        }
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
        var center = (_raycastBottomLeft.x + _raycastBottomLeft.x) / 2;
        var direction = -Vector2.up;

        var slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
        var slopeVector = new Vector2(center, _raycastBottomLeft.y);

        var rayHit = Physics2D.Raycast(slopeVector, direction, slopeDistance, platformMask);

        if (!rayHit)
            return;

        var isMovingDownSlope = Mathf.Sign(rayHit.normal.x) == Mathf.Sign(deltaMovement.x);

        if (!isMovingDownSlope)
            return;

        var angle = Vector2.Angle(rayHit.normal, Vector2.up);

        if (Mathf.Abs(angle) < .0001f)
            return;

        State.isMovingDownSlope = true;
        State.slopeAnge = angle;
        deltaMovement.y = rayHit.point.y - slopeVector.y;


    }
    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
    {

        if (Mathf.RoundToInt(angle) == 90)
            return false;

        if (angle > Parameters.SlopLimit)
        {
            deltaMovement.x = 0;
            return false;
        }

        if (deltaMovement.y > .07f)
            return true;

        deltaMovement.x += isGoingRight ? -skinWidth : skinWidth;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Rad2Deg) * deltaMovement.x);
        State.isMovingUpSlope = true;
        State.isCollidingBelow = true;
        return true;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
    }
}
