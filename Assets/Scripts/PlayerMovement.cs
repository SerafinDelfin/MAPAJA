using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    GameManager gameManager;

    [Header("Ajustes de Velocidad")]
    public float speed = 5f;
    public float jumpForce = 12f;
    public float teleportOffset = 0.5f;
    public float autoMoveDirection = 1f;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float checkRadius = 0.25f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    // Referencias internas
    private Rigidbody2D rb;
    public Animator anim;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction attackAction;
    private InputAction heavyAttackAction; // La tecla E

    [Header("Ajustes de Deslizamiento")]
    public float slideDuration = 0.8f;
    public Vector2 slideColliderSize = new Vector2(1f, 0.5f);
    public Vector2 slideColliderOffset = new Vector2(0f, -0.25f);
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private BoxCollider2D playerCollider;

    [Header("Ajustes de Ataque")]
    public GameObject attackZone;
    public float attackDuration = 0.15f;

    [Header("Ajustes de Combo")]
    public float comboWindow = 0.5f; // Tiempo máximo entre clics para seguir el combo
    private int comboStep = 0;       // En qué paso del combo estamos (0 o 1)
    private float lastAttackTime;    // Cuándo fue el último clic

    [Header("Ajustes de Impulso (Dash E)")]
    public float heavyAttackDashForce = 15f; // Fuerza del empujón
    public float dashDuration = 0.25f;       // Tiempo que dura el impulso
    private bool isDashing = false;          // Interruptor para el FixedUpdate

    [Header("Sonidos")]
    public AudioClip walkSound;
    public AudioClip attackSound1;
    public AudioClip attackSound2;
    public AudioClip heavyAttackSound;
    [Range(0, 1)] public float attackVolume = 0.7f;
    [Range(0, 1)] public float heavyVolume = 1.0f;
    private AudioSource audioSource;

    private bool isSliding = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerCollider = GetComponent<BoxCollider2D>();
        

        if (playerCollider != null)
        {
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset;
        }

        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        // Configuración de Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = walkSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Configuración de Inputs
        if (playerInput != null)
        {
            moveAction = playerInput.actions.FindAction("Move");
            jumpAction = playerInput.actions.FindAction("Jump");
            attackAction = playerInput.actions.FindAction("Attack");
            slideAction = playerInput.actions.FindAction("Deslizar");
            heavyAttackAction = playerInput.actions.FindAction("HeavyAttack");

            if (moveAction != null) moveAction.performed += OnMovePerformed;
            if (jumpAction != null) jumpAction.performed += OnJumpPerformed;

            // Clic normal
            if (attackAction != null)
                attackAction.performed += ctx => TryAttack();

            // Tecla E (Ataque fuerte con impulso)
            if (heavyAttackAction != null)
                heavyAttackAction.performed += ctx => PerformHeavyAttack();

            if (slideAction != null)
            {
                slideAction.performed += ctx => StartSlide();
                slideAction.canceled += ctx => StopSlide();
            }
        }
    }

    private void Start()
    {
        // Inicializar GameManager
        gameManager = GameManager.instance;
    }
    // --- LÓGICA DE MOVIMIENTO (FIXED UPDATE) ---

    private void FixedUpdate()
    {
        // 1. Detección de suelo
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // 2. MOVIMIENTO AUTOMÁTICO (Solo si NO estamos en Dash)
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(autoMoveDirection * speed, rb.linearVelocity.y);
        }

        // 3. Volteo del sprite
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * autoMoveDirection, transform.localScale.y, 1);

        // 4. Actualización Animator
        if (anim != null)
        {
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("isSliding", isSliding);
            anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
            
            // Forzar trigger de salto si acaba de perder contacto con el suelo
            if (wasGrounded && !isGrounded && rb.linearVelocity.y > 0)
            {
                anim.SetTrigger("Jump");
            }
        }

        // 5. Sonido de pasos
        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f && !isSliding && !isDashing)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == walkSound) audioSource.Stop();
        }
    }

    // --- SISTEMA DE ATAQUES ---

    private void TryAttack()
    {
        // 1. Verificar si ha pasado mucho tiempo desde el último ataque para reiniciar el combo
        if (Time.time - lastAttackTime > comboWindow)
        {
            comboStep = 0;
        }

        // 2. Ejecutar la animación correspondiente
        if (anim != null)
        {
            // Reiniciamos los triggers para evitar que se acumulen
            anim.ResetTrigger("Attack1");
            anim.ResetTrigger("Attack2");

            if (comboStep == 0)
            {
                anim.SetTrigger("Attack1");
                comboStep = 1; // El siguiente será el 2
            }
            else
            {
                anim.SetTrigger("Attack2");
                comboStep = 0; // Reiniciamos al 1
            }
        }

        // 3. Lógica de daño y sonido
        lastAttackTime = Time.time;
        StartCoroutine(AttackRoutine(false));
    }

    public void PerformHeavyAttack()
    {
        if (anim != null)
        {
            anim.ResetTrigger("BigAttack");
            anim.SetTrigger("BigAttack");
        }

        // Ejecutamos el impulso
        StartCoroutine(DashRoutine());

        if (heavyAttackSound != null)
            audioSource.PlayOneShot(heavyAttackSound, heavyVolume);

        Debug.Log("<color=orange>¡ATAQUE FUERTE CON IMPULSO (E)!</color>");
        StartCoroutine(AttackRoutine(true));
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        // Aplicamos velocidad de golpe en la dirección que mira
        rb.linearVelocity = new Vector2(autoMoveDirection * heavyAttackDashForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    private IEnumerator AttackRoutine(bool isHeavy)
    {
        if (attackZone != null)
        {
            if (!isHeavy)
            {
                // Ahora el sonido coincide con el paso del combo
                AudioClip clip = (comboStep == 1) ? attackSound1 : attackSound2;
                if (clip != null) audioSource.PlayOneShot(clip, attackVolume);
            }

            attackZone.SetActive(true);
            float duration = isHeavy ? attackDuration * 2.5f : attackDuration;
            yield return new WaitForSeconds(duration);
            attackZone.SetActive(false);
        }
    }

    // --- ACCIONES SECUNDARIAS ---

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        float inputX = context.ReadValue<Vector2>().x;
        if (inputX != 0) autoMoveDirection = Mathf.Sign(inputX);
    }

    public void StartSlide()
    {
        if (isGrounded && !isSliding) StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        playerCollider.size = slideColliderSize;
        playerCollider.offset = slideColliderOffset;
        if (anim != null) anim.SetBool("isSliding", true);
        yield return new WaitForSeconds(slideDuration);
        StopSlide();
    }

    public void StopSlide()
    {
        isSliding = false;
        playerCollider.size = originalColliderSize;
        playerCollider.offset = originalColliderOffset;
        if (anim != null) anim.SetBool("isSliding", false);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (anim != null) anim.SetTrigger("Jump");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) autoMoveDirection *= -1;

        // Teleport
        print(gameManager == null || gameManager.LWall == null || gameManager.RWall == null);
        if (gameManager == null || gameManager.LWall == null || gameManager.RWall == null) return;

        Debug.Log("He tocado algo llamado: " + collision.gameObject.name);

        Camera mainCamera = Camera.main;

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float screenLeft = mainCamera.transform.position.x - (cameraWidth / 2f);
        float screenRight = mainCamera.transform.position.x + (cameraWidth / 2f);

        if (collision.gameObject == gameManager.LWall)
        {
            Debug.Log("Teletransportando a la DERECHA");

            // Teletransportar al lado derecho (justo fuera del borde derecho)
            float newX = screenRight + teleportOffset;
            transform.position = new Vector3(newX, transform.position.y, 0);
        }
        else if (collision.gameObject == gameManager.RWall)
        {
            Debug.Log("Teletransportando a la IZQUIERDA");

            // Teletransportar al lado izquierdo (justo fuera del borde izquierdo)
            float newX = screenLeft - teleportOffset;
            this.transform.position = new Vector3(newX, transform.position.y, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}