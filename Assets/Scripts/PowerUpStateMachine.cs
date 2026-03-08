using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpStateMachine : MonoBehaviour
{
    // Añadir todos los power ups necesarios
    public enum PowerUps
    {
        Basic,
        Hammer,
        Gun
    }

    public static PowerUps powerUps = PowerUps.Basic;

    [SerializeField]
    private List<GameObject> powerUpList;

    private static PowerUpStateMachine[] members;
    private static bool membersFound;

    private void Start()
    {
        ToStart();
    }

    private void ToStart()
    {
        if (membersFound) return;
        membersFound = true;
        members = FindObjectsByType<PowerUpStateMachine>(FindObjectsSortMode.None);
    }

    public static void ChangePowerUp(PowerUps newPowerUp) 
    {
        foreach (PowerUpStateMachine member in members)
        {
            member.powerUpList[(int)powerUps].SetActive(false);
            member.powerUpList[(int)newPowerUp].SetActive(true);
        }
        powerUps = newPowerUp;
    }
}
