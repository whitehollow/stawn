﻿using UnityEngine;

public class PathedFireShot : MonoBehaviour, ITakeDamage
{
    private Transform _destination;
    private float _speed;

    public GameObject DestroyEffect;
    public int PointsToGiveToPlayer;

    bool isColliding = false;

    public void Initialize(Transform destination, float speed)
    {
        _destination = destination;
        _speed = speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "platform")
        {
            isColliding = true;
        }
    }

    public void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);

        var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;

        if (distanceSquared > .01f * .01f && !isColliding)
            return;

        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);

        //Destroy(gameObject);
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if(DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);

        var projectile = instigator.GetComponent<Projectile>();

        if (projectile != null && projectile.Owner.GetComponent<Player>() != null && PointsToGiveToPlayer != 0)
        {
            GameManager.Instance.AddPoints(PointsToGiveToPlayer);
            FloatingText.Show(string.Format("+{0}!", PointsToGiveToPlayer), "PointsText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
        }
    }
}
