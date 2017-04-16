using UnityEngine;

class StartScreen : MonoBehaviour
{
    public string Level;
    private AudioSource click;
    
    public void Start()
    {
        click = GetComponent<AudioSource>();
    }
    
    public void OnMouseDown()
    {
        click.Play();
        GameManager.Instance.Reset();
        
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Application.LoadLevel(Level);
    }
}

