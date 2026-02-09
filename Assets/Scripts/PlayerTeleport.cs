using Unity.VisualScripting;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    GameManager gameManager;
    Rigidbody2D rb;
    private GameObject clone;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void CreateClone(string position) 
    {
        if (position == "L") { 
        var instantiatedClone = Instantiate(clone, gameManager.LWall.transform.position, Quaternion.identity);
        instantiatedClone.gameObject.name = "Clone";
        }
        if (position == "R")
        {
            var instantiatedClone = Instantiate(clone, gameManager.RWall.transform.position, Quaternion.identity);
            instantiatedClone.gameObject.name = "Clone";
        }
    }





}
