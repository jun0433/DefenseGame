using UnityEngine;
using System.Collections;
using Unity.AppUI.Core;
using Unity.VisualScripting;

public enum WeaponType { Cannon = 0, Laser = 1 };
public enum WeaponState { SearchClosetTarget = 0, TryAttackCannon, TryAttackLaser }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplateSO towerTemplate;  // 타워 정보
    [SerializeField]
    private Transform spawnPoint;   // 발사체 생성 위치
    [SerializeField]
    private WeaponType weaponType;  // 무기 속성 설정

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab; // 발사체 프리팹

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;  // 레이저 선
    [SerializeField]
    private Transform hitEffect;    // 피격 효과
    [SerializeField]
    private LayerMask targetLayer;  // 광선 충돌 레이어 설정

    private EnemySpawner enemySpawner;  // 게임에 존재하는 적 정보 획득용
    private SpriteRenderer spriteRenderer;  // 타워 오브젝트 이미지 변겨용ㅇ
    private PlayerGold playerGold;  // 플레이어 골드 정보
    private Transform attackTarget = null;  // 공격 대상
    private Tile ownerTile; // 현재 타일 배치 상태
    private WeaponState weaponState;    //타워 무기의 상태
    private int level;  // 타워 레벨

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;


    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        if(weaponType == WeaponType.Laser)
        {
            DisableLaser();
        }

        // 첫 상태는 목표를 탐색하는 SearchClosetTarget으로 설정
        ChangeState(WeaponState.SearchClosetTarget);
    }

    public void ChangeState(WeaponState newState)
    {
        // 이전에 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());

        // 상태 변경
        weaponState = newState;

        // 새로운 상태 재생
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        // 원점으로부터의 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = acrtan(y/x)
        // x,y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        // x,y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위이기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchClosetTarget()
    {
        while (true)
        {

            // 현재 타워에 가장 가까이 있는 적 탐색
            attackTarget = FindClosetTarget();

            if (attackTarget != null)
            {
                if(weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if(weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }

    }

    private IEnumerator TryAttackCacnnon()
    {
        while (true)
        {
            // 적 공격 가능 확인
            if(IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchClosetTarget);
                break;
            }
            // 3. attackRate 시간만큼 대기
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. 공격 (발사체 생성)
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 피격 효과 활성화
        EnableLaser();

        while (true)
        {
            // 공격 가능 확인
            if(IsPossibleToAttackTarget() == false)
            {
                // 레이저, 레이저 피격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchClosetTarget);
                break;
            }

            // 레이저 공격

            SpawnLaser();

            yield return null;
        }
    }


    private void SpawnProjectile()
    {
        var clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // 생선된 발사체에게 공격 대상(attackTarget) 전달
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }

    public bool Upgrade()
    {
        // 타워 업그레이드 가능 확인
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }

        // 타워 레벨 증가
        level++;
        // 타워 외형 변경
        spriteRenderer.sprite = TowerSprite;
        // 골드 사용
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // 무기 속성에 따라
        if(weaponType == WeaponType.Laser)
        {
            // 레벨에 따라 레이저 굵기 설정
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        // 현태 타일에 다시 건설 가능하도록 설정
        ownerTile.isBuildTower = false;
        // 타워 파괴
        Destroy(gameObject);
    }

    private Transform FindClosetTarget()
    {
        // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closetDistSqr = Mathf.Infinity;

        // EnemySpanwer의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            // 현재 검사중인 적과 거리가 공격 범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
            if (distance <= towerTemplate.weapon[level].range && distance <= closetDistSqr)
            {
                closetDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        // 1. target이 있는지 검사 (다른 발사체에 의해 제거, Goal 지점까지 이동해 삭제 등)
        if (attackTarget == null)
        {
            return false;
        }

        // 2. target이 공격 범위 안에 있는지 검사 (범위를 벗어나면 새로운 적 탐색)
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;

            return false;
        }

        return true;

    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = (attackTarget.position - spawnPoint.position).normalized;
        // 레이저 관통되게 설정
        var hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        // 같은 방향으로 여러 개의 광선을 쏴서 현재 target과 동일한 오브젝트 검출
        for(int i =0; i < hit.Length; ++i)
        {
            if (hit[i].transform == attackTarget)
            {
                // 광선의 시작, 목표 지점
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);

                // 피격 효과 위치 설정
                hitEffect.position = hit[i].point;

                // 적 체력 감소(초당 dmg만큼)
                attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
            }
        }
    }
}
