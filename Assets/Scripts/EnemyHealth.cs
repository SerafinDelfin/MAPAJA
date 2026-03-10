using UnityEngine;
using UnityEngine.UI; // Necesario para el Slider

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    public float maxHealth = 50f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public GameObject canvasVida; // Para ocultar la barra al morir

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Feedback visual: un pequeño flash rojo al recibir daño
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " ha sido derrotado.");
        // Desactivamos el canvas para que no flote la barra vacía
        if (canvasVida != null) canvasVida.SetActive(false);
        GameManager.instance.OnEnemyDeath();
        // Aquí puedes poner una animación de muerte o simplemente destruir el objeto
        Destroy(gameObject);
    }
}