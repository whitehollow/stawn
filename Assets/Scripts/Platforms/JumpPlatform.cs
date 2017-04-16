using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {
    public float JumpMagnitude = 20;

    public void Controller2DEnter(PlayerController2D controller)
    {
        controller.SetVerticalForce(JumpMagnitude);
    }
}
