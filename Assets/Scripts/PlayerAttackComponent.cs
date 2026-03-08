using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAttackComponent : MonoBehaviour
{
    // Enum tipos de ataques
    public enum AttackType
    {
        Normal,
        Special
    }

    // Acciones para los ataques
    [SerializeField]
    InputActionReference normalAttack, specialAttack;

    // Evento al que se deben suscribir todos los que reaccionen a los ataques
    // Estos son el detector de colisiones, el manager de animaciones, etc.
    public UnityEvent<AttackType, int> onAttackEvent;

    // Tiempo de descanso de ataques
    [SerializeField]
    private float attackCooldown, specialCooldown;

    // Daño que puede ser seleccionado
    [SerializeField]
    private int normalDamage, specialDamage;

    // Timers para saber si ha pasado el tiempo de cooldown
    private float attackTimer, specialTimer;

    private void OnEnable()
    {
        // CUIDADO CON LOS DELEGADOS
        // ESTA ES LA FORMA MÁS RÁPIDA DE MANEJAR INPUTS

        // Es importante que el input esté en press only, si no detectará al pulsar y al soltar
        normalAttack.action.performed += OnNormalAttack;
        specialAttack.action.performed += OnSpecialAttack;
    }

    private void OnDisable()
    {
        // CUIDADO CON LOS DELEGADOS
        // ESTA ES LA FORMA MÁS RÁPIDA DE MANEJAR INPUTS

        // Es importante que el input esté en press only, si no detectará al pulsar y al soltar
        normalAttack.action.performed -= OnNormalAttack;
        specialAttack.action.performed -= OnSpecialAttack;
    }

    private void OnNormalAttack(InputAction.CallbackContext context)
    {
        // Si el timer no es 0 o menos, no se ejecuta
        if (attackTimer > 0) return;
        print("Attack performed");
        onAttackEvent?.Invoke(AttackType.Normal, normalDamage);
        attackTimer = attackCooldown;
    }

    private void OnSpecialAttack(InputAction.CallbackContext context)
    {
        // Si el timer no es 0 o menos, no se ejecuta
        if (specialTimer > 0) return;
        print("Special performed");
        onAttackEvent?.Invoke(AttackType.Special, specialDamage);
        specialTimer = specialCooldown;
    }

    private void Update()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;
        if (specialTimer > 0) specialTimer -= Time.deltaTime;
    }
}
