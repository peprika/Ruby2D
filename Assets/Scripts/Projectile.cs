using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    // Awake is called when object in instantiated
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the project is too far from the map center, destroy it
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }

    // Launch the projectile
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    // When the projectile hits something...
    void OnCollisionEnter2D(Collision2D other)
    {
        // If it's a robot, fix it
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }

        // Destroy the projecile
        Destroy(gameObject);
    }
}
