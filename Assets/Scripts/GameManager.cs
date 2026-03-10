using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Wall
    public GameObject Player;
    public GameObject LWall;
    public GameObject RWall;
    public float XOffset = 18f;

    // End game
    public Transform Enemies;
    public GameObject WinCanvas;

    private void Awake()
    {
        instance = this;
    }

    public void OnEnemyDeath()
    {
        int enemiesLeft = Enemies.childCount;
        print("enemies left: " + enemiesLeft.ToString());
        if (enemiesLeft == 1)
        {
            Time.timeScale = 0.0f;
            WinCanvas.SetActive(true); 
        }
    }

}
