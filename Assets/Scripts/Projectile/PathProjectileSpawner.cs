using UnityEngine;
using System.Collections;

public class PathProjectileSpawner : MonoBehaviour
{

    public Transform Destination;
    public PathedFireShot FireShot;
    public GameObject SpawnEffect;

    public float Speed;
    public float FireRate;
    public AudioSource shootSound;
    
    private float _nextShotInSeconds;

    public void Start()
    {
        _nextShotInSeconds = FireRate;
        shootSound = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if ((_nextShotInSeconds -= Time.deltaTime) > 0)
            return;

        _nextShotInSeconds = FireRate;

        var projectile = (PathedFireShot)Instantiate(FireShot, transform.position, transform.rotation);

        projectile.Initialize(Destination, Speed);
        shootSound.Play();
        if (SpawnEffect != null)
            Instantiate(SpawnEffect, transform.position, transform.rotation);

    }

    public void OnDrawGizmos()
    {
        if (Destination == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, Destination.position);
    }
	
}
