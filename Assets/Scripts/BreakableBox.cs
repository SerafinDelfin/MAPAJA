using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    public int health = 1;
    public GameObject breakEffect; // Opcional: Un sistema de partículas o trozos de madera

    [Header("Sonido")]
    public AudioClip breakSound; // Arrastra tu sonido aquí
    [Range(0f, 1f)] public float volume = 1f; // La barra para el volumen


    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if (health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        // Sonido rápido antes de destruir
        if (breakSound != null)
        {
            // Reproduce el sonido en la posición de la cámara (se oye igual en todo el mapa)
            AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position, volume);
        }


        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        // Aquí podrías activar un trigger de animación si la caja tiene Animator
        // GetComponent<Animator>().SetTrigger("Break");

        Destroy(gameObject);
    }
}