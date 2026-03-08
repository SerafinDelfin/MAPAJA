using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    GameManager gameManager;
    private float speed;
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

    private void Start()
    {
        speed = gameManager.playerSpeed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.x != 0)
            currentDirection = new Vector2(Mathf.Sign(input.x), 0);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        rb.AddForce(Vector2.up * 400f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(currentDirection.x * speed, rb.linearVelocity.y);
    }

    //Al llegar a uno de los límites, teletransportamos ese jugador al lado contrario
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == gameManager.LWall.gameObject)
        {
            float x1 = gameManager.GetHigherX();
            this.transform.position = new
                Vector3(x1 + gameManager.XOffset, gameManager.GetMidY(), 0);
        }
        else if (collision.gameObject == gameManager.RWall.gameObject)
        {
            float x2 = gameManager.GetLowerX();
            this.transform.position = new
                Vector3(x2 - gameManager.XOffset, gameManager.GetMidY(), 0);
        }
    }



}
