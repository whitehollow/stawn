using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class RayCastGravity : MonoBehaviour
{
    public LayerMask collisionMask;
    public int numberOfRays = 4;
   
    public GameObject standingOn { get; private set; }
	
    public float speed = 8f;
    public float acceleration = 12f;
    public float gravity = -20f;
    public float skinWidth = 1.4f;

    private float distanceBetweenRays;
    private float rayDistance;
	
    private float currentSpeed;
    private float maxSpeed;
    private Vector2 amountToMove;
    private bool isGrounded;
    
    private Vector2 rayOrigin;
    
    
    private Vector2 _localScale, _velocity;
    private Transform _transform;
    private BoxCollider2D collider;
    private Vector2 colliderSize;
    private Vector2 colliderCenter;
    
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();  
        _transform = transform;
        _localScale = transform.localScale;
        distanceBetweenRays = (collider.size.x * Mathf.Abs(_localScale.x) - (2 * skinWidth)) / (numberOfRays - 1);
        colliderSize = new Vector2(collider.size.x * Mathf.Abs(transform.localScale.x), collider.size.y * Mathf.Abs(transform.localScale.y)) / 2;
        colliderCenter = new Vector2(collider.center.x * transform.localScale.x, collider.center.y * transform.localScale.y);
        _velocity.y = 0;
    }
	
    void LateUpdate()
    {
        _velocity.y += gravity * Time.deltaTime;
    
        Move(_velocity * Time.deltaTime);
        
    }
	
    void calculateOrigins()
    {
        
    }
    
    void Move(Vector2 moveAmount)
    {
        var deltaY = moveAmount.y;
        rayDistance = Mathf.Abs(moveAmount.y) + skinWidth;
        
        
        rayOrigin = _transform.position + new Vector3(colliderCenter.x - colliderSize.x + skinWidth, colliderCenter.y - colliderSize.y + skinWidth);
        rayOrigin.x += moveAmount.x;
        
        float standingDistance = float.MaxValue;
        
        for (int i = 0; i < numberOfRays; i++)
        {
            Debug.Log(distanceBetweenRays);
            var rayVector = new Vector2(rayOrigin.x + (i * distanceBetweenRays), rayOrigin.y);
            
            Debug.DrawRay(rayVector, -Vector2.up * rayDistance, Color.red);
            
            var rayHit = Physics2D.Raycast(rayVector, -Vector2.up, rayDistance, collisionMask);
            
            
            if (rayHit)
            {
                Debug.Log("here");
                moveAmount.y = 0;
                isGrounded = true;
            }
            
            moveAmount.y = rayHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(moveAmount.y);
            
            moveAmount.y += skinWidth;
                       
            if (rayDistance < skinWidth + .0001f)
                break;
            
            
        }
        rayDistance = Mathf.Abs(deltaY);
        transform.Translate(new Vector2(moveAmount.x, deltaY), Space.World);
     
        if (Time.deltaTime > 0)
            _velocity = moveAmount / Time.deltaTime;
        _velocity.y = Mathf.Min(_velocity.y, _velocity.y);
        
    }
    
    float IncrementTowards(float currSpeed, float targetSpeed, float accel)
    {
        if (currSpeed == targetSpeed)
            return currentSpeed;
		
        float dir = Mathf.Sign(targetSpeed - currSpeed);
        currSpeed += accel * Time.deltaTime * dir;
		
        return (dir == Mathf.Sin(targetSpeed - currSpeed)) ? currSpeed : maxSpeed;
    }   
}
