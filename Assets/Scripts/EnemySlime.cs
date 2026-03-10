using UnityEngine;
using System.Collections;

public class EnemySlime : MonoBehaviour
{
    [Header("Estadísticas")]
    public float maxHealth = 3;
    private float currentHealth;
    public float speed = 3f; // Velocidad constante como el player
    public float chaseSpeed = 4.5f; // Un poco más rápido al perseguir
    public int damageToPlayer = 1;

    [Header("Detección del Jugador")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    private Transform playerTransform;
    private bool playerDetected = false;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Movimiento (Estilo Player)")]
    private float moveDirection = 1f; // 1 derecha, -1 izquierda

    [Header("Efectos de Daño")]
    public Color hitColor = Color.red;
    private Color originalColor;
    public float hitColorDuration = 0.2f;
    public float knockbackForceX = 5f;
    public float knockbackForceY = 3f;
    private bool isKnockback = false;

    [Header("Referencias Internas")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    // --- ARREGLO GIRO ---
    [Header("Configuración Visual")]
    [Tooltip("Marca esto si tu sprite original mira hacia la IZQUIERDA.")]
    public bool spriteMiraIzquierdaBase = true; // Por defecto true, ya que es tu caso



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !isKnockback && currentHealth > 0)
        {
            DetectPlayer();

            float currentSpeed = speed;

            if (playerDetected && playerTransform != null)
            {
                moveDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);
                currentSpeed = chaseSpeed;
            }

            rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);

            // --- ARREGLO GIRO ---
            // Usamos la nueva función Flip arreglada
            Flip(moveDirection);
            // -------------------

            if (anim != null) anim.SetBool("isMoving", true);
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (playerCollider != null)
        {
            playerDetected = true;
            playerTransform = playerCollider.transform;
        }
        else
        {
            playerDetected = false;
        }
    }

    // --- Rebote en paredes (Igual que tu Player) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Rebote en paredes
        if (collision.gameObject.CompareTag("Wall") && !playerDetected)
        {
            moveDirection *= -1;
        }

        // 2. DAÑO AL JUGADOR: Si el Slime toca al player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth pHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                // Le pasamos la posición del slime para el knockback
                pHealth.TakeDamage(damageToPlayer, transform.position);
            }
        }
    }

    // --- Lógica de Daño y Muerte ---
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth > 0) StartCoroutine(HitEffectsRoutine());
        else Die();
    }

    // --- ARREGLO GIRO (Nueva función Flip) ---
    private void Flip(float direction)
    {
        if (direction == 0) return;

        // Factor de corrección: si mira a la izquierda base, invertimos el signo de la dirección visual
        float correccion = spriteMiraIzquierdaBase ? -1f : 1f;
        float finalVisualDirection = direction * correccion;

        // Aplicamos la escala final corregida
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * finalVisualDirection, transform.localScale.y, 1);
    }

    private IEnumerator HitEffectsRoutine()
    {
        isKnockback = true;
        spriteRenderer.color = hitColor;

        // Empuje opuesto a donde esté el jugador
        float kDir = (playerTransform != null) ? Mathf.Sign(transform.position.x - playerTransform.position.x) : -moveDirection;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(kDir * knockbackForceX, knockbackForceY), ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitColorDuration);
        spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        isKnockback = false;
    }

    private void Die()
    {
        if (anim != null) anim.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}