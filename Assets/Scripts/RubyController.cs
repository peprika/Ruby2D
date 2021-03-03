using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public int maxHealth = 5;
    public float speed = 3.0f;
    public float timeInvincible = 2.0f;
    public GameObject projectilePrefab;
    public float projectileForce = 300.0f;
    public ParticleSystem ouchEffectPrefab;
    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; } }
    int currentHealth;
    bool isInvincible;
    float invincibleTimer;
    float horizontal;
    float vertical;
    Vector2 lookDirection = new Vector2(1, 0);

    Rigidbody2D rigidbody2d;
    Animator animator;
    AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Get movement inputs
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);

        // Get the direction the character is facing, and send it to animator
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // Timer for invincibility
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        // If C key is pressed, launch a projectile
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        // If X key is pressed...
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Create a raycast and check if it hits NPC characters
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                // If a character is found, show its dialog
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Move the character
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    void Launch()
    {
        // Create a projectile object, and launch it
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, projectileForce);

        PlaySound(throwSound);
        animator.SetTrigger("Launch");
    }

    public void ChangeHealth(int amount)
    {
        // If the character takes damage, make it invincible for a moment
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            Instantiate(ouchEffectPrefab, rigidbody2d.position, Quaternion.identity);
            if (isInvincible)
                return;

            PlaySound(hitSound);
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        // "Ouch" particle effect
        Instantiate(ouchEffectPrefab, rigidbody2d.position, Quaternion.identity);

        // Change health
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}