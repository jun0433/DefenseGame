using UnityEngine;
using System.Collections;

public enum EnemyDestroyType { Kill = 0, Arrive = 1 };

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform hudPoint;

    [SerializeField]
    private Transform rotator;

    [SerializeField]
    private float timeOffset = 1f;  // 이동시간 = 거리 * timeOffset

    [SerializeField]
    private int gold = 10;  // 적 사망시 획득 골드

    private Transform[] wayPoints;  // 이동 경로 정보
    private int wayPointCount;      // 이동 경로 개수
    private int currentIndex = 0;   // 현재 목표지점 인덱스
    private EnemySpawner enemySpawner;  // 적의 삭제를 본인이 하지 않고 EnemySpanwer에게 알려서 삭제
    private float baseTimeOffset;

    public Transform HUDPoint => hudPoint;
    public float TimeOffset
    {
        get => timeOffset;
        set => timeOffset = Mathf.Min(value, 5.0f); // 타일 한 칸 이동에 최댓값 설정(5초)
    }

    public void Setup(EnemySpawner enemySpanwer, Transform[] wayPoints)
    {
        baseTimeOffset = timeOffset;
        this.enemySpawner = enemySpanwer;

        // 적 이동 경로(WayPoints) 정보 설정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적 위치를 첫 번쨰 wayPoint 위치로 설정
        transform.position = wayPoints[currentIndex].position;

        currentIndex++;

        // 적 이동 제어
        StartCoroutine(nameof(Process));

    }

    private void Update()
    {
        // 적 캐릭터를 화면에 출력하는 오브젝트 회전
        rotator.Rotate(Vector3.forward, 360 * Time.deltaTime);
    }

    private IEnumerator Process()
    {
        while (true)
        {
            // 현재 위치에서 목표 위치(WayPoint)까지 이동
            yield return StartCoroutine(MoveAToB(transform.position, wayPoints[currentIndex].position));

            // 다음 이동 위치(WayPoint) 설정    
            if (currentIndex < wayPointCount - 1)
            {
                currentIndex++;
            }
            else
            {
                // 목표 지점에 도달해서 사망시 gold 처리 X
                gold = 0;

                OnDie(EnemyDestroyType.Arrive);
            }
        }
    }

    private IEnumerator MoveAToB(Vector3 start, Vector3 end)
    {
        float percent = 0;
        float moveTime = Vector3.Distance(start, end) * timeOffset;

        while (percent < 1f)
        {
            percent += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

    }
    
    public void ResetTimeOffset()
    {
        timeOffset = baseTimeOffset;
    }

    public void OnDie(EnemyDestroyType type)
    {
        // EnemySpanwer에서 리스트로 적 정보를 관리하기 때문에 Destory()를 직접하지 않고,
        // EnemySpanwer에게 본인이 삭제될 떄 필요한 처리를 하도록 DestoryEnemy() 호출
        enemySpawner.DestroyEnemy(type, this, gold);
    }

}
