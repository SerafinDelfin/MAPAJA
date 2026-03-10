using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Detectar cualquier cosa con vida (Enemigos)
        if (collision.CompareTag("Enemy"))
        {
            // Buscamos el script UNIFICADO de vida
            if (collision.TryGetComponent<EnemyHealth>(out EnemyHealth health))
            {
                health.TakeDamage(damage);
                Debug.Log("Golpeado Enemigo: " + collision.name + " | Vida restante: " + health.healthSlider.value);
            }
        }

        // 2. Detectar objetos rompibles
        if (collision.CompareTag("Breakable"))
        {
            if (collision.TryGetComponent<BreakableBox>(out BreakableBox box))
                box.TakeDamage(damage);

            if (collision.TryGetComponent<LootBox>(out LootBox lBox))
                lBox.TakeDamage(damage);

            Debug.Log("Objeto roto: " + collision.name);
        }
    }
}