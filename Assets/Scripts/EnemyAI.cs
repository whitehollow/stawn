using UnityEngine;

class EnemyAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
    public float Speed;
    public bool EnableShooting = true;
    public float FireRate = 1;
    public Projectile Projectile;
    public Transform ProjectileFireLocation;
    public GameObject DestroyEffect;
    public int PointsToGivePlayer;

    public AudioClip DeadSound;
    public AudioClip ShootSound;

    private PlayerController2D _controller;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private float _canFireIn;
    private AudioSource shoot;

    public void Start()
    {
        _controller = GetComponent<PlayerController2D>();
        _direction = new Vector2(-1, 0);
        _startPosition = transform.position;
        shoot = GetComponent<AudioSource>();
    }

    public void Update()
    {
        _controller.SetHorizontalForce(_direction.x * Speed);

        if ((_direction.x < 0 && _controller.State.isCollidingLeft) || (_direction.x > 0 && _controller.State.isCollidingRight))
        {
            _direction = -_direction;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if ((_canFireIn -= Time.deltaTime) > 0 && EnableShooting == true)
            return;

        var raycast = Physics2D.Raycast(transform.position, _direction, 100, 1 << LayerMask.NameToLayer("Player"));

        if (!raycast)
            return;
        
        if (EnableShooting == true)
        {
            var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
                
            shoot.Play();
            projectile.Initialize(gameObject, _direction, _controller.Velocity);
            
            _canFireIn = FireRate;
    
            if (ShootSound != null)
                AudioSource.PlayClipAtPoint(ShootSound, transform.position);
        }
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if (PointsToGivePlayer != null)
        {
            var projectile = instigator.GetComponent<Projectile>();

            FloatingText.Show(string.Format("+{0}!", PointsToGivePlayer), "PointDiamondText", new FromWorldPointTextPositioner(Camera.main, transform.position, 5f, 50));
        }

        Instantiate(DestroyEffect, transform.position, transform.rotation);
        if (DeadSound != null)
            AudioSource.PlayClipAtPoint(DeadSound, transform.position);

        gameObject.SetActive(false);
    }

    public void OnPlayerRespawnInThisCheckpoint(CheckPoint checkpoin, Player player)
    {
        _direction = new Vector2(-1, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = _startPosition;
        gameObject.SetActive(true);
    }
}