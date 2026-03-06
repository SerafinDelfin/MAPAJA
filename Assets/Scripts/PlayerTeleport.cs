using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private GameManager gameManager;
    private Camera mainCamera;
    public float barrierOffset = 2f;

    private void Awake()
    {
        gameManager = GameManager.instance;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        PositionTeleportBarriers();
    }

    private void Update()
    {
        UpdateBarriersYPosition();
    }

    private void PositionTeleportBarriers()
    {
        if (gameManager == null || gameManager.LWall == null || gameManager.RWall == null) return;
        if (mainCamera == null) return;

        // Calcular los límites de la cámara
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Obtener el ancho de las barreras
        float wallWidth = GetWallWidth(gameManager.LWall);

        // Posicionar la barrera izquierda (fuera del borde izquierdo de la pantalla)
        Vector3 leftPos = gameManager.LWall.transform.position;
        leftPos.x = mainCamera.transform.position.x - (cameraWidth / 2f) - wallWidth - barrierOffset;
        gameManager.LWall.transform.position = leftPos;

        // Posicionar la barrera derecha (fuera del borde derecho de la pantalla)
        Vector3 rightPos = gameManager.RWall.transform.position;
        rightPos.x = mainCamera.transform.position.x + (cameraWidth / 2f) + wallWidth + barrierOffset;
        gameManager.RWall.transform.position = rightPos;
    }

    private void UpdateBarriersYPosition()
    {
        if (gameManager == null || gameManager.LWall == null || gameManager.RWall == null) return;
        if (gameManager.Player == null) return;

        // Obtener la posición Y del jugador
        float playerY = gameManager.Player.transform.position.y;

        // Actualizar posición Y de la barrera izquierda
        Vector3 leftPos = gameManager.LWall.transform.position;
        leftPos.y = playerY;
        gameManager.LWall.transform.position = leftPos;

        // Actualizar posición Y de la barrera derecha
        Vector3 rightPos = gameManager.RWall.transform.position;
        rightPos.y = playerY;
        gameManager.RWall.transform.position = rightPos;
    }

    private float GetWallWidth(GameObject wall)
    {
        // Intentar obtener el ancho desde el SpriteRenderer
        SpriteRenderer spriteRenderer = wall.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds.size.x;
        }

        // Si no tiene SpriteRenderer, intentar con Collider2D
        Collider2D collider = wall.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds.size.x;
        }

        // Valor por defecto
        return 1f;
    }
}
