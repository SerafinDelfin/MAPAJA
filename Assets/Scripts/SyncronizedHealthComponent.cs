using UnityEngine;

public class SyncronizedHealthComponent : HealthComponent
{
    private static HealthParams sharedParams;

    private void Start()
    {
        sharedParams = healthParams; 
    }
    public override void GetDamage(int damage)
    {
        sharedParams.health += damage;
        sharedParams.health = Mathf.Clamp(sharedParams.health, 0, sharedParams.maxHealth);
        Die();
    }
    public HealthParams GetHealth()
    {
        return sharedParams;
    }
}
