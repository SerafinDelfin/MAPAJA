using UnityEngine;
using System.Collections;

public class EnemyFly : MonoBehaviour
{
    

    [Header("Estadísticas de Vuelo")]
    public float detectionRadius = 8f;
    public float shootCooldown = 2f;
    private float lastShootTime;

    [Header("Ataque")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 5f;

    [Header("Configuración Visual")]
    public bool spriteMiraIzquierdaBase = true;
    private Transform player;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Levitación")]
    public float amplitude = 0.5f;
    public float frequency = 2f;
    private Vector3 startPosition;

    [Header("Sonidos")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        // Si el jugador no existe, no hacemos nada
        if (player == null) return;

        // --- LÓGICA DE LEVITACIÓN ---
        float newY = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPosition + new Vector3(0, newY, 0);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Siempre mira al jugador
        LookAtPlayer();

        // 2. Si está en rango y pasó el tiempo de recarga, dispara
        if (distanceToPlayer <= detectionRadius)
        {
            if (Time.time >= lastShootTime + shootCooldown)
            {
                StartCoroutine(ShootRoutine());
                lastShootTime = Time.time;
            }
        }
    }

    private void LookAtPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        float correccion = spriteMiraIzquierdaBase ? -1f : 1f;
        float finalVisualDirection = direction * correccion;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * finalVisualDirection, transform.localScale.y, 1);
    }

    IEnumerator ShootRoutine()
    {
        if (anim != null) anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        if (bulletPrefab != null && shootPoint != null && player != null)
        {
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            Vector2 direction = (player.position - shootPoint.position).normalized;
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

            if (rbBullet != null)
            {
                rbBullet.linearVelocity = direction * bulletSpeed;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            Destroy(bullet, 4f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}