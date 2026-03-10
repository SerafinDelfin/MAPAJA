using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    public int health = 3;
    public int maxHealth = 3;
    public float knockbackForce = 8f;
    public float invulnerabilityDuration = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isInvulnerable = false;

    [Header("Sonidos")]
    public AudioClip hurtSound; // Arrastra aquí el sonido de daño
    private AudioSource audioSource;

    [Header("UI")]
    public Slider healthSlider;

    [Header("UI Game Over")]
    public GameObject gameOverPanel; // Arrastra aquí el panel desde el Canvas

    private void Start()
    {
        // slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        // Buscamos el SpriteRenderer en este objeto o en los hijos
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount, Vector2 enemyPosition)
    {
        if (isInvulnerable) return;

        // --- NUEVA LÍNEA PARA EL SONIDO ---
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        ActualizarInterfaz();

        health -= amount;
        Debug.Log("Vida Jugador: " + health);

        if (health > 0)
        {
            // IMPORTANTE: El nombre debe coincidir con la corrutina de abajo
            StartCoroutine(DamageFeedbackRoutine(enemyPosition));
        }
        else
        {
            Die();
        }
    }

    IEnumerator DamageFeedbackRoutine(Vector2 enemyPosition)
    {
        isInvulnerable = true;

        // --- LÓGICA DE EMPUJÓN (Knockback) ---
        if (rb != null)
        {
            // Calculamos dirección opuesta al enemigo
            Vector2 knockbackDir = ((Vector2)transform.position - enemyPosition).normalized;
            // Aplicamos la fuerza (añadimos un poco de fuerza hacia arriba para que salte un poco)
            rb.linearVelocity = Vector2.zero; // Limpiamos velocidad actual para que el golpe sea seco
            rb.AddForce(new Vector2(knockbackDir.x, 0.5f) * knockbackForce, ForceMode2D.Impulse);
        }

        // --- FEEDBACK VISUAL ---
        // 1. Rojo instantáneo
        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        if (sr != null) sr.color = Color.white;

        // 2. Parpadeo de invulnerabilidad
        float timer = 0;
        while (timer < invulnerabilityDuration)
        {
            if (sr != null) sr.color = new Color(1, 1, 1, 0.3f); // Transparente
            yield return new WaitForSeconds(0.1f);
            if (sr != null) sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        isInvulnerable = false;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth; // No pasarse del máximo

        Debug.Log("¡Curado! Vida actual: " + health);

        ActualizarInterfaz();

        // Feedback visual opcional: que parpadee en verde al curarse
        StartCoroutine(HealFeedbackRoutine());
    }

    private IEnumerator HealFeedbackRoutine()
    {
        if (sr != null) sr.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        if (sr != null) sr.color = Color.white;
    }

    private void ActualizarInterfaz()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }


    public void Die()
    {
        Debug.Log("<color=red>PLAYER MUERTO</color>");

        // 1. BUSCAMOS EL AUDIOSOURCE DEL JUGADOR Y LO PARAMOS
        AudioSource playerAudio = GetComponent<AudioSource>();
        if (playerAudio != null)
        {
            playerAudio.Stop(); // Esto detiene el sonido de pasos al instante
        }

        // 2. Activar el panel de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 3. Congelar el juego
        Time.timeScale = 0f;

        // Opcional: Desactivar el script de movimiento para que no detecte más inputs
        PlayerMovement moveScript = GetComponent<PlayerMovement>();
        if (moveScript != null) moveScript.enabled = false;
    }

    // Función para el botón de Reintentar
    public void RestartGame()
    {
        Time.timeScale = 1f; // Muy importante devolver el tiempo a la normalidad
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // FUNCIÓN PARA EL BOTÓN "EXIT"
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esto cierra el juego (solo funciona en el .exe compilado)
    }
}