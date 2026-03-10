using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Ajustes de Curación")]
    public int healthAmount = 1; // Cuánta vida recupera

    [Header("Efectos")]
    public AudioClip collectSound; // Sonido al recogerlo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Comprobamos si es el jugador
        if (collision.CompareTag("Player"))
        {
            PlayerHealth healthScript = collision.GetComponent<PlayerHealth>();

            if (healthScript != null)
            {
                // 2. Intentamos curar al jugador
                healthScript.Heal(healthAmount);

                // 3. Sonido (opcional)
                if (collectSound != null)
                {
                    // Lo reproducimos en la cámara o un punto global para que se oiga aunque el objeto se destruya
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);
                }

                // 4. Desaparece el objeto
                Destroy(gameObject);
            }
        }
    }
}