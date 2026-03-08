using System;
using System.Collections;
using UnityEngine;

// Esta clase sirve como detector de colisiones
// Utilizar una instancia del componente por cada ataque
public class HitDetector : MonoBehaviour
{
    // Este struct contiene todo lo necesario para el overlap
    [Serializable]
    public struct OverlapBoxDefinition
    {
        public Vector2 size;
        public Vector2 offset;
        public float angle;
        // HACE FALTA LAYER DE ENEMIGOS
        // Se puede reutilizar para los hits de los enemigos hacia el player
        public LayerMask targetLayer;
    }

    // Arrastrar el componente esté donde esté
    // Usar attack si es player o enemy si es enemy
    [SerializeField]
    PlayerAttackComponent attackComponent;
    [SerializeField]
    EnemyAttackComponent enemyAttackComponent;

    [SerializeField]
    PlayerAttackComponent.AttackType HitType;

    // TAMAÑO DEL ATAQUE, ROTACIÓN, ETC.
    [SerializeField]
    OverlapBoxDefinition overlapBox;

    // Tiempo extra para cuadrar con animaciones por ejemplo
    [SerializeField]
    float delay;

    [SerializeField]
    private bool isEnemy;

    private void Start()
    {
        if (isEnemy)
        {
            enemyAttackComponent.onEnemyAttack.AddListener(OnHit);
        } 
        else 
        {
            attackComponent.onAttackEvent.AddListener(OnHit);
        }    
    }

    private void OnHit(PlayerAttackComponent.AttackType type, int damageToDo)
    {
        if (type != HitType) return;
        StartCoroutine(DelayHit(damageToDo));
    }

    private IEnumerator DelayHit(int damageToDo)
    {
        yield return new WaitForSeconds(delay);

        // Detecta los colliders de los enemigos
        var hits = Physics2D.OverlapBoxAll
            (
            (Vector2)transform.position + overlapBox.offset,
            overlapBox.size,
            overlapBox.angle,
            overlapBox.targetLayer
            );

        foreach (var hit in hits)
        {
            print(hit.gameObject.name);
            if (!hit.TryGetComponent<HealthComponent>(out var healthComponent))
                continue;
            healthComponent.GetDamage(damageToDo);
        }
    }

    public Color debugColor;
    private void OnDrawGizmos()
    {
        // Debug Hits
        Gizmos.color = debugColor;

        Gizmos.DrawWireCube((Vector2)transform.position + overlapBox.offset, overlapBox.size);
    }
}
