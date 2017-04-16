using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ITakeDamage {

	private bool isFacingRight;

    public Transform ProjectileFireLocation;
    public Projectile Projectile;

    public Transform BombSpawnLocation;
    public GameObject Bomb;

    public bool isDead { get; private set; }

    public float maxSpeed;
    public float accelerationOnGround = 10f;
    public float accelerationOnAir = 5f;

    public int MaxHealth = 100;
    public int Health { get; private set; }
    public GameObject HurtEffect;

   
    public float FireRate;

    public AudioClip HurtSound;
    public AudioClip ShootSound;
    public AudioClip JumpSound;
    public AudioClip DeadSound;
    private AudioSource _backgroundSound;


    private float DropBombRate = 1;
    private float DropBomb;

    private float _canFireIn;   
    private float normalizeHorizontalSpeed;
	private PlayerController2D controller;
    private Animator _anim;



	public void Awake(){
        Health = MaxHealth;
        controller = GetComponent<PlayerController2D>();
		isFacingRight = transform.localScale.x > 0;

        _backgroundSound = GameObject.Find("ThemeSound").GetComponent<AudioSource>();

        _anim = gameObject.GetComponent<Animator>();
	}

	public void Update(){
        _canFireIn -= Time.deltaTime;

        DropBomb -= Time.deltaTime;

        var movementFactor = controller.State.isGrounded ? accelerationOnGround : accelerationOnAir;

        if (!isDead)
        {
            HandleInput();
            controller.SetHorizontalForce(Mathf.Lerp(controller.Velocity.x, normalizeHorizontalSpeed * maxSpeed, Time.deltaTime * movementFactor));
        }
        else
        {
            controller.SetHorizontalForce(0);
        }

        _anim.SetBool("IsGrounded", controller.State.isGrounded);
        _anim.SetBool("IsDead", isDead);
        _anim.SetFloat("Speed", Mathf.Abs(controller.Velocity.x) / maxSpeed);


	}

    public void FinishLevel()
    {
        enabled = false;
        controller.enabled = false;
        collider2D.enabled = false;
        _backgroundSound.Stop();
    }


    public void Kill()
    {
        if (DeadSound != null)
            AudioSource.PlayClipAtPoint(DeadSound, transform.position);

        _backgroundSound.Stop();

        controller.HandleCollisions = false;
        collider2D.enabled = false;

        Health = 0;
        isDead = true;
        // add vertical force to the player when it is killed
        // it will jump up a little
        controller.SetForce(new Vector2(0, 10));
    }
    public void TakeDamage(int damage, GameObject instigator)
    {
        if(HurtSound != null)
            AudioSource.PlayClipAtPoint(HurtSound, transform.position);

        FloatingText.Show(string.Format("-{0}!", damage), "PlayerDamageText", new FromWorldPointTextPositioner(Camera.main, transform.position, 5f, 80));

        Instantiate(HurtEffect, transform.position, transform.rotation);

        Health -= damage;

        if (Health <= 0)
            LevelManager.Instance.KillPlayer();
    }

    public void GiveHealth(int health, GameObject instigator)
    {
        FloatingText.Show(string.Format("+{0}!", health), "PointDiamondText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60));
        Health = Mathf.Min(Health + health, MaxHealth);
    }

    public void RespwanAt(Transform spawnPoint)
    {
        _backgroundSound.Play();

        if (!isFacingRight)
            Flip();

        Health = MaxHealth;
        isDead = false;

        collider2D.enabled = true;
        controller.HandleCollisions = true;
        transform.position = spawnPoint.position;
    }

	public void HandleInput(){

        // allow the player to jump
        if (Input.GetButton("Horizontal") || Input.GetButton("Jump"))
        {

            if (Input.GetButton("Jump") && controller.CanJump)
            {
                if(JumpSound != null)
                    AudioSource.PlayClipAtPoint(JumpSound, transform.position);

                controller.Jump();
            }

            // move to the right
            else if (Input.GetAxis("Horizontal") > 0)
            {
                normalizeHorizontalSpeed = 1;

                if (!isFacingRight)
                    Flip();
            }
            // move to the left
            else if (Input.GetAxis("Horizontal") < 0)
            {
                normalizeHorizontalSpeed = -1;

                if (isFacingRight)
                    Flip();
            }         
        }
        else if (Input.GetMouseButtonDown(0))
            FireProjectile();
        else if (Input.GetButton("DropItem") )
            DropBom();
        // the playe is standing
        else
            normalizeHorizontalSpeed = 0;
	}

    private void DropBom(){
        if ((DropBomb - Time.deltaTime) > 0)
            return;


        if ( BombSpawnLocation != null  && Bomb != null ){
            var theBomb = (GameObject)Instantiate(Bomb, BombSpawnLocation.position, BombSpawnLocation.rotation);
        }

        DropBomb = DropBombRate;
        
    }

    private void FireProjectile()
    {
        if ( (_canFireIn - Time.deltaTime ) > 0)
            return;

        var direction = isFacingRight ? Vector2.right : -Vector2.right;
        var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);

        projectile.Initialize(gameObject, direction, controller.Velocity);

        _canFireIn = FireRate;

        if(ShootSound != null)
            AudioSource.PlayClipAtPoint(ShootSound, transform.position);

        _anim.SetTrigger("Fire");
    }

	private void Flip(){
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		isFacingRight = transform.localScale.x > 0;
	}
}
