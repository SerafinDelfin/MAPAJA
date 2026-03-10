using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth p = collision.GetComponent<PlayerHealth>();
            if (p != null)
            {
                // Llamamos al daño y le pasamos la posición de la bala para el empuje
                p.TakeDamage(damage, transform.position);
            }

            // La bala desaparece al tocarte
            Destroy(gameObject);
        }

        // Opcional: Que la bala se destruya al tocar el suelo (Capa "Ground" o similar)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}