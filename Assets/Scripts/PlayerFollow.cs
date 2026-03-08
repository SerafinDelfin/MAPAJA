using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public GameObject player;
    private float xOffset;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        xOffset = gameManager.XOffset;
    }

    private void Update()
    {
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x + xOffset, playerPos.y, transform.position.z);

    }

}
