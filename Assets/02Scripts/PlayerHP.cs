using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    private UIScreenRed screenUI;

    [SerializeField]
    private float maxHP = 20;   // 최대 체력

    private float currentHP;    // 현재 체력

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP; // 체력 초기화
    }

    public void TakeDamage(float damage)
    {
        // 현재 체력을 damgage만큼 감소
        currentHP -= damage;

        screenUI.OnHitAnimation();

        // 체력이 0이 되면 끝
        if (currentHP <= 0)
        {

        }
    }
}
