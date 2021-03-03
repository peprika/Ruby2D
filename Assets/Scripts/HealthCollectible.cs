using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        // If Ruby hits a health item...
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            // ...and she's not at max health, add health, destroy the item and play a sound
            if (controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(1);
                Destroy(gameObject);

                controller.PlaySound(collectedClip);
            }
        }

    }
}