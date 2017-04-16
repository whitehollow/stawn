using UnityEngine;
using System.Collections;

public class AutoDestroyObject : MonoBehaviour, IPlayerRespawnListener
{
    public float DestroyDelay = 2f;
    public GameObject DestoryEffect;
    private bool _isTriggered;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTriggered)
            return;
           
        // check if the collision is with the player or not
        if (other.GetComponent<Player>() == null)
            return;
            
        if (DestoryEffect != null)
        {
            Instantiate(DestoryEffect, transform.position, transform.rotation);
        }
        StartCoroutine(WaitMe());
        _isTriggered = true;
    }
    
    public void OnPlayerRespawnInThisCheckpoint(CheckPoint checkpoint, Player player)
    {
        _isTriggered = false;
        // restore the diamons after the player was revived
        gameObject.SetActive(true);
    }
    
    public IEnumerator WaitMe()
    {   
        yield return new WaitForSeconds(DestroyDelay);
        gameObject.SetActive(false);   
    }
    
}
