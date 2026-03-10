using UnityEngine;
using UnityEngine.Rendering;

public class LootBox : MonoBehaviour
{
    public int health = 1;
    [Header("Ajustes de Drop")]
    public GameObject[] lootPrefabs; // Lista de cosas que puede soltar (Monedas, Vida, etc.)
    [Range(0, 100)] public float dropChance = 100f; // Probabilidad de que suelte algo

    [Header("Sonido")]
    public AudioClip breakSound; // Arrastra tu sonido aquí
    [Range(0f, 1f)] public float volume = 1f; // La barra para el volumen


    public void TakeDamage(float damage)
    {
        health -= (int)damage;
        if (health <= 0) Break();
    }

    private void Break()
    {
        // Sonido rápido antes de destruir
        if (breakSound != null)
        {
            // Reproduce el sonido en la posición de la cámara (se oye igual en todo el mapa)
            AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position, volume);
        }

        // Lógica de Drop
        if (lootPrefabs.Length > 0 && Random.Range(0f, 100f) <= dropChance)
        {
            int randomIndex = Random.Range(0, lootPrefabs.Length);
            Instantiate(lootPrefabs[randomIndex], transform.position, Quaternion.identity);
        }

        // Destruir la caja
        Destroy(gameObject);
    }
}