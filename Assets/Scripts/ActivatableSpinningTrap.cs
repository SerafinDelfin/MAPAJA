using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))] // Esto obliga a que el objeto tenga AudioSource
public class ActivatableSpinningTrap : MonoBehaviour
{
    [Header("Estado")]
    public bool isMoving = false; // ¿Está patrullando ahora mismo?

    [Header("Ajustes de Movimiento")]
    public float moveDistance = 3f; // Distancia máxima a cada lado
    public float moveSpeed = 2f;    // Rapidez del vaivén

    [Header("Ajustes de Rotación")]
    public float rotationSpeed = 450f; // Siempre estará rotando

    [Header("Daño y Empuje")]
    public float damage = 1f;
    public float knockbackForce = 12f;

    private Vector3 startPosition;
    private SpriteRenderer sr;

    [Header("Sonido")]
    public AudioClip spinSound; // Arrastra aquí el sonido de la sierra
    private AudioSource audioSource;
    [Range(0f, 1f)] public float maxVolume = 0.5f; // ESTA ES TU BARRA EN EL INSPECTOR
    public float soundDistance = 10f; // A qué distancia se deja de oír


    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        // Guardamos la posición donde la colocaste en el editor como centro
        startPosition = transform.position;

        // Configuración del AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (spinSound != null)
        {
            audioSource.clip = spinSound;
            audioSource.loop = true;

            // CONFIGURACIÓN 3D POR CÓDIGO:
            audioSource.spatialBlend = 1.0f; // 0 es 2D (se oye igual siempre), 1 es 3D (depende de distancia)
            audioSource.rolloffMode = AudioRolloffMode.Linear; // El sonido baja de forma constante
            audioSource.minDistance = 1f;
            audioSource.maxDistance = soundDistance;
            audioSource.volume = maxVolume;

            audioSource.Play();
        }
    }

    private void Update()
    {
        // Actualizamos el volumen por si lo mueves en el inspector mientras juegas
        audioSource.volume = maxVolume;
        audioSource.maxDistance = soundDistance;

        // 1. ROTACIÓN CONSTANTE (Esto no para nunca)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // 2. MOVIMIENTO LATERAL (Solo si está activado)
        if (isMoving)
        {
            // Usamos Mathf.Sin para un movimiento suave de vaivén
            float offsetX = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = startPosition + new Vector3(offsetX, 0, 0);
        }
    }

    // --- ACTIVACIÓN DEL MOVIMIENTO POR CLIC ---
    private void OnMouseDown()
    {
        isMoving = !isMoving;

        // Si la apagamos, vuelve al centro suavemente o se queda donde está
        // En este caso, si quieres que al apagarla vuelva al centro:
        if (!isMoving) transform.position = startPosition;

        Debug.Log("Movimiento de trampa: " + (isMoving ? "ACTIVADO" : "PARADO"));
    }

    // --- SISTEMA DE DAÑO UNIVERSAL ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // La trampa siempre hace daño si rotas, independientemente de si se mueve o no
        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            

            // Daño al Player
            if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth p))
                p.TakeDamage((int)damage, transform.position);

            // Aplicar el retroceso (Knockback)
            ApplyKnockback(collision);
        }
    }

    private void ApplyKnockback(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (col.transform.position - transform.position).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }
    }

    // Visualizar el rango en el Editor de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Vector3 left = center + Vector3.left * moveDistance;
        Vector3 right = center + Vector3.right * moveDistance;
        Gizmos.DrawLine(left, right);
    }
}