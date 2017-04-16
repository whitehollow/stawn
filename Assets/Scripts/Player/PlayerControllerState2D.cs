using UnityEngine;
using System.Collections;

public class PlayerControllerState2D {
	public bool isCollidingRight {get; set;}
	public bool isCollidingLeft {get; set;}
	public bool isCollidingAbove {get; set;}
	public bool isCollidingBelow {get; set;}
	public bool isMovingDownSlope {get; set;}
	public bool isMovingUpSlope {get; set;}
	public bool isGrounded {get {return isCollidingBelow;} }
    public float slopeAnge { get; set; }

	public bool hasCollisions { get { return isCollidingAbove || isCollidingBelow || isCollidingLeft || isCollidingRight; }}

	public void reset(){
		isMovingUpSlope =
		isMovingDownSlope =
		isCollidingLeft =
		isCollidingRight =
		isCollidingAbove =
		isCollidingBelow = false;

		slopeAnge = 0;
	}	

	public override string ToString ()
	{
		return string.Format("(Controller: r:{0} l:{1} a:{2} b:{3} down-slope:{4} up-slope:{5} angle{6})",
		                     isCollidingRight, isCollidingLeft, isCollidingAbove, isCollidingBelow, isMovingDownSlope, isMovingUpSlope, slopeAnge );
	}
}
