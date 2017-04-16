using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class PlayerControllerParameter2D {
	public enum JumpBehavior {
		canJumpGrounded,
		canJumpAnywhere,
		cantJump
	}

	public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

	[Range(0,50)]
	public float SlopLimit = 30;

	public float Gravity = -25f;

	public JumpBehavior jumpRestrictions;

	public float jumpFrequncy = .25f;

	public float jumpMagnitude = 12;
}
