using UnityEngine;

class PointEntity : MonoBehaviour, IPlayerRespawnListener
{
    public ParticleSystem Effect;
    public AudioClip PickUpSound;
    
    public int pointsToAdd = 10;
    public Animator Animator;

    private bool _isCollected;

    public void OnTriggerEnter2D(Collider2D other) {

        if (_isCollected)
            return;


        // check if the collision is with the player or not
        if (other.GetComponent<Player>() == null)
            return;

        if (PickUpSound != null)
            AudioSource.PlayClipAtPoint(PickUpSound, transform.position);

        // increment the player points
        GameManager.Instance.AddPoints(pointsToAdd);

        if(Effect != null)
            // create a clone of the effect
            Instantiate(Effect, transform.position, transform.rotation);

        _isCollected = true;

        Animator.SetTrigger("Collect");

        // disable the point(diamond) but don't destroy it

        FloatingText.Show(string.Format("+{0}!", pointsToAdd), "PointsCollect", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 80));
    }

    public void FinishAnimationEvent()
    {
        gameObject.SetActive(false);
    }

    public void OnPlayerRespawnInThisCheckpoint(CheckPoint checkpoint, Player player)
    {
        _isCollected = false;
        // restore the diamons after the player was revived
        gameObject.SetActive(true);
    }
}