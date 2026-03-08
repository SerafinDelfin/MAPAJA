using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMenu : MonoBehaviour
{
    // Player Life
    [SerializeField]
    SyncronizedHealthComponent healthComponent;

    [SerializeField]
    TMP_Text lifeText;

    // Ataques
    [SerializeField]
    PlayerAttackComponent attackComponent;

    [SerializeField]
    TMP_Text attackText;

    // Mostrar y ocultar
    [SerializeField]
    InputActionReference toolAction;

    private void Start()
    {
        attackComponent.onAttackEvent.AddListener(OnAttackPerformed);
        InvokeRepeating(nameof(CheckParams), 1, 1);
    }

    private void CheckParams()
    {
        // Vida
        lifeText.text = "MaxLife: " + healthComponent.GetHealth().maxHealth.ToString() + 
        "\nLife: " + healthComponent.GetHealth().health.ToString();
    }

    private void OnAttackPerformed(PlayerAttackComponent.AttackType type, int damage)
    {
        attackText.text = "Tipo: " + type.ToString() + "\n Daño: " + damage.ToString();
    }

    private void Update() 
    {
        if (toolAction.action.WasPressedThisFrame())
        {
            GameObject child = transform.GetChild(0).gameObject;
            child.SetActive(!child.activeSelf); 
        }
    }
}
