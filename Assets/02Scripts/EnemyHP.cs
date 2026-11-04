using UnityEngine;
using System.Collections;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP;    // 최대 체력

    private float currentHP; // 현재 체력
    private bool isDie = false; // 적 사망 상태
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 현재 체력 초기화
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        // 적의 체력이 dmg만큼 감소해서 죽는 상황
        // 여러 타워의 공격을 동시에 받으면 enemy.OnDie() 메소드가 중복 실행 가능
        // 사망상태일 때는 실행하지 않도록 통제
        if (isDie == true) return;

        // 현재 체력을 dmg만큼 감소
        currentHP -= damage;

        StopCoroutine(nameof(HitAlphaAnimation));
        StartCoroutine(nameof(HitAlphaAnimation));

        // 체력 0이하 사망
        if (currentHP <= 0) {
            isDie = true;
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 현재 적의 색상을 color 변수에 저장
        Color color = spriteRenderer.color;

        // 적 투명도 40% 설정
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 대기
        yield return new WaitForSeconds(0.05f);

        // 투명도 100% 설정
        color.a = 1.0f;
        spriteRenderer.color = color;

    }
}
