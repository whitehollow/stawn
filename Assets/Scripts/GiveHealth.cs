using UnityEngine;

public class GiveHealth : MonoBehaviour, IPlayerRespawnListener
{

    public GameObject Effect;
    public int HealthToGive;
    private AudioSource drink;
    
    
    public void Start()
    {
        drink = GetComponent<AudioSource>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
    
        drink.Play();
        player.GiveHealth(HealthToGive, gameObject);

        Instantiate(Effect, transform.position, transform.rotation);
        
        gameObject.SetActive(false);

    }
    
    public void OnPlayerRespawnInThisCheckpoint(CheckPoint checkpoin, Player player)
    {
        gameObject.SetActive(true);
    }
}
