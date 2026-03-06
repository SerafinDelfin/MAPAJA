using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    GameManager gameManager;
    public float speed;
    public float jumpForce;
    public float teleportOffset = 0.5f;
    InputAction moveAction;
    InputAction jumpAction;
    Rigidbody2D rb;
    Vector2 currentDirection = Vector2.zero;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += OnMovePerformed;

        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpAction.performed += OnJumpPerformed;
    }


    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.x != 0)
        {
            currentDirection = new Vector2(Mathf.Sign(input.x), 0);
            
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * currentDirection.x;
            transform.localScale = scale;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        rb.AddForce(Vector2.up * jumpForce);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(currentDirection.x * speed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cambiar de dirección cuando choca con una pared
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Invertir la dirección
            currentDirection.x = -currentDirection.x;
            
            // Voltear el sprite
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * currentDirection.x;
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        float screenLeft = mainCamera.transform.position.x - (cameraWidth / 2f);
        float screenRight = mainCamera.transform.position.x + (cameraWidth / 2f);

        if (collision.gameObject == gameManager.LWall)
        {
            // Teletransportar al lado derecho (justo fuera del borde derecho)
            float newX = screenRight + teleportOffset;
            this.transform.position = new Vector3(newX, transform.position.y, 0);
        }
        else if (collision.gameObject == gameManager.RWall)
        {
            // Teletransportar al lado izquierdo (justo fuera del borde izquierdo)
            float newX = screenLeft - teleportOffset;
            this.transform.position = new Vector3(newX, transform.position.y, 0);
        }
    }



}
