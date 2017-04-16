using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour
{

    public float speed = 1.2f;
    public float distance = 10;
    private float direction = 1;
    
    public void FixedUpdate()
    {
    
        transform.Translate(direction * distance * speed * Time.deltaTime, transform.position.y, transform.position.z);
        direction = -1;   
    }
}
