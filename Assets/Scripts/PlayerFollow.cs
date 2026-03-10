using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public GameObject player;
    public float smoothSpeed = 5f;
    public float yOffset = 6.5f;
    public float minY = 10f; 
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 currentPos = transform.position;
        
        // Solo seguir la Y del jugador con offset, mantener X y Z fijos
        Vector3 targetPos = new Vector3(currentPos.x, playerPos.y + yOffset, currentPos.z);
        
        // Aplicar límite mínimo de Y
        targetPos.y = Mathf.Max(targetPos.y, minY);
        
        // Interpolar suavemente solo en el eje Y
        transform.position = Vector3.Lerp(currentPos, targetPos, smoothSpeed * Time.deltaTime);
    }

}
