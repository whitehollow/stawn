using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour
{
    private AudioSource click;
    
    public void Start()
    {
        click = GetComponent<AudioSource>();
    }
    public void OnMouseDown()
    {
        click.Play();
        
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Application.Quit();
    }
}
