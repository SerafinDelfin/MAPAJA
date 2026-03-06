using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Player;
    public GameObject LWall;
    public GameObject RWall;
    public float XOffset = 18f;


    private void Awake()
    {
        instance = this;
    }



}
