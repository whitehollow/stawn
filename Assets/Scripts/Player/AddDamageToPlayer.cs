using UnityEngine;

class AddDamageToPlayer : MonoBehaviour
{
    public int DamageToGive = 10;
    private Vector2
        lastPosition,
        velocity;

    public void LateUpdate()
    {
        velocity = (lastPosition - (Vector2)transform.position) / Time.deltaTime;
        lastPosition = transform.position;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();

        if (player == null)
            return;

        player.TakeDamage(DamageToGive, gameObject);


        var controller = player.GetComponent<PlayerController2D>();
        var totalVelocity = controller.Velocity + velocity;
        controller.SetForce(new Vector2(
            -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 5, 6, 15), 
            -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 2, 3, 8)
            ));
    }
}

