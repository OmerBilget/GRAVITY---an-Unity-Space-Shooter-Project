using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShockField : MonoBehaviour
{

    public Boss Boss;
    public CircleCollider2D col;

    public PlayerHealth playerHealth;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float angle = Vector2.Angle(transform.up, (transform.position - collision.transform.position));
            if (angle > 15 && playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
    
    // Start is called before the first frame update

}
