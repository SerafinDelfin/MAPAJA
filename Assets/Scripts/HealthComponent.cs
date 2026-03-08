using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    public UnityEvent OnDeath;

    [Serializable]
    public struct HealthParams
    {
        [SerializeField]
        public int maxHealth;
        public int health;

        public void Init()
        {
            health = maxHealth;
        }
    }

    [SerializeField]
    protected HealthParams healthParams;

    private void Awake()
    {
        healthParams.Init();
    }

    public virtual void GetDamage(int damage)
    {
        healthParams.health -= damage;
        healthParams.health = Mathf.Clamp(healthParams.health, 0, healthParams.maxHealth);
        print(gameObject.name + "tiene " +  healthParams.health + " de daño, recibió: " + damage);
        Die();
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }
}
