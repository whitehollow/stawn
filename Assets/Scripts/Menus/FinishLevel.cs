using UnityEngine;

class FinishLevel : MonoBehaviour
{
    public string LevelName; 
    private AudioSource levelEnd;
    
    public void Start()
    {
        levelEnd = GetComponent<AudioSource>();
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
    
        if (other.GetComponent<Player>() == null)
            return;

        levelEnd.Play();
                        
        other.GetComponent<Animator>().SetFloat("Speed", 0);
               
        LevelManager.Instance.GoToNextLevel(LevelName);
    }
}

