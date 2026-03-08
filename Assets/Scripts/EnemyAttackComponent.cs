using UnityEngine;
using UnityEngine.Events;

public class EnemyAttackComponent : MonoBehaviour
{
    public UnityEvent<PlayerAttackComponent.AttackType, int> onEnemyAttack;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
