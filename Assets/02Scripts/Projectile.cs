using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;

    private Transform target;

    private float damage;

    public void Setup(Transform target, float damage)
    {
        movement2D = GetComponent<Movement2D>();

        // 타워로부터 전달받은 목표 저장 (공격 대상)
        this.target = target;
        this.damage = damage;

    }

    private void Update()
    {
        if (target != null)
        {
            // 발사체를 target의 위치로 이동
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else
        {
            // 발사체 오브젝트 삭제
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;     // 적이 아닌 대상과 부딪혔을 떄
        if ((collision.transform != target)) return;    // 현재 target이 적이 아닐 떄

        collision.GetComponent<EnemyHP>().TakeDamage(damage);    // 적 체력 감소
        Destroy(gameObject);    // 발사체 삭제
    }
}
