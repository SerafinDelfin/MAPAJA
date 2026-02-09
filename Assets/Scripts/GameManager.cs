using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject[] Players;
    public GameObject LWall;
    public GameObject RWall;
    public float XOffset = 18f;
    public float playerSpeed;


    private void Start()
    {
        
    }

    public float GetLowerX() 
    {
        float LowerPos = Players[0].transform.position.x;
        for (int i = 1; i < Players.Length; i++) 
        {
            if (Players[i].transform.position.x < LowerPos) 
            {
                LowerPos = Players[i].transform.position.x;
            }
        }
        return LowerPos;

    }

    public float GetHigherX()
    {
        float HigherPos = Players[0].transform.position.x;
        for (int i = 1; i < Players.Length; i++)
        {
            if (Players[i].transform.position.x > HigherPos)
            {
                HigherPos = Players[i].transform.position.x;
            }
        }
        return HigherPos;
    }

    public float GetMidY() 
    {

        var ordered = System.Linq.Enumerable.OrderBy(Players, p => p.transform.position.x).ToArray();

        return ordered[1].transform.position.y;
    }


}
